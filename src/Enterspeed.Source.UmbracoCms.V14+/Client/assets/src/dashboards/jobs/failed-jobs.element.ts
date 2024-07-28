import {
  html,
  customElement,
  state,
  css,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { EnterspeedContext } from "../../enterspeed.context";
import { EnterspeedFailedJob } from "../../types";
import {
  UUIBooleanInputEvent,
  UUISelectEvent,
} from "@umbraco-cms/backoffice/external/uui";
import "../../components/pagination/pagination.element";
import "../shared/server-message.element";
import { JobIdsToDelete } from "../../generated";

@customElement("enterspeed-failed-jobs")
export class enterspeedFailedJobsElement extends UmbLitElement {
  #enterspeedContext!: EnterspeedContext;

  constructor() {
    super();
    this.#enterspeedContext = new EnterspeedContext(this);
    this.allFailedJobs = [];
    this.filteredFailedJobs = [];
    this.selectedDeleteMode = "";
    this.pagination.pageIndex = 0;
    this.getFailedJobs();
  }

  @state()
  private deletingFailedJobs: boolean = false;

  @state()
  private loadingFailedJobs: boolean = true;

  @state()
  private allFailedJobs: EnterspeedFailedJob[];

  @state()
  private filteredFailedJobs: EnterspeedFailedJob[];

  @state()
  private activeException: number = -1;

  @state()
  private deleteModes = [
    <Option>{
      name: "Everything",
      value: "Everything",
    },
    <Option>{ name: "Selected", value: "Selected" },
  ];

  @state()
  private selectedDeleteMode = this.deleteModes[0].value;

  @state()
  private pagination = {
    pageIndex: 0,
    pageNumber: 1,
    totalPages: 1,
    pageSize: 50,
  };

  #setDefaultDeleteModes() {
    this.deleteModes = [
      <Option>{
        name: "Everything",
        value: "Everything",
      },
      <Option>{ name: "Selected", value: "Selected" },
    ];
  }

  nextPage() {
    this.pagination.pageIndex = this.pagination.pageIndex + 1;
    this.setFilteredJobs();
    this.requestUpdate();
  }

  prevPage() {
    this.pagination.pageIndex = this.pagination.pageIndex - 1;
    this.setFilteredJobs();
    this.requestUpdate();
  }

  goToPage(pageNumber: number) {
    this.pagination.pageIndex = pageNumber;
    this.setFilteredJobs();
    this.requestUpdate();
  }

  async getFailedJobs(): Promise<void> {
    this.allFailedJobs = [];
    await this.#enterspeedContext.getFailedJobs().then((response) => {
      this.loadingFailedJobs = false;

      this.allFailedJobs = (response.data?.data as EnterspeedFailedJob[]) ?? [];

      this.pagination.totalPages = Math.ceil(
        this.allFailedJobs.length / this.pagination.pageSize
      );
      if (this.pagination.pageIndex === 0) {
        this.pagination.pageIndex = 1;
      }

      this.setFilteredJobs();
    });
  }

  private setFilteredJobs() {
    this.filteredFailedJobs = [];
    this.requestUpdate();
    const startIndex = this.pagination.pageIndex * this.pagination.pageSize;
    const endIndex = startIndex + this.pagination.pageSize;

    this.filteredFailedJobs = this.allFailedJobs.slice(startIndex, endIndex);
    this.requestUpdate();
  }

  getSelectedFailedJobs() {
    var selectedJobsToDelete = this.filteredFailedJobs.filter(
      (fj) => fj.selected === true
    );
    return selectedJobsToDelete;
  }

  toggleException(index: number) {
    if (index === this.activeException) {
      this.activeException = -1;
    } else {
      this.activeException = index;
    }
  }

  #setDeleteMode(deleteMode: string) {
    if (deleteMode === "") {
      this.#setDefaultDeleteModes();
    } else {
      this.deleteModes.filter((dm) => dm.value === deleteMode)[0].selected =
        true;
    }

    this.selectedDeleteMode = deleteMode;
  }

  async deleteFailedJobs() {
    if (!confirm("Are you sure you want to delete the failed job(s)?")) {
      return;
    }
    this.deletingFailedJobs = true;

    if (this.selectedDeleteMode === "Selected") {
      let failedJobsToDelete = this.getSelectedFailedJobs();

      if (failedJobsToDelete.length) {
        let idsToDelete: JobIdsToDelete = {
          ids: failedJobsToDelete.map((fj) => fj.id),
        };

        await this.#enterspeedContext
          .deleteSelectedFailedJobs(idsToDelete)
          .then(async () => {
            await this.getFailedJobs();
          });
      }
    } else {
      await this.#enterspeedContext.deleteFailedJobs().then(async () => {
        await this.getFailedJobs();
      });
    }
    this.#setDeleteMode("");
    this.deletingFailedJobs = false;
  }

  #renderCellValues(failedJob: EnterspeedFailedJob, index: number) {
    let selectedDeleteModeHtml;
    if (
      this.selectedDeleteMode === "Everything" ||
      this.selectedDeleteMode === ""
    ) {
      selectedDeleteModeHtml = html`<div
        class="dashboard-list-item-property"
        style="width: 3%"
      >
        ${index !== this.activeException
          ? html`<uui-icon name="icon-navigation-right"></uui-icon>`
          : html`<uui-icon name="icon-navigation-down"></uui-icon>`}
      </div>`;
    } else {
      selectedDeleteModeHtml = html`<div
        class="dashboard-list-item-property "
        style="width: 3%"
      >
        <uui-checkbox
          label=" "
          @change=${(e: UUIBooleanInputEvent) => {
            failedJob.selected = e.target.value == "on" ? true : false;
            this.requestUpdate();
          }}
        ></uui-checkbox>
      </div>`;
    }

    let activeException;
    if (index === this.activeException) {
      activeException = html`<div
        class="dashboard-list-item-exception"
        style="width: 100%"
      >
        <h4>Exception</h4>
        ${failedJob.exception === undefined || failedJob.exception === null
          ? "No exception"
          : failedJob.exception}
      </div>`;
    }

    return html` <li>
      <div
        class="dashboard-list-item background-hover pointer"
        @click=${() => this.toggleException(index)}
      >
        ${selectedDeleteModeHtml}
        <div class="dashboard-list-item-property" style="width:12%">
          ${failedJob.id}
        </div>
        <div class="dashboard-list-item-property" style="width:10%">
          ${failedJob.entityId}
        </div>
        <div class="dashboard-list-item-property" style="width:10%">
          ${failedJob.entityType}
        </div>
        <div class="dashboard-list-item-property" style="width:10%">
          ${failedJob.culture}
        </div>
        <div class="dashboard-list-item-property" style="width:15%">
          ${failedJob.jobType}
        </div>
        <div class="dashboard-list-item-property" style="width:20%">
          ${failedJob.createdAt}
        </div>
        <div class="dashboard-list-item-property" style="width:20%">
          ${failedJob.updatedAt}
        </div>
      </div>
      <div class="dashboard-list-item">${activeException}</div>
    </li>`;
  }

  render() {
    let jobCellsHtml = this.filteredFailedJobs?.map((job, index) => {
      return this.#renderCellValues(job, index);
    });

    if (this.loadingFailedJobs) {
      return html`<uui-loader-bar></uui-loader-bar>`;
    } else {
      return html`
        <div class="failedjobs-dashboard">
          <enterspeed-server-message></enterspeed-server-message>
          <div class="failedjobs-dashboard-content">
            <ul class="dashboard-list">
              <li class="dashboard-list-header">
                <div
                  class="dashboard-list-item-property"
                  style="width:3%"
                ></div>
                <div class="dashboard-list-item-property" style="width:12%">
                  ID
                </div>
                <div class="dashboard-list-item-property" style="width:10%">
                  Entity ID
                </div>
                <div class="dashboard-list-item-property" style="width:10%">
                  Type
                </div>
                <div class="dashboard-list-item-property" style="width:10%">
                  Culture
                </div>
                <div class="dashboard-list-item-property" style="width:15%">
                  Job type
                </div>
                <div class="dashboard-list-item-property" style="width:20%">
                  Created at
                </div>
                <div class="dashboard-list-item-property" style="width:20%">
                  Updated at
                </div>
              </li>
              ${jobCellsHtml}
            </ul>
            <enterspeed-pagination
              .pageNumber=${this.pagination.pageNumber}
              .totalPages=${this.pagination.totalPages}
              .pageIndex=${this.pagination.pageIndex}
              @next-page=${() => this.nextPage()}
              @prev-page=${() => this.prevPage()}
              @go-to-page=${(e: CustomEvent) => this.goToPage(e.detail)}
            ></enterspeed-pagination>

            ${this.allFailedJobs.length
              ? html` <div>
                  <div class="seed-dashboard-text block-form">
                    <h4>Delete failed jobs</h4>
                    <div class="umb-control-group">
                      <uui-select
                        .options=${this.deleteModes}
                        @change=${(e: UUISelectEvent) =>
                          this.#setDeleteMode(e.target.value.toString())}
                        placeholder="Select an option"
                      ></uui-select>

                      <uui-button
                        type="button"
                        look="primary"
                        color="danger"
                        label="Delete"
                        type="button"
                        @click=${() => this.deleteFailedJobs()}
                        .disabled="${this.deletingFailedJobs ||
                        this.selectedDeleteMode === ""}"
                      >
                        Delete ${this.selectedDeleteMode}
                      </uui-button>
                    </div>
                  </div>
                </div>`
              : html``}
          </div>
        </div>
      `;
    }
  }

  static styles = css`
    :host {
      display: block;
      padding: 15px;
    }
    .dashboard-list {
      display: flex;
      flex-direction: column;
      padding: 0;
      margin: 10px;
    }

    .dashboard-list-item,
    .dashboard-list-header {
      display: flex;
      justify-content: space-between;
    }

    .pointer {
      cursor: pointer;
    }

    .background-hover:hover {
      background-color: #f3f3f5;
    }

    .dashboard-list-header {
      font-weight: 700;
    }

    .dashboard-list-item-property {
      display: flex;
      padding: 5px 20px 5px 0;
    }

    .dashboard-list-item-property .checkbox {
      margin-top: 0px;
    }

    .dashboard-list-item-exception {
      padding: 20px;
      margin: 5px 0 20px 0;
      border: 1px solid #e9e9eb;
    }

    .dashboard-list li {
      list-style-type: none;
      margin: 0;
      padding: 0;
    }
  `;
}

export default enterspeedFailedJobsElement;

declare global {
  interface HtmlElementTagNameMap {
    "enterspeed-failed-jobs": enterspeedFailedJobsElement;
  }
}
