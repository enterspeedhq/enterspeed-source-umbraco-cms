import { LitElement, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { EnterspeedContext } from "../../enterspeed.context.ts";
import { seedResponse } from "../../types.ts";
import { UmbNotificationContext } from "@umbraco-cms/backoffice/notification";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import "./seed-response.element.ts";

@customElement("seed-modes")
export class seedModesElement extends UmbElementMixin(LitElement) {
  #disableSeedButton?: boolean;

  @state()
  private numberOfPendingJobs = 0;

  @property({ type: Object })
  enterspeedContext!: EnterspeedContext;

  @property({ type: String })
  selectedSeedMode?: string;

  @property({ type: Object })
  notificationContext!: UmbNotificationContext;

  @property({ type: Object })
  seedResponse: seedResponse | undefined | null;

  constructor() {
    super();
    this.initGetNumberOfPendingJobs();
  }

  initGetNumberOfPendingJobs() {
    let intervalId = setInterval(
      () => this.getNumberOfPendingJobs(this.enterspeedContext!),
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
        this.notificationContext?.peek("danger", {
          data: {
            headline: "Failed to check queue length",
            message: error.data.message,
          },
        });
      });
  }

  async seed() {
    this.#disableSeedButton = true;
    this.enterspeedContext!.seed()
      .then((response) => {
        if (response.isSuccess) {
          this.seedResponse = response.data;
          this.notificationContext?.peek("positive", {
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
        this.notificationContext?.peek("danger", {
          data: {
            headline: "Seed",
            message: error.data.message,
          },
        });
      });

    this.#disableSeedButton = false;
  }

  async clearJobQueue() {
    this.enterspeedContext!.clearJobQueue()
      .then((response) => {
        if (response.isSuccess) {
          this.notificationContext?.peek("positive", {
            data: {
              headline: "Clear job queue",
              message: "Successfully cleared the queue of pending jobs",
            },
          });
          this.numberOfPendingJobs = 0;
        }
      })
      .catch((error) => {
        this.notificationContext?.peek("danger", {
          data: {
            headline: "Clear job queue",
            message: error.data.message,
          },
        });
      });

    this.seedResponse = null;
  }

  render() {
    return html` ${this.renderSeedModes()}
    <seed-response .seedResponse=${this.seedResponse}></seed-response>`;
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

  renderSeedButton() {
    if (this.#disableSeedButton) {
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

export default seedModesElement;

declare global {
  interface HtmlElementTagNameMap {
    "seed-modes": seedModesElement;
  }
}
