import {
  html,
  customElement,
  state,
  css,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { EnterspeedContext } from "../../enterspeed.context";
import { enterspeedJob, jobIdsToDelete } from "../../types";

@customElement("failed-jobs")
export class failedJobsElement extends UmbLitElement {
  private _enterspeedContext!: EnterspeedContext;

  constructor() {
    super();
    this._enterspeedContext = new EnterspeedContext(this);
    this.getFailedJobs();
  }
  @state()
  private _deletingFailedJobs: boolean = false;

  @state()
  private _loadingFailedJobs: boolean = true;

  @state()
  private _failedJobs: enterspeedJob[] = [];

  @state()
  private _activeException: number = -1;

  @state()
  private _deleteModes = ["Everything", "Selected"];

  @state()
  private _selectedDeleteMode = this._deleteModes[0];

  async getFailedJobs(): Promise<void> {
    await this._enterspeedContext.getFailedJobs().then((response) => {
      if (response.isSuccess) {
        this._loadingFailedJobs = false;
        this._failedJobs = response.data;
      }
    });
  }

  getSelectedFailedJobs() {
    return this._failedJobs.filter((fj) => fj.selected === true);
  }

  toggleException(index: number) {
    if (index === this._activeException) {
      this._activeException = -1;
    } else {
      this._activeException = index;
    }
  }

  setDeleteMode(deleteMode: string) {
    console.log(deleteMode);
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
          .then((response) => {
            if (response.isSuccess) {
              this.getFailedJobs();
            }
          });
      }
    } else {
      await this._enterspeedContext.deleteFailedJobs().then((response) => {
        if (response.isSuccess) {
          this.getFailedJobs();
        }
      });
    }

    this._deletingFailedJobs = false;
  }

  renderCellValues(failedJob: enterspeedJob, index: number) {
    let selectedDeleteModeHtml;
    if (this._selectedDeleteMode === "Everything") {
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
        <uui-checkbox label=" " .checked=${failedJob.selected}></uui-checkbox>
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
    const options: Array<Option> = [
      { name: "Everything", value: "Everything" },
      { name: "Selected", value: "Selected" },
    ];

    var jobCellsHtml = this._failedJobs?.map((job, index) => {
      return this.renderCellValues(job, index);
    });

    return html`
      <div class="failedjobs-dashboard">
        <umb-load-indicator> </umb-load-indicator>
        <server-message></server-message>
        <div class="failedjobs-dashboard-content">
          <ul class="dashboard-list">
            <li class="dashboard-list-header">
              <div class="dashboard-list-item-property" style="width:3%"></div>
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

          <div
            ng-if="vm.pagination.totalPages > 1 && !vm.dashboard.loading"
            class="flex justify-center "
          >
            <umb-pagination
              page-number="vm.pagination.pageNumber"
              total-pages="vm.pagination.totalPages"
              on-next="vm.nextPage"
              on-prev="vm.prevPage"
              on-go-to-page="vm.goToPage"
            >
            </umb-pagination>
          </div>

          ${this._failedJobs.length
            ? html` <div>
                <div class="seed-dashboard-text block-form">
                  <br />
                  <h4>Delete failed jobs</h4>
                  <br />

                  <div class="umb-control-group">
                    <uui-select
                      .options=${options}
                      .onchange=${(e) => this.setDeleteMode(e.target.value)}
                      placeholder="Select an option"
                    ></uui-select>

                    <uui-button
                      type="button"
                      look="primary"
                      color="danger"
                      label="Delete"
                      type="button"
                      @click=${() => this.deleteFailedJobs()}
                      .disabled="${this._deletingFailedJobs}"
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

  static styles = css`
    :host {
      display: block;
      padding: 20px;
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

export default failedJobsElement;

declare global {
  interface HtmlElementTagNameMap {
    "failed-jobs": failedJobsElement;
  }
}
