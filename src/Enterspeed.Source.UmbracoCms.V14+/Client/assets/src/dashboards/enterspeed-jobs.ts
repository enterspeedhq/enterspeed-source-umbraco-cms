import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { HelloDirective } from "./seed.modes.element";
import {
  LitElement,
  html,
  css,
  customElement,
  property,
} from "@umbraco-cms/backoffice/external/lit";

import { EnterspeedContext } from "../enterspeed.context";
import { seedResponse } from "../types";
import {
  UMB_NOTIFICATION_CONTEXT,
  UmbNotificationContext,
} from "@umbraco-cms/backoffice/notification";
import { directive } from "lit/directive.js";

@customElement("enterspeed-jobs")
export class enterspeed_dashboard extends UmbElementMixin(LitElement) {
  #enterspeedContext = new EnterspeedContext(this);
  #notificationContext!: UmbNotificationContext;

  constructor() {
    super();

    this.consumeContext(UMB_NOTIFICATION_CONTEXT, (context) => {
      this.#notificationContext = context;
    });

    this.initGetNumberOfPendingJobs();
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

  @property({ type: String })
  pendingJobState?: string;

  @property({ type: Number })
  numberOfPendingJobs = 0;

  @property({ attribute: false })
  seedModes: Array<Option> = [
    { name: "Seed mode: Everything", value: "Everything", selected: true },
    { name: "Seed mode: Custom", value: "Custom" },
  ];

  @property({ type: Object })
  seedResponse: seedResponse | undefined | null;

  initGetNumberOfPendingJobs() {
    let intervalId = setInterval(
      () => this.getNumberOfPendingJobs(this.#enterspeedContext),
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
          this.numberOfPendingJobs = response.data.numberOfPendingJobs;
        } else {
          this.numberOfPendingJobs = 0;
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

  async seed() {
    this.disableSeedButton = true;
    this.#enterspeedContext
      .seed()
      .then((response) => {
        if (response.isSuccess) {
          this.seedResponse = response.data;
          this.#notificationContext?.peek("positive", {
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
        this.#notificationContext?.peek("danger", {
          data: {
            headline: "Seed",
            message: error.data.message,
          },
        });
      });

    this.disableSeedButton = false;
  }

  async clearJobQueue() {
    this.#enterspeedContext
      .clearJobQueue()
      .then((response) => {
        if (response.isSuccess) {
          this.pendingJobState = "success";
          this.#notificationContext?.peek("positive", {
            data: {
              headline: "Clear job queue",
              message: "Successfully cleared the queue of pending jobs",
            },
          });
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

    this.seedResponse = null;
  }

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

  renderServerMessage() {
    return html` <div ?hidden=${!this.runJobsOnServer} class="info-box">
      <strong>Enterspeed jobs will not run on this current server.</strong>
      Enterspeed jobs will only run on servers with Umbraco role of
      <i>SchedulingPublisher</i> or <i>Single</i>. Current Umbraco server role
      is <i>${this.serverRole}</i>.
    </div>`;
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

  renderDasboardResponse() {
    if (this.seedResponse != null) {
      return html` <div class="seed-dashboard-response">
        <h4>Seed Response</h4>
        <div>
          Jobs added: ${this.seedResponse.jobsAdded}
          <div>
            <div>Content items: ${this.seedResponse.contentCount}</div>
            <div>Dictionary items: ${this.seedResponse.dictionaryCount}</div>
            <div>Media items: ${this.seedResponse.mediaCount}</div>
          </div>
        </div>
      </div>`;
    }
  }

  renderSeedModes() {
    if (this.selectedSeedMode == "Everything") {
      return html`
        <div class="seed-dashboard-text">
          <h4>Full seed</h4>
          <p>
            Seeding will queue jobs for all content, media and dictionary item
            for all cultures and publish and preview (if configured) within this
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
    } else {
      return html` <div class="seed-dashboard-text">
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
            <div
              ui-sortable="sortableOptions"
              ng-if="vm.selectedNodesToSeed['content'].length"
              ng-model="vm.selectedNodesToSeed['content']"
            >
              <uii-node-preview
                ng-repeat="link in vm.selectedNodesToSeed['content']"
                icon="link.icon"
                name="link.name"
                published="link.published"
                description="link.includeDescendants ? 'Including descendants' : 'Excluding descendants'"
                sortable="true"
                allow-remove="true"
                on-remove="vm.removeSelectNode('content', $index)"
              >
              </uii-node-preview>
            </div>

            <umb-controller-host-provider>
              <umb-input-tree type="content"></umb-input-tree>
            </umb-controller-host-provider>
          </div>
          <div class="custom-seed-content-type-box">
            <h5>Media</h5>
            <div
              ui-sortable="sortableOptions"
              ng-if="vm.selectedNodesToSeed['media'].length"
              ng-model="vm.selectedNodesToSeed['media']"
            >
              <uii-node-preview
                ng-repeat="link in vm.selectedNodesToSeed['media']"
                icon="link.icon"
                name="link.name"
                published="link.published"
                description="link.includeDescendants ? 'Including descendants' : 'Excluding descendants'"
                sortable="true"
                allow-remove="true"
                on-remove="vm.removeSelectNode('media', $index)"
              >
              </uii-node-preview>
            </div>
            <umb-controller-host-provider>
              <umb-input-tree type="content"></umb-input-tree>
            </umb-controller-host-provider>
          </div>
          <div class="custom-seed-content-type-box">
            <h5>Dictionary</h5>
            <div
              ui-sortable="sortableOptions"
              ng-if="vm.selectedNodesToSeed['dictionary'].length"
              ng-model="vm.selectedNodesToSeed['dictionary']"
            >
              <uii-node-preview
                ng-repeat="link in vm.selectedNodesToSeed['dictionary']"
                icon="link.icon"
                name="link.name"
                published="link.published"
                description="link.includeDescendants ? 'Including descendants' : 'Excluding descendants'"
                sortable="true"
                allow-remove="true"
                on-remove="vm.removeSelectNode('dictionary', $index)"
              >
              </uii-node-preview>
            </div>
            <umb-controller-host-provider>
              <umb-input-tree type="content"></umb-input-tree>
            </umb-controller-host-provider>
          </div>
        </div>
      </div>`;
    }
  }

  render() {
    return html`
      <uui-box>
        <h1>${this.disableSeedButton}</h1>
        <div class="seed-dashboard">
          <uui-load-indicator ng-if="vm.loadingConfiguration">
          </uui-load-indicator>
          ${this.renderServerMessage()} ${this.renderSeedModeSelects()}
          ${this.renderSeedModes()} ${this.renderDasboardResponse()}
        </div>
      </uui-box>
    `;
  }

  static styles = css`
    :host {
      display: block;
      padding: 20px;
    }

    .seed-dashboard-response {
      padding: 5px 0 20px 5px;
    }

    .info-box {
      width: 750px;
      border-radius: 3px;
      background-color: #2152a3;
      font-size: 15px;
      line-height: 20px;
      margin-bottom: 0;
      padding: 6px 14px;
      vertical-align: middle;
      color: white;
      margin-bottom: 10px;
    }

    .seed-dashboard-text {
      padding: 0 0 20px 5px;
    }

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

export default enterspeed_dashboard;

declare global {
  interface HtmlElementTagNameMap {
    enterspeed_dashboard: enterspeed_dashboard;
  }
}
