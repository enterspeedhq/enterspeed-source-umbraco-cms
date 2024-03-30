import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import "./jobs/seed.element.ts";

import {
  LitElement,
  html,
  customElement,
  property,
  css,
} from "@umbraco-cms/backoffice/external/lit";
import { tab } from "../types.ts";

@customElement("enterspeed-dashboard")
export class enterspeed_dashboard extends UmbElementMixin(LitElement) {
  constructor() {
    super();
  }

  @property({ type: Array })
  tabs: tab[] = [
    {
      alias: "failedJobsTab",
      label: "Failed Jobs",
    },
    {
      alias: "seedsTab",
      label: "Seed",
      active: true,
    },
  ];

  @property()
  title = "Enterspeed jobs";

  handleTabChange(selectedTab: tab) {
    this.tabs = this.tabs.map((tab) => {
      tab.active = tab.alias === selectedTab.alias;
      return tab;
    });
  }

  render() {
    return html`
      <uui-box>
        <uui-tab-group>
          ${this.tabs.map(
            (tab) =>
              html`
                <uui-tab
                  .label=${tab.label}
                  .active=${tab.active}
                  @click=${() => this.handleTabChange(tab)}
                ></uui-tab>
              `
          )}
        </uui-tab-group>
        ${this.renderContent()}
      </uui-box>
    `;
  }

  renderContent() {
    if (this.tabs[0].active) {
      return html``;
    } else if (this.tabs[1].active) {
      return html`<seed-view></seed-view>`;
    }
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
  `;
}

export default enterspeed_dashboard;

declare global {
  interface HtmlElementTagNameMap {
    enterspeed_dashboard: enterspeed_dashboard;
  }
}
