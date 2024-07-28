import {
  customElement,
  css,
  property,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { html } from "lit/static-html.js";

@customElement("enterspeed-pagination")
export class enterspeedPaginationElement extends UmbLitElement {
  @property({ type: Number })
  pageNumber!: number;

  @property({ type: Number })
  totalPages!: number;

  @property({ type: Number })
  pageIndex!: number;

  goToNextPage() {
    this.dispatchEvent(
      new CustomEvent("next-page", {
        bubbles: true,
        detail: true,
      })
    );
  }

  goToPreviousPage() {
    this.dispatchEvent(
      new CustomEvent("prev-page", {
        bubbles: true,
        detail: true,
      })
    );
  }

  goToPage(index: number) {
    this.dispatchEvent(
      new CustomEvent("go-to-page", {
        bubbles: true,
        detail: index,
      })
    );
  }

  renderPageButtons(index: number) {
    return html`<uui-button
      look="outline"
      role="listitem"
      label="Go to page 1"
      class="${this.pageIndex === index ? "page active" : "page"}"
      style="${this.pageIndex === index
        ? "--uui-button-background-color: #f5c1bc;"
        : ""}"
      pristine=""
      type="button"
      color="default"
      @click="${() => this.goToPage(index)}"
    >
      ${index}
    </uui-button>`;
  }

  render() {
    const previousButton =
      this.pageIndex > 1
        ? html` <uui-button
            look="outline"
            role="listitem"
            label="Previous"
            tabindex=""
            type="button"
            color="default"
            @click="${() => this.goToPreviousPage()}"
          >
            Previous
          </uui-button>`
        : "";

    const nextButton =
      this.pageIndex < this.totalPages - 1
        ? html` <uui-button
            look="outline"
            role="listitem"
            label="Next"
            tabindex=""
            type="button"
            color="default"
            @click="${() => this.goToNextPage()}"
          >
            Next
          </uui-button>`
        : "";

    const pageButtons = [];
    for (let i = 1; i < this.totalPages; i++) {
      pageButtons.push(this.renderPageButtons(i));
    }

    return html`
      <div class="flex justify-center">
        <uui-button-group role="list" id="pages">
          ${previousButton} ${pageButtons} ${nextButton}
        </uui-button-group>
      </div>
    `;
  }

  static styles = css`
    :host {
      display: block;
      padding: 15px;
    }
    .page-active {
      background-color: #f5c1bc;
    }
  `;
}

export default enterspeedPaginationElement;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-pagination": enterspeedPaginationElement;
  }
}
