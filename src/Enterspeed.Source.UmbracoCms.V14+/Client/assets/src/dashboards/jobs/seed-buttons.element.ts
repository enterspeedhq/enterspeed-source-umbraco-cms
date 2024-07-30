import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { EnterspeedContext } from "../../enterspeed.context";
import {
  UMB_NOTIFICATION_CONTEXT,
  UmbNotificationContext,
} from "@umbraco-cms/backoffice/notification";
import { customElement, property } from "lit/decorators.js";
import { SeedResponse } from "../../generated";
import { html } from "lit";
import { customNodesSelected as customNodesSelected } from "../../types.ts";

@customElement("enterspeed-seed-buttons")
export class enterspeedSeedButtonsElement extends UmbLitElement {
  #enterspeedContext!: EnterspeedContext;
  #notificationContext!: UmbNotificationContext;
  #numberOfPendingJobs = 0;
  #seedResponse: SeedResponse | undefined | null;

  @property({ type: Boolean })
  disableSeedButton: boolean = true;

  @property({ type: Object })
  customNodesSelected: customNodesSelected | undefined;

  constructor() {
    super();
    this.#initGetNumberOfPendingJobs();

    this.#enterspeedContext = new EnterspeedContext(this);
    this.consumeContext(
      UMB_NOTIFICATION_CONTEXT,
      (instance: UmbNotificationContext) => {
        this.#notificationContext = instance;
      }
    );
  }

  render() {
    return html` <div class="seed-dashboard-content">
      ${this.#renderSeedButton()} ${this.#renderClearJobQueueButton()}
    </div>`;
  }

  async #seed() {
    this.disableSeedButton = true;
    this.#enterspeedContext!.seed(this.customNodesSelected)
      .then((response) => {
        if (response.data?.isSuccess) {
          this.#seedResponse = response.data.data;

          this.dispatchEvent(
            new CustomEvent("seed-response", {
              bubbles: true,
              detail: this.#seedResponse,
            })
          );

          this.#notificationContext?.peek("positive", {
            data: {
              headline: "Seed",
              message: "Successfully started seeding to Enterspeed",
            },
          });

          this.#numberOfPendingJobs =
            this.#seedResponse?.numberOfPendingJobs || 0;
        } else {
          this.#seedResponse = null;
        }
      })
      .catch((error) => {
        this.#notificationContext?.peek("danger", {
          data: {
            headline: "Seed",
            message: error.data.message,
          },
        });
      });

    this.disableSeedButton = false;
  }

  async #clearJobQueue() {
    this.#enterspeedContext!.clearJobQueue()
      .then((response) => {
        if (response.data?.isSuccess) {
          this.#notificationContext?.peek("positive", {
            data: {
              headline: "Clear job queue",
              message: "Successfully cleared the queue of pending jobs",
            },
          });
          this.#numberOfPendingJobs = 0;
        }
      })
      .catch((error) => {
        this.#notificationContext?.peek("danger", {
          data: {
            headline: "Clear job queue",
            message: error.data.message,
          },
        });
      });

    this.#seedResponse = null;
  }

  #initGetNumberOfPendingJobs() {
    let intervalId = setInterval(
      () => this.#getNumberOfPendingJobs(),
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

  #getNumberOfPendingJobs() {
    this.#enterspeedContext
      .getNumberOfPendingJobs()
      .then((response) => {
        if (response.data?.isSuccess) {
          this.#numberOfPendingJobs =
            response.data?.data?.numberOfPendingJobs ?? 0;
          if (this.#numberOfPendingJobs === 0) {
            this.#seedResponse = null;
          }
        } else {
          this.#numberOfPendingJobs = 0;
        }
      })
      .catch((error) => {
        this.#notificationContext?.peek("danger", {
          data: {
            headline: "Failed to check queue length",
            message: error.data.message,
          },
        });
      });
  }

  #renderSeedButton() {
    return html`<uui-button
      .disabled=${this.disableSeedButton}
      type="button"
      look="primary"
      color="default"
      label="Seed"
      @click=${() => this.#seed()}
    ></uui-button>`;
  }

  #renderClearJobQueueButton() {
    return html` <uui-button
      .disabled=${this.#numberOfPendingJobs <= 0}
      type="button"
      look="secondary"
      color="default"
      label="Clear job queue ${this.#numberOfPendingJobs}"
      @click="${async () => this.#clearJobQueue()}"
    ></uui-button>`;
  }
}