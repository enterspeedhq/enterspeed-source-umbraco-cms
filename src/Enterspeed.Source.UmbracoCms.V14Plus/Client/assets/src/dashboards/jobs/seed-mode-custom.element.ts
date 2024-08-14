import { css, html } from "lit";
import { customElement, property } from "lit/decorators.js";
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
import { CustomSeedModel, CustomSeedNode } from "../../generated";

@customElement("enterspeed-seed-mode-custom")
export class enterspeedCustomSeedModeElement extends UmbLitElement {
  #modalManagerContext?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

  @property({ type: Array })
  documentNodes!: Array<EnterspeedUniqueItemModel>;

  @property({ type: Boolean })
  everythingSelectedDocumentNodes = false;

  @property({ type: Array })
  mediaNodes!: Array<EnterspeedUniqueItemModel>;

  @property({ type: Boolean })
  everythingSelectedMediaNodes = false;

  @property({ type: Array })
  dictionaryNodes!: Array<EnterspeedUniqueItemModel>;

  @property({ type: Boolean })
  everythingSelectedDictionaryNodes = false;

  constructor() {
    super();
    this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
      this.#modalManagerContext = instance;
    });
  }

  willUpdate(changedProperties: any) {
    if (changedProperties.has("documentNodes")) {
      if (this.documentNodes.length == 0) {
        this.everythingSelectedDocumentNodes = false;
      }
    }
    if (changedProperties.has("mediaNodes")) {
      if (this.mediaNodes.length == 0) {
        this.everythingSelectedMediaNodes = false;
      }
    }
    if (changedProperties.has("dictionaryNodes")) {
      if (this.dictionaryNodes.length == 0) {
        this.everythingSelectedDictionaryNodes = false;
      }
    }
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
            UMB_DOCUMENT_TREE_ALIAS,
            this.everythingSelectedDocumentNodes
          )}
          ${this.#renderCustomSeedContentTypeBox(
            this.mediaNodes,
            "Media",
            UMB_MEDIA_TREE_ALIAS,
            this.everythingSelectedMediaNodes
          )}
          ${this.#renderCustomSeedContentTypeBox(
            this.dictionaryNodes,
            "Dictionary",
            UMB_DICTIONARY_TREE_ALIAS,
            this.everythingSelectedDictionaryNodes
          )}
        </div>
      </div>
    `;
  }

  #renderCustomSeedContentTypeBox(
    nodes: Array<EnterspeedUniqueItemModel>,
    contentType: string,
    alias: string,
    everythingSelected: boolean
  ) {
    return html`<div class="custom-seed-content-type-box">
      ${this.#renderNodes(nodes, contentType)}
      ${this.#renderTreeNodeButton(alias, everythingSelected)}
    </div>`;
  }

  #renderTreeNodeButton(alias: string, everythingSelected: boolean) {
    return html`
      <uui-button
        look="placeholder"
        label="Choose"
        class="full-width-btn"
        .disabled=${everythingSelected}
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

    let detailText = "";
    if (item.unique.toLowerCase() != "everything") {
      detailText = item.includeDescendants
        ? "Including descendants"
        : "Excluding descendants";
    }

    return html`
      <uui-ref-node name=${item.name} id=${item.unique} detail=${detailText}>
        <umb-icon slot="icon" name=${item.icon ?? "icon-document"}></umb-icon>
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
    let isEverything = item.unique.toLowerCase() === "everything";

    switch (item.documentType) {
      case "media":
        this.mediaNodes.splice(
          this.mediaNodes.findIndex((n) => n.unique == item.unique),
          1
        );
        if (isEverything) {
          this.everythingSelectedMediaNodes = false;
        }
        break;
      case "dictionary":
        this.dictionaryNodes.splice(
          this.dictionaryNodes.findIndex((n) => n.unique == item.unique),
          1
        );
        if (isEverything) {
          this.everythingSelectedDictionaryNodes = false;
        }
        break;
      case "document":
        this.documentNodes.splice(
          this.documentNodes.findIndex((n) => n.unique == item.unique),
          1
        );
        if (isEverything) {
          this.everythingSelectedDocumentNodes = false;
        }
        break;
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
      if (data == null) return;
      switch (data.treeAlias) {
        case UMB_DOCUMENT_TREE_ALIAS:
          this.documentNodes = this.#mapNodes(this.documentNodes, data.nodes);

          this.everythingSelectedDocumentNodes =
            this.documentNodes.findIndex(
              (n) => n.unique.toLowerCase() === "everything"
            ) >= 0;
          break;
        case UMB_DICTIONARY_TREE_ALIAS:
          this.dictionaryNodes = this.#mapNodes(
            this.dictionaryNodes,
            data.nodes
          );

          this.everythingSelectedDictionaryNodes =
            this.dictionaryNodes.findIndex(
              (n) => n.unique.toLowerCase() === "everything"
            ) >= 0;
          break;
        case UMB_MEDIA_TREE_ALIAS:
          this.mediaNodes = this.#mapNodes(this.mediaNodes, data.nodes);

          this.everythingSelectedMediaNodes =
            this.mediaNodes.findIndex(
              (n) => n.unique.toLowerCase() === "everything"
            ) >= 0;
          break;
      }

      this.#onDataUpdated();
    });
  }

  #onDataUpdated() {
    super.requestUpdate("everythingSelectedMediaNodes");
    super.requestUpdate("mediaNodes");
    super.requestUpdate("everythingSelectedDictionaryNodes");
    super.requestUpdate("dictionaryNodes");
    super.requestUpdate("everythingSelectedDocumentNodes");
    super.requestUpdate("documentNodes");

    let customNodesSelected: CustomSeedModel = {
      contentNodes: this.documentNodes.map((n) => this.#mapCustomSeedNode(n)),
      dictionaryNodes: this.dictionaryNodes.map((n) =>
        this.#mapCustomSeedNode(n)
      ),
      mediaNodes: this.mediaNodes.map((n) => this.#mapCustomSeedNode(n)),
    };

    this.dispatchEvent(
      new CustomEvent("custom-nodes-selected", {
        bubbles: true,
        composed: true,
        detail: customNodesSelected,
      })
    );
  }

  #mapCustomSeedNode(e: EnterspeedUniqueItemModel): CustomSeedNode {
    let node: CustomSeedNode = {
      includeDescendants: e.includeDescendants,
      id: e.unique,
    };

    return node;
  }

  #mapNodes(
    existingNodes: EnterspeedUniqueItemModel[],
    nodesToAdd: EnterspeedUniqueItemModel[]
  ): Array<EnterspeedUniqueItemModel> {
    nodesToAdd = nodesToAdd.filter(
      (node, index, self) =>
        self.findIndex((n) => n.unique === node.unique) === index
    );

    for (let index = 0; index < nodesToAdd.length; index++) {
      const nodeToAdd = nodesToAdd[index];
      let existingIndex = existingNodes.findIndex(
        (e) => e.unique == nodeToAdd.unique
      );

      if (existingIndex > -1) {
        existingNodes.splice(existingIndex, 1);
      }

      if (nodeToAdd.unique === "Everything") {
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
