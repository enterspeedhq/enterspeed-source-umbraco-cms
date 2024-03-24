import { UmbDataSourceResponse } from "@umbraco-cms/backoffice/repository";
import { seedResponse, customSeedNodes, apiResponse } from "../types";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { SeedResource } from "./seed.resources";
import { tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";

export interface ISeedDataSource {
  seed(): Promise<UmbDataSourceResponse<apiResponse<seedResponse>>>;
  customSeed(
    customSeedNodes: customSeedNodes
  ): Promise<UmbDataSourceResponse<apiResponse<seedResponse>>>;
}

export class SeedDataSource implements ISeedDataSource {
  #host: UmbControllerHost;

  constructor(host: UmbControllerHost) {
    this.#host = host;
  }

  async seed(): Promise<UmbDataSourceResponse<apiResponse<seedResponse>>> {
    return await tryExecuteAndNotify(this.#host, SeedResource.seed());
  }

  async customSeed(
    customSeedNodes: customSeedNodes
  ): Promise<UmbDataSourceResponse<apiResponse<seedResponse>>> {
    return await tryExecuteAndNotify(
      this.#host,
      SeedResource.customSeed(customSeedNodes)
    );
  }
}
