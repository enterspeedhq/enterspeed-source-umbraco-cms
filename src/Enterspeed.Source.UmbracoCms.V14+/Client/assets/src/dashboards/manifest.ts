import type { ManifestDashboard } from "@umbraco-cms/backoffice/extension-registry";

const dashboards: Array<ManifestDashboard> = [
  {
    type: "dashboard",
    name: "Enterspeed Jobs",
    alias: "enterspeed_jobs",
    elementName: "enterspeed-jobs",
    js: () => import("./enterspeed-jobs.js"),
    weight: -10,
    meta: {
      label: "Enterspeed Jobs",
      pathname: "Enterspeed.Jobs",
    },
    conditions: [
      {
        alias: "Umb.Condition.SectionAlias",
        match: "Umb.Section.Content",
      },
    ],
  },
];

export const manifests = [...dashboards];
