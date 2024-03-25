import { UmbContextBase } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { EnterspeedRepository } from "./repository/enterspeed.repository";
import {
  apiResponse,
  apiResponseBase,
  customSeedNodes,
  seedResponse,
} from "./types";

export class EnterspeedContext extends UmbContextBase<EnterspeedContext> {
  private readonly enterspeedRepository = new EnterspeedRepository(this);

  constructor(host: UmbControllerHost) {
    super(host, ENTERSPEED_CONTEXT);
  }

  public async customSeed(
    customSeedNodes: customSeedNodes
  ): Promise<apiResponse<seedResponse>> {
    return await this.enterspeedRepository.customSeed(customSeedNodes);
  }

  public async seed(): Promise<apiResponse<seedResponse>> {
    return await this.enterspeedRepository.seed();
  }

  public async clearJobQueue(): Promise<apiResponseBase> {
    return await this.enterspeedRepository.clearPendingJobs();
  }
}

export const ENTERSPEED_CONTEXT = new UmbContextToken<EnterspeedContext>(
  EnterspeedContext.name
);
