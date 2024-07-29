import {
  html,
  LitElement,
  property,
  customElement,
  state,
  css,
  ifDefined,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import type { UmbModalContext } from "@umbraco-cms/backoffice/modal";
import { NodePickerData, NodePickerValue } from "./node-picker-modal.token";
import { UmbModalExtensionElement } from "@umbraco-cms/backoffice/extension-registry";
import {
  UMB_DOCUMENT_TREE_ALIAS,
  UmbDocumentItemModel,
  UmbDocumentItemRepository,
} from "@umbraco-cms/backoffice/document";
import {
  UmbTreeElement,
  UmbTreeSelectionConfiguration,
} from "@umbraco-cms/backoffice/tree";
import {
  UmbDeselectedEvent,
  UmbSelectedEvent,
} from "@umbraco-cms/backoffice/event";
import { UUIBooleanInputEvent } from "@umbraco-cms/backoffice/external/uui";
import {
  UMB_MEDIA_TREE_ALIAS,
  UmbMediaItemModel,
  UmbMediaItemRepository,
} from "@umbraco-cms/backoffice/media";
import {
  UMB_DICTIONARY_TREE_ALIAS,
  UmbDictionaryItemModel,
  UmbDictionaryItemRepository,
} from "@umbraco-cms/backoffice/dictionary";

@customElement("enterspeed-node-picker-modal")
export default class EnterspeedNodePickerModal
  extends UmbElementMixin(LitElement)
  implements UmbModalExtensionElement<NodePickerData, NodePickerValue>
{
  #documentRepository = new UmbDocumentItemRepository(this);
  #mediaItemRepository = new UmbMediaItemRepository(this);
  #dictionaryRepository = new UmbDictionaryItemRepository(this);

  @property({ attribute: false })
  modalContext?: UmbModalContext<NodePickerData, NodePickerValue>;

  #nodePickerData?: NodePickerData;
  #nodePickerValue?: NodePickerValue;

  @state()
  private selectionConfiguration: UmbTreeSelectionConfiguration = {
    multiple: false,
    selectable: true,
    selection: [],
  };

  constructor() {
    super();
    this.#nodePickerValue =  new NodePickerValue();;
  }

  #handleCancel() {
    this.modalContext?.submit();
  }

  #handleSubmit() {
    if (this.#nodePickerValue != null) {
      this.modalContext?.setValue(this.#nodePickerValue);
    }

    this.modalContext?.submit();
  }

  async #onSelected(event: UmbSelectedEvent) {
    const treeElement = event.target as UmbTreeElement;

    let selection = treeElement.getSelection();
    if (selection.length) {
      if (this.modalContext?.data.treeAlias === UMB_DOCUMENT_TREE_ALIAS) {
        await this.#handleDocumentNodes(selection);
      } else if (this.modalContext?.data.treeAlias === UMB_MEDIA_TREE_ALIAS) {
        await this.#handleMediaNodes(selection);
      } else if (
        this.modalContext?.data.treeAlias === UMB_DICTIONARY_TREE_ALIAS
      ) {
        await this.#handleDictionaryNodes(selection);
      }
    }

    event.stopPropagation();
  }

  async #handleDocumentNodes(selection: string[]) {
    let nodes = (await this.#documentRepository.requestItems(selection)).data;
    if (nodes != null) {
      for (let node of nodes) {
        if (node != null) {
          this.#nodePickerValue?.documentNodes?.push(node);
        }
      }
    }
  }

  async #handleMediaNodes(selection: string[]) {
    let nodes = (await this.#mediaItemRepository.requestItems(selection)).data;
    if (nodes != null) {
      for (let node of nodes) {
        if (node != null) {
          this.#nodePickerValue?.mediaNodes?.push(node);
        }
      }
    }
  }

  async #handleDictionaryNodes(selection: string[]) {
    let nodes = (await this.#dictionaryRepository.requestItems(selection)).data;
    if (nodes != null) {
      for (let node of nodes) {
        if (node != null) {
          this.#nodePickerValue?.dictionaryNodes?.push(node);
        }
      }
    }
  }

  #onDeselected(event: UmbDeselectedEvent) {
    event.stopPropagation();
  }

  #onSelectAllContentNodes(event: UUIBooleanInputEvent) {
    console.log(event.target.checked);
  }

  #onIncludeDescendants(event: UUIBooleanInputEvent) {
    console.log(event.target.checked);
  }

  render() {
    return html`
      <umb-body-layout headline=${ifDefined(this.#nodePickerData?.headline)}>
        <uui-box>
          <umb-property-layout
            label="Include all content nodes"
            orientation="vertical"
            ><div slot="editor">
              <uui-toggle @change=${(e: any) => {
                this.#onSelectAllContentNodes(e);
              }} ></uui-toggle>
            </div>
          </umb-property-layout>
          <umb-property-layout
            label="Include descendants"
            orientation="vertical"
            ><div slot="editor">
              <uui-toggle @change=${(e: UUIBooleanInputEvent) => {
                this.#onIncludeDescendants(e);
              }}></uui-toggle>
            </div>
          </umb-property-layout>
          <umb-tree
            alias=${
              this.modalContext?.data.treeAlias ?? UMB_DOCUMENT_TREE_ALIAS
            }
            .props=${{
              hideTreeItemActions: true,
              selectionConfiguration: this.selectionConfiguration,
            }}
            @selected=${this.#onSelected}
						@deselected=${this.#onDeselected}></umb-tree>
          </umb-tree>
        </uui-box>

        <div slot="actions">
          <uui-button look="secondary" @click=${this.#handleCancel}
            >Cancel</uui-button
          >
          <uui-button
            look="primary"
            color="positive"
            @click=${this.#handleSubmit}
            >Submit</uui-button
          >
        </div>
      </umb-body-layout>
    `;
  }

  static styles = css`
    uui-button:first-of-type {
      margin-right: 15px;
    }

    umb-property-layout {
      padding: 5px;
    }

    umb-property-layout:last-of-type {
      margin-bottom: 5px;
    }
    h5 {
      margin-top: 0;
      margin-bottom: 0;
    }
  `;
}
