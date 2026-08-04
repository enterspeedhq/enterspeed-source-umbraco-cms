import {
  html,
  css,
  customElement,
  property,
  ifDefined,
  nothing,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";

export interface EnterspeedNotificationListData {
  headline?: string;
  messages: string[];
  hint?: string;
}

/**
 * Notification layout that lists each message on its own line. Umbraco's default layout renders the message as a
 * single block of text, which is hard to read when more than one thing failed. Pass the element name to `peek`:
 *
 *   peek("danger", { elementName: "enterspeed-notification-list", data: { headline, messages } })
 */
@customElement("enterspeed-notification-list")
export class enterspeedNotificationList extends UmbLitElement {
  @property({ attribute: false })
  data?: EnterspeedNotificationListData;

  render() {
    return html`<uui-toast-notification-layout
      headline=${ifDefined(this.data?.headline)}
      class="uui-text"
    >
      <ul id="messages">
        ${this.data?.messages.map((message) => html`<li>${message}</li>`)}
      </ul>
      ${this.data?.hint ? html`<p id="hint">${this.data.hint}</p>` : nothing}
    </uui-toast-notification-layout>`;
  }

  static styles = css`
    /* Set explicitly, as the backoffice styles strip list markers from ul elements. */
    #messages {
      list-style: disc outside;
      margin: 0;
      padding-left: 18px;
    }

    #messages li + li {
      margin-top: 3px;
    }

    #hint {
      margin: 6px 0 0;
    }
  `;
}

export default enterspeedNotificationList;

declare global {
  interface HTMLElementTagNameMap {
    "enterspeed-notification-list": enterspeedNotificationList;
  }
}
