import { UmbModalToken } from "@umbraco-cms/backoffice/modal";
import { UmbUniqueItemModel } from "@umbraco-cms/backoffice/models";

export type NodePickerData = {
  treeAlias: string;
  headline: string;
};

export class NodePickerValue {
  nodes: Array<EnterspeedUniqueItemModel> =
    new Array<EnterspeedUniqueItemModel>();
  treeAlias: string = "";
  includeAllContentNodes = false;
}

export const ENTERSPEED_NODEPICKER_MODAL_TOKEN = new UmbModalToken<
  NodePickerData,
  NodePickerValue
>("Enterspeed.NodePicker.Modal", {
  modal: {
    type: "sidebar",
    size: "small",
  },
});

export class EnterspeedUniqueItemModelImpl
  implements EnterspeedUniqueItemModel
{
  constructor(
    includeDescendants: boolean,
    unique: string,
    name: string,
  ) {
    this.includeDescendants = includeDescendants;
    this.unique = unique;
    this.name = name;
  }

  includeDescendants: boolean;
  unique: string;
  name: string;
  icon?: string | undefined;
}

export interface EnterspeedUniqueItemModel extends UmbUniqueItemModel {
  includeDescendants: boolean;
}
