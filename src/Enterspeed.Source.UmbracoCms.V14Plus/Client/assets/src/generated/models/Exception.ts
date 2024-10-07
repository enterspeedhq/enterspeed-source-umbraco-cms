/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { MethodBase } from './MethodBase';

export type Exception = {
    readonly targetSite?: MethodBase | null;
    readonly message: string;
    readonly data: Record<string, any>;
    readonly innerException?: Exception | null;
    helpLink?: string | null;
    source?: string | null;
    hResult: number;
    readonly stackTrace?: string | null;
};
