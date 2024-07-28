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
import type {
  NodePickerData,
  NodePickerValue,
  SeedNode,
} from "./node-picker-modal.token";
import { UmbModalExtensionElement } from "@umbraco-cms/backoffice/extension-registry";
import { UMB_DOCUMENT_TREE_ALIAS } from "@umbraco-cms/backoffice/document";
import {
  UmbTreeElement,
  UmbTreeSelectionConfiguration,
} from "@umbraco-cms/backoffice/tree";
import {
  UmbDeselectedEvent,
  UmbSelectedEvent,
} from "@umbraco-cms/backoffice/event";
import { UUIBooleanInputEvent } from "@umbraco-cms/backoffice/external/uui";

@customElement("enterspeed-node-picker-modal")
export default class EnterspeedNodePickerModal
  extends UmbElementMixin(LitElement)
  implements UmbModalExtensionElement<NodePickerData, NodePickerValue>
{
  @property({ attribute: false })
  modalContext?: UmbModalContext<NodePickerData, NodePickerValue>;

  @property({ attribute: false })
  nodePickerData?: NodePickerData;

  @property({ attribute: false })
  nodePickerValue?: NodePickerValue;

  @state()
  private selectionConfiguration: UmbTreeSelectionConfiguration = {
    multiple: false,
    selectable: true,
    selection: [],
  };

  @state()
  private includeDescendants: boolean;

  #handleCancel() {
    this.modalContext?.submit();
  }

  #handleSubmit() {
    this.modalContext?.updateValue({
      treeAlias: "this.nodePickerData?.treeAlias,"
    });
    console.log("submitting");
    this.modalContext?.submit();
  }

  #onSelected(event: UmbSelectedEvent) {
    console.log(event);
    const element = event.target as UmbTreeElement;
    
    event.stopPropagation();
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
      <umb-body-layout headline=${ifDefined(this.nodePickerData?.headline)}>
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
            alias=${this.nodePickerData?.treeAlias ?? UMB_DOCUMENT_TREE_ALIAS}
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
