import { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { OpenAPI } from "./generated/core/OpenAPI.ts";

// load up the manifests here.
import { manifests as dashboardManifests } from "./dashboards/manifest.ts";
import { manifests as modalManifests } from "./components/modals/manifests";

const manifests: Array<UmbExtensionManifest> = [
  ...dashboardManifests,
  ...modalManifests,
];

export const onInit: UmbEntryPointOnInit = (_host, extensionRegistry) => {
  // register them here.
  extensionRegistry.registerMany(manifests);

  _host.consumeContext(UMB_AUTH_CONTEXT, (_auth) => {
    if (!_auth) {
      return;
    }

    const umbOpenApi = _auth.getOpenApiConfiguration();
    OpenAPI.TOKEN = async () => (await umbOpenApi.token()) ?? "";
    OpenAPI.BASE = umbOpenApi.base ?? "";
    // Backoffice 14-17 exposes withCredentials (boolean); 18 replaced it with
    // the fetch credentials option. The bundle serves all hosts at runtime.
    OpenAPI.WITH_CREDENTIALS =
      "withCredentials" in umbOpenApi
        ? (umbOpenApi as { withCredentials: boolean }).withCredentials
        : umbOpenApi.credentials === "include";
  });
};
