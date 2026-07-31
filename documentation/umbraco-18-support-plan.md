# Umbraco 18 Support — Implementation Plan

**Status:** Validated — compile-prototyped against Umbraco 18.0.1 (22 errors / 13 files, all accounted for below) and API-verified against the `release-17.0.0` / `release-18.0.2` source tags
**Target:** `Enterspeed.Source.UmbracoCms` 6.0.0 (Umbraco 18+) alongside continued 5.x (Umbraco 14–17)
**Strategy decision:** Single branch (`develop`), two pack passes driven by an MSBuild `UmbracoMajor` property

---

## 1. Background

- Umbraco 18 shipped **25 June 2026**; latest patch **18.0.2** (security release — use 18.0.2 / 17.5.3 as minimum verified versions). It is an **STS release; Umbraco 17 remains the LTS** and will hold the majority of the install base for years. 18.1.0-rc (23 July 2026) contains **no** additional breaking changes.
- Umbraco 18 targets **.NET 10 — the same TFM as Umbraco 17**. Our packaging model maps TFM → Umbraco major (`net8.0`→14, `net9.0`→15/16, `net10.0`→17), so the TFM trick cannot disambiguate 17 vs 18.
- Umbraco 18 is largely a "code tidy" major: everything marked `[Obsolete]` in 16/17 with "scheduled for removal in V18" has been deleted.

### Confirmed breaking changes affecting this codebase

All verified by compiling the codebase against Umbraco 18.0.1. "Dual-safe" = the replacement exists with an identical signature in 17, so one gated code path serves both pack passes.

| # | Break in Umbraco 18 | Our usage | Replacement | Dual-safe (17+18)? |
|---|---|---|---|---|
| 1 | `MigrationBase` removed | 4 classes in `Base/Data/Migration/` | `AsyncMigrationBase` + `MigrateAsync()` (verified: all builder APIs used — `Logger`, `Database`, `Create`, `TableExists`, `ColumnExists` — exist on the async base; `MigrationPlan.To<T>()` accepts them) | Yes |
| 2 | `ILocalizationService` removed | 5 files (see §4.2) | `ILanguageService.GetAllAsync()` + `IDictionaryItemService.GetAsync(Guid)` / `GetDescendantsAsync(Guid?, string?)` — signatures byte-identical in 17 and 18 | Yes, except the `int`-keyed lookup (§4.2) |
| 3 | Swashbuckle removed — no longer even a transitive dependency; `ISchemaIdSelector`/`ISchemaIdHandler`/`IOperationIdSelector`/`IOperationIdHandler` deleted from `Umbraco.Cms.Api.Common.OpenApi` | `ConfigureEnterspeedApiSwaggerGenOptions`, `EnterspeedSchemaIdSelector`, `EnterspeedOperationIdSelector` + registrations and the file-level `using` in `EnterspeedComposer.cs` | `AddBackOfficeOpenApiDocument("enterspeed", …)` et al. (§5) — exists **only** in 18 | **No — zero API overlap; the one `UMBRACO_18_OR_GREATER` surface** |
| 4 | `IPublishedContent.Parent` property removed (interface slimmed; `Children` etc. also gone) | `UmbracoContentEntity.cs:37`, `UmbracoMasterContentEntity.cs:28` | Parameterless `Parent()` extension (`Umbraco.Extensions.FriendlyPublishedContentExtensions`, assembly Umbraco.Cms.Web.Common) — probe-compiled cleanly on 18, no obsolete warning | Yes |
| 5 | `IComponent` removed (`Umbraco.Cms.Core.Composing`) | `EnterspeedJobsComponent.cs:16` | `IAsyncComponent` (`InitializeAsync(bool, CancellationToken)` / `TerminateAsync(...)`); `builder.Components().Append<T>()` accepts it | Yes (confirm 17 shape at implementation — expected, as `IComponent` was removal-scheduled) |
| 6 | `Upgrader.Execute(...)` (sync) removed | `EnterspeedJobsComponent.cs:48` | `ExecuteAsync(IMigrationPlanExecutor, ICoreScopeProvider, IKeyValueService)` — exists in 17 (where sync was `[Obsolete]`) and 18; our injected `IScopeProvider` implements `ICoreScopeProvider`. Dovetails with #5: `InitializeAsync` provides the async context | Yes |
| 7 | `IMemberGroupService.GetByIds(IEnumerable<int>)` removed | `DefaultMemberGroupPickerPropertyValueConverter.cs:34` | Async variant (verify exact 17/18-common signature at implementation) | Yes (expected) |
| 8 | `IDomainService.GetAssignedDomains(int, bool)` removed | `EnterspeedPropertyService.cs:121` | `GetAssignedDomainsAsync(...)` — note the async API is **Guid-keyed**; call site has `content.Id` (int) but `IPublishedContent.Key` is available | Yes |
| 9 | Swagger endpoint moved `/umbraco/swagger/{doc}/swagger.json` → `/umbraco/openapi/{doc}.json`; spec now OpenAPI 3.1; documents **disabled in Production by default** in 18 | Client `generate` npm script | Update URL; run generation against a non-production-mode site; codegen tool must support 3.1 (§6) | n/a (dev-time only) |

Full prototype error list: 22 compile errors across 13 files — 15 "wave 1" (type/base-type declarations) and 7 "wave 2" (method-body errors that only surface once wave 1 is fixed; a naive single build under-reports). Items #1–#8 cover all of them.

### Confirmed non-issues (validated by the 18.0.1 compile and source diff)

- Controllers use plain `ControllerBase` (not the removed `UmbracoApiController`). All controller attributes exist unchanged in 18: `MapToApiAttribute`, `BackOfficeRouteAttribute`, `JsonOptionsNameAttribute` (`Umbraco.Cms.Api.Common.Filters`), `Constants.JsonOptionsNames.BackOffice`, `AuthorizationPolicies.BackOfficeAccess`.
- No usage of `GetAtRoot`, `IFileService`, `PackageMigrationBase`, `ContentFinderByUrlNew`, `NewDefaultUrlProvider`, renamed EF Core scoping types, or `.Children` property access. `MoveEventInfo` is consumed in 4 notification handlers but **only `.Entity` is read** — the removed `NewParent` member is untouched.
- Notification pipeline (`INotificationHandler<T>`, all notifications we subscribe to) is unchanged in 18.
- `IAuditService.GetPagedItemsByEntity`, `HtmlLocalLinkParser`/`HtmlImageSourceParser`, `UmbracoHelper`/`IUmbracoHelperAccessor`, `UdiParser`/`GuidUdi` usages, `IUserService.GetUserById(int)` — all compile cleanly on 18.0.1.
- Reflection probe on `ContentCacheRefresher.JsonPayload` (`PublishedCultures`/`UnpublishedCultures`, [EnterspeedContentCacheRefresherNotificationHandler.cs:33](../src/Enterspeed.Source.UmbracoCms.Base/NotificationHandlers/EnterspeedContentCacheRefresherNotificationHandler.cs)) — both properties exist unchanged in the 18 payload.
- Backoffice client: none of the v18 client removals (tiptap import path, `user`→`current-user` module, `UmbSectionContext.setManifest`) are used; all `@umbraco/*` imports are externalised in the Vite build and resolved by the host at runtime. The `tryExecute` arity-sniffing shim added in 5.3.6 already anticipates 18. `umbraco-package.json` manifest format is unchanged.

### Obsolete-but-compiling watch list (not in scope, likely Umbraco 19 casualties)

- `RecurringHostedServiceBase.PerformExecuteAsync(object?)` override → CS0672 in all 3 hosted services (`Base/HostedServices/`). Successor: `IRecurringBackgroundJob`.
- `SpecialDbTypes.NTEXT` → CS0618 in `EnterspeedJobSchema.cs:30` ("Use NVARCHARMAX instead").
- Consider tackling both in this story since we're touching adjacent code, or explicitly park them for the Umbraco 19 story (19 lands Q4 2026).

---

## 2. Versioning & packaging strategy

Two package lines produced **from the same commit** on `develop`:

| Pass | `UmbracoMajor` | Package version | TFMs | Umbraco refs |
|---|---|---|---|---|
| 1 | `17` (default) | **5.x** (continues current line) | `net8.0;net9.0;net10.0` | 14.0.0 / 15.0.0 / 17.0.0 (unchanged) |
| 2 | `18` | **6.x** (new line) | `net10.0` only | `[18.0.2, 19.0.0)` |

Rules:

- Same `PackageId` (`Enterspeed.Source.UmbracoCms`), two majors — same precedent as the 4.x (V9Plus) / 5.x (V14Plus) split.
- 6.x ships **net10.0 only**. Keeping net8.0/net9.0 in 6.x would produce a package supporting 14/15/16/18-but-not-17, which is impossible to communicate.
- Closed upper bound `[18.0.2, 19.0.0)` for the 18 reference: 18 removed binaries aggressively, 19 is due Q4 2026, and 18.0.2 is the security floor.
- README/changelog must state clearly: **Umbraco 14–17 → install 5.x; Umbraco 18 → install 6.x.**

### Compilation-symbol convention

- **TFM symbols (`NET10_0_OR_GREATER` etc.)** answer "which .NET / which minimum Umbraco". They remain correct wherever 17 and 18 accept the same code — breaks #1, #2, #4–#8, i.e. everything except OpenAPI, because the replacements are all dual-safe.
- **`UMBRACO_18_OR_GREATER`** (new, defined when `UmbracoMajor=18`) answers "which Umbraco major on a shared TFM". Only needed for break #3, the OpenAPI surface. Named `_OR_GREATER` so gates compose when Umbraco 19 arrives.

---

## 3. Phase 1 — Build plumbing

### 3.1 `Enterspeed.Source.UmbracoCms.V14Plus.csproj`

```xml
<PropertyGroup>
  <UmbracoMajor Condition="'$(UmbracoMajor)' == ''">17</UmbracoMajor>
</PropertyGroup>

<PropertyGroup Condition="'$(UmbracoMajor)' == '18'">
  <TargetFrameworks>net10.0</TargetFrameworks>
  <DefineConstants>$(DefineConstants);UMBRACO_18_OR_GREATER</DefineConstants>
</PropertyGroup>

<!-- Existing net10.0 ItemGroup gains the UmbracoMajor condition -->
<ItemGroup Condition="'$(TargetFramework)' == 'net10.0' and '$(UmbracoMajor)' != '18'">
  <PackageReference Include="Umbraco.Cms" Version="17.0.0" />
  <PackageReference Include="Umbraco.Cms.Web.Website" Version="17.0.0" />
  <PackageReference Include="Umbraco.Cms.Core" Version="17.0.0" />
</ItemGroup>
<ItemGroup Condition="'$(TargetFramework)' == 'net10.0' and '$(UmbracoMajor)' == '18'">
  <PackageReference Include="Umbraco.Cms" Version="[18.0.2, 19.0.0)" />
  <PackageReference Include="Umbraco.Cms.Web.Website" Version="[18.0.2, 19.0.0)" />
  <PackageReference Include="Umbraco.Cms.Core" Version="[18.0.2, 19.0.0)" />
  <!-- Umbraco's v18 extension template adds this DIRECT reference: the package's
       InterceptorsNamespaces targets ship in build/ (not buildTransitive/), so they
       only auto-apply on a direct reference; it also pins Microsoft.OpenApi to avoid
       version conflicts that make custom documents silently disappear -->
  <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.7" />
</ItemGroup>
```

(The direct `Microsoft.AspNetCore.OpenApi` reference supersedes setting `<InterceptorsNamespaces>` by hand — it is what Umbraco's own v18 extension template does. If we ever drop the direct reference, we must add `<InterceptorsNamespaces>$(InterceptorsNamespaces);Microsoft.AspNetCore.OpenApi.Generated</InterceptorsNamespaces>` manually.)

### 3.2 `Enterspeed.Source.UmbracoCms.Base.csproj`

Same pattern: `UmbracoMajor` default `17`; when `18` → `TargetFrameworks` collapses to `net10.0`, the `net10.0` Umbraco ItemGroup splits 17 vs 18, `UMBRACO_18_OR_GREATER` defined. (The net5.0–net9.0 rows serving the V9Plus 4.x package and the 5.x pass are untouched; the V9Plus solution builds Base with `UmbracoMajor` unset → default 17 → byte-identical output.)

Housekeeping while there: bump Base's net10.0 `Enterspeed.Source.Sdk` reference from `2.0.7-alpha.2` to `2.0.7` (V14Plus already got this in `0d2441b`).

### 3.3 Checks

- Verify `dotnet restore /p:UmbracoMajor=18` and `=17` both succeed from a clean checkout. (Prototype confirmed: 18.0.1 restores and the full error surface is exactly §1.)
- Verify pack output: pass 1 nupkg has `lib/net8.0|net9.0|net10.0` (17-flavour), pass 2 nupkg has `lib/net10.0` (18-flavour) only.
- Verify the `CopyProjectReferencesToPackage` workaround (Base injected into `lib/<tfm>`) still behaves under both passes.
- Remove the stale `App_Plugins\Enterspeed.Dashboard\**` Content glob (leftover from V9Plus, folder doesn't exist).

---

## 4. Phase 2 — Shared code migrations (TFM-gated, no new symbol needed)

All replacements below exist in Umbraco 17 with identical signatures, so `#if NET10_0_OR_GREATER` gates them; both pack passes compile the same branch. Old TFMs (Umbraco 9–16) keep the current code. **Every one of these files is also compiled into the V9Plus TFMs (net5.0–net8.0), so the gates must straddle whole declarations — base-class token + method signature for the migrations, fields + constructor parameters (dual constructors) for the service swaps — not just call bodies.**

### 4.1 Migrations → `AsyncMigrationBase`

Files (all in `src/Enterspeed.Source.UmbracoCms.Base/Data/Migration/`):

- `EnterspeedJobsTableMigration.cs`
- `AddEntityTypeToJobsTable.cs`
- `AddContentStateToJobsTable.cs`
- `AddFailedCountToJobsTable.cs`

Change: base class `MigrationBase` → `AsyncMigrationBase`, `protected override void Migrate()` → `protected override Task MigrateAsync()`. The migration bodies are unchanged — prototype-verified that everything they use exists on the async base.

### 4.2 `ILocalizationService` → `ILanguageService` + `IDictionaryItemService`

Files (line numbers verified exact):

- `Base/Services/EnterspeedJobService.cs:277,313,319`
- `Base/Handlers/EnterspeedPostJobsHandler.cs:87`
- `Base/Handlers/Dictionaries/EnterspeedDictionaryItemPublishJobHandler.cs:67`
- `Base/Handlers/PreviewDictionaries/EnterspeedPreviewDictionaryItemPublishJobHandler.cs:67`
- `Base/NotificationHandlers/EnterspeedDictionaryItemDeletingNotificationHandler.cs:87`

Mapping:

| Current call | Replacement | Note |
|---|---|---|
| `GetDictionaryItemById(Guid)` | `IDictionaryItemService.GetAsync(Guid)` | 1:1 |
| `GetDictionaryItemDescendants(Guid?)` — incl. the `null` "all items" call at `EnterspeedJobService.cs:277` | `IDictionaryItemService.GetDescendantsAsync(Guid? parentId, string? filter = null)` | `parentId` is nullable in the new API too — 1:1 including the null case |
| `GetAllLanguages()` | `ILanguageService.GetAllAsync()` | 1:1 |
| **`GetDictionaryItemById(int)`** at `EnterspeedJobService.cs:313` (`dictionarySeedNode.Id` is `int`, `Base/Models/Api/CustomSeed.cs:12`) | **No int-keyed lookup exists on `IDictionaryItemService`.** Resolve the key first — either extend `CustomSeedNode` to carry the Guid from the dashboard (preferred; the V14Plus `EnterspeedJobService` populates it and already has the `IDictionaryItem` in hand at `V14Plus/Services/EnterspeedJobService.cs:78`), or map via `IIdKeyMap` | The one non-mechanical change in this phase |

The new services are async-only — call sites in sync methods bridge with `.GetAwaiter().GetResult()` (consistent with existing async-bridging in the codebase), or go async where the chain allows.

### 4.3 `IPublishedContent.Parent` → `Parent()` extension

- `Base/Models/UmbracoContentEntity.cs:37`
- `Base/Models/UmbracoMasterContentEntity.cs:28`

`_content.Parent` → `_content.Parent()` under `#if NET10_0_OR_GREATER`, adding `using Umbraco.Extensions;`. Prototype-verified: the parameterless extension resolves on 18 with no service arguments and no obsolete warning, so no constructor changes ripple into the 4 handlers that `new` these entities up.

### 4.4 `EnterspeedJobsComponent`: `IComponent` → `IAsyncComponent`, `Execute` → `ExecuteAsync`

`Base/Components/EnterspeedJobsComponent.cs` — two breaks, one fix:

- `IComponent` no longer exists in 18; implement `IAsyncComponent` (`InitializeAsync(bool isRestarting, CancellationToken token)` / `TerminateAsync(...)`). The registration `builder.Components().Append<EnterspeedJobsComponent>()` (`UmbracoBuilderExtensions.cs:199`) accepts it unchanged.
- `upgrader.Execute(...)` → `await upgrader.ExecuteAsync(_migrationPlanExecutor, _scopeProvider, _keyValueService)` — same parameters (`IScopeProvider` implements the `ICoreScopeProvider` the signature asks for), and `InitializeAsync` conveniently provides the async context so no sync-over-async bridge is needed.
- Gate both with `#if NET10_0_OR_GREATER` (dual class shape, like the migrations). Old TFMs keep `IComponent` + sync `Execute`.

### 4.5 Removed sync service methods

- `IMemberGroupService.GetByIds(IEnumerable<int>)` at `DefaultMemberGroupPickerPropertyValueConverter.cs:34` → async variant (confirm the exact 17/18-common signature at implementation).
- `IDomainService.GetAssignedDomains(content.Id, false)` at `EnterspeedPropertyService.cs:121` → `GetAssignedDomainsAsync(...)`. The async API is Guid-keyed; use `content.Key` (available — the call site has the `IPublishedContent`).
- Both gated `#if NET10_0_OR_GREATER` with `.GetAwaiter().GetResult()` bridges.

### 4.6 NEW — Umbraco 18 feature support (story sc-10222 scope)

Source-verified against `release-18.0.2` (see [umbraco-18-test-plan.md](umbraco-18-test-plan.md) for the underlying facts). Three work items, all in the 18-only surface (gate `UMBRACO_18_OR_GREATER` or place in 18-only compile items):

1. **`Umbraco.ElementPicker` property value converter.** New editor in 18; stores element GUID keys, resolved at read time to `IEnumerable<IPublishedElement>` via `IPublishedElementCache`. Without a converter, element-referencing properties fall through our EditorAlias-keyed registry. Resolve by value into the page payload (mirrors the Delivery API precedent, which inlines `IApiElement` content into referencing documents). Deleted/unpublished elements are silently omitted by the core converter — ours must match, and must not leak draft element content into the publish source.
2. **`Umbraco.SingleBlock` property value converter.** The v18 upgrade **auto-migrates every single-mode Block List data type** to this editor (rewriting nested data recursively, incl. inside Block Grid/RTE blocks). Existing customers' properties silently stop matching our `Umbraco.BlockList` converter after upgrade. The core converter returns a single `BlockListItem` (value shape: `SingleBlockValue`), not a `BlockListModel` — a dedicated converter, not an alias addition.
3. **Element change → referencing-page reingest fan-out.** `ContentCacheRefresherNotification` does **not** fire for referencing documents when an element changes. Add a handler for the new element notifications (`ElementCacheRefresherNotification`, or `ElementPublished/Unpublished/DeletedNotification`), resolve referencing documents via the automatic **`umbElement`** relations (`IRelationService`/`ITrackedReferencesService`), and enqueue publish jobs for them. This is the pattern Umbraco core itself uses for Delivery API output-cache eviction.

Design decision to confirm with the team: inline elements by value only (recommended for 18.0 — matches the Delivery API and the story's AC), or additionally ingest elements as standalone Enterspeed entities. Block-editor integration of Elements arrives in Umbraco 19 — parking a follow-up story for that is recommended.

### 4.7 Audit of existing `NET10_0_OR_GREATER` gates

Their meaning shifts from "Umbraco 17" to "Umbraco 17 or 18". Both existing gates verified safe:

- `DefaultMultiUrlPickerPropertyValueConverter.cs:90` (`GuidUdi` + `GetById(Guid)`) — compiles on both. Keep.
- `ConfigureEnterspeedApiSwaggerGenOptions.cs:3` (OpenApi using-swap) — file becomes 17-only via Phase 3 exclusion; gate becomes irrelevant.

---

## 5. Phase 3 — OpenAPI split (the only `UMBRACO_18_OR_GREATER` surface)

### 5.1 17-only files (excluded from the 18 pass)

In `V14Plus.csproj` (paths verified; `EnterspeedVersionedRouteAttribute.cs` stays — it is version-agnostic):

```xml
<ItemGroup Condition="'$(UmbracoMajor)' == '18'">
  <Compile Remove="Configuration\ConfigureEnterspeedApiSwaggerGenOptions.cs" />
  <Compile Remove="Configuration\EnterspeedSchemaIdSelector.cs" />
  <Compile Remove="Configuration\EnterspeedOperationIdSelector.cs" />
</ItemGroup>
```

### 5.2 New 18-only registration

New file `Configuration/Umbraco18/EnterspeedOpenApiConfiguration.cs` (whole file wrapped in `#if UMBRACO_18_OR_GREATER` as belt-and-braces). Exact 18 API surface (source-verified):

- `AddBackOfficeOpenApiDocument(string documentName, Action<BackOfficeOpenApiDocumentBuilder>? configure)` — extension on `IUmbracoBuilder`, namespace `Umbraco.Cms.Api.Common.OpenApi`. Sets the document info that `ConfigureEnterspeedApiSwaggerGenOptions` used to set (title "Enterspeed API"). Note: document names are lowercased on registration (18.0.1 fix) — ours ("enterspeed") already is.
- Security: `WithBackOfficeAuthentication()` on the builder, or `AddBackofficeSecurityRequirements()` on `OpenApiOptions` (namespace `Umbraco.Cms.Api.Management.OpenApi`).
- Schema/operation-id customisation replacements: schema ids via the `CreateSchemaReferenceId` delegate on `OpenApiOptions` (default `UmbracoSchemaIdGenerator.CreateSchemaReferenceId`), operation ids via `UmbracoOperationIdTransformer` (`IOpenApiOperationTransformer`) — both reachable through `BackOfficeOpenApiDocumentBuilder.ConfigureOpenApiOptions(...)`. Assess during implementation whether the Umbraco defaults are acceptable — the ids only affect the generated TypeScript client, which we regenerate anyway (§6).

### 5.3 Composer

`src/Enterspeed.Source.UmbracoCms.V14Plus/EnterspeedComposer.cs` — note the **file-level `using Umbraco.Cms.Api.Common.OpenApi;` (line 10) must be gated too**: `ISchemaIdSelector`/`IOperationIdSelector` are deleted from that namespace in 18, so the registrations at lines 21–22 *and* their using directive both break.

```csharp
#if UMBRACO_18_OR_GREATER
using Enterspeed.Source.UmbracoCms.V14Plus.Configuration.Umbraco18;
#else
using Umbraco.Cms.Api.Common.OpenApi;
#endif
...
#if UMBRACO_18_OR_GREATER
        // AddBackOfficeOpenApiDocument("enterspeed", ...) registration
#else
        builder.Services.AddSingleton<ISchemaIdSelector, EnterspeedSchemaIdSelector>();
        builder.Services.AddSingleton<IOperationIdSelector, EnterspeedOperationIdSelector>();
        builder.Services.ConfigureOptions<ConfigureEnterspeedApiSwaggerGenOptions>();
#endif
```

### 5.4 Routing/attribute surface — verified, no work needed

`MapToApiAttribute`, `BackOfficeRouteAttribute`, `JsonOptionsNameAttribute`, `AuthorizationPolicies.BackOfficeAccess` are unchanged in 18 (source-diffed). The `Asp.Versioning` attributes on `DashboardController` (`[ApiVersion]`, `[MapToApiVersion]`, `[ApiExplorerSettings(GroupName = "Dashboard")]`) and the `{version:apiVersion}` route constraint compiled cleanly in the 18 prototype; confirm at runtime that the ApiExplorer group surfaces correctly in the new OpenAPI document (§9 checklist).

---

## 6. Phase 4 — Backoffice client

Directory: `src/Enterspeed.Source.UmbracoCms.V14Plus/Client/assets/`

1. Bump devDependency `@umbraco-cms/backoffice` `^14.0.0` → **pinned `18.0.2`** (npm's `latest` dist-tag currently points at 18.1.0-rc — a caret install today pulls an RC); run `tsc` and fix any type breaks (none expected — audited imports avoid all v18 removals).
2. **Replace `openapi-typescript-codegen` with `@hey-api/openapi-ts`** — the old generator does not support OpenAPI 3.1 (maintenance mode; its own maintainer endorses the hey-api fork, which Umbraco itself uses). Update the `generate` script URL: `http://localhost:21142/umbraco/swagger/enterspeed/swagger.json` → `http://localhost:21142/umbraco/openapi/enterspeed.json`, and regenerate `src/generated/**` against an Umbraco 18 dev site. Two caveats: OpenAPI documents are **disabled in Production mode by default in 18** (generate against a dev-mode site), and expect a large generated diff — the current output even contains .NET reflection-type models (`Assembly.ts`, `MethodInfo.ts`, …) leaked via an `Exception`-shaped response; don't mistake the churn for a break.
3. Keep the Vite `external: [/^@umbraco/]` setup — one bundle continues to serve 14–18 at runtime; the shims in `src/repository/enterspeed.repository.ts` (arity sniffing + `normalizeResponse`) stay as-is.
4. Smoke-test the dashboard on 14, 17 and 18 hosts (§9).

---

## 7. Phase 5 — CI/CD (`azure-pipelines-u14+.yml`)

1. Add second version variable set for the 6.x line (`major18Version: 6`, minor/patch), with its own `$[counter(...)]` per branch — same suffix rules as today.
2. Build/pack twice (loop or duplicated steps) — **each pass needs its own restore** (the pipeline currently restores once and builds `--no-restore`; the two passes have different `TargetFrameworks` and package graphs, so `project.assets.json` differs):
   - `dotnet restore /p:UmbracoMajor=17` → `dotnet build /p:UmbracoMajor=17 /p:Version=$(semVersion5)` → `dotnet pack` → `$(Build.ArtifactStagingDirectory)/NuGet/5x`
   - `dotnet restore /p:UmbracoMajor=18` → `dotnet build /p:UmbracoMajor=18 /p:Version=$(semVersion6)` → `dotnet pack` → `.../NuGet/6x`
   - Use separate intermediate output (`/p:BaseIntermediateOutputPath` or `dotnet clean` between passes) so obj/ artefacts don't leak between passes.
3. npm client build once (shared by both passes; runs before either build, as today).
4. Bump `NodeTool@0` from `18.x` to **`24.x`** — `@umbraco-cms/backoffice` 18 declares `engines: node >= 24.13, npm >= 11`.
5. Release stage: tag both versions, push both nupkgs (the existing `**/*.nupkg` glob at line 200 picks up both without change).
6. SDK installs unchanged — .NET 10 SDK is already installed and builds both passes (prototype-verified with 10.0.302).

---

## 8. Phase 6 — Schemas, docs, changelog, marketplace

1. Regenerate from an Umbraco 18 site (per-major ritual, dev-time IntelliSense only):
   - `src/Enterspeed.Source.UmbracoCms.V14Plus/appsettings-schema.Umbraco.Cms.json`
   - `src/Enterspeed.Source.UmbracoCms.V14Plus/umbraco-package-schema.json`
2. `CHANGELOG-Enterspeed.Source.UmbracoCms.md`: `## [6.0.0] — Added Umbraco 18 support` + note that 5.x continues to serve Umbraco 14–17.
3. README + README-NUGET: version matrix (4.x → Umbraco 9–13, 5.x → 14–17, 6.x → 18+).
4. Umbraco Marketplace JSON: no per-version fields — no change needed; review description text mentions.

---

## 9. Phase 7 — Verification

There is **no automated test coverage for V14Plus** (the legacy test project references a renamed path, targets net6, and is in neither solution), so verification is manual against real sites. Checklist, run on **Umbraco 18.0.2** and regression-run on **17** (and ideally 14, the floor of the 5.x line):

- [ ] Package installs; site boots; `EnterspeedJobs` table migration plan runs (fresh DB **and** upgrade from a 5.x-provisioned DB — the key-value migration state must carry over across the sync→async migration rewrite).
- [ ] Dashboard loads in Content and Settings sections; save configuration; test connection.
- [ ] Seed (full + custom — **custom seed with dictionary nodes specifically**, exercising the reworked int→Guid dictionary lookup, §4.2), pending/failed job counts, clear/delete jobs.
- [ ] Publish, unpublish, move to recycle bin (content); variants across two cultures.
- [ ] Dictionary item create/save/delete flows.
- [ ] Media save/move/trash flows; media URLs with `MediaDomain` configured.
- [ ] Property converters spot-check: block list, block grid, rich text (local links + images), multi-URL picker, media picker 3, content picker, nested content, **member group picker** (reworked call, §4.5).
- [ ] Domain/culture URL resolution on a multi-domain site (reworked `GetAssignedDomainsAsync` call, §4.5).
- [ ] Culture-specific republish jobs are enqueued (exercises the reflection probe, confirmed present in 18 but verify end-to-end).
- [ ] Hosted services process the queue on a scheduling-publisher/single server role.
- [ ] `/umbraco/openapi/enterspeed.json` serves the document on 18 (dev mode); `/umbraco/swagger/enterspeed/swagger.json` still works on 17 (5.x pass); ApiExplorer "Dashboard" group appears in the 18 document.

Follow-up (out of scope for this story, worth its own): resurrect an automated test project against the V14Plus solution so future majors aren't verified purely by hand.

---

## 10. Risks & mitigations

| Risk | Mitigation |
|---|---|
| Undocumented binary/API changes in 18 beyond the researched list | **Retired** — full compile prototype against 18.0.1 done; complete two-wave error list captured (22 errors / 13 files) and every item is scoped in §1/§4/§5 |
| `@hey-api/openapi-ts` output shape differs from old generator, breaking the dashboard repository layer | Regeneration is followed by `tsc` + dashboard smoke tests on all hosts; the client is internal to the repo |
| Int-keyed dictionary custom-seed lookup (§4.2) regresses | Dedicated checklist item; prefer carrying the Guid through `CustomSeedNode` over `IIdKeyMap` |
| Two-pass build leaks intermediates between passes | Restore per pass + separate `BaseIntermediateOutputPath` (or clean between) |
| Users on Umbraco 17 upgrade to 6.x by accident | Closed version range `[18.0.2, 19.0.0)` makes restore fail loudly on 17; README matrix; changelog note |
| npm caret pulls the 18.1 RC of `@umbraco-cms/backoffice` | Pin exact `18.0.2` |
| Umbraco 19 (Q4 2026) removes the obsolete APIs on the watch list (§1) | Park explicitly; `UMBRACO_18_OR_GREATER` convention already composes for a 19 symbol |

---

## 11. Why we are making these changes

1. **The package hard-crashes on Umbraco 18 today.** Version 18 deleted the obsolete APIs we depend on — compile-verified as 22 errors across 13 files: `MigrationBase`, `ILocalizationService`, `IComponent`, `Upgrader.Execute`, two sync service methods, the entire Swashbuckle/OpenAPI stack, and `IPublishedContent.Parent`. Without these changes an 18 site fails at startup: DI cannot resolve our handlers, the database migration plan cannot run, and our types fail to load. This is mandatory API migration, not a version bump.
2. **Umbraco 17 and 18 both target .NET 10, which breaks our TFM-per-major packaging model.** Every previous major got its own target framework; that mechanism can no longer distinguish the two. The `UmbracoMajor` build dimension + a new 6.x package line is the only way to ship an 18-compatible assembly while continuing to deliver fixes to Umbraco 17 LTS customers — the majority of the install base — from a single branch with no cherry-picking. Because every replacement API except OpenAPI is signature-identical in 17 and 18, the code divergence stays down to a single conditional surface.
3. **The backoffice dashboard and its generated API client must keep working against the new OpenAPI pipeline.** Umbraco 18 moved the API document endpoint, switched to OpenAPI 3.1 (which our current generator cannot parse), and disables documents in production by default; our TypeScript client is generated from that document and has only ever been type-checked against backoffice 14. Re-registering the document via the new API, switching generators, and type-checking against the 18 packages is what keeps the dashboard (seed, configuration, job management) functional — the one surface where our runtime shims cannot compensate.

---

## 12. Effort estimate

| Work | Estimate |
|---|---|
| Phase 1 build plumbing + local two-pass verification | 0.5–1 day |
| Phase 2 shared code migrations (incl. component/upgrader + dictionary int→Guid rework) | 1–1.5 days |
| Phase 2b Umbraco 18 feature support: ElementPicker + SingleBlock converters, element reingest fan-out (§4.6) | 2–3 days |
| Phase 3 OpenAPI split | 0.5–1 day (replacement APIs now mapped exactly) |
| Phase 4 client: generator switch + regeneration + type-check | 0.5–1 day |
| Phase 5 CI/CD | 0.5–1 day |
| Phases 6–7 schemas, docs, manual verification on 17 + 18 | 1–1.5 days |
| **Total** | **~7–10 days** (story sc-10222 includes the Elements scope; consider splitting baseline compatibility from Elements support, as the story's INVEST notes suggest) |

---

## Appendix — validation record (31 July 2026)

The plan was validated by three independent passes:

1. **Compile prototype**: the codebase was built against Umbraco 18.0.1 (net10.0, .NET SDK 10.0.302) in a throwaway worktree. All four originally predicted break categories confirmed; four additional breaks discovered (`IComponent`, `Upgrader.Execute`, `IMemberGroupService.GetByIds`, `IDomainService.GetAssignedDomains`) and folded in. Shim-assisted rebuild proved the 22-error list is exhaustive.
2. **Codebase inventory audit**: every file path/line cited was verified exact; completeness sweep surfaced the int-keyed dictionary lookup, the composer `using` gap, the dual-constructor requirement for V9Plus-shared files, and the per-pass restore requirement.
3. **Source/release verification**: all replacement APIs diffed between the `release-17.0.0` and `release-18.0.2` tags; 18.0.1/18.0.2/18.1.0-rc release notes swept (no new breaking changes); OpenAPI replacement surface, codegen tooling, and Node/npm requirements confirmed against Umbraco's own v18 extension template and npm metadata.
