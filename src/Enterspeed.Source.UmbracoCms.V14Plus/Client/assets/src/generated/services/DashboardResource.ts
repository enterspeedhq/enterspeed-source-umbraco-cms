/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { ApiResponse } from '../models/ApiResponse';
import type { ApiResponse_EnterspeedUmbracoConfigurationResponse_ } from '../models/ApiResponse_EnterspeedUmbracoConfigurationResponse_';
import type { ApiResponse_GetNumberOfPendingJobsResponse_ } from '../models/ApiResponse_GetNumberOfPendingJobsResponse_';
import type { ApiResponse_List_1_ } from '../models/ApiResponse_List_1_';
import type { ApiResponse_SeedResponse_ } from '../models/ApiResponse_SeedResponse_';
import type { CustomSeedModel } from '../models/CustomSeedModel';
import type { EnterspeedUmbracoConfiguration } from '../models/EnterspeedUmbracoConfiguration';
import type { JobIdsToDelete } from '../models/JobIdsToDelete';
import type { Response } from '../models/Response';

import type { CancelablePromise } from '../core/CancelablePromise';
import { OpenAPI } from '../core/OpenAPI';
import { request as __request } from '../core/request';

export class DashboardResource {

    /**
     * @returns any OK
     * @throws ApiError
     */
    public static clearPendingJobs(): CancelablePromise<ApiResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/umbraco/enterspeed/api/v1/ClearPendingJobs',
        });
    }

    /**
     * @returns any OK
     * @throws ApiError
     */
    public static customSeed({
requestBody,
}: {
requestBody?: CustomSeedModel,
}): CancelablePromise<ApiResponse_SeedResponse_> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/umbraco/enterspeed/api/v1/CustomSeed',
            body: requestBody,
            mediaType: 'application/json',
        });
    }

    /**
     * @returns any OK
     * @throws ApiError
     */
    public static deleteFailedJobs(): CancelablePromise<ApiResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/umbraco/enterspeed/api/v1/DeleteFailedJobs',
        });
    }

    /**
     * @returns any OK
     * @throws ApiError
     */
    public static deleteJobs({
requestBody,
}: {
requestBody?: JobIdsToDelete,
}): CancelablePromise<ApiResponse> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/umbraco/enterspeed/api/v1/DeleteJobs',
            body: requestBody,
            mediaType: 'application/json',
        });
    }

    /**
     * @returns any OK
     * @throws ApiError
     */
    public static getEnterspeedConfiguration(): CancelablePromise<ApiResponse_EnterspeedUmbracoConfigurationResponse_> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/umbraco/enterspeed/api/v1/GetEnterspeedConfiguration',
        });
    }

    /**
     * @returns any OK
     * @throws ApiError
     */
    public static getFailedJobs(): CancelablePromise<ApiResponse_List_1_> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/umbraco/enterspeed/api/v1/GetFailedJobs',
        });
    }

    /**
     * @returns any OK
     * @throws ApiError
     */
    public static getNumberOfPendingJobs(): CancelablePromise<ApiResponse_GetNumberOfPendingJobsResponse_> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/umbraco/enterspeed/api/v1/GetNumberOfPendingJobs',
        });
    }

    /**
     * @returns any OK
     * @throws ApiError
     */
    public static saveConfiguration({
requestBody,
}: {
requestBody?: EnterspeedUmbracoConfiguration,
}): CancelablePromise<Response> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/umbraco/enterspeed/api/v1/SaveConfiguration',
            body: requestBody,
            mediaType: 'application/json',
        });
    }

    /**
     * @returns any OK
     * @throws ApiError
     */
    public static seed(): CancelablePromise<ApiResponse_SeedResponse_> {
        return __request(OpenAPI, {
            method: 'GET',
            url: '/umbraco/enterspeed/api/v1/Seed',
        });
    }

    /**
     * @returns any OK
     * @throws ApiError
     */
    public static testConfigurationConnection({
requestBody,
}: {
requestBody?: EnterspeedUmbracoConfiguration,
}): CancelablePromise<Response> {
        return __request(OpenAPI, {
            method: 'POST',
            url: '/umbraco/enterspeed/api/v1/TestConfigurationConnection',
            body: requestBody,
            mediaType: 'application/json',
        });
    }

}
