import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import {
  apiResponse,
  apiResponseBase,
  customSeedNodes,
  getNumberOfPendingJobsResponse,
  seedResponse,
} from "../types";
import { tryExecuteAndNotify } from "@umbraco-cms/backoffice/resources";

export class EnterspeedRepository extends UmbControllerBase {
  constructor(host: UmbControllerHost) {
    super(host);
  }

  async seed(): Promise<apiResponse<seedResponse>> {
    const request: RequestInit = {
      method: "get",
    };

    const responsePromise = fetch(
      "/umbraco/enterspeed/api/dashboard/seed",
      request
    );

    const response = await tryExecuteAndNotify(this._host, responsePromise);
    const data: apiResponse<seedResponse> = await response.data?.json();

    return data;
  }

  async customSeed(
    customSeedNodes: customSeedNodes
  ): Promise<apiResponse<seedResponse>> {
    const request: RequestInit = {
      method: "post",
      body: JSON.stringify(customSeedNodes),
    };

    const responsePromise = fetch(
      "/umbraco/enterspeed/api/dashboard/customseed",
      request
    );

    const response = await tryExecuteAndNotify(this._host, responsePromise);
    const data: apiResponse<seedResponse> = await response.data?.json();

    return data; 
  }

  async clearPendingJobs(): Promise<apiResponseBase> { 
    const request: RequestInit = {
      method: "post",
    };

    const responsePromise = fetch(
      "/umbraco/enterspeed/api/dashboard/clearpendingjobs",
      request
    );

    const response = await tryExecuteAndNotify(this._host, responsePromise);
    const data: apiResponse<seedResponse> = await response.data?.json();

    return data;
  }

  async getNumberOfPendingJobs(): Promise<
    apiResponse<getNumberOfPendingJobsResponse>
  > {
    const request: RequestInit = {
      method: "get",
    };

    const responsePromise = fetch(
      "/umbraco/enterspeed/api/dashboard/getnumberofpendingjobs",
      request
    );

    const response = await tryExecuteAndNotify(this._host, responsePromise);
    const data: apiResponse<getNumberOfPendingJobsResponse> =
      await response.data?.json();

    return data;
  }
}
