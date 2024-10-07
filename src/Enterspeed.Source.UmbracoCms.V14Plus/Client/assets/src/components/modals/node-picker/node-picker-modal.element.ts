import {
  html,
  LitElement,
  property,
  customElement,
  state,
  css,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import type { UmbModalContext } from "@umbraco-cms/backoffice/modal";
import {
  NodePickerData,
  NodePickerValue,
  EnterspeedUniqueItemModelImpl,
} from "./node-picker-modal.token";
import { UmbModalExtensionElement } from "@umbraco-cms/backoffice/extension-registry";
import {
  UMB_DOCUMENT_TREE_ALIAS,
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
  UmbMediaItemRepository,
} from "@umbraco-cms/backoffice/media";
import {
  UMB_DICTIONARY_TREE_ALIAS,
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
  #documentType = "";

  @property({ attribute: false })
  modalContext!: UmbModalContext<NodePickerData, NodePickerValue>;

  #nodePickerValue?: NodePickerValue;
  #includeDescendants: boolean = false;

  @state()
  private selectionConfiguration: UmbTreeSelectionConfiguration = {
    multiple: true,
    selectable: true,
    selection: [],
  };

  @state()
  includeAllContentNodes: boolean = false;

  constructor() {
    super();
    this.#nodePickerValue = new NodePickerValue();
  }

  render() {
    switch (this.modalContext.data.treeAlias) {
      case UMB_MEDIA_TREE_ALIAS:
        this.#documentType = "media";
        break;
      case UMB_DICTIONARY_TREE_ALIAS:
        this.#documentType = "dictionary";
        break;
      case UMB_DOCUMENT_TREE_ALIAS:
        this.#documentType = "document";
        break;
    }

    return html`
      <umb-body-layout headline=${this.modalContext.data.headline}>
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
          <div .hidden=${this.includeAllContentNodes}>
              <umb-property-layout 
              label="Include descendants"
              orientation="vertical"
              ><div slot="editor">
                <uui-toggle 
                @change=${(e: UUIBooleanInputEvent) => {
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
                hideTreeRoot: true,
              }}
              @selected=${this.#onSelected}
              @deselected=${this.#onDeselected}></umb-tree>
            </umb-tree>
          </div>
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

  #handleCancel() {
    this.modalContext?.submit();
  }

  #handleSubmit() {
    if (
      this.#nodePickerValue != null &&
      this.modalContext?.data.treeAlias != null
    ) {
      this.#nodePickerValue.treeAlias = this.modalContext?.data.treeAlias;
    }
    if (this.#nodePickerValue != null) {
      this.modalContext?.setValue(this.#nodePickerValue);
    }

    this.modalContext?.submit();
  }

  async #onSelected(event: UmbSelectedEvent) {
    const treeElement = event.target as UmbTreeElement;

    let selection = treeElement.getSelection();
    if (selection.length) {
      await this.#handleNodes(selection);
    }
    event.stopPropagation();
  }

  #onDeselected(event: UmbDeselectedEvent) {
    if (this.#nodePickerValue == null || this.modalContext == null) return;

    if (event.unique) {
      this.#nodePickerValue.nodes = this.#nodePickerValue.nodes?.filter(
        (node) => event.unique != node.unique
      );
    }

    event.stopPropagation();
  }

  #onSelectAllContentNodes(event: UUIBooleanInputEvent) {
    this.#nodePickerValue!.includeAllContentNodes = event.target.checked;
    this.includeAllContentNodes = event.target.checked;
    this.#nodePickerValue!.nodes = [];

    if (this.#nodePickerValue!.includeAllContentNodes) {
      this.#nodePickerValue?.nodes.push(
        new EnterspeedUniqueItemModelImpl(
          false,
          "Everything",
          "Everything",
          "icon-thumbnail-list",
          this.#documentType
        )
      );
    }
  }

  #onIncludeDescendants(event: UUIBooleanInputEvent) {
    this.#includeDescendants = event.target.checked;
    this.#nodePickerValue?.nodes.forEach((element) => {
      element.includeDescendants = event.target.checked;
    });
  }

  async #handleNodes(selection: string[]) {
    if (this.modalContext?.data.treeAlias == UMB_DOCUMENT_TREE_ALIAS) {
      let nodes = (await this.#documentRepository.requestItems(selection)).data;

      if (nodes != null) {
        for (let node of nodes) {
          if (node != null) {
            let mappedNode = new EnterspeedUniqueItemModelImpl(
              this.#includeDescendants,
              node.unique,
              node.name,
              node.documentType.icon,
              this.#documentType
            );
            this.#nodePickerValue?.nodes?.push(mappedNode);
          }
        }
      }
    } else if (this.modalContext?.data.treeAlias == UMB_MEDIA_TREE_ALIAS) {
      let nodes = (await this.#mediaItemRepository.requestItems(selection))
        .data;

      if (nodes != null) {
        for (let node of nodes) {
          if (node != null) {
            let mappedNode = new EnterspeedUniqueItemModelImpl(
              this.#includeDescendants,
              node.unique,
              node.name,
              node.mediaType.icon,
              this.#documentType
            );
            this.#nodePickerValue?.nodes?.push(mappedNode);
          }
        }
      }
    } else if (this.modalContext?.data.treeAlias == UMB_DICTIONARY_TREE_ALIAS) {
      let nodes = (await this.#dictionaryRepository.requestItems(selection))
        .data;

      if (nodes != null) {
        for (let node of nodes) {
          if (node != null) {
            let mappedNode = new EnterspeedUniqueItemModelImpl(
              this.#includeDescendants,
              node.unique,
              node.name,
              "icon-book-alt",
              this.#documentType
            );
            this.#nodePickerValue?.nodes?.push(mappedNode);
          }
        }
      }
    }
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
