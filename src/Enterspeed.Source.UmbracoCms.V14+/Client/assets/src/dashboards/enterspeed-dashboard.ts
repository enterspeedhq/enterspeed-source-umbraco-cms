import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { EnterspeedContext } from "../enterspeed.context.ts";
import "./jobs/jobs.element.ts";

import {
  LitElement,
  html,
  customElement,
  property,
  css,
} from "@umbraco-cms/backoffice/external/lit";

import {
  UMB_NOTIFICATION_CONTEXT,
  UmbNotificationContext,
} from "@umbraco-cms/backoffice/notification";

@customElement("enterspeed-dashboard")
export class enterspeed_dashboard extends UmbElementMixin(LitElement) {
  #enterspeedContext = new EnterspeedContext(this);
  #notificationContext!: UmbNotificationContext;

  constructor() {
    super();

    this.consumeContext(UMB_NOTIFICATION_CONTEXT, (context) => {
      this.#notificationContext = context;
    });
  }

  @property()
  title = "Enterspeed jobs";

  render() {
    return html`
      <uui-box>
        <enterspeed-jobs
          .enterspeedContext=${this.#enterspeedContext}
          .notificationContext=${this.#notificationContext}
        ></enterspeed-jobs>
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
