/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { CustomAttributeData } from './CustomAttributeData';
import type { EventAttributes } from './EventAttributes';
import type { MemberTypes } from './MemberTypes';
import type { MethodInfo } from './MethodInfo';
import type { Module } from './Module';
import type { Type } from './Type';

export type EventInfo = {
    readonly name: string;
    readonly declaringType?: Type | null;
    readonly reflectedType?: Type | null;
    readonly module: Module;
    readonly customAttributes: Array<CustomAttributeData>;
    readonly isCollectible: boolean;
    readonly metadataToken: number;
    memberType: MemberTypes;
    attributes: EventAttributes;
    readonly isSpecialName: boolean;
    readonly addMethod?: MethodInfo | null;
    readonly removeMethod?: MethodInfo | null;
    readonly raiseMethod?: MethodInfo | null;
    readonly isMulticast: boolean;
    readonly eventHandlerType?: Type | null;
};
