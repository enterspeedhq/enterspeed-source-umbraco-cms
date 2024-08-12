/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { CustomAttributeData } from './CustomAttributeData';
import type { MemberTypes } from './MemberTypes';
import type { Module } from './Module';
import type { Type } from './Type';

export type MemberInfo = {
    memberType: MemberTypes;
    readonly name: string;
    readonly declaringType?: Type | null;
    readonly reflectedType?: Type | null;
    readonly module: Module;
    readonly customAttributes: Array<CustomAttributeData>;
    readonly isCollectible: boolean;
    readonly metadataToken: number;
};
