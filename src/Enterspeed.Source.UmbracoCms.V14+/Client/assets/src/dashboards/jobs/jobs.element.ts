import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import "./seed-modes.element.ts";
import "./server-message.element.ts";
import {
  LitElement,
  html,
  css,
  customElement,
  property,
} from "@umbraco-cms/backoffice/external/lit";

import { UmbNotificationContext } from "@umbraco-cms/backoffice/notification";

import { seedResponse } from "../../types.ts";
import { EnterspeedContext } from "../../enterspeed.context.ts";

@customElement("enterspeed-jobs")
export class enterspeedJobsElement extends UmbElementMixin(LitElement) {
  constructor() {
    super();
  }

  @property({ type: Object })
  enterspeedContext!: EnterspeedContext;

  @property({ type: Object })
  notificationContext!: UmbNotificationContext;

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
        <server-message
          .runJobsOnServer=${this.runJobsOnServer}
          .serverRole=${this.serverRole}
        >
        </server-message>
        ${this.renderSeedModeSelects()}
        <seed-modes
          .enterspeedContext=${this.enterspeedContext}
          .selectedSeedMode=${this.selectedSeedMode}
          .notificationContext=${this.notificationContext}
          .seedResponse=${this.seedResponse}
        ></seed-modes>
      </div>
    `;
  }
}

export default enterspeedJobsElement;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-jobs": enterspeedJobsElement;
  }
}
