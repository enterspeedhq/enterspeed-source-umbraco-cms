import "./seed-modes.element.ts";
import "../shared/server-message.element.ts";
import "./seed-buttons.element.ts";
import "./seed-mode-select.element.ts";
import {
  html,
  customElement,
  css,
  state,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";

@customElement("enterspeed-seed")
export class seedElement extends UmbLitElement {
  @state()
  disableSeedButton: boolean = false;

  @state()
  selectedSeedMode?: string;

  #onSeedModeUpdated(e: CustomEvent) {
    this.selectedSeedMode = e.detail.toString();
    if (this.selectedSeedMode === "Everything") {
      this.disableSeedButton = false;
    } else {
      this.disableSeedButton = true;
    }
  }

  #onCustomNodesSelected(e: CustomEvent) {
    this.disableSeedButton = e.detail !== 1;
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
          .selectedSeedMode=${this.selectedSeedMode}
          @custom-nodes-selected=${(e: CustomEvent) =>
            this.#onCustomNodesSelected(e)}
        ></enterspeed-seed-modes>
        <enterspeed-seed-buttons .disableSeedButton=${this.disableSeedButton}>
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
