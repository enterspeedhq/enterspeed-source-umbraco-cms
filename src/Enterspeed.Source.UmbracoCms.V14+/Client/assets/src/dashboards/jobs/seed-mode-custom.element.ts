import { css, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { SeedResponse } from "../../generated";
import { ENTERSPEED_NODEPICKER_MODAL_TOKEN } from "../../components/modals/node-picker/node-picker-modal.token";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import { UMB_DICTIONARY_TREE_ALIAS } from "@umbraco-cms/backoffice/dictionary";
import { UMB_MEDIA_TREE_ALIAS } from "@umbraco-cms/backoffice/media";
import { UMB_DOCUMENT_TREE_ALIAS } from "@umbraco-cms/backoffice/document";

@customElement("enterspeed-seed-mode-custom")
export class enterspeedCustomSeedModeElement extends UmbLitElement {
  #modalManagerContext?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

  @property({ type: Array })
  selectedContentIds!: string[];

  @property({ type: Array })
  selectedMediaIds!: string[];

  @property({ type: Array })
  selectedDictionaryIds!: string[];

  @state()
  disableSeedButton?: boolean;

  @property({ type: Number })
  numberOfPendingJobs = 0;

  @property({ type: Object })
  seedResponse: SeedResponse | undefined | null;

  constructor() {
    super();
    this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
      this.#modalManagerContext = instance;
    });
  }

  render() {
    return html`
      <div class="seed-dashboard-text">
        <div>
          <h4>Custom seed</h4>
          <p>
            With a custom seed you can select the nodes you want to seed for all
            cultures and publish and preview (if configured). This action can
            take a while to finish.
          </p>
          <p>
            <i
              >The job queue length is the queue length on Umbraco before the
              nodes are ingested into Enterspeed.</i
            >
          </p>
        </div>
        <div class="custom-seed-content-type-container">
          <div class="custom-seed-content-type-box">
            <h5>Content</h5>
            <uui-ref-list>
              <uui-ref-node name="test" detail="details">
                <uui-action-bar slot="actions">
                  <uui-button
                    label=${this.localize.term("general_remove")}
                  ></uui-button>
                </uui-action-bar>
              </uui-ref-node>
            </uui-ref-list>
            <uui-button
              look="placeholder"
              label="Choose"
              class="full-width-btn"
              @click=${() => this.openNodePickerModal(UMB_DOCUMENT_TREE_ALIAS)}
            ></uui-button>
          </div>
          <div class="custom-seed-content-type-box">
            <h5>Media</h5>
            <uui-ref-list>
              <uui-ref-node name="test" detail="details">
                <uui-action-bar slot="actions">
                  <uui-button
                    label=${this.localize.term("general_remove")}
                  ></uui-button>
                </uui-action-bar>
              </uui-ref-node>
            </uui-ref-list>
            <uui-button
              look="placeholder"
              label="Choose"
              class="full-width-btn"
              @click=${() => this.openNodePickerModal(UMB_MEDIA_TREE_ALIAS)}
            ></uui-button>
          </div>
          <div class="custom-seed-content-type-box">
            <h5>Dictionary</h5>
            <uui-ref-list>
              <uui-ref-node name="test" detail="details">
                <uui-action-bar slot="actions">
                  <uui-button
                    label=${this.localize.term("general_remove")}
                  ></uui-button>
                </uui-action-bar>
              </uui-ref-node>
            </uui-ref-list>
            <uui-button
              look="placeholder"
              label="Choose"
              class="full-width-btn"
              @click=${async () =>
                await this.openNodePickerModal(UMB_DICTIONARY_TREE_ALIAS)}
            ></uui-button>
          </div>
        </div>
      </div>
    `;
  }

  async openNodePickerModal(treeAlias: string) {
    let headline = "";
    switch (treeAlias) {
      case UMB_DICTIONARY_TREE_ALIAS:
        headline = "Select dictionary node";
        break;
      case UMB_DOCUMENT_TREE_ALIAS:
        headline = "Select content node";
        break;
      case UMB_MEDIA_TREE_ALIAS:
        headline = "Select media node";
    }

    var modal = this.#modalManagerContext?.open(
      this,
      ENTERSPEED_NODEPICKER_MODAL_TOKEN,
      {
        data: {
          treeAlias: treeAlias,
          headline: headline,
        },
      }
    );

    await modal?.onSubmit().then((data) => {
      console.log(data.documentNodes)
      console.log(data.mediaNodes)
      console.log(data.dictionaryNodes)
    });
  }

  static styles = css`
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

    .full-width-btn {
      width: 100%;
    }
  `;
}

export default enterspeedCustomSeedModeElement;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-custom-seed-mode": enterspeedCustomSeedModeElement;
  }
}
