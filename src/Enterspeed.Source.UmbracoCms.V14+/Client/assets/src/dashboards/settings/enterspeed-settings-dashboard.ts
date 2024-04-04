import "../shared/enterspeed-server-message.element";

import {
  html,
  customElement,
  css,
  state,
  property,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { EnterspeedContext } from "../../enterspeed.context";
import {
  connectionResponse,
  enterspeedUmbracoConfigurationResponse,
} from "../../types";
import {
  UMB_NOTIFICATION_CONTEXT,
  UmbNotificationContext,
} from "@umbraco-cms/backoffice/notification";

@customElement("enterspeed-settings-dashboard")
export class enterspeedSettingsDashboard extends UmbLitElement {
  private _enterspeedContext: EnterspeedContext;
  private _notificationContext!: UmbNotificationContext;

  @state()
  private _loadingConfiguration = true;

  @state()
  private _enterspeedConfiguration!: enterspeedUmbracoConfigurationResponse;

  @state()
  private _buttonState: string;

  constructor() {
    super();
    this._buttonState = "";

    this.consumeContext(
      UMB_NOTIFICATION_CONTEXT,
      (instance: UmbNotificationContext) => {
        this._notificationContext = instance;
      }
    );

    this._enterspeedContext = new EnterspeedContext(this);

    this.getConfiguration();
  }

  getConfiguration() {
    this._buttonState = "busy";
    this._enterspeedContext
      .getEnterspeedConfiguration()
      .then((response) => {
        this._enterspeedConfiguration = response.data;
        this._loadingConfiguration = false;
      })
      .catch((error) => {
        this._notificationContext?.peek("danger", {
          data: {
            headline: "Error loading configuration",
            message: error.data.message,
          },
        });
      });
    this._buttonState = "";
  }

  async saveConfiguration() {
    this._enterspeedContext
      .saveConfiguration(this._enterspeedConfiguration.configuration)
      .then((response) => {
        if (response.success) {
          this._notificationContext?.peek("positive", {
            data: {
              headline: "Configuration saved",
              message: "The configuration has been saved.",
            },
          });
        } else {
          this._notificationContext?.peek("danger", {
            data: {
              headline: "Error saving configuration",
              message: response.message,
            },
          });
        }
      })
      .catch((error) => {
        this._notificationContext?.peek("danger", {
          data: {
            headline: "Error saving configuration",
            message: error.data.message,
          },
        });
      });
  }

  render() {
    if (!this._loadingConfiguration) {
      let configuredFromSettingsFile = this._enterspeedConfiguration
        .configuration.configuredFromSettingsFile
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
              .disabled=${this._enterspeedConfiguration.configuration
                .configuredFromSettingsFile || this._buttonState === "busy"}
              .value=${this._enterspeedConfiguration.configuration.baseUrl}
              @input="${(e) => {
                this._enterspeedConfiguration.configuration.baseUrl =
                  e.target.value;
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
              .disabled=${this._enterspeedConfiguration.configuration
                .configuredFromSettingsFile || this._buttonState === "busy"}
              .value=${this._enterspeedConfiguration.configuration
                .mediaDomain ?? ""}
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
              .disabled=${this._enterspeedConfiguration.configuration
                .configuredFromSettingsFile || this._buttonState === "busy"}
              .value=${this._enterspeedConfiguration.configuration.apiKey ?? ""}
              @input="${(e) => {
                this._enterspeedConfiguration.configuration.apiKey =
                  e.target.value;
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
              .disabled=${this._enterspeedConfiguration.configuration
                .configuredFromSettingsFile || this._buttonState === "busy"}
              .value=${this._enterspeedConfiguration.configuration
                .previewApiKey ?? ""}
              @input="${(e) => {
                this._enterspeedConfiguration.configuration.previewApiKey =
                  e.target.value;
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
              .disabled=${this._buttonState == "busy" ||
              !this._enterspeedConfiguration.configuration.apiKey ||
              !this._enterspeedConfiguration.configuration.baseUrl ||
              this._enterspeedConfiguration.configuration
                .configuredFromSettingsFile}
              @click="${() => this.saveConfiguration()}"
              >Save configuration</uui-button
            >
            <uui-button
              type="button"
              style=""
              look="primary"
              color="default"
              label="Basic"
              .disabled=${this._buttonState == "busy" ||
              !this._enterspeedConfiguration.configuration.apiKey ||
              !this._enterspeedConfiguration.configuration.baseUrl}
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
