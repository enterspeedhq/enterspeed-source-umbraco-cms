import {
  html,
  customElement,
  property,
} from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";

@customElement("failed-jobs")
export class failedJobsElement extends UmbLitElement {
  constructor() {
    super();
  }

  @property({ type: Boolean })
  runJobsOnServer = false;

  @property({ type: String })
  serverRole = "";

  render() {
    return html`
      <div class="failedjobs-dashboard">
        <umb-load-indicator> </umb-load-indicator>
        <server-message> </server-message>
        <div
          class="failedjobs-dashboard-content"
          ng-if="!vm.failedjobs-loading"
        >
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
            <li
              ng-repeat="job in vm.failedJobs | filter:q | startFrom:vm.pagination.pageIndex*vm.pagination.pageSize | limitTo:vm.pagination.pageSize"
            >
              <div
                class="dashboard-list-item background-hover pointer"
                ng-click="vm.toggleException($index)"
              >
                <div
                  ng-if="vm.selectedDeleteMode == 'Everything'"
                  class="dashboard-list-item-property"
                  style="width: 3%"
                >
                  <span
                    ng-class="{'icon-navigation-right': $index !== vm.activeException, 'icon-navigation-down': $index === vm.activeException}"
                  ></span>
                </div>
                <div
                  ng-if="vm.selectedDeleteMode == 'Selected'"
                  class="dashboard-list-item-property "
                  style="width: 3%"
                >
                  <umb-checkbox model="job.selected"></umb-checkbox>
                </div>
                <div class="dashboard-list-item-property" style="width:12%">
                  {{job.id}}
                </div>
                <div class="dashboard-list-item-property" style="width:10%">
                  {{job.entityId}}
                </div>
                <div class="dashboard-list-item-property" style="width:10%">
                  {{job.entityType}}
                </div>
                <div class="dashboard-list-item-property" style="width:10%">
                  {{job.culture}}
                </div>
                <div class="dashboard-list-item-property" style="width:15%">
                  {{job.jobType}}
                </div>
                <div class="dashboard-list-item-property" style="width:20%">
                  {{ job.createdAt | date:'yyyy-MM-dd HH:mm:ss' }}
                </div>
                <div class="dashboard-list-item-property" style="width:20%">
                  {{ job.updatedAt | date:'yyyy-MM-dd HH:mm:ss' }}
                </div>
              </div>
              <div class="dashboard-list-item">
                <div
                  class="dashboard-list-item-exception"
                  style="width: 100%"
                  ng-show="$index === vm.activeException"
                >
                  <h4>Exception</h4>
                  {{job.exception === undefined || job.exception === null ? 'No
                  exception' : job.exception}}
                </div>
              </div>
            </li>
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
          <div ng-if="vm.failedJobs.length">
            <div class="seed-dashboard-text block-form">
              <br />
              <h4>Delete failed jobs</h4>
              <br />

              <div class="umb-control-group">
                <umb-button
                  type="button"
                  label="Mode: {{vm.selectedDeleteMode}}"
                  action="vm.toggleDeleteModeSelect()"
                >
                </umb-button>
                <umb-dropdown
                  ng-if="vm.deleteModeSelectOpen"
                  on-close="vm.deleteModeSelectOpen = false"
                >
                  <umb-dropdown-item ng-repeat="deleteMode in vm.deleteModes">
                    <button
                      type="button"
                      class="btn-reset"
                      ng-click="vm.setDeleteMode(deleteMode); vm.deleteModeSelectOpen = false;"
                    >
                      {{deleteMode}}
                    </button>
                  </umb-dropdown-item>
                </umb-dropdown>

                <umb-button
                  type="button"
                  action="vm.deleteFailedJobs()"
                  button-style="danger"
                  label="Delete"
                  disabled="vm.deletingFailedJobs || (vm.selectedDeleteMode === 'Selected' && !vm.getSelectedFailedJobs().length)"
                >
                </umb-button>
              </div>
            </div>
          </div>
        </div>
      </div>
    `;
  }
}

export default failedJobsElement;

declare global {
  interface HtmlElementTagNameMap {
    "failed-jobs": failedJobsElement;
  }
}
