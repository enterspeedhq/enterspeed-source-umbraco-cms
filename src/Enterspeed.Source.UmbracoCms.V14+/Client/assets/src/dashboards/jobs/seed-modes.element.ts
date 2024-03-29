import { html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { EnterspeedContext } from "../../enterspeed.context.ts";
import { seedResponse } from "../../types.ts";
import {
  UMB_NOTIFICATION_CONTEXT,
  UmbNotificationContext,
} from "@umbraco-cms/backoffice/notification";
import "./seed-response.element.ts";
import "./custom-seed-mode.element.ts";
import "./full-seed-mode.element.ts";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";

@customElement("seed-modes")
export class seedModesElement extends UmbLitElement {
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
    this.consumeContext(UMB_NOTIFICATION_CONTEXT, (instance) => {
      this._notificationContext = instance;
    });
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
        if (response.isSuccess) {
          this._numberOfPendingJobs = response.data.numberOfPendingJobs;
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
      <seed-response .seedResponse=${this.seedResponse}></seed-response>`;
  }

  renderSeedModes() {
    if (this.selectedSeedMode == "Everything") {
      return html`<full-seed-mode
        .seedResponse=${this.seedResponse}
        .numberOfPendingJobs=${this._numberOfPendingJobs}
      ></full-seed-mode>`;
    } else {
      return html`<custom-seed-mode></custom-seed-mode>`;
    }
  }
}

export default seedModesElement;

declare global {
  interface HtmlElementTagNameMap {
    "seed-modes": seedModesElement;
  }
}
