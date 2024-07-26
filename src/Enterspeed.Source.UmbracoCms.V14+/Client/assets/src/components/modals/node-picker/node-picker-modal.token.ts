import { UmbModalToken } from "@umbraco-cms/backoffice/modal";

export type MyModalData = {
  treeAlias: string;
  headline: string;
};

export type MyModalValue = {
  seedNodes: Array<SeedNode>;
  treeAlias: string;
};

export const ENTERSPEED_NODEPICKER_MODAL_TOKEN = new UmbModalToken<
  MyModalData,
  MyModalValue
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
