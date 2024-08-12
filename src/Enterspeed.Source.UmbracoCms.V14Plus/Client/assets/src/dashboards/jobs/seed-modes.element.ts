import { html } from "lit";
import { customElement, property } from "lit/decorators.js";
import "./seed-response.element.ts";
import "./seed-mode-custom.element.ts";
import "./seed-mode-full.element.ts";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { SeedResponse } from "../../generated/index.ts";
import { EnterspeedUniqueItemModel } from "../../components/modals/node-picker/node-picker-modal.token.ts";

@customElement("enterspeed-seed-modes")
export class enterspeedSeedModesElement extends UmbLitElement {
  @property({ type: Number })
  numberOfPendingJobs = 0;

  @property({ type: String })
  selectedSeedMode?: string;

  @property({ type: Object })
  seedResponse: SeedResponse | undefined | null;

  @property({ type: Boolean })
  clearSelectedNodes: boolean = false;

  @property({ type: Array })
  documentNodes!: Array<EnterspeedUniqueItemModel>;

  @property({ type: Array })
  mediaNodes!: Array<EnterspeedUniqueItemModel>;

  @property({ type: Array })
  dictionaryNodes!: Array<EnterspeedUniqueItemModel>;

  render() {
    return html` ${this.#renderSeedModes()}
      <enterspeed-seed-response
        .seedResponse=${this.seedResponse}
      ></enterspeed-seed-response>`;
  }

  #renderSeedModes() {
    if (
      this.selectedSeedMode == "Everything" ||
      this.selectedSeedMode == null
    ) {
      return html`<enterspeed-seed-mode-full></enterspeed-seed-mode-full>`;
    } else {
      return html`<enterspeed-seed-mode-custom
        .documentNodes=${this.documentNodes}
        .dictionaryNodes=${this.dictionaryNodes}
        .mediaNodes=${this.mediaNodes}
      ></enterspeed-seed-mode-custom>`;
    }
  }
}

export default enterspeedSeedModesElement;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-seed-modes": enterspeedSeedModesElement;
  }
}
