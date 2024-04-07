/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { EnterspeedConfiguration } from './EnterspeedConfiguration';

export type EnterspeedUmbracoConfiguration = (EnterspeedConfiguration & {
mediaDomain?: string | null;
isConfigured: boolean;
configuredFromSettingsFile: boolean;
previewApiKey?: string | null;
rootDictionariesDisabled: boolean;
runJobsOnAllServerRoles: boolean;
enableMasterContent: boolean;
});
