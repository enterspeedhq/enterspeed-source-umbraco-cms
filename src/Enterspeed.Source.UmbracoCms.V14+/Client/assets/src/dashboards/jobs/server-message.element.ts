import "./seed-modes.element.ts";
import {
  html,
  customElement,
  state,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { EnterspeedContext } from "../../enterspeed.context.ts";

@customElement("server-message")
export class serverMessageElement extends UmbLitElement {
  private _enterspeedContext = new EnterspeedContext(this);
  constructor() {
    super();

    this._enterspeedContext.getEnterspeedConfiguration().then((response) => {
      if (response.isSuccess) {
        this._runJobsOnServer = response.data.runJobsOnServer;
        this._serverRole = response.data.serverRole.toString();
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

export default serverMessageElement;

declare global {
  interface HtmlElementTagNameMap {
    "server-message": serverMessageElement;
  }
}
