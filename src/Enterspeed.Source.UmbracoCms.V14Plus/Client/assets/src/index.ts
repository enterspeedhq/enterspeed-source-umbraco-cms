import { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";
import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { client } from "./generated/client.gen.ts";

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
    client.setConfig({
      baseUrl: umbOpenApi.base ?? "",
      auth: async () => (await umbOpenApi.token()) ?? "",
      // Backoffice 14-17 exposes withCredentials (boolean); 18 exposes the fetch
      // credentials option directly. The bundle serves all hosts at runtime.
      credentials:
        "withCredentials" in umbOpenApi
          ? (umbOpenApi as { withCredentials: boolean }).withCredentials
            ? "include"
            : "same-origin"
          : umbOpenApi.credentials ?? "same-origin",
    });
  });
};
