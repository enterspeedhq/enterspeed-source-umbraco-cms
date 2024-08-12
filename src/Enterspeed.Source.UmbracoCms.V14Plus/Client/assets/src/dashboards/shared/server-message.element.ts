import {
  html,
  customElement,
  state,
  css,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { EnterspeedContext } from "../../enterspeed.context.ts";

@customElement("enterspeed-server-message")
export class enterspeedServerMessageElement extends UmbLitElement {
  #enterspeedContext = new EnterspeedContext(this);
    
  @state()
  serverRole = "";

  @state()
  runJobsOnServer = true;

  constructor() {
    super();

    this.#enterspeedContext.getEnterspeedConfiguration().then((response) => {
      if (response.data?.isSuccess) {
        this.runJobsOnServer = response.data?.data?.runJobsOnServer ?? false;
        this.serverRole = response.data?.data?.serverRole.toString() ?? "";
      }
    });
  }

  render() {
    if (!this.runJobsOnServer) {
      return html` <div class="info-box">
        <strong>Enterspeed jobs will not run on this current server.</strong>
        Enterspeed jobs will only run on servers with Umbraco role of
        <i>SchedulingPublisher</i> or <i>Single</i>. Current Umbraco server role
        is <i>${this.serverRole}</i>.
      </div>`;
    }
  }

  static styles = css`
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
  `;
}

export default enterspeedServerMessageElement;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-server-message": enterspeedServerMessageElement;
  }
}
