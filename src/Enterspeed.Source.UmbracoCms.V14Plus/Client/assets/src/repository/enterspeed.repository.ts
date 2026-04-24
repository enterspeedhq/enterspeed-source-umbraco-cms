import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecute } from "@umbraco-cms/backoffice/resources";
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

  // Umbraco 14-16: tryExecute(promise) — no host, returns { data: T, error? }
  // Umbraco 17+:   tryExecute(host, promise, options?) — host required
  // We detect the version by checking the function's arity and call accordingly.
  private async executeResource<T>(promise: Promise<T>): Promise<any> {
    return (tryExecute as any).length >= 2
      ? (tryExecute as any)(this._host, promise, { disableNotifications: true })
      : (tryExecute as any)(promise);
  }

  // Normalizes the response which differs between Umbraco versions.
  // Umbraco 14-15 (UmbDataSourceResponse<T>): wraps result as { data: T, error? }
  // Umbraco 16+   (UmbApiResponse<T> = T & { error? }): returns T directly with error intersected
  // We normalize to the { data, error } shape so all callers work across versions.
  private normalizeResponse(result: any): any {
    if (result == null) return result;
    // U16+/U17: result IS the data (no wrapping 'data' property)
    // ApiResponse types have 'isSuccess'; Response types have 'success' but not 'data'
    if ('isSuccess' in result || ('success' in result && !('data' in result))) {
      return { data: result, error: result.error };
    }
    return result;
  }

  async seed() {
    return this.normalizeResponse(
      await this.executeResource(DashboardResource.seed())
    );
  }

  async customSeed(customSeedModel: CustomSeedModel) {
    return this.normalizeResponse(
      await this.executeResource(
        DashboardResource.customSeed({ requestBody: customSeedModel })
      )
    );
  }

  async clearPendingJobs() {
    return this.normalizeResponse(
      await this.executeResource(DashboardResource.clearPendingJobs())
    );
  }

  async getNumberOfPendingJobs() {
    return this.normalizeResponse(
      await this.executeResource(DashboardResource.getNumberOfPendingJobs())
    );
  }

  async getEnterspeedConfiguration() {
    return this.normalizeResponse(
      await this.executeResource(DashboardResource.getEnterspeedConfiguration())
    );
  }

  async testConfigurationConnection(
    enterspeedUmbracoConfiguration: EnterspeedUmbracoConfiguration
  ) {
    return this.normalizeResponse(
      await this.executeResource(
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
      await this.executeResource(
        DashboardResource.saveConfiguration({
          requestBody: enterspeedUmbracoConfiguration,
        })
      )
    );
  }

  async getFailedJobs() {
    return this.normalizeResponse(
      await this.executeResource(DashboardResource.getFailedJobs())
    );
  }

  public async deleteSelectedFailedJobs(ids: JobIdsToDelete) {
    return this.normalizeResponse(
      await this.executeResource(
        DashboardResource.deleteJobs({ requestBody: ids })
      )
    );
  }

  public async deleteFailedJobs() {
    return this.normalizeResponse(
      await this.executeResource(DashboardResource.deleteFailedJobs())
    );
  }
}
