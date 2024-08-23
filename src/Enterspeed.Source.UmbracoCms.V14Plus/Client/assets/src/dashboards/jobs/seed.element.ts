import "./seed-modes.element.ts";
import "../shared/server-message.element.ts";
import "./seed-buttons.element.ts";
import "./seed-mode-select.element.ts";
import {
  html,
  customElement,
  css,
  state,
  property,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { CustomSeedModel, SeedResponse } from "../../generated/index.ts";
import { EnterspeedUniqueItemModel } from "../../components/modals/node-picker/node-picker-modal.token.ts";

@customElement("enterspeed-seed")
export class seedElement extends UmbLitElement {
  @state()
  disableSeedButton: boolean = false;

  @state()
  selectedSeedMode?: string;

  @state()
  customSeedModel?: CustomSeedModel;

  @state()
  seedResponse?: SeedResponse;

  @property({ type: Array })
  documentNodes: Array<EnterspeedUniqueItemModel> =
    new Array<EnterspeedUniqueItemModel>();

  @property({ type: Array })
  mediaNodes: Array<EnterspeedUniqueItemModel> =
    new Array<EnterspeedUniqueItemModel>();

  @property({ type: Array })
  dictionaryNodes: Array<EnterspeedUniqueItemModel> =
    new Array<EnterspeedUniqueItemModel>();

  #onSeedModeUpdated(e: CustomEvent) {
    this.selectedSeedMode = e.detail.toString();
    if (this.selectedSeedMode === "Everything") {
      this.disableSeedButton = false;
    } else {
      this.disableSeedButton = true;
    }
  }

  #onCustomNodesSelected(e: CustomEvent) {
    let event = e.detail as CustomSeedModel;
    if (
      event!.contentNodes!.length > 0 ||
      event!.mediaNodes!.length > 0 ||
      event!.dictionaryNodes!.length > 0
    ) {
      this.disableSeedButton = false;
    } else {
      this.disableSeedButton = true;
    }

    this.customSeedModel = event;
  }

  #onAfterSeed(e: CustomEvent) {
    this.customSeedModel = undefined;
    this.documentNodes = new Array<EnterspeedUniqueItemModel>();
    this.mediaNodes = new Array<EnterspeedUniqueItemModel>();
    this.dictionaryNodes = new Array<EnterspeedUniqueItemModel>();
    this.seedResponse = e.detail as SeedResponse;
  }

  #onIngestFinshed() {
    this.seedResponse = undefined;
  }

  render() {
    return html`
      <div class="seed-dashboard">
        <uui-load-indicator
          ng-if="vm.loadingConfiguration"
        ></uui-load-indicator>
        <enterspeed-server-message> </enterspeed-server-message>
        <enterspeed-seed-mode-select
          .disableSeedButton=${this.disableSeedButton}
          @updated=${(e: CustomEvent) => this.#onSeedModeUpdated(e)}
        ></enterspeed-seed-mode-select>
        <enterspeed-seed-modes
          .documentNodes=${this.documentNodes}
          .dictionaryNodes=${this.dictionaryNodes}
          .mediaNodes=${this.mediaNodes}
          .selectedSeedMode=${this.selectedSeedMode}
          .seedResponse=${this.seedResponse}
          @custom-nodes-selected=${(e: CustomEvent) =>
            this.#onCustomNodesSelected(e)}
        ></enterspeed-seed-modes>
        <enterspeed-seed-buttons
          @after-seed=${(e: CustomEvent) => this.#onAfterSeed(e)}
          @ingest-finished=${() => this.#onIngestFinshed()}
          .disableSeedButton=${this.disableSeedButton}
          .customSeedModel=${this.customSeedModel}
        >
        </enterspeed-seed-buttons>
      </div>
    `;
  }

  static styles = css`
    :host {
      display: block;
      padding: 15px;
    }
  `;
}

export default seedElement;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-seed": seedElement;
  }
}
