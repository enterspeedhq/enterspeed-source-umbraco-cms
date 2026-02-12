import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";
import {
  CustomSeedModel,
  DashboardResource,
  EnterspeedUmbracoConfiguration,
  JobIdsToDelete,
} from "../generated";

export class EnterspeedRepository extends UmbControllerBase {
  constructor(host: UmbControllerHost) {
    super(host);
  }

  // Normalizes the response from tryExecuteAndNotify which differs between Umbraco versions.
  // Umbraco 14-16 (UmbDataSourceResponse<T>): wraps result as { data: T, error? }
  // Umbraco 17+  (UmbApiResponse<T> = T & { error? }): returns T directly with error intersected
  // We normalize to the { data, error } shape so all callers work across versions.
  private normalizeResponse(result: any): any {
    if (result != null && 'isSuccess' in result) {
      return { data: result, error: result.error };
    }
    return result;
  }

  async seed() {
    return this.normalizeResponse(
      await tryExecuteAndNotify(this._host, DashboardResource.seed())
    );
  }

  async customSeed(customSeedModel: CustomSeedModel) {
    return this.normalizeResponse(
      await tryExecuteAndNotify(
        this._host,
        DashboardResource.customSeed({ requestBody: customSeedModel })
      )
    );
  }

  async clearPendingJobs() {
    return this.normalizeResponse(
      await tryExecuteAndNotify(
        this._host,
        DashboardResource.clearPendingJobs()
      )
    );
  }

  async getNumberOfPendingJobs() {
    return this.normalizeResponse(
      await tryExecuteAndNotify(
        this._host,
        DashboardResource.getNumberOfPendingJobs()
      )
    );
  }

  async getEnterspeedConfiguration() {
    return this.normalizeResponse(
      await tryExecuteAndNotify(
        this._host,
        DashboardResource.getEnterspeedConfiguration()
      )
    );
  }

  async testConfigurationConnection(
    enterspeedUmbracoConfiguration: EnterspeedUmbracoConfiguration
  ) {
    return this.normalizeResponse(
      await tryExecuteAndNotify(
        this._host,
        DashboardResource.testConfigurationConnection({
          requestBody: enterspeedUmbracoConfiguration,
        })
      )
    );
  }

  async saveConfiguration(
    enterspeedUmbracoConfiguration: EnterspeedUmbracoConfiguration
  ) {
    return this.normalizeResponse(
      await tryExecuteAndNotify(
        this._host,
        DashboardResource.saveConfiguration({
          requestBody: enterspeedUmbracoConfiguration,
        })
      )
    );
  }

  async getFailedJobs() {
    return this.normalizeResponse(
      await tryExecuteAndNotify(
        this._host,
        DashboardResource.getFailedJobs()
      )
    );
  }

  public async deleteSelectedFailedJobs(ids: JobIdsToDelete) {
    return this.normalizeResponse(
      await tryExecuteAndNotify(
        this._host,
        DashboardResource.deleteJobs({ requestBody: ids })
      )
    );
  }

  public async deleteFailedJobs() {
    return this.normalizeResponse(
      await tryExecuteAndNotify(
        this._host,
        DashboardResource.deleteFailedJobs()
      )
    );
  }
}
