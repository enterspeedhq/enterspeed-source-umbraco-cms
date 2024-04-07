/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { EnterspeedUmbracoConfiguration } from './EnterspeedUmbracoConfiguration';
import type { ServerRoleModel } from './ServerRoleModel';

export type EnterspeedUmbracoConfigurationResponse = (EnterspeedUmbracoConfiguration & {
serverRole: ServerRoleModel;
runJobsOnServer: boolean;
readonly configuration?: (EnterspeedUmbracoConfiguration | EnterspeedUmbracoConfigurationResponse) | null;
});
