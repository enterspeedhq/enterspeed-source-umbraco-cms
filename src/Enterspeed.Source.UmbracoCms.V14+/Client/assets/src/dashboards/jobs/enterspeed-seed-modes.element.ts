import { html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { EnterspeedContext } from "../../enterspeed.context.ts";
import { seedResponse } from "../../types.ts";
import {
  UMB_NOTIFICATION_CONTEXT,
  UmbNotificationContext,
} from "@umbraco-cms/backoffice/notification";
import "./enterspeed-seed-response.element.ts";
import "./enterspeed-custom-seed-mode.element.ts";
import "./enterspeed-full-seed-mode.element.ts";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";

@customElement("enterspeed-seed-modes")
export class enterspeedSeedModesElement extends UmbLitElement {
  private _enterspeedContext!: EnterspeedContext;
  private _notificationContext!: UmbNotificationContext;

  @state()
  private _numberOfPendingJobs = 0;

  @property({ type: String })
  selectedSeedMode?: string;

  @property({ type: Object })
  seedResponse: seedResponse | undefined | null;

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
    if (this.selectedSeedMode == "Everything") {
      return html`<enterspeed-full-seed-mode
        .seedResponse=${this.seedResponse}
        .numberOfPendingJobs=${this._numberOfPendingJobs}
      ></enterspeed-full-seed-mode>`;
    } else {
      return html`<enterspeed-custom-seed-mode
        .seedResponse=${this.seedResponse}
        .numberOfPendingJobs=${this._numberOfPendingJobs}
      ></enterspeed-custom-seed-mode>`;
    }
  }
}

export default enterspeedSeedModesElement;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-seed-modes": enterspeedSeedModesElement;
  }
}
