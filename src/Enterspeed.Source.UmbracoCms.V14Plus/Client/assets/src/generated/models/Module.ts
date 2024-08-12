/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { Assembly } from './Assembly';
import type { CustomAttributeData } from './CustomAttributeData';
import type { ModuleHandle } from './ModuleHandle';

export type Module = {
    readonly assembly: Assembly;
    readonly fullyQualifiedName: string;
    readonly name: string;
    readonly mdStreamVersion: number;
    readonly moduleVersionId: string;
    readonly scopeName: string;
    readonly moduleHandle: ModuleHandle;
    readonly customAttributes: Array<CustomAttributeData>;
    readonly metadataToken: number;
};
