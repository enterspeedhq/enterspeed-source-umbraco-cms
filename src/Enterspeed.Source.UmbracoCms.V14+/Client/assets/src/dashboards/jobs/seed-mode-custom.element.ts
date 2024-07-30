import { css, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import {
  ENTERSPEED_NODEPICKER_MODAL_TOKEN,
  EnterspeedUniqueItemModel,
} from "../../components/modals/node-picker/node-picker-modal.token";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import { UMB_DICTIONARY_TREE_ALIAS } from "@umbraco-cms/backoffice/dictionary";
import { UMB_MEDIA_TREE_ALIAS } from "@umbraco-cms/backoffice/media";
import { UMB_DOCUMENT_TREE_ALIAS } from "@umbraco-cms/backoffice/document";
import { repeat } from "lit/directives/repeat.js";

@customElement("enterspeed-seed-mode-custom")
export class enterspeedCustomSeedModeElement extends UmbLitElement {
  #modalManagerContext?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

  @state()
  documentNodes: Array<EnterspeedUniqueItemModel> =
    new Array<EnterspeedUniqueItemModel>();
  @state()
  mediaNodes: Array<EnterspeedUniqueItemModel> =
    new Array<EnterspeedUniqueItemModel>();
  @state()
  dictionaryNodes: Array<EnterspeedUniqueItemModel> =
    new Array<EnterspeedUniqueItemModel>();

  @property({ type: Array })
  selectedContentIds!: string[];

  @property({ type: Array })
  selectedMediaIds!: string[];

  @property({ type: Array })
  selectedDictionaryIds!: string[];

  constructor() {
    super();
    this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
      this.#modalManagerContext = instance;
    });
  }

  render() {
    return html`
      <div class="seed-dashboard-text">
        <div>
          <h4>Custom seed</h4>
          <p>
            With a custom seed you can select the nodes you want to seed for all
            cultures and publish and preview (if configured). This action can
            take a while to finish.
          </p>
          <p>
            <i
              >The job queue length is the queue length on Umbraco before the
              nodes are ingested into Enterspeed.</i
            >
          </p>
        </div>
        <div class="custom-seed-content-type-container">
          ${this.#renderCustomSeedContentTypeBox(
            this.documentNodes,
            "Content",
            UMB_DOCUMENT_TREE_ALIAS
          )}
          ${this.#renderCustomSeedContentTypeBox(
            this.mediaNodes,
            "Media",
            UMB_MEDIA_TREE_ALIAS
          )}
          ${this.#renderCustomSeedContentTypeBox(
            this.dictionaryNodes,
            "Dictionary",
            UMB_DICTIONARY_TREE_ALIAS
          )}
        </div>
      </div>
    `;
  }

  #renderCustomSeedContentTypeBox(
    nodes: Array<EnterspeedUniqueItemModel>,
    contentType: string,
    alias: string
  ) {
    return html`<div class="custom-seed-content-type-box">
      ${this.#renderNodes(nodes, contentType)}
      ${this.#renderTreeNodeButtons(alias)}
    </div>`;
  }

  #renderTreeNodeButtons(alias: string) {
    return html`
      <uui-button
        look="placeholder"
        label="Choose"
        class="full-width-btn"
        @click=${() => this.#openNodePickerModal(alias)}
      ></uui-button>
    `;
  }

  #renderNodes(nodes: EnterspeedUniqueItemModel[], headline: string) {
    return html`
      <h5>${headline}</h5>
      <uui-ref-list>
        ${repeat(
          nodes,
          (item) => item.unique,
          (item) => this.#renderItem(item)
        )}
      </uui-ref-list>
    `;
  }

  #renderItem(item: EnterspeedUniqueItemModel) {
    if (!item.unique) return;
    return html`
      <uui-ref-node name=${item.name} id=${item.unique}>
        <uui-action-bar slot="actions">
          <uui-button
            @click=${() => this.#removeNode(item)}
            label=${this.localize.term("general_remove")}
          ></uui-button>
        </uui-action-bar>
      </uui-ref-node>
    `;
  }
  #removeNode(item: EnterspeedUniqueItemModel) {
    let index = this.documentNodes.findIndex((n) => n.unique == item.unique);
    if (index > -1) {
      this.documentNodes.splice(index, 1);
      super.requestUpdate("documentNodes");
    }

    index = this.dictionaryNodes.findIndex((n) => n.unique == item.unique);
    if (index > -1) {
      this.dictionaryNodes.splice(index, 1);
      super.requestUpdate("dictionaryNodes");
    }

    index = this.mediaNodes.findIndex((n) => n.unique == item.unique);
    if (index > -1) {
      this.mediaNodes.splice(index, 1);
      super.requestUpdate("mediaNodes");
    }

    this.#onDataUpdated();
  }

  async #openNodePickerModal(treeAlias: string) {
    let headline = "";
    switch (treeAlias) {
      case UMB_DICTIONARY_TREE_ALIAS:
        headline = "Select dictionary node";
        break;
      case UMB_DOCUMENT_TREE_ALIAS:
        headline = "Select content node";
        break;
      case UMB_MEDIA_TREE_ALIAS:
        headline = "Select media node";
    }

    var modal = this.#modalManagerContext?.open(
      this,
      ENTERSPEED_NODEPICKER_MODAL_TOKEN,
      {
        data: {
          treeAlias: treeAlias,
          headline: headline,
        },
      }
    );

    await modal?.onSubmit().then((data) => {
      switch (data.treeAlias) {
        case UMB_DOCUMENT_TREE_ALIAS:
          this.documentNodes = this.#mapNodes(
            this.documentNodes,
            data.nodes
          );
          super.requestUpdate("documentNodes");
          break;
        case UMB_DICTIONARY_TREE_ALIAS:
          this.dictionaryNodes = this.#mapNodes(
            this.dictionaryNodes,
            data.nodes
          );
          super.requestUpdate("dictionaryNodes");
          break;
        case UMB_MEDIA_TREE_ALIAS:
          this.mediaNodes = this.#mapNodes(this.mediaNodes, data.nodes);
          super.requestUpdate("mediaNodes");
          break;
      }
      this.#onDataUpdated();
    });
  }

  #onDataUpdated() {
    this.dispatchEvent(
      new CustomEvent("custom-nodes-selected", {
        bubbles: true,
        composed: true,
        detail:
          this.mediaNodes.length ||
          this.documentNodes.length ||
          this.dictionaryNodes.length,
      })
    );
  }

  #mapNodes(
    existingNodes: EnterspeedUniqueItemModel[],
    nodesToAdd: EnterspeedUniqueItemModel[]
  ): Array<EnterspeedUniqueItemModel> {
    for (let index = 0; index < nodesToAdd.length; index++) {
      const nodeToAdd = nodesToAdd[index];
      let existingIndex = existingNodes.findIndex(
        (e) => e.name == nodeToAdd.name
      );

      if (existingIndex > 0) {
        existingNodes.splice(existingIndex, 1);
      }
      
      if(nodeToAdd.name === "Everything") {
        existingNodes = [];
      }

      existingNodes.push(nodeToAdd);
    }

    return existingNodes;
  }

  static styles = css`
    .custom-seed-content-type-container {
      display: flex;
      margin-bottom: 20px;
    }

    .custom-seed-content-type-box {
      flex: 1;
      margin: 0 5px 0 5px;
      padding: 0 10px 10px 10px;
      border: solid #e9e9ec 1px;
      border-radius: 3px;
      box-shadow: 0px 1px 1px 0px rgba(0, 0, 0, 0.16);
    }

    .full-width-btn {
      width: 100%;
    }
  `;
}

export default enterspeedCustomSeedModeElement;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-custom-seed-mode": enterspeedCustomSeedModeElement;
  }
}
