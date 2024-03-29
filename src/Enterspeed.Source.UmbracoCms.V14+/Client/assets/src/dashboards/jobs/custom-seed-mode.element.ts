import { LitElement, html } from "lit";
import { customElement } from "lit/decorators.js";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

@customElement("custom-seed-mode")
export class customSeedModeElement extends UmbElementMixin(LitElement) {
  constructor() {
    super();
  }

  render() {
    return html` <div class="seed-dashboard-text">
      <div>
        <h4>Custom seed</h4>
        <p>
          With a custom seed you can select the nodes you want to seed for all
          cultures and publish and preview (if configured). This action can take
          a while to finish.
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

export default customSeedModeElement;

declare global {
  interface HtmlElementTagNameMap {
    "custom-seed-mode": customSeedModeElement;
  }
}
