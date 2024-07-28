import { UmbModalToken } from "@umbraco-cms/backoffice/modal";

export type NodePickerData = {
  treeAlias: string;
  headline: string;
};

export type NodePickerValue = {
  seedNodes: Array<SeedNode>;
  treeAlias: string;
};

export const ENTERSPEED_NODEPICKER_MODAL_TOKEN = new UmbModalToken<
  NodePickerData,
  NodePickerValue
>("Enterspeed.NodePicker.Modal", {
  modal: {
    type: "sidebar",
    size: "small",
  },
});

export type SeedNode = {
  id: string;
  Descendentants: boolean;
};
