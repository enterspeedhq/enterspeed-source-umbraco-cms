import type { ManifestModal } from "@umbraco-cms/backoffice/modal";

const modals: Array<ManifestModal> = [
  {
    type: "modal",
    alias: "Enterspeed.NodePicker.Modal",
    name: "Enterspeed Node Picker Modal",
    js: () => import("./node-picker-modal.element.js"),
  },
];

export const manifests = [...modals];
