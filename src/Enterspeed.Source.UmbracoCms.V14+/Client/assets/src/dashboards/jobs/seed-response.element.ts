import { LitElement, html } from "lit";
import { customElement, property } from "lit/decorators.js";
import { seedResponse } from "../../types";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";

@customElement("seed-response")
export class seedResponseELement extends UmbElementMixin(LitElement) {

  @property({ type: Object })
  seedResponse: seedResponse | undefined | null;

  constructor() {
    super();
  }

  render() {
    if (this.seedResponse != null) {
      return html` 
      <div class="seed-dashboard-response">
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

export default seedResponseELement;

declare global {
  interface HtmlElementTagNameMap {
    "seed-response": seedResponseELement;
  }
}
