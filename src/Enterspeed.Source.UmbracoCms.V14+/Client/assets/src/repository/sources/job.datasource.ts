import { UmbDataSourceResponse } from "@umbraco-cms/backoffice/repository";
import { seedResponse, customSeedNodes, apiResponse } from "../../types";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";
import { JobResource } from "../resources/job.resource";

export interface IJobDataSource {
  seed(): Promise<UmbDataSourceResponse<apiResponse<seedResponse>>>;
  customSeed(
    customSeedNodes: customSeedNodes
  ): Promise<UmbDataSourceResponse<apiResponse<seedResponse>>>;
}

export class JobDataSource implements IJobDataSource {
  #host: UmbControllerHost;

  constructor(host: UmbControllerHost) {
    this.#host = host;
  }

  async seed(): Promise<UmbDataSourceResponse<apiResponse<seedResponse>>> {
    return await tryExecuteAndNotify(this.#host, JobResource.seed());
  }

  async customSeed(
    customSeedNodes: customSeedNodes
  ): Promise<UmbDataSourceResponse<apiResponse<seedResponse>>> {
    return await tryExecuteAndNotify(
      this.#host,
      JobResource.customSeed(customSeedNodes)
    );
  }
}
