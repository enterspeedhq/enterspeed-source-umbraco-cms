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

  async seed() {
    return await tryExecuteAndNotify(this._host, DashboardResource.seed());
  }

  async customSeed(customSeedModel: CustomSeedModel) {
    return await tryExecuteAndNotify(
      this._host,
      DashboardResource.customSeed({ requestBody: customSeedModel })
    );
  }

  async clearPendingJobs() {
    return await tryExecuteAndNotify(
      this._host,
      DashboardResource.clearPendingJobs()
    );
  }

  async getNumberOfPendingJobs() {
    return await tryExecuteAndNotify(
      this._host,
      DashboardResource.getNumberOfPendingJobs()
    );
  }

  async getEnterspeedConfiguration() {
    return await tryExecuteAndNotify(
      this._host,
      DashboardResource.getEnterspeedConfiguration()
    );
  }

  async testConfigurationConnection(
    enterspeedUmbracoConfiguration: EnterspeedUmbracoConfiguration
  ) {
    return await tryExecuteAndNotify(
      this._host,
      DashboardResource.testConfigurationConnection({
        requestBody: enterspeedUmbracoConfiguration,
      })
    );
  }

  async saveConfiguration(
    enterspeedUmbracoConfiguration: EnterspeedUmbracoConfiguration
  ) {
    return await tryExecuteAndNotify(
      this._host,
      DashboardResource.saveConfiguration({
        requestBody: enterspeedUmbracoConfiguration,
      })
    );
  }

  async getFailedJobs() {
    return await tryExecuteAndNotify(
      this._host,
      DashboardResource.getFailedJobs()
    );
  }

  public async deleteSelectedFailedJobs(ids: JobIdsToDelete) {
    return await tryExecuteAndNotify(
      this._host,
      DashboardResource.deleteJobs({ requestBody: ids })
    );
  }

  public async deleteFailedJobs() {
    return await tryExecuteAndNotify(
      this._host,
      DashboardResource.deleteFailedJobs()
    );
  }
}
