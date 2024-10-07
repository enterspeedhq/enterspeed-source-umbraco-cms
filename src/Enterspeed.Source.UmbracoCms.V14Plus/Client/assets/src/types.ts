import { EnterspeedUniqueItemModel } from "./components/modals/node-picker/node-picker-modal.token";
import { EnterspeedJob } from "./generated";

export interface EnterspeedFailedJob extends EnterspeedJob {
  selected: boolean;
}

export type tab = {
  alias: string;
  label: string;
  active?: boolean;
};

export type customNodesSelected = {
  documentNodes: Array<EnterspeedUniqueItemModel>;
  dictionaryNodes: Array<EnterspeedUniqueItemModel>;
  mediaNodes: Array<EnterspeedUniqueItemModel>;
};
