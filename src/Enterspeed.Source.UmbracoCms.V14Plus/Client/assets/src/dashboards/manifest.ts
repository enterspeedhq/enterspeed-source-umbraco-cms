import type { ManifestDashboard } from "@umbraco-cms/backoffice/extension-registry";

const dashboards: Array<ManifestDashboard> = [
  {
    type: "dashboard",
    name: "Enterspeed Jobs",
    alias: "enterspeedDashboard",
    elementName: "enterspeedDashboard",
    js: () => import("./jobs/dashboard.element.js"),
    weight: -10,
    meta: {
      label: "Enterspeed Jobs",
      pathname: "enterspeed-jobs",
    },
    conditions: [
      {
        alias: "Umb.Condition.SectionAlias",
        match: "Umb.Section.Content",
      },
    ],
  },
  {
    type: "dashboard",
    name: "Enterspeed Settings",
    alias: "enterspeedSettingsDashboard",
    elementName: "enterspeedSettingsDashboard",
    js: () => import("./settings/settings-dashboard.js"),
    weight: -10,
    meta: {
      label: "Enterspeed Settings",
      pathname: "enterspeed-settings",
    },
    conditions: [
      {
        alias: "Umb.Condition.SectionAlias",
        match: "Umb.Section.Settings",
      },
    ],
  },
];

export const manifests = [...dashboards];
