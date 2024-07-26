import { html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { EnterspeedContext } from "../../enterspeed.context.ts";
import {
  UMB_NOTIFICATION_CONTEXT,
  UmbNotificationContext,
} from "@umbraco-cms/backoffice/notification";
import "./seed-response.element.ts";
import "./custom-seed-mode.element.ts";
import "./full-seed-mode.element.ts";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { SeedResponse } from "../../generated/index.ts";

@customElement("enterspeed-seed-modes")
export class enterspeedSeedModesElement extends UmbLitElement {
  private _enterspeedContext!: EnterspeedContext;
  private _notificationContext!: UmbNotificationContext;

  @property({ type: Number })
  numberOfPendingJobs = 0;

  @state()
  private _numberOfPendingJobs = 0;

  @property({ type: String })
  selectedSeedMode?: string;

  @property({ type: Object })
  seedResponse: SeedResponse | undefined | null;

  @state()
  disableSeedButton?: boolean;

  constructor() {
    super();
    this.initGetNumberOfPendingJobs();

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
        if (response.data?.isSuccess) {
          this.seedResponse = response.data.data;

          this.dispatchEvent(
            new CustomEvent("seed-response", {
              bubbles: true,
              detail: this.seedResponse,
            })
          );

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
        if (response.data?.isSuccess) {
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

  initGetNumberOfPendingJobs() {
    let intervalId = setInterval(
      () => this.getNumberOfPendingJobs(this._enterspeedContext!),
      10 * 1000
    );
    window.addEventListener(
      "hashchange",
      () => {
        clearInterval(intervalId);
      },
      false
    );
  }

  getNumberOfPendingJobs(enterspeedContext: EnterspeedContext) {
    enterspeedContext
      .getNumberOfPendingJobs()
      .then((response) => {
        if (response.data?.isSuccess) {
          this._numberOfPendingJobs =
            response.data?.data?.numberOfPendingJobs ?? 0;
          if (this._numberOfPendingJobs === 0) {
            this.seedResponse = null;
          }
        } else {
          this._numberOfPendingJobs = 0;
        }
      })
      .catch((error) => {
        this._notificationContext?.peek("danger", {
          data: {
            headline: "Failed to check queue length",
            message: error.data.message,
          },
        });
      });
  }

  render() {
    return html` ${this.renderSeedModes()}
      <enterspeed-seed-response
        .seedResponse=${this.seedResponse}
      ></enterspeed-seed-response>`;
  }

  renderSeedModes() {
    return html`${this.selectedSeedMode == "Everything"
      ? this.renderFullSeed()
      : this.renderCustomSeed()}
    ${this.renderButtons()}`;
  }

  renderFullSeed() {
    return html`<enterspeed-full-seed-mode
      .seedResponse=${this.seedResponse}
      .numberOfPendingJobs=${this._numberOfPendingJobs}
      @seed-response=${(e: CustomEvent) => {
        this.seedResponse = e.detail;
        this.requestUpdate();
      }}
    ></enterspeed-full-seed-mode>`;
  }

  renderCustomSeed() {
    return html`<enterspeed-custom-seed-mode
      .seedResponse=${this.seedResponse}
      .numberOfPendingJobs=${this._numberOfPendingJobs}
    ></enterspeed-custom-seed-mode>`;
  }

  renderButtons() {
    return html` <div class="seed-dashboard-content">
      ${this.renderSeedButton()} ${this.renderClearJobQueueButton()}
    </div>`;
  }

  renderSeedButton() {
    if (this.disableSeedButton) {
      return html`<uui-button
        disabled
        type="button"
        look="primary"
        color="default"
        label="Seed"
      ></uui-button>`;
    } else {
      return html`<uui-button
        type="button"
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
        look="secondary"
        color="default"
        label="Clear job queue ${this.numberOfPendingJobs}"
      ></uui-button>`;
    }
  }
}

export default enterspeedSeedModesElement;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-seed-modes": enterspeedSeedModesElement;
  }
}
