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
import type { MyModalData, MyModalValue } from "./node-picker-modal.token";
import { UmbModalExtensionElement } from "@umbraco-cms/backoffice/extension-registry";
import { UMB_DOCUMENT_TREE_ALIAS } from "@umbraco-cms/backoffice/document";
import { UmbTreeSelectionConfiguration } from "@umbraco-cms/backoffice/tree";
import {
  UmbDeselectedEvent,
  UmbSelectedEvent,
  UmbSelectionChangeEvent,
} from "@umbraco-cms/backoffice/event";
import { UUIBooleanInputEvent } from "@umbraco-cms/backoffice/external/uui";

@customElement("enterspeed-node-picker-modal")
export default class EnterspeedNodePickerModal
  extends UmbElementMixin(LitElement)
  implements UmbModalExtensionElement<MyModalData, MyModalValue>
{
  @property({ attribute: false })
  modalContext?: UmbModalContext<MyModalData, MyModalValue>;

  @property({ attribute: false })
  data?: MyModalData;

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
    this.modalContext?.updateValue({ treeAlias: this.data?.treeAlias });
    this.modalContext?.submit();
  }

  #onTreeSelectionChange(event: UmbSelectionChangeEvent) {
    event.stopPropagation();
  }

  #onSelected(event: UmbSelectedEvent) {
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
      <umb-body-layout headline=${ifDefined(this.data?.headline)}>
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
            alias=${this.data?.treeAlias ?? UMB_DOCUMENT_TREE_ALIAS}
            .props=${{
              hideTreeItemActions: true,
              selectionConfiguration: this.selectionConfiguration,
            }}
            @selection-change=${this.#onTreeSelectionChange}
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
