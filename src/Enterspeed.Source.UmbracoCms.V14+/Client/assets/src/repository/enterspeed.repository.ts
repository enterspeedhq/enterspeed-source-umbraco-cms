import { UmbControllerBase } from "@umbraco-cms/backoffice/class-api";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import {
  apiResponse,
  apiResponseBase,
  configuration,
  connectionResponse,
  customSeedNodes,
  enterspeedJob,
  enterspeedUmbracoConfigurationResponse,
  getNumberOfPendingJobsResponse,
  jobIdsToDelete,
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
      headers: this.getDefaultPostHeader(),
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

  async getEnterspeedConfiguration(): Promise<
    apiResponse<enterspeedUmbracoConfigurationResponse>
  > {
    const request: RequestInit = {
      method: "get",
    };

    const responsePromise = fetch(
      "/umbraco/enterspeed/api/dashboard/getenterspeedconfiguration",
      request
    );

    const response = await tryExecuteAndNotify(this._host, responsePromise);
    const data: apiResponse<enterspeedUmbracoConfigurationResponse> =
      await response.data?.json();

    return data;
  }

  async testConfigurationConnection(
    configuration: configuration
  ): Promise<connectionResponse> {
    const request: RequestInit = {
      method: "post",
      headers: this.getDefaultPostHeader(),
      body: JSON.stringify(configuration),
    };

    const responsePromise = fetch(
      "/umbraco/enterspeed/api/dashboard/testconfigurationconnection",
      request
    );

    const response = await tryExecuteAndNotify(this._host, responsePromise);
    const data: connectionResponse = await response.data?.json();

    return data;
  }

  async saveConfiguration(
    configuration: configuration
  ): Promise<connectionResponse> {
    const request: RequestInit = {
      method: "post",
      headers: this.getDefaultPostHeader(),
      body: JSON.stringify(configuration),
    };

    const responsePromise = fetch(
      "/umbraco/enterspeed/api/dashboard/saveconfiguration",
      request
    );

    const response = await tryExecuteAndNotify(this._host, responsePromise);
    const data: connectionResponse = await response.data?.json();

    return data;
  }

  async getFailedJobs(): Promise<apiResponse<enterspeedJob[]>> {
    const request: RequestInit = {
      method: "get",
    };

    const responsePromise = fetch(
      "/umbraco/enterspeed/api/dashboard/getfailedjobs",
      request
    );

    const response = await tryExecuteAndNotify(this._host, responsePromise);
    const data: apiResponse<enterspeedJob[]> = await response.data?.json();

    return data;
  }

  public async deleteSelectedFailedJobs(
    ids: jobIdsToDelete
  ): Promise<apiResponseBase> {
    const request: RequestInit = {
      method: "post",
      headers: this.getDefaultPostHeader(),
      body: JSON.stringify(ids),
    };

    const responsePromise = fetch(
      "/umbraco/enterspeed/api/dashboard/deletejobs",
      request
    );

    const response = await tryExecuteAndNotify(this._host, responsePromise);
    const data: apiResponse<apiResponseBase> = await response.data?.json();

    return data;
  }

  public async deleteFailedJobs(): Promise<apiResponseBase> {
    const request: RequestInit = {
      method: "post",
      headers: this.getDefaultPostHeader(),
    };

    const responsePromise = fetch(
      "/umbraco/enterspeed/api/dashboard/deletefailedjobs",
      request
    );

    const response = await tryExecuteAndNotify(this._host, responsePromise);
    const data: apiResponse<apiResponseBase> = await response.data?.json();

    return data;
  }

  getDefaultPostHeader(): HeadersInit {
    return {
      "Content-Type": "application/json",
    };
  }
}
