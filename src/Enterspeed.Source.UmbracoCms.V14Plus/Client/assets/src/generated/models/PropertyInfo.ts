/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { CustomAttributeData } from './CustomAttributeData';
import type { MemberTypes } from './MemberTypes';
import type { MethodInfo } from './MethodInfo';
import type { Module } from './Module';
import type { PropertyAttributes } from './PropertyAttributes';
import type { Type } from './Type';

export type PropertyInfo = {
    readonly name: string;
    readonly declaringType?: Type | null;
    readonly reflectedType?: Type | null;
    readonly module: Module;
    readonly customAttributes: Array<CustomAttributeData>;
    readonly isCollectible: boolean;
    readonly metadataToken: number;
    memberType: MemberTypes;
    readonly propertyType: Type;
    attributes: PropertyAttributes;
    readonly isSpecialName: boolean;
    readonly canRead: boolean;
    readonly canWrite: boolean;
    readonly getMethod?: MethodInfo | null;
    readonly setMethod?: MethodInfo | null;
};
