import { html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { EnterspeedContext } from "../../enterspeed.context";
import {
  UMB_NOTIFICATION_CONTEXT,
  UmbNotificationContext,
} from "@umbraco-cms/backoffice/notification";
import { seedResponse } from "../../types";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";

@customElement("full-seed-mode")
export class fullSeedModeElement extends UmbLitElement {
  private _enterspeedContext!: EnterspeedContext;
  private _notificationContext!: UmbNotificationContext;

  @state()
  disableSeedButton?: boolean;

  @property({ type: Number })
  numberOfPendingJobs = 0;

  @property({ type: Object })
  seedResponse: seedResponse | undefined | null;

  constructor() {
    super();

    this._enterspeedContext = new EnterspeedContext(this);
    this.consumeContext(
      UMB_NOTIFICATION_CONTEXT,
      (instance: UmbNotificationContext) => {
        this._notificationContext = instance;
      }
    );
  }

  async seed() {
    this.disableSeedButton = true;
    this._enterspeedContext!.seed()
      .then((response) => {
        if (response.isSuccess) {
          this.seedResponse = response.data;
          this._notificationContext?.peek("positive", {
            data: {
              headline: "Seed",
              message: "Successfully started seeding to Enterspeed",
            },
          });

          this.numberOfPendingJobs =
            this.seedResponse?.numberOfPendingJobs || 0;
        } else {
          this.seedResponse = null;
        }
      })
      .catch((error) => {
        this._notificationContext?.peek("danger", {
          data: {
            headline: "Seed",
            message: error.data.message,
          },
        });
      });

    this.disableSeedButton = false;
  }

  async clearJobQueue() {
    this._enterspeedContext!.clearJobQueue()
      .then((response) => {
        if (response.isSuccess) {
          this._notificationContext?.peek("positive", {
            data: {
              headline: "Clear job queue",
              message: "Successfully cleared the queue of pending jobs",
            },
          });
          this.numberOfPendingJobs = 0;
        }
      })
      .catch((error) => {
        this._notificationContext?.peek("danger", {
          data: {
            headline: "Clear job queue",
            message: error.data.message,
          },
        });
      });

    this.seedResponse = null;
  }

  render() {
    return html`
      <div class="seed-dashboard-text">
        <h4>Full seed</h4>
        <p>
          Seeding will queue jobs for all content, media and dictionary item for
          all cultures and publish and preview (if configured) within this
          Umbraco installation. This action can take a while to finish.
        </p>
        <p>
          <i
            >The job queue length is the queue length on Umbraco before the
            nodes are ingested into Enterspeed.</i
          >
        </p>
        <div class="seed-dashboard-content">
          ${this.renderSeedButton()} ${this.renderClearJobQueueButton()}
        </div>
      </div>
    `;
  }

  renderSeedButton() {
    if (this.disableSeedButton) {
      return html`<uui-button
        disabled
        type="button"
        style=""
        look="primary"
        color="default"
        label="Seed"
      ></uui-button>`;
    } else {
      return html`<uui-button
        type="button"
        style=""
        look="primary"
        color="default"
        label="Seed"
        @click="${async () => this.seed()}"
      ></uui-button>`;
    }
  }

  renderClearJobQueueButton() {
    if (this.numberOfPendingJobs > 0) {
      return html`  <uui-button type="button" style="" look="secondary" color="default" label="Clear job queue ${
        this.numberOfPendingJobs
      }" @click="${async () => this.clearJobQueue()}"></uui-button>
      </uui-button>`;
    } else {
      return html` <uui-button
        type="button"
        disabled
        style=""
        look="secondary"
        color="default"
        label="Clear job queue ${this.numberOfPendingJobs}"
      ></uui-button>`;
    }
  }
}

export default fullSeedModeElement;

declare global {
  interface HtmlElementTagNameMap {
    "full-seed-mode": fullSeedModeElement;
  }
}
