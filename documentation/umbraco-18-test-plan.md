# Umbraco 18 Support — Test Plan

**Story:** [sc-10222 — Add support for Umbraco 18 in the Enterspeed Umbraco package](https://app.shortcut.com/enterspeed/story/10222)
**Companion document:** [umbraco-18-support-plan.md](umbraco-18-support-plan.md) (implementation plan)
**When to run:** after implementation is complete, before releasing 6.0.0 to NuGet. All verification is manual — there is no automated test coverage for the V14Plus package.

## Facts this plan is built on (source-verified, Umbraco 18.0.2)

The story's two open questions are resolved:

1. **Reference resolution: BY REFERENCE, never copied.** Pages reference library Elements via the new `Umbraco.ElementPicker` property editor, which stores a JSON array of element GUID keys. The core value converter resolves them at read time from the dedicated element cache (`IPublishedElementCache`) into `IEnumerable<IPublishedElement>`. Block List/Block Grid **cannot** reference library Elements in 18.0 — that integration arrives in Umbraco 19. The Element Picker is the only page→element bridge to test.
2. **Reingestion on update: NOT automatic.** `ContentCacheRefresherNotification` does **not** fire for referencing documents when an element changes. Umbraco fires a new parallel notification set (`ElementPublishedNotification`, `ElementCacheRefresherNotification`, etc.), and reference tracking is first-class: picked elements create automatic `umbElement` relations queryable via `IRelationService`/`ITrackedReferencesService`. The package must handle the element notifications and fan out reingest jobs to referencing documents itself (this is the sanctioned pattern — Umbraco's own Delivery API output-cache eviction does exactly this).

Two further facts drive dedicated test sections:

3. **Deleted/unpublished elements are silently omitted** by the core converter (`WhereNotNull()` + published filter) — no exception, no stale copy. Our converter must behave the same, and the ingested payload must reflect the removal.
4. **The v18 upgrade auto-migrates every single-mode Block List data type to the `Umbraco.SingleBlock` editor** (rewriting nested property data recursively). A converter registry keyed on `Umbraco.BlockList` silently stops matching those properties after upgrade — this affects *existing customers' data*, not just new features.

---

## Test matrix

| Env | Umbraco | Package | Purpose |
|---|---|---|---|
| **E1** | 18.0.2, fresh install | 6.0.0-rc | Primary target — all parts |
| **E2** | 17.x site **upgraded to 18.0.2**, previously running 5.3.x with seeded data and single-mode block lists | 6.0.0-rc | Upgrade path: DB migration carry-over + Single Block auto-migration |
| **E3** | 17.5.3 | 5.x built from the same commit | Regression — the 17 pass must be unaffected |
| **E4** (nice-to-have) | 14.x | 5.x from same commit | Floor of the 5.x line |

Common site fixture (E1/E2/E3): two languages with culture variants and assigned domains; a content tree ≥3 levels; media library with images + files; dictionary items with nested descendants; data types covering the converter sweep (Part E); an Enterspeed test tenant with separate publish + preview sources. E1/E2 additionally: ≥2 Element Types allowed in the Library, ≥3 element instances, an Element Picker property (multi-select) on a page doc type, and ≥2 pages referencing the same element.

---

## Part A — Installation & startup

*Covers AC: "installs and runs on an Umbraco 18 solution without errors".*

| # | Test | Expected |
|---|---|---|
| A1 | `dotnet add package Enterspeed.Source.UmbracoCms --version 6.0.0-rc.*` on E1; boot | Restores (no version conflicts), site starts, no startup errors in log |
| A2 | First boot runs the migration plan | `EnterspeedJobs` table created; `umbracoKeyValue` contains the `EnterspeedJobs` plan state at `enterspeedjobs-db-v4` |
| A3 | E2: upgrade site from 17/5.3.x → 18/6.0.0-rc; boot | Migration state carries over (plan does **not** re-run destructive steps); existing job rows intact; Umbraco's own `MigrateSingleBlockList` migration completes |
| A4 | Install 6.0.0-rc on a **17** site | Restore fails loudly (closed range `[18.0.2, 19.0.0)`) — no silent Umbraco upgrade |
| A5 | Inspect both nupkgs from the release build | 5.x: `lib/net8.0|net9.0|net10.0` (17-flavour); 6.x: `lib/net10.0` only; both contain the Base assembly, App_Plugins static assets, buildTransitive targets, appsettings schema |
| A6 | App_Plugins assets copied into consuming project on build; dashboard bundle served | `App_Plugins/Enterspeed.Source.UmbracoCms/` present; no 404 on `assets.js` |

## Part B — Baseline ingest regression

*Covers AC: "existing ingest behaviour (publish, unpublish, delete) continues to work as before". Run on E1, E2 and E3.*

| # | Test | Expected |
|---|---|---|
| B1 | Configure API key/base URL in the settings dashboard; save; test connection | Saved (persisted via key-value store); connection test green |
| B2 | Full seed from dashboard | Jobs enqueued for all published content, media, dictionary; processed by hosted services; entities visible in Enterspeed |
| B3 | Custom seed: content subtree, media folder, **dictionary node** | Only selected subtrees ingested. Dictionary node exercises the reworked int→Guid lookup (§4.2 of the implementation plan) — must not fail |
| B4 | Publish a page (both cultures) | One job per culture; entity per culture in Enterspeed with correct URL (domain-based, exercises reworked `GetAssignedDomainsAsync`), parent id (exercises `Parent()` extension), properties, metadata |
| B5 | Save-as-draft on a published page | Preview source updated; publish source untouched |
| B6 | Unpublish a page / move to recycle bin | Delete jobs enqueued; entity removed from Enterspeed views |
| B7 | Publish a branch (parent + descendants via "publish with descendants") | All descendants reingested (cache-refresher `RefreshBranch` path; also verifies the `PublishedCultures` reflection probe end-to-end — culture-specific jobs, not all-culture republish) |
| B8 | Dictionary item create/edit/delete (incl. nested item) | Ingest/delete jobs for the item and descendants |
| B9 | Media upload/edit/move/trash; page referencing media | Media entity ingested with correct URL (incl. `MediaDomain` config); trashed media removed |
| B10 | Redirects: rename a published page's URL segment | Redirect captured on the ingested entity (`IRedirectUrlService` path) |
| B11 | Failed-jobs flow: point base URL at an unreachable host, publish, restore URL | Jobs fail with exception recorded, `FailedCount` increments, dashboard lists them; "delete failed jobs" works; jobs retry via failed-jobs hosted service |
| B12 | Job queue admin: pending count, clear pending jobs | Counts correct; clear works |
| B13 | Load-balancing gate (if feasible): subscriber-role instance | Subscriber does not process jobs (`IServerRoleAccessor` gate) |

## Part C — Reusable Elements (story focus; E1 + E2)

*Covers AC: element content ingested, references resolved, reingestion on update, delete/unpublish edge cases. Prerequisite: the implementation includes the `Umbraco.ElementPicker` converter and the element-notification → `umbElement`-relation fan-out handler (implementation plan §4.7).*

| # | Test | Expected |
|---|---|---|
| C1 | Publish page P1 with an Element Picker property referencing published elements X and Y | P1's ingested payload contains the **resolved content** of X and Y (properties, content type alias) — not bare GUIDs |
| C2 | Two pages P1, P2 reference element X. Edit X and **publish** it | Reingest jobs enqueued for **both** P1 and P2 (relation fan-out completeness); within the normal ingest window both entities in Enterspeed show X's updated content. No manual republish of P1/P2 needed |
| C3 | Edit X, **save without publishing** | Publish source unchanged; preview source (if preview configured) reflects the draft via the preview pipeline only |
| C4 | **Unpublish** element X while P1/P2 still reference it | No unhandled errors; P1/P2 reingested; their payloads omit X (matching core converter behaviour: silently filtered) and retain Y |
| C5 | **Delete** element X (recycle bin, then permanent delete) | Same as C4 — no exception in handlers/converters, referencing pages reingested without X |
| C6 | Re-publish / restore X | P1/P2 reingested; X's content back in their payloads |
| C7 | Element with culture variants (if the Element Type varies by culture): edit + publish one culture only | Only the affected culture's referencing-page entities are reingested/updated |
| C8 | Element X referenced only from an **unpublished** page P3; publish X | No publish job for P3 (unpublished pages are not ingested); no errors |
| C9 | Element edited that is referenced by **no** pages | No spurious content jobs; no errors |
| C10 | **Draft-leak check (security criterion):** with element X having unpublished changes (published v1, draft v2), reingest P1 | Publish-source payload contains **v1 only**. Draft v2 must never appear in the publish source — only in the preview source |
| C11 | Full seed on a site with element-referencing pages | Seeded page payloads contain resolved element content; no ordering failures (element cache available during seed) |
| C12 | Scale sanity: element referenced by ~50 pages; publish it | All 50 reingest jobs enqueued and processed; no timeout/duplicate storms (dedupe via job queue) |

> **Design note to verify against implementation:** if the implementation *also* ingests elements as standalone Enterspeed entities (rather than only inlining by value, which is the Delivery API precedent), add: element entity created/updated/deleted in Enterspeed mirroring element lifecycle, with referencing pages carrying references rather than copies. Amend C1–C6 expectations accordingly.

## Part D — Single Block migration (E2 primarily, E1 for fresh usage)

*Guards existing customers: the v18 upgrade silently rewrites single-mode Block List data types to `Umbraco.SingleBlock`.*

| # | Test | Expected |
|---|---|---|
| D1 | E2 pre-upgrade: page with a single-mode Block List property, ingested by 5.3.x. Post-upgrade to 18: republish the page | Property is now editor alias `Umbraco.SingleBlock`; ingested payload still contains the block content in the same shape as before the upgrade (converter for `Umbraco.SingleBlock` present); **not** silently dropped |
| D2 | E2: single-mode block list **nested inside** a Block Grid / RTE block | Nested data migrated by Umbraco; ingest still resolves the nested block content |
| D3 | E1: create a new Single Block property on a fresh 18 site; publish | Ingested correctly |
| D4 | Diff check: compare an entity's payload ingested by 5.3.x-on-17 vs 6.0.0-on-18 for an identical content node | Structurally identical except intentional changes; any shape change is documented in the changelog |

## Part E — Property converter sweep (E1)

One page exercising each supported editor; publish; verify each property lands correctly in the payload: block list, **single block**, block grid, **element picker**, rich text (TinyMCE + `Umbraco.RichText`, incl. local links and images), multi-URL picker, media picker 3, legacy media picker (E3/E4 only — compiled out on 15+), content picker, multinode tree picker, nested content, dropdown, checkbox list, radio button list, tags, slider, color picker, image cropper (incl. focal point), date/time, decimal, integer, textbox, textarea, multiple textstring, markdown, email, upload field, member group picker (**reworked async call**), member picker, user picker, true/false.

## Part F — Dashboard & Management API (E1, E3; spot-check E4)

| # | Test | Expected |
|---|---|---|
| F1 | Dashboard loads in Content + Settings sections on 18 | Renders; no console errors; auth token flows (`UMB_AUTH_CONTEXT`) |
| F2 | All dashboard actions on 18: get failed jobs, seed, custom seed (node pickers open), config save, test connection, pending count, clear/delete jobs | All succeed; error toasts render properly on induced failures (the `normalizeResponse`/arity shims) |
| F3 | `/umbraco/openapi/enterspeed.json` on E1 (dev mode) | Document served (OpenAPI 3.1); "Dashboard" ApiExplorer group present; note it is off by default in Production mode |
| F4 | `/umbraco/swagger/enterspeed/swagger.json` on E3 | Still served by the 5.x pass (Swashbuckle path intact) |
| F5 | Regenerate the TS client from the 18 document with `@hey-api/openapi-ts`; `tsc` passes; rebuilt dashboard still works on 14/17/18 hosts | Type-check clean; runtime OK on all three |

## Part G — 17 regression with the same commit (E3)

Run Part A (A1–A2, A6), Part B in full, and F1/F2/F4 on Umbraco 17.5.3 with the 5.x package built from the **same commit** as 6.0.0-rc. Expected: zero behaviour change vs 5.3.6. This proves the `UmbracoMajor` split leaves the 17 pass untouched. (E4/Umbraco 14 spot-check: A1, B2, B4, B6, F1.)

---

## Traceability to story acceptance criteria

| Acceptance criterion | Test cases |
|---|---|
| Installs and runs on Umbraco 18 without errors | A1–A3, A6 |
| Existing ingest behaviour continues to work | Part B, Part D (upgrade continuity), Part G |
| Reusable-element content correctly ingested | C1, C11, D-independent |
| Referenced content resolved in the payload (reference, not copy — confirmed) | C1, C7 |
| Referencing pages reingested when element updated | C2, C6, C7, C12 |
| Deleted/unpublished element handled without unhandled errors | C4, C5, C8 |
| Security: no draft/unpublished leakage | C3, C10 |
| Published to NuGet, versioning safe | A4, A5 |

## Exit criteria

- All Part A–C and G cases pass; Part D–F pass or have documented, accepted deviations.
- No unhandled exceptions in Umbraco logs across the full run.
- Payload diff (D4) reviewed and signed off.
- Failures triaged: blocker = any AC-mapped case; non-blocker deviations recorded in the story before release.
