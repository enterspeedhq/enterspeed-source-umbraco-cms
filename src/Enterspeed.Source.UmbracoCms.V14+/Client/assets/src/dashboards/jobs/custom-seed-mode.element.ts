import { css, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { EnterspeedContext } from "../../enterspeed.context";
import {
  UMB_NOTIFICATION_CONTEXT,
  UmbNotificationContext,
} from "@umbraco-cms/backoffice/notification";
import { customSeedNodes, seedResponse } from "../../types";

@customElement("custom-seed-mode")
export class customSeedModeElement extends UmbLitElement {
  private _enterspeedContext!: EnterspeedContext;
  private _notificationContext!: UmbNotificationContext;

  @property({ type: Array })
  selectedContentIds!: string[];

  @property({ type: Array })
  selectedMediaIds!: string[];

  @state()
  disableSeedButton?: boolean;

  @property({ type: Number })
  numberOfPendingJobs = 0;

  @property({ type: Object })
  seedResponse: seedResponse | undefined | null;

  constructor() {
    super();
    this.selectedContentIds = [""];

    this._enterspeedContext = new EnterspeedContext(this);
    this.consumeContext(UMB_NOTIFICATION_CONTEXT, (instance) => {
      this._notificationContext = instance;
    });
  }

  async seed() {
    this.disableSeedButton = true;

    var customSeed = new customSeedNodes();

    customSeed.contentNodes = this.selectedContentIds;
    customSeed.mediaNodes = this.selectedMediaIds;  
    customSeed.dictionaryNodes = [];

    this._enterspeedContext!.customSeed(customSeed)
      .then((response) => {
        if (response.isSuccess) {
          this.seedResponse = response.data;
          this._notificationContext?.peek("positive", {
            data: {
              headline: "Seed",
              message: "Successfully started seeding to Enterspeed",
            }
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

  updateSelectedContentIds(ids: String) {
    this.selectedContentIds = ids.split(",");
  }

  updateSelectedMediaIds(ids: String) {
    this.selectedMediaIds = ids.split(",");
  }

  render() {
    return html`
      <div class="seed-dashboard-text">
        <div>
          <h4>Custom seed</h4>
          <p>
            With a custom seed you can select the nodes you want to seed for all
            cultures and publish and preview (if configured). This action can
            take a while to finish.
          </p>
          <p>
            <i
              >The job queue length is the queue length on Umbraco before the
              nodes are ingested into Enterspeed.</i
            >
          </p>
        </div>
        <div class="custom-seed-content-type-container">
          <div class="custom-seed-content-type-box">
            <h5>Content</h5>
            <umb-controller-host-provider>
              <umb-input-tree
                .onchange=${(e) =>
                  this.updateSelectedContentIds(e.target.value)}
                type="content"
              >
                ></umb-input-tree
              >
            </umb-controller-host-provider>
          </div>
          <div class="custom-seed-content-type-box">
            <h5>Media</h5>
            <umb-controller-host-provider>
              <umb-input-tree
                .onchange=${(e) => this.updateSelectedMediaIds(e.target.value)}
                type="media"
              ></umb-input-tree>
            </umb-controller-host-provider>
          </div>
          <div class="custom-seed-content-type-box">
            <h5>Dictionary</h5>
            <umb-controller-host-provider>
              <umb-input-tree type="content"></umb-input-tree>
            </umb-controller-host-provider>
          </div>
        </div>
      </div>
      <div class="seed-dashboard-content">
        ${this.renderSeedButton()} ${this.renderClearJobQueueButton()}
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
  static styles = css`
    .custom-seed-content-type-container {
      display: flex;
      margin-bottom: 20px;
    }

    .custom-seed-content-type-box {
      flex: 1;
      margin: 0 5px 0 5px;
      padding: 0 10px 10px 10px;
      border: solid #e9e9ec 1px;
      border-radius: 3px;
      box-shadow: 0px 1px 1px 0px rgba(0, 0, 0, 0.16);
    }
  `;
}

export default customSeedModeElement;

declare global {
  interface HtmlElementTagNameMap {
    "custom-seed-mode": customSeedModeElement;
  }
}
