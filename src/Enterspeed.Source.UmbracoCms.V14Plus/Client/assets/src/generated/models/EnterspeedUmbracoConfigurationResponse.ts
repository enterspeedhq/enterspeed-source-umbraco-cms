/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { EnterspeedUmbracoConfiguration } from './EnterspeedUmbracoConfiguration';
import type { ServerRoleModel } from './ServerRoleModel';

export type EnterspeedUmbracoConfigurationResponse = {
    apiKey?: string | null;
    baseUrl?: string | null;
    connectionTimeout: number;
    readonly ingestVersion?: string | null;
    systemInformation?: string | null;
    mediaDomain?: string | null;
    isConfigured: boolean;
    configuredFromSettingsFile: boolean;
    previewApiKey?: string | null;
    rootDictionariesDisabled: boolean;
    runJobsOnAllServerRoles: boolean;
    enableMasterContent: boolean;
    serverRole: ServerRoleModel;
    runJobsOnServer: boolean;
    readonly configuration?: EnterspeedUmbracoConfiguration | null;
};
