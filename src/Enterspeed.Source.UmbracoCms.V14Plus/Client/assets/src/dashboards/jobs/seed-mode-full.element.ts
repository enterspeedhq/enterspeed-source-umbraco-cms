import { html } from "lit";
import { customElement } from "lit/decorators.js";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";

@customElement("enterspeed-seed-mode-full")
export class enterspeedFullSeedModeElement extends UmbLitElement {
  constructor() {
    super();
  }

  render() {
    return html`
      <div class="seed-dashboard-text">
        <h4>Full seed</h4>
        <p>
          Seeding will queue jobs for all content, media and dictionary item for
          all cultures and publish and preview (if configured) within this
          Umbraco installation. This action can take a while to finish.
        </p>
        <p>
          <i
            >The job queue length is the queue length on Umbraco before the
            nodes are ingested into Enterspeed.</i
          >
        </p>
      </div>
    `;
  }
}

export default enterspeedFullSeedModeElement;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-full-seed-mode": enterspeedFullSeedModeElement;
  }
}
