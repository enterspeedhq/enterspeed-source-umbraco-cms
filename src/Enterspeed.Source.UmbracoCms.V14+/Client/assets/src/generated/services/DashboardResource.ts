/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ApiResponse } from '../models/ApiResponse';
import type { CustomSeedModel } from '../models/CustomSeedModel';
import type { Enterspeed_Source_UmbracoCms_Models_Api_ApiResponse_1 } from '../models/Enterspeed_Source_UmbracoCms_Models_Api_ApiResponse_1';
import type { Enterspeed_Source_UmbracoCms_Models_Api_GetNumberOfPendingJobsResponse } from '../models/Enterspeed_Source_UmbracoCms_Models_Api_GetNumberOfPendingJobsResponse';
import type { Enterspeed_Source_UmbracoCms_Models_Api_SeedResponse } from '../models/Enterspeed_Source_UmbracoCms_Models_Api_SeedResponse';
import type { EnterspeedJob } from '../models/EnterspeedJob';
import type { EnterspeedUmbracoConfiguration } from '../models/EnterspeedUmbracoConfiguration';
import type { EnterspeedUmbracoConfigurationResponse } from '../models/EnterspeedUmbracoConfigurationResponse';
import type { JobIdsToDelete } from '../models/JobIdsToDelete';
import type { Response } from '../models/Response';
import type { T } from '../models/T';

import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class DashboardResource {

    /**
     * @returns any Success
     * @throws ApiError
     */
    public static postClearPendingJobs(): CancelablePromise<(ApiResponse | Enterspeed_Source_UmbracoCms_Models_Api_ApiResponse_1)> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/ClearPendingJobs',
        });
    }

    /**
     * @returns Enterspeed_Source_UmbracoCms_Models_Api_ApiResponse_1<Enterspeed_Source_UmbracoCms_Models_Api_SeedResponse> Success
     * @throws ApiError
     */
    public static postCustomSeed({
requestBody,
}: {
requestBody?: CustomSeedModel,
}): CancelablePromise<Enterspeed_Source_UmbracoCms_Models_Api_ApiResponse_1> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/CustomSeed',
            body: requestBody,
            mediaType: 'application/json',
        });
    }

    /**
     * @returns any Success
     * @throws ApiError
     */
    public static postDeleteFailedJobs(): CancelablePromise<(ApiResponse | Enterspeed_Source_UmbracoCms_Models_Api_ApiResponse_1)> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/DeleteFailedJobs',
        });
    }

    /**
     * @returns any Success
     * @throws ApiError
     */
    public static postDeleteJobs({
requestBody,
}: {
requestBody?: JobIdsToDelete,
}): CancelablePromise<(ApiResponse | Enterspeed_Source_UmbracoCms_Models_Api_ApiResponse_1)> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/DeleteJobs',
            body: requestBody,
            mediaType: 'application/json',
        });
    }

    /**
     * @returns any Success
     * @throws ApiError
     */
    public static getGetFailedJobs(): CancelablePromise<Array<EnterspeedJob>> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/GetFailedJobs',
        });
    }

    /**
     * @returns Enterspeed_Source_UmbracoCms_Models_Api_ApiResponse_1<Enterspeed_Source_UmbracoCms_Models_Api_GetNumberOfPendingJobsResponse> Success
     * @throws ApiError
     */
    public static getGetNumberOfPendingJobs(): CancelablePromise<Enterspeed_Source_UmbracoCms_Models_Api_ApiResponse_1> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/GetNumberOfPendingJobs',
        });
    }

    /**
     * @returns Response Success
     * @throws ApiError
     */
    public static postSaveConfiguration({
requestBody,
}: {
requestBody?: (EnterspeedUmbracoConfiguration | EnterspeedUmbracoConfigurationResponse),
}): CancelablePromise<Response> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/SaveConfiguration',
            body: requestBody,
            mediaType: 'application/json',
        });
    }

    /**
     * @returns Enterspeed_Source_UmbracoCms_Models_Api_ApiResponse_1<Enterspeed_Source_UmbracoCms_Models_Api_SeedResponse> Success
     * @throws ApiError
     */
    public static getSeed(): CancelablePromise<Enterspeed_Source_UmbracoCms_Models_Api_ApiResponse_1> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/Seed',
        });
    }

    /**
     * @returns Response Success
     * @throws ApiError
     */
    public static postTestConfigurationConnection({
requestBody,
}: {
requestBody?: (EnterspeedUmbracoConfiguration | EnterspeedUmbracoConfigurationResponse),
}): CancelablePromise<Response> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/TestConfigurationConnection',
            body: requestBody,
            mediaType: 'application/json',
        });
    }

}
