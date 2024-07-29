import { UmbDictionaryItemModel } from "@umbraco-cms/backoffice/dictionary";
import { UmbDocumentItemModel } from "@umbraco-cms/backoffice/document";
import { UmbMediaItemModel } from "@umbraco-cms/backoffice/media";
import { UmbModalToken } from "@umbraco-cms/backoffice/modal";

export type NodePickerData = {
  treeAlias: string;
  headline: string;
};

export class NodePickerValue {
  documentNodes: Array<UmbDocumentItemModel> | undefined;
  mediaNodes: Array<UmbMediaItemModel> | undefined;
  dictionaryNodes: Array<UmbDictionaryItemModel> | undefined;
  treeAlias: string | undefined;
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
