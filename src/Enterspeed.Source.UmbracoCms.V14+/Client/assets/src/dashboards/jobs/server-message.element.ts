import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import "./seed-modes.element.ts";
import {
  LitElement,
  html,
  customElement,
  property,
} from "@umbraco-cms/backoffice/external/lit";


@customElement("server-message")
export class serverMessageElement extends UmbElementMixin(LitElement) {

  constructor() {
    super();
  }

  @property({ type: Boolean })
  runJobsOnServer = false;

  @property({ type: String })
  serverRole = "";


  render() {
    return html` <div ?hidden=${!this.runJobsOnServer} class="info-box">
      <strong>Enterspeed jobs will not run on this current server.</strong>
      Enterspeed jobs will only run on servers with Umbraco role of
      <i>SchedulingPublisher</i> or <i>Single</i>. Current Umbraco server role
      is <i>${this.serverRole}</i>.
    </div>`;
  }
}

export default serverMessageElement;

declare global {
  interface HtmlElementTagNameMap {
    "server-message": serverMessageElement;
  }
}
