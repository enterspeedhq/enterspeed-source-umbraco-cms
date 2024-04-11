import { html } from "lit";
import { customElement, property } from "lit/decorators.js";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { SeedResponse } from "../../generated";

@customElement("enterspeed-seed-response")
export class enterspeedSeedResponseELement extends UmbLitElement {
  @property({ type: Object })
  seedResponse: SeedResponse | undefined | null;

  constructor() {
    super();
  }

  render() {
    if (this.seedResponse != null) {
      return html` <div class="seed-dashboard-response">
        <h4>Seed Response</h4>
        <div>
          Jobs added: ${this.seedResponse.jobsAdded}
          <div>
            <div>Content items: ${this.seedResponse.contentCount}</div>
            <div>Dictionary items: ${this.seedResponse.dictionaryCount}</div>
            <div>Media items: ${this.seedResponse.mediaCount}</div>
          </div>
        </div>
      </div>`;
    }
  }
}

export default enterspeedSeedResponseELement;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-seed-response": enterspeedSeedResponseELement;
  }
}
