import "./seed-modes.element.ts";
import "../shared/server-message.element.ts";
import {
  html,
  customElement,
  property,
  css,
  state,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UUISelectEvent } from "@umbraco-cms/backoffice/external/uui";
import { SeedResponse } from "../../generated/index.ts";

@customElement("enterspeed-seed")
export class enterspeedSeedElement extends UmbLitElement {
  constructor() {
    super();
  }
  @property({ type: Boolean })
  runJobsOnServer = false;

  @property({ type: String })
  serverRole = "";

  @property({ type: Boolean })
  loadingConfiguration = false;

  @state()
  disableSeedButton?: boolean;

  @property({ type: String })
  selectedSeedMode = "Everything";

  @property({ attribute: false })
  seedModes: Array<Option> = [
    { name: "Seed mode: Everything", value: "Everything", selected: true },
    { name: "Seed mode: Custom", value: "Custom" },
  ];

  @property({ type: Object })
  seedResponse: SeedResponse | undefined | null;

  #onSeedModeSelected(e: UUISelectEvent) {
    if (e.target.value.toString() === "Custom") {
      this.disableSeedButton = true;
    }
  }

  #renderSeedModeSelects() {
    return html` <div class="seed-dashboard-text block-form">
      <h2>What to seed</h2>
      <div class="umb-control-group">
        <uui-select
          .options=${this.seedModes}
          @change=${this.#onSeedModeSelected}
          label="Select seed mode"
          placeholder="Select an option"
        ></uui-select>
      </div>
    </div>`;
  }

  render() {
    return html`
      <div class="seed-dashboard">
        <uui-load-indicator ng-if="vm.loadingConfiguration">
        </uui-load-indicator>
        <enterspeed-server-message> </enterspeed-server-message>
        ${this.#renderSeedModeSelects()}
        <enterspeed-seed-modes
          .disableSeedButton=${this.disableSeedButton}
          .selectedSeedMode=${this.selectedSeedMode}
          .seedResponse=${this.seedResponse}
        ></enterspeed-seed-modes>
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

export default enterspeedSeedElement;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-seed": enterspeedSeedElement;
  }
}
