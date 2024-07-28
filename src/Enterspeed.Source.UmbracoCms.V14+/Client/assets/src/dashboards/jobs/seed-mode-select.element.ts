import {
  customElement,
  html,
  property,
  state,
} from "@umbraco-cms/backoffice/external/lit";
import { UUISelectEvent } from "@umbraco-cms/backoffice/external/uui";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";

@customElement("enterspeed-seed-mode-select")
export class seedModeSelect extends UmbLitElement {
  @property({ type: Boolean })
  disableSeedButton?: boolean;

  @state()
  selectedSeedMode = "Everything";

  @state()
  seedModes: Array<Option> = [
    { name: "Seed mode: Everything", value: "Everything", selected: true },
    { name: "Seed mode: Custom", value: "Custom" },
  ];

  constructor() {
    super();
  }

  #onSeedModeSelected(e: UUISelectEvent) {
    this.disableSeedButton = false;

    if (e.target.value.toString() === this.seedModes[1].value) {
      this.disableSeedButton = true;
      this.selectedSeedMode = this.seedModes[1].value;
    } else {
      this.selectedSeedMode = this.seedModes[0].value;
    }

    this.dispatchEvent(
      new CustomEvent("updated", {
        bubbles: true,
        detail: this.selectedSeedMode,
      })
    );
  }

  render() {
    return html` <div class="seed-dashboard-text block-form">
      <h2>What to seed</h2>
      <div class="umb-control-group">
        <uui-select
          .options=${this.seedModes}
          @change=${this.#onSeedModeSelected}
          label="Select seed mode"
          placeholder="Select an option"
        ></uui-select>
      </div>
    </div>`;
  }
}
