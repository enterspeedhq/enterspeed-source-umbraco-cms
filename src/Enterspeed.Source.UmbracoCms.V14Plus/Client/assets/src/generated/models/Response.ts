/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { Exception } from './Exception';
import type { HttpStatusCode } from './HttpStatusCode';

export type Response = {
    status: HttpStatusCode;
    readonly statusCode: number;
    message?: string | null;
    exception?: Exception | null;
    success: boolean;
    errors?: Record<string, string | null> | null;
    errorCode?: string | null;
    content?: string | null;
};
