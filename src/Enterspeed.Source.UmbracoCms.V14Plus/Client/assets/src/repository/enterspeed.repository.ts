import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import {
  CustomSeedModel,
  EnterspeedUmbracoConfiguration,
  JobIdsToDelete,
  getGetEnterspeedConfiguration,
  getGetFailedJobs,
  getGetNumberOfPendingJobs,
  getSeed,
  postClearPendingJobs,
  postCustomSeed,
  postDeleteFailedJobs,
  postDeleteJobs,
  postSaveConfiguration,
  postTestConfigurationConnection,
} from "../generated";

export class EnterspeedRepository extends UmbControllerBase {
  constructor(host: UmbControllerHost) {
    super(host);
  }

  // The generated hey-api client does not throw; every call resolves to a
  // { data, error, ... } envelope regardless of the backoffice host version,
  // which is exactly the shape the dashboard consumes. (The previous generator
  // returned bare data and threw on errors, which required version-sniffing
  // tryExecute/normalizeResponse shims here - no longer needed.)
  private async executeResource<TData>(
    promise: Promise<{ data?: TData; error?: any }>
  ): Promise<{ data?: TData; error?: any }> {
    try {
      const { data, error } = await promise;
      return { data, error };
    } catch (error) {
      return { error };
    }
  }

  async seed() {
    return this.executeResource(getSeed());
  }

  async customSeed(customSeedModel: CustomSeedModel) {
    return this.executeResource(postCustomSeed({ body: customSeedModel }));
  }

  async clearPendingJobs() {
    return this.executeResource(postClearPendingJobs());
  }

  async getNumberOfPendingJobs() {
    return this.executeResource(getGetNumberOfPendingJobs());
  }

  async getEnterspeedConfiguration() {
    return this.executeResource(getGetEnterspeedConfiguration());
  }

  async testConfigurationConnection(
    enterspeedUmbracoConfiguration: EnterspeedUmbracoConfiguration
  ) {
    return this.executeResource(
      postTestConfigurationConnection({ body: enterspeedUmbracoConfiguration })
    );
  }

  async saveConfiguration(
    enterspeedUmbracoConfiguration: EnterspeedUmbracoConfiguration
  ) {
    return this.executeResource(
      postSaveConfiguration({ body: enterspeedUmbracoConfiguration })
    );
  }

  async getFailedJobs() {
    return this.executeResource(getGetFailedJobs());
  }

  public async deleteSelectedFailedJobs(ids: JobIdsToDelete) {
    return this.executeResource(postDeleteJobs({ body: ids }));
  }

  public async deleteFailedJobs() {
    return this.executeResource(postDeleteFailedJobs());
  }
}
