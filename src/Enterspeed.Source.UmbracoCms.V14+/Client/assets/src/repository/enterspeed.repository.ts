import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { IJobDataSource, JobDataSource } from "./sources/job.datasource";
import { customSeedNodes } from "../types";

export class EnterspeedRepository extends UmbControllerBase {
  #jobDataSource: IJobDataSource;

  constructor(host: UmbControllerHost) {
    super(host);
    this.#jobDataSource = new JobDataSource(host);
  }

  async seed() {
    return await this.#jobDataSource.seed();
  }

  async customSeed(customSeedNodes: customSeedNodes) {
    return await this.#jobDataSource.customSeed(customSeedNodes);
  }
}
