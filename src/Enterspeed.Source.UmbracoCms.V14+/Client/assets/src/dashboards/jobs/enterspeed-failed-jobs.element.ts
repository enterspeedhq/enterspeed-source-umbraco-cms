import {
  html,
  customElement,
  state,
  css,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { EnterspeedContext } from "../../enterspeed.context";
import { enterspeedJob, jobIdsToDelete } from "../../types";
import {
  UUIBooleanInputEvent,
  UUISelectEvent,
} from "@umbraco-cms/backoffice/external/uui";
import "./enterspeed-pagination.element";

@customElement("enterspeed-failed-jobs")
export class enterspeedFailedJobsElement extends UmbLitElement {
  private _enterspeedContext!: EnterspeedContext;

  constructor() {
    super();
    this._enterspeedContext = new EnterspeedContext(this);
    this._allFailedJobs = [];
    this._filteredFailedJobs = [];
    this._selectedDeleteMode = "";
    this._pagination.pageIndex = 0;
    this.getFailedJobs();
  }

  @state()
  private _deletingFailedJobs: boolean = false;

  @state()
  private _loadingFailedJobs: boolean = true;

  @state()
  private _allFailedJobs: enterspeedJob[];

  @state()
  private _filteredFailedJobs: enterspeedJob[];

  @state()
  private _activeException: number = -1;

  @state()
  private _deleteModes = [
    <Option>{
      name: "Everything",
      value: "Everything",
    },
    <Option>{ name: "Selected", value: "Selected" },
  ];

  @state()
  private _selectedDeleteMode = this._deleteModes[0].value;

  @state()
  private _pagination = {
    pageIndex: 0,
    pageNumber: 1,
    totalPages: 1,
    pageSize: 50,
  };

  setDefaultDeleteModes() {
    this._deleteModes = [
      <Option>{
        name: "Everything",
        value: "Everything",
      },
      <Option>{ name: "Selected", value: "Selected" },
    ];
  }

  nextPage() {
    this._pagination.pageIndex = this._pagination.pageIndex + 1;
    this.setFilteredJobs();
    this.requestUpdate();
  }

  prevPage() {
    this._pagination.pageIndex = this._pagination.pageIndex - 1;
    this.setFilteredJobs();
    this.requestUpdate();
  }

  goToPage(pageNumber: number) {
    this._pagination.pageIndex = pageNumber;
    this.setFilteredJobs();
    this.requestUpdate();
  }

  async getFailedJobs(): Promise<void> {
    this._allFailedJobs = [];
    await this._enterspeedContext.getFailedJobs().then((response) => {
      if (response.isSuccess) {
        this._loadingFailedJobs = false;
        this._allFailedJobs = response.data;
        this._pagination.totalPages = Math.ceil(
          this._allFailedJobs.length / this._pagination.pageSize
        );
        if (this._pagination.pageIndex === 0) {
          this._pagination.pageIndex = 1;
        }

        this.setFilteredJobs();
      }
    });
  }

  private setFilteredJobs() {
    this._filteredFailedJobs = [];
    this.requestUpdate();
    const startIndex = this._pagination.pageIndex * this._pagination.pageSize;
    const endIndex = startIndex + this._pagination.pageSize;
    this._filteredFailedJobs = this._allFailedJobs.slice(startIndex, endIndex);
    this.requestUpdate();
  }

  getSelectedFailedJobs() {
    var selectedJobsToDelete = this._filteredFailedJobs.filter(
      (fj) => fj.selected === true
    );

    return selectedJobsToDelete;
  }

  toggleException(index: number) {
    if (index === this._activeException) {
      this._activeException = -1;
    } else {
      this._activeException = index;
    }
  }

  setDeleteMode(deleteMode: string) {
    if (deleteMode === "") {
      this.setDefaultDeleteModes();
    } else {
      this._deleteModes.filter((dm) => dm.value === deleteMode)[0].selected =
        true;
    }

    this._selectedDeleteMode = deleteMode;
  }

  async deleteFailedJobs() {
    if (!confirm("Are you sure you want to delete the failed job(s)?")) {
      return;
    }
    this._deletingFailedJobs = true;

    if (this._selectedDeleteMode === "Selected") {
      let failedJobsToDelete = this.getSelectedFailedJobs();

      if (failedJobsToDelete.length) {
        let idsToDelete = new jobIdsToDelete(
          failedJobsToDelete.map((fj) => fj.id)
        );

        await this._enterspeedContext
          .deleteSelectedFailedJobs(idsToDelete)
          .then(async (response) => {
            await this.getFailedJobs();
          });
      }
    } else {
      await this._enterspeedContext
        .deleteFailedJobs()
        .then(async (response) => {
          await this.getFailedJobs();
        });
    }
    this.setDeleteMode("");
    this._deletingFailedJobs = false;
  }

  renderCellValues(failedJob: enterspeedJob, index: number) {
    let selectedDeleteModeHtml;
    if (
      this._selectedDeleteMode === "Everything" ||
      this._selectedDeleteMode === ""
    ) {
      selectedDeleteModeHtml = html`<div
        class="dashboard-list-item-property"
        style="width: 3%"
      >
        ${index !== this._activeException
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
    if (index === this._activeException) {
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
    var jobCellsHtml = this._filteredFailedJobs?.map((job, index) => {
      return this.renderCellValues(job, index);
    });

    if (this._loadingFailedJobs) {
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
              .pageNumber=${this._pagination.pageNumber}
              .totalPages=${this._pagination.totalPages}
              .pageIndex=${this._pagination.pageIndex}
              @next-page=${() => this.nextPage()}
              @prev-page=${() => this.prevPage()}
              @go-to-page=${(e: CustomEvent) => this.goToPage(e.detail)}
            ></enterspeed-pagination>

            ${this._allFailedJobs.length
              ? html` <div>
                  <div class="seed-dashboard-text block-form">
                    <h4>Delete failed jobs</h4>
                    <div class="umb-control-group">
                      <uui-select
                        .options=${this._deleteModes}
                        @change=${(e: UUISelectEvent) =>
                          this.setDeleteMode(e.target.value.toString())}
                        placeholder="Select an option"
                      ></uui-select>

                      <uui-button
                        type="button"
                        look="primary"
                        color="danger"
                        label="Delete"
                        type="button"
                        @click=${() => this.deleteFailedJobs()}
                        .disabled="${this._deletingFailedJobs ||
                        this._selectedDeleteMode === ""}"
                      >
                        Delete ${this._selectedDeleteMode}
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
