# Local demo sites & TypeScript client generation

The demo/test sites live **outside this repo** in a sibling folder
(`../enterspeed-umbraco-demo`) — see its README for the full workflow. Summary:

- Two sites installed from a local NuGet feed fed by both pack passes:
  - **Umbraco 18.0.2** + `6.999.0-local` (UmbracoMajor=18 pass) on `http://localhost:21142`
  - **Umbraco 17.5.3** + `5.999.0-local` (UmbracoMajor=17 pass) on `http://localhost:21143`
- `pack-local.sh` in the demo folder rebuilds the client bundle, packs both passes
  and clears the NuGet cache so a site restart picks up fresh bits.

To recreate the setup from scratch: `dotnet new install Umbraco.Templates::<version>`,
then `dotnet new umbraco -n <Name> --friendly-name "Demo Admin" --email admin@example.com
--password <local-only password> --development-database-type SQLite`, add a `NuGet.config`
with a local `packages/` source, and `dotnet add package Enterspeed.Source.UmbracoCms
--version <x>.999.0-local`.

## Generating the TypeScript client

From `src/Enterspeed.Source.UmbracoCms.V14Plus/Client/assets`, against a **running** demo site:

| Script | Source document | Umbraco |
|---|---|---|
| `npm run generate` | `/umbraco/openapi/enterspeed.json` (OpenAPI 3.1) | 18 — canonical |
| `npm run generate:u17` | `/umbraco/swagger/enterspeed/swagger.json` (OpenAPI 3.0) | 17 — fallback |

Both documents generate an **identical client surface**: the 17-pass swagger
configuration mimics Umbraco 18's schema/operation-id naming
(`ApiResponseOfSeedResponse`, `PostClearPendingJobs`), so the generated types and
SDK functions match regardless of which site they were generated from. Prefer the
18 document — the 17 one additionally contains .NET reflection schemas leaked via
an `Exception`-shaped response model.

Note: the Enterspeed swagger document on Umbraco 17.5.x requires package 5.3.7+ —
older versions 500 on `swagger.json` because the custom schema-id selector lost the
DI registration race against Umbraco's own (fixed via `IPostConfigureOptions`).
