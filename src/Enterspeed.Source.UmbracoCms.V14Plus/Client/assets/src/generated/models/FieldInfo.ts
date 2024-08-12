/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { CustomAttributeData } from './CustomAttributeData';
import type { FieldAttributes } from './FieldAttributes';
import type { MemberTypes } from './MemberTypes';
import type { Module } from './Module';
import type { RuntimeFieldHandle } from './RuntimeFieldHandle';
import type { Type } from './Type';

export type FieldInfo = {
    readonly name: string;
    readonly declaringType?: Type | null;
    readonly reflectedType?: Type | null;
    readonly module: Module;
    readonly customAttributes: Array<CustomAttributeData>;
    readonly isCollectible: boolean;
    readonly metadataToken: number;
    memberType: MemberTypes;
    attributes: FieldAttributes;
    readonly fieldType: Type;
    readonly isInitOnly: boolean;
    readonly isLiteral: boolean;
    /**
     * @deprecated
     */
    readonly isNotSerialized: boolean;
    readonly isPinvokeImpl: boolean;
    readonly isSpecialName: boolean;
    readonly isStatic: boolean;
    readonly isAssembly: boolean;
    readonly isFamily: boolean;
    readonly isFamilyAndAssembly: boolean;
    readonly isFamilyOrAssembly: boolean;
    readonly isPrivate: boolean;
    readonly isPublic: boolean;
    readonly isSecurityCritical: boolean;
    readonly isSecuritySafeCritical: boolean;
    readonly isSecurityTransparent: boolean;
    readonly fieldHandle: RuntimeFieldHandle;
};
