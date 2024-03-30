import "./seed-modes.element.ts";
import "./server-message.element.ts";
import {
  html,
  customElement,
  property,
} from "@umbraco-cms/backoffice/external/lit";
import { seedResponse } from "../../types.ts";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";

@customElement("seed-view")
export class seedElement extends UmbLitElement {
  constructor() {
    super();
  }

  @property()
  title = "Enterspeed jobs";

  @property({ type: Boolean })
  runJobsOnServer = false;

  @property({ type: String })
  serverRole = "";

  @property({ type: Boolean })
  loadingConfiguration = false;

  @property({ type: Boolean })
  disableSeedButton?: boolean;

  @property({ type: String })
  selectedSeedMode = "Everything";

  @property({ attribute: false })
  seedModes: Array<Option> = [
    { name: "Seed mode: Everything", value: "Everything", selected: true },
    { name: "Seed mode: Custom", value: "Custom" },
  ];

  @property({ type: Object })
  seedResponse: seedResponse | undefined | null;

  renderSeedModeSelects() {
    return html` <div class="seed-dashboard-text block-form">
      <h2>What to seed</h2>
      <div class="umb-control-group">
        <uui-select
          .options=${this.seedModes}
          @change=${(e) => (this.selectedSeedMode = e.target.value)}
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
        <server-message>
        </server-message>
        ${this.renderSeedModeSelects()}
        <seed-modes
          .selectedSeedMode=${this.selectedSeedMode}
          .seedResponse=${this.seedResponse}
        ></seed-modes>
      </div>
    `;
  }
}

export default seedElement;

declare global {
  interface HtmlElementTagNameMap {
    "seed-view": seedElement;
  }
}
