import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import {
  LitElement,
  html,
  css,
  customElement,
  property,
} from "@umbraco-cms/backoffice/external/lit";

@customElement("enterspeed-jobs")
export class enterspeed_dashboard extends UmbElementMixin(LitElement) {
  constructor() {
    super();
  }

  @property()
  title = "Enterspeed jobs";

  @property({ type: Boolean })
  runJobsOnServer = false;

  @property({ type: String })
  serverRole = "";

  @property({ type: Boolean })
  loadingConfiguration = false;

  @property({ type: String })
  selectedSeedMode = "Everything";

  @property({ attribute: false })
  seedModes: Array<Option> = [
    { name: "Seed mode: Everything", value: "Everything", selected: true },
    { name: "Seed mode: Custom", value: "Custom" },
  ];

  @property({ type: Object })
  seedResponse = null;

  seed() {
    
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

  renderSeedModes() {
    if (this.selectedSeedMode == "Everything") {
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
        <uui-button type="button" style="" look="primary" color="default" label="Seed" 
        @click="${() => this.seed()}"></uui-button>
        </uui-button>
        <uui-button type="button" style="" look="secondary" color="default" label="Clear job queue (33)"></uui-button>
        </uui-button>
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

  renderDasboardResponse() {
    if (this.seedResponse != null) {
      return html` <div
        class="seed-dashboard-response"
        ng-if="vm.seedResponse !== null"
      >
        <h4>Seed Response</h4>
        <div>Jobs added: {{vm.seedResponse.jobsAdded}}</div>
        <div>Content items: {{vm.seedResponse.contentCount}}</div>
        <div>Dictionary items: {{vm.seedResponse.dictionaryCount}}</div>
        <div>Media items: {{vm.seedResponse.mediaCount}}</div>
      </div>`;
    }
  }

  render() {
    return html`
      <uui-box>
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
