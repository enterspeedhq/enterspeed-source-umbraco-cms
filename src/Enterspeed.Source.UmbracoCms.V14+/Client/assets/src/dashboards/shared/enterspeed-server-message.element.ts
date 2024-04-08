import {
  html,
  customElement,
  state,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { EnterspeedContext } from "../../enterspeed.context.ts";

@customElement("enterspeed-server-message")
export class enterspeedServerMessageElement extends UmbLitElement {
  private _enterspeedContext = new EnterspeedContext(this);
  constructor() {
    super();

    this._enterspeedContext.getEnterspeedConfiguration().then((response) => {
      console.log(response)
      if (response.data?.isSuccess) {
        this._runJobsOnServer = response.data?.data?.runJobsOnServer ?? false;
        this._serverRole = response.data?.data?.serverRole.toString() ?? "";
      }
    });
  }

  @state()
  private _runJobsOnServer = false;

  @state()
  private _serverRole = "";

  render() {
    if (!this._runJobsOnServer) {
      return html` <div class="info-box">
        <strong>Enterspeed jobs will not run on this current server.</strong>
        Enterspeed jobs will only run on servers with Umbraco role of
        <i>SchedulingPublisher</i> or <i>Single</i>. Current Umbraco server role
        is <i>${this._serverRole}</i>.
      </div>`;
    }
  }
}

export default enterspeedServerMessageElement;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-server-message": enterspeedServerMessageElement;
  }
}
