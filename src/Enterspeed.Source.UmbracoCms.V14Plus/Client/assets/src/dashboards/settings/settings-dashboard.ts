import "../shared/server-message.element";
import "../shared/notification-list.element";

import type { EnterspeedNotificationListData } from "../shared/notification-list.element";

import {
  html,
  customElement,
  css,
  state,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { EnterspeedContext } from "../../enterspeed.context";
import {
  UMB_NOTIFICATION_CONTEXT,
  UmbNotificationContext,
} from "@umbraco-cms/backoffice/notification";
import {
  EnterspeedUmbracoConfiguration,
  EnterspeedUmbracoConfigurationResponse,
} from "../../generated";

// Subset of Umbraco's UmbNotificationColor that this dashboard uses. Declared locally rather than imported so it does
// not depend on the type being exported under the same name across Umbraco 14-17.
type NotificationColor = "positive" | "warning" | "danger";

@customElement("enterspeed-settings-dashboard")
export class enterspeedSettingsDashboard extends UmbLitElement {
  #enterspeedContext: EnterspeedContext;
  #notificationContext!: UmbNotificationContext;
  #enterspeedConfiguration:
    | EnterspeedUmbracoConfiguration
    | EnterspeedUmbracoConfigurationResponse
    | null
    | undefined;

  @state()
  loadingConfiguration = true;

  @state()
  buttonState = "";

  constructor() {
    super();

    this.consumeContext(
      UMB_NOTIFICATION_CONTEXT,
      (instance: UmbNotificationContext) => {
        this.#notificationContext = instance;
      }
    );

    this.#enterspeedContext = new EnterspeedContext(this);
  }

  connectedCallback() {
    super.connectedCallback();
    this.getConfiguration();
  }

  async getConfiguration() {
    this.buttonState = "busy";
    try {
      const response = await this.#enterspeedContext.getEnterspeedConfiguration();
      this.#enterspeedConfiguration = response.data?.data?.configuration;
      this.loadingConfiguration = false;
    } catch (error) {
      this.#notify("danger", "Error loading configuration", this.#messageFrom(error));
    } finally {
      this.buttonState = "";
    }
  }

  async testConfigurationConnection() {
    if (
      !this.#enterspeedConfiguration?.apiKey ||
      !this.#enterspeedConfiguration.baseUrl
    ) {
      this.#notify("danger", "Cannot test connection", "Enter an API key and a base url first.");
      return;
    }

    this.buttonState = "busy";
    try {
      const response = await this.#enterspeedContext.testConfigurationConnection(
        this.#enterspeedConfiguration
      );
      if (response.data?.success) {
        this.#notify(
          "positive",
          "Connection successful",
          response.data.message ?? "The connection to Enterspeed was successful."
        );
      } else {
        // Publishing still works when only the preview key fails, so that is not a failed connection.
        this.#notifyFailure(response, "Connection failed", "Preview connection failed");
      }
    } catch (error) {
      this.#notify("danger", "Connection failed", this.#messageFrom(error));
    } finally {
      this.buttonState = "";
    }
  }

  async saveConfiguration() {
    if (this.#enterspeedConfiguration == null) {
      return;
    }

    this.buttonState = "busy";
    try {
      const response = await this.#enterspeedContext.saveConfiguration(
        this.#enterspeedConfiguration
      );
      if (response.data?.success) {
        // An invalid preview api key does not block the save, so it comes back as a success carrying errors.
        const errors = response.data.errors;
        if (errors && Object.keys(errors).length > 0) {
          this.#notifyList(
            "warning",
            "Configuration saved",
            this.#keyStatuses(errors),
            this.#hint(errors)
          );
        } else {
          this.#notify(
            "positive",
            "Configuration saved",
            response.data.message ?? "The configuration has been saved."
          );
        }
      } else {
        this.#notifyFailure(response, "Error saving configuration");
      }
    } catch (error) {
      this.#notify("danger", "Error saving configuration", this.#messageFrom(error));
    } finally {
      this.buttonState = "";
    }
  }

  // Reports whatever the server returned rather than branching on the status code. A failing publish api key is an
  // error; a preview-only failure leaves publishing working, so it is only a warning.
  #notifyFailure(response: any, headline: string, warningHeadline?: string) {
    const errors = response?.data?.errors;
    const publishFailed = !errors || "publishApiKey" in errors;
    const statuses = this.#keyStatuses(errors);

    if (statuses.length > 0) {
      this.#notifyList(
        publishFailed ? "danger" : "warning",
        publishFailed ? headline : warningHeadline ?? headline,
        statuses,
        this.#hint(errors)
      );
    } else {
      this.#notify(
        publishFailed ? "danger" : "warning",
        headline,
        this.#messageFrom(response)
      );
    }
  }

  // Reports every key that was checked rather than only the failures, so a mixed result also confirms which key does
  // work. Only meaningful when the server returned per key errors - without them the request never reached validation,
  // and claiming a key is valid would be a guess.
  #keyStatuses(errors: Record<string, string | null> | null | undefined) {
    if (!errors || Object.keys(errors).length === 0) {
      return [];
    }

    const statuses = [errors.publishApiKey ?? "Publish API key is valid"];
    if (this.#enterspeedConfiguration?.previewApiKey) {
      statuses.push(errors.previewApiKey ?? "Preview API key is valid");
    }

    return statuses;
  }

  #hint(errors: Record<string, string | null> | null | undefined) {
    const failures = errors
      ? Object.values(errors).filter(Boolean).length
      : 0;

    return failures > 1
      ? "Verify your API keys under Settings > Data sources in the Enterspeed app."
      : "Verify the API key under Settings > Data sources in the Enterspeed app.";
  }

  #messageFrom(source: any) {
    return (
      source?.data?.message ??
      source?.error?.message ??
      source?.message ??
      "An unexpected error occurred"
    );
  }

  #notify(color: NotificationColor, headline: string, message: string) {
    this.#notificationContext?.peek(color, {
      data: { headline, message },
    });
  }

  // Uses the list layout so each key reads as its own line rather than one paragraph. The hint is given once here
  // instead of being repeated in every message. `message` is kept populated so the payload still satisfies Umbraco's
  // default notification data, which is what renders if the custom element is ever unavailable.
  #notifyList(
    color: NotificationColor,
    headline: string,
    messages: string[],
    hint: string
  ) {
    const data: EnterspeedNotificationListData & { message: string } = {
      headline,
      messages,
      hint,
      message: messages.join(" "),
    };

    this.#notificationContext?.peek(color, {
      elementName: "enterspeed-notification-list",
      data,
    });
  }


  render() {
    if (!this.loadingConfiguration) {
      let configuredFromSettingsFile = this.#enterspeedConfiguration
        ?.configuredFromSettingsFile
        ? html` <div class="info-box">
            <strong
              >The configuration is loaded from the
              <i>appsettings.json</i> file.</strong
            ><br />
            Go to the <i>appsettings.json</i> file to change the values.
          </div>`
        : html``;

      return html`<uui-box>
        <div
          class="configuration-dashboard-content"
          ng-if="!vm.loadingConfiguration"
        >
          <enterspeed-server-message></enterspeed-server-message>
          ${configuredFromSettingsFile}
          <div class="configuration-dashboard-property">
            <label class="custom-tooltip" for="api-key">
              Enterspeed endpoint *
              <span class="custom-tooltiptext"
                >The value should be https://api.enterspeed.com</span
              > </label
            ><br />

            <uui-input
              placeholder="Enterspeed base url"
              label="enterspeed base url"
              .disabled=${this.#enterspeedConfiguration
          ?.configuredFromSettingsFile || this.buttonState === "busy"}
              .value=${this.#enterspeedConfiguration?.baseUrl}
              @input="${(e: any) => {
          this.#enterspeedConfiguration!.baseUrl = e.target.value;
        }})}"
              @change=${() => this.requestUpdate()}
            ></uui-input>
          </div>
          <div class="configuration-dashboard-property">
            <label class="custom-tooltip" for="media-domain">
              Media domain
              <span class="custom-tooltiptext">
                Enter a custom domain (can include a path as well) if you want
                to use another domain than the Umbraco domain for your media
                files.</span
              > </label
            ><br />

            <uui-input
              placeholder="Media domain (optional)"
              label="Media domain (optional)"
              .disabled=${this.#enterspeedConfiguration
          ?.configuredFromSettingsFile || this.buttonState === "busy"}
              .value=${this.#enterspeedConfiguration?.mediaDomain ?? ""}
              @input="${(e: any) => {
          this.#enterspeedConfiguration!.mediaDomain = e.target.value;
        }})}"
              @change=${() => this.requestUpdate()}
            ></uui-input>
          </div>
          <div class="configuration-dashboard-property">
            <label class="custom-tooltip" for="api-key">
              Api key *
              <span class="custom-tooltiptext"
                >You can find your API key under Settings and Data Source in the
                <a
                  href="https://app.enterspeed.com/settings/data-sources"
                  target="_blank"
                  >Enterspeed App</a
                ></span
              > </label
            ><br />

            <uui-input
              placeholder="Enterspeed API Key"
              label="Enterspeed API Key"
              .disabled=${this.#enterspeedConfiguration
          ?.configuredFromSettingsFile || this.buttonState === "busy"}
              .value=${this.#enterspeedConfiguration?.apiKey ?? ""}
              @input="${(e: any) => {
          this.#enterspeedConfiguration!.apiKey = e.target.value;
        }}"
              @change=${() => this.requestUpdate()}
            ></uui-input>
          </div>
          <div class="configuration-dashboard-property">
            <label class="custom-tooltip" for="preview-api-key">
              Preview api key
              <span class="custom-tooltiptext"
                >Create a separate source in the Enterspeed app and provide the
                API key here.</span
              > </label
            ><br />

            <uui-input
              placeholder="Enterspeed preview API Key (optional)"
              label="Enterspeed preview API Key (optional)"
              .disabled=${this.#enterspeedConfiguration
          ?.configuredFromSettingsFile || this.buttonState === "busy"}
              .value=${this.#enterspeedConfiguration?.previewApiKey ?? ""}
              @input="${(e: any) => {
          this.#enterspeedConfiguration!.previewApiKey = e.target.value;
        }}"
              @change=${() => this.requestUpdate()}
            ></uui-input>
          </div>
          <div class="configuration-dashboard-buttons">
            <uui-button
              type="button"
              style=""
              look="primary"
              color="positive"
              label="Basic"
              .disabled=${this.buttonState == "busy" ||
        !this.#enterspeedConfiguration?.apiKey ||
        !this.#enterspeedConfiguration?.baseUrl ||
        this.#enterspeedConfiguration?.configuredFromSettingsFile}
              @click="${() => this.saveConfiguration()}"
              >Save configuration</uui-button
            >
            <uui-button
              type="button"
              style=""
              look="primary"
              color="default"
              label="Basic"
              .disabled=${this.buttonState == "busy" ||
        !this.#enterspeedConfiguration?.apiKey ||
        !this.#enterspeedConfiguration?.baseUrl}
              @click="${() => this.testConfigurationConnection()}"
              >Test connection</uui-button
            >
          </div>
        </div>
      </uui-box>`;
    }
  }

  static styles = css`
    :host {
      display: block;
      padding: 20px;
    }

    .configuration-dashboard-property {
      padding: 10px 5px;
    }

    .configuration-dashboard-buttons {
      padding: 10px 5px;
    }

    .configuration-dashboard-buttons uui-button {
      margin-right: 10px;
    }

    .configuration-dashboard-property label {
      font-weight: 700;
      margin-bottom: 5px;
    }

    .configuration-dashboard-property label:hover {
      cursor: pointer;
    }

    .configuration-dashboard-property uui-input,
    .configuration-dashboard-property uui-input input {
      width: 100%;
      max-width: 750px;
    }

    .configuration-dashboard-property uui-input input {
      margin-top: 5px;
    }

    .info-box {
      width: 100%;
      max-width: 500px;
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

    .custom-tooltip {
      position: relative;
      display: inline-block;
      border-bottom: 1px dotted black;
    }

    .custom-tooltip .custom-tooltiptext {
      visibility: hidden;
      width: 220px;
      background-color: black;
      color: #fff;
      text-align: center;
      border-radius: 3px;
      padding: 6px;
      /* Position the tooltip */
      position: absolute;
      z-index: 1;
      font-weight: normal;
      font-size: 12px;
    }

    .custom-tooltip .custom-tooltiptext a {
      color: white;
      font-weight: bold;
    }

    .custom-tooltip:hover .custom-tooltiptext {
      visibility: visible;
    }

    input type[type="text"] {
      width: 100%;
    }
  `;
}

export default enterspeedSettingsDashboard;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-settings-dashboard": enterspeedSettingsDashboard;
  }
}
