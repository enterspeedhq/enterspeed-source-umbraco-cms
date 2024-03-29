import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import "./jobs/jobs.element.ts";

import {
  LitElement,
  html,
  customElement,
  property,
  css,
} from "@umbraco-cms/backoffice/external/lit";

@customElement("enterspeed-dashboard")
export class enterspeed_dashboard extends UmbElementMixin(LitElement) {

  constructor() {
    super();
  }

  @property()
  title = "Enterspeed jobs";

  render() {
    return html`
      <uui-box>
        <enterspeed-jobs></enterspeed-jobs>
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
  `;
}

export default enterspeed_dashboard;

declare global {
  interface HtmlElementTagNameMap {
    enterspeed_dashboard: enterspeed_dashboard;
  }
}
