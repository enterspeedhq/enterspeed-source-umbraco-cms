import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { EnterspeedContext } from "../../enterspeed.context";
import {
  UMB_NOTIFICATION_CONTEXT,
  UmbNotificationContext,
} from "@umbraco-cms/backoffice/notification";
import { customElement, property, state } from "lit/decorators.js";
import { CustomSeedModel, SeedResponse } from "../../generated";
import { html } from "lit";

@customElement("enterspeed-seed-buttons")
export class enterspeedSeedButtonsElement extends UmbLitElement {
  #enterspeedContext!: EnterspeedContext;
  #notificationContext!: UmbNotificationContext;
  #seedResponse: SeedResponse | undefined | null;

  private interval: any;

  @state()
  private numberOfPendingJobs = 0;

  @property({ type: Boolean })
  disableSeedButton: boolean = true;

  @property({ type: Object })
  customSeedModel: CustomSeedModel | undefined;

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

  disconnectedCallback(): void {
    clearInterval(this.interval);
  }

  #initGetNumberOfPendingJobs() {
    this.interval = setInterval(
      () => this.#getNumberOfPendingJobs(),
      10 * 1000
    );
  }

  async #seed() {
    this.disableSeedButton = true;
    try {
      const response = await this.#enterspeedContext!.seed(this.customSeedModel);

      if (response.error) {
        console.error("Enterspeed seed error:", response.error);
        this.#notificationContext?.peek("danger", {
          data: {
            headline: "Seed",
            message: response.error?.message ?? "An error occurred while seeding",
          },
        });
        return;
      }

      if (response.data?.isSuccess) {
        this.#seedResponse = response.data.data;

        this.dispatchEvent(
          new CustomEvent("after-seed", {
            bubbles: true,
            detail: this.#seedResponse,
            composed: true,
          })
        );

        this.#notificationContext?.peek("positive", {
          data: {
            headline: "Seed",
            message: "Successfully started seeding to Enterspeed",
          },
        });

        this.numberOfPendingJobs =
          this.#seedResponse?.numberOfPendingJobs || 0;
      } else {
        this.#seedResponse = null;
      }
    } catch (error: any) {
      console.error("Enterspeed seed exception:", error);
      this.#notificationContext?.peek("danger", {
        data: {
          headline: "Seed",
          message: error?.message ?? error?.data?.message ?? "An unexpected error occurred",
        },
      });
    } finally {
      this.disableSeedButton = false;
      super.requestUpdate("numberOfPendingJobs");
    }
  }

  async #clearJobQueue() {
    try {
      const response = await this.#enterspeedContext!.clearJobQueue();

      if (response.error) {
        console.error("Enterspeed clear job queue error:", response.error);
        this.#notificationContext?.peek("danger", {
          data: {
            headline: "Clear job queue",
            message: response.error?.message ?? "An error occurred while clearing the job queue",
          },
        });
        return;
      }

      if (response.data?.isSuccess) {
        this.#notificationContext?.peek("positive", {
          data: {
            headline: "Clear job queue",
            message: "Successfully cleared the queue of pending jobs",
          },
        });
        this.numberOfPendingJobs = 0;
      }
    } catch (error: any) {
      console.error("Enterspeed clear job queue exception:", error);
      this.#notificationContext?.peek("danger", {
        data: {
          headline: "Clear job queue",
          message: error?.message ?? error?.data?.message ?? "An unexpected error occurred",
        },
      });
    }

    this.#seedResponse = null;
  }

  async #getNumberOfPendingJobs() {
    try {
      const response = await this.#enterspeedContext.getNumberOfPendingJobs();

      if (response.error) {
        console.error("Enterspeed pending jobs error:", response.error);
        return;
      }

      if (response.data?.isSuccess) {
        this.numberOfPendingJobs =
          response.data?.data?.numberOfPendingJobs ?? 0;
        if (this.numberOfPendingJobs === 0) {
          this.#seedResponse = null;

          this.dispatchEvent(
            new CustomEvent("ingest-finished", {
              bubbles: true,
              composed: true,
              detail: true,
            })
          );
        }
      } else {
        this.numberOfPendingJobs = 0;
      }
    } catch (error: any) {
      console.error("Enterspeed pending jobs exception:", error);
    }
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
      .disabled=${this.numberOfPendingJobs <= 0}
      type="button"
      look="secondary"
      color="default"
      label="Clear job queue ${this.numberOfPendingJobs}"
      @click="${async () => this.#clearJobQueue()}"
    ></uui-button>`;
  }
}
