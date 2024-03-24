import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { ISeedDataSource, SeedDataSource } from "./seed.datasource";
import { customSeedNodes } from "../types";

export class EnterspeedRepository extends UmbControllerBase {
  #seedDataSource: ISeedDataSource;

  constructor(host: UmbControllerHost) {
    super(host);
    this.#seedDataSource = new SeedDataSource(host);
  }

  async seed() {
    return await this.#seedDataSource.seed();
  }

  async customSeed(customSeedNodes: customSeedNodes) {
    return await this.#seedDataSource.customSeed(customSeedNodes);
  }
}
