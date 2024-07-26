import { UmbModalToken } from "@umbraco-cms/backoffice/modal";

export type MyModalData = {
    treeAlias: string;
}

export type MyModalValue = {
    myData: string;
}

export const ENTERSPEED_NODEPICKER_MODAL_TOKEN = new UmbModalToken<MyModalData, MyModalValue>('Enterspeed.NodePicker.Modal', {
    modal: {
        type: 'sidebar',
        size: 'small'
    }
});