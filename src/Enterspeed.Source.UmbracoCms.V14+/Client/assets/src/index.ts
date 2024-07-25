import { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { ManifestTypes } from "@umbraco-cms/backoffice/extension-registry";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { OpenAPI } from "./generated/core/OpenAPI.ts";

// load up the manifests here.
import { manifests as dashboardManifests } from "./dashboards/manifest.ts";
import { manifests as modalManifests } from "./components/modals/manifests";

const manifests: Array<ManifestTypes> = [
  ...dashboardManifests,
  ...modalManifests,
];

export const onInit: UmbEntryPointOnInit = (_host, extensionRegistry) => {
  // register them here.
  extensionRegistry.registerMany(manifests);

  _host.consumeContext(UMB_AUTH_CONTEXT, (_auth) => {
    const umbOpenApi = _auth.getOpenApiConfiguration();
    OpenAPI.TOKEN = umbOpenApi.token;
    OpenAPI.BASE = umbOpenApi.base;
    OpenAPI.WITH_CREDENTIALS = umbOpenApi.withCredentials;
  });
};
