import { UmbDictionaryItemModel } from "@umbraco-cms/backoffice/dictionary";
import { UmbDocumentItemModel } from "@umbraco-cms/backoffice/document";
import { UmbMediaItemModel } from "@umbraco-cms/backoffice/media";
import { UmbModalToken } from "@umbraco-cms/backoffice/modal";

export type NodePickerData = {
  treeAlias: string;
  headline: string;
};

export class NodePickerValue {
  documentNodes: Array<UmbDocumentItemModel> = new Array<UmbDocumentItemModel>();
  mediaNodes: Array<UmbMediaItemModel> = new Array<UmbMediaItemModel>();
  dictionaryNodes: Array<UmbDictionaryItemModel> = new Array<UmbDictionaryItemModel>();
  treeAlias: string = "";
  includeDescendants: boolean = false;
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
