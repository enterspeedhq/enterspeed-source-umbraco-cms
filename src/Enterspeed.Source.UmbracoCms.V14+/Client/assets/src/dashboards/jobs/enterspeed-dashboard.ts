import "./enterspeed-seed.element.ts";
import "./enterspeed-failed-jobs.element.ts";

import {
  html,
  customElement,
  property,
  css,
} from "@umbraco-cms/backoffice/external/lit";
import { tab } from "../../types.ts";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";

@customElement("enterspeed-dashboard")
export class enterspeedDashboard extends UmbLitElement {
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
      return html`<enterspeed-failed-jobs></enterspeed-failed-jobs>`;
    } else if (this.tabs[1].active) {
      return html`<enterspeed-seed></enterspeed-seed>`;
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

export default enterspeedDashboard;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-dashboard": enterspeedDashboard;
  }
}
