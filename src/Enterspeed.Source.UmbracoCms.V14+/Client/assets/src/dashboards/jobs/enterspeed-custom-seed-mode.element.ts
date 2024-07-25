import { css, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { EnterspeedContext } from "../../enterspeed.context";
import { SeedResponse, CustomSeedModel } from "../../generated";
import {
  UMB_NOTIFICATION_CONTEXT,
  UmbNotificationContext,
} from "@umbraco-cms/backoffice/notification";
import { ENTERSPEED_NODEPICKER_MODAL_TOKEN } from "../../components/modals/node-picker/node-picker-modal.token";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";

@customElement("enterspeed-custom-seed-mode")
export class enterspeedCustomSeedModeElement extends UmbLitElement {
  private _enterspeedContext!: EnterspeedContext;
  private _notificationContext!: UmbNotificationContext;
  #modalManagerContext?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

  @property({ type: Array })
  selectedContentIds!: string[];

  @property({ type: Array })
  selectedMediaIds!: string[];

  @state()
  disableSeedButton?: boolean;

  @property({ type: Number })
  numberOfPendingJobs = 0;

  @property({ type: Object })
  seedResponse: SeedResponse | undefined | null;

  constructor() {
    super();
    this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
      this.#modalManagerContext = instance;
    });
    this.selectedContentIds = [""];

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

    let customSeedModel: CustomSeedModel = {};
    customSeedModel.contentNodes = this.selectedContentIds;
    customSeedModel.mediaNodes = this.selectedMediaIds;
    customSeedModel.dictionaryNodes = [];

    this._enterspeedContext!.customSeed(customSeedModel)
      .then((response) => {
        if (response.data?.isSuccess) {
          this.seedResponse = response.data.data;
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
            <uui-button
              look="placeholder"
              label="Choose"
              class="full-width-btn"
              @click=${this._openNodePickerModal}
            ></uui-button>
          </div>
          <div class="custom-seed-content-type-box">
            <h5>Media</h5>
              <umb-input-media
                @change=${(e: any) =>
                  this.updateSelectedMediaIds(e.target.value)}"
              ></umb-input-media>
          </div>
          <div class="custom-seed-content-type-box">
            <h5>Dictionary</h5>
                <umb-input-document></umb-input-document>
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

  private _openNodePickerModal() {
    this.#modalManagerContext?.open(this, ENTERSPEED_NODEPICKER_MODAL_TOKEN, {
      data: {
        headline: "My modal headline",
      },
    });
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

    .full-width-btn {
      width: 100%;
    }
  `;
}

export default enterspeedCustomSeedModeElement;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-custom-seed-mode": enterspeedCustomSeedModeElement;
  }
}
