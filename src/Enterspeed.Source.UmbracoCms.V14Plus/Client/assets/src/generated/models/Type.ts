/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { Assembly } from './Assembly';
import type { ConstructorInfo } from './ConstructorInfo';
import type { CustomAttributeData } from './CustomAttributeData';
import type { GenericParameterAttributes } from './GenericParameterAttributes';
import type { MemberTypes } from './MemberTypes';
import type { MethodBase } from './MethodBase';
import type { Module } from './Module';
import type { RuntimeTypeHandle } from './RuntimeTypeHandle';
import type { StructLayoutAttribute } from './StructLayoutAttribute';
import type { TypeAttributes } from './TypeAttributes';

export type Type = {
    readonly name: string;
    readonly customAttributes: Array<CustomAttributeData>;
    readonly isCollectible: boolean;
    readonly metadataToken: number;
    readonly isInterface: boolean;
    memberType: MemberTypes;
    readonly namespace?: string | null;
    readonly assemblyQualifiedName?: string | null;
    readonly fullName?: string | null;
    readonly assembly: Assembly;
    readonly module: Module;
    readonly isNested: boolean;
    readonly declaringType?: Type | null;
    readonly declaringMethod?: MethodBase | null;
    readonly reflectedType?: Type | null;
    readonly underlyingSystemType: Type;
    readonly isTypeDefinition: boolean;
    readonly isArray: boolean;
    readonly isByRef: boolean;
    readonly isPointer: boolean;
    readonly isConstructedGenericType: boolean;
    readonly isGenericParameter: boolean;
    readonly isGenericTypeParameter: boolean;
    readonly isGenericMethodParameter: boolean;
    readonly isGenericType: boolean;
    readonly isGenericTypeDefinition: boolean;
    readonly isSZArray: boolean;
    readonly isVariableBoundArray: boolean;
    readonly isByRefLike: boolean;
    readonly isFunctionPointer: boolean;
    readonly isUnmanagedFunctionPointer: boolean;
    readonly hasElementType: boolean;
    readonly genericTypeArguments: Array<Type>;
    readonly genericParameterPosition: number;
    genericParameterAttributes: GenericParameterAttributes;
    attributes: TypeAttributes;
    readonly isAbstract: boolean;
    readonly isImport: boolean;
    readonly isSealed: boolean;
    readonly isSpecialName: boolean;
    readonly isClass: boolean;
    readonly isNestedAssembly: boolean;
    readonly isNestedFamANDAssem: boolean;
    readonly isNestedFamily: boolean;
    readonly isNestedFamORAssem: boolean;
    readonly isNestedPrivate: boolean;
    readonly isNestedPublic: boolean;
    readonly isNotPublic: boolean;
    readonly isPublic: boolean;
    readonly isAutoLayout: boolean;
    readonly isExplicitLayout: boolean;
    readonly isLayoutSequential: boolean;
    readonly isAnsiClass: boolean;
    readonly isAutoClass: boolean;
    readonly isUnicodeClass: boolean;
    readonly isCOMObject: boolean;
    readonly isContextful: boolean;
    readonly isEnum: boolean;
    readonly isMarshalByRef: boolean;
    readonly isPrimitive: boolean;
    readonly isValueType: boolean;
    readonly isSignatureType: boolean;
    readonly isSecurityCritical: boolean;
    readonly isSecuritySafeCritical: boolean;
    readonly isSecurityTransparent: boolean;
    readonly structLayoutAttribute?: StructLayoutAttribute | null;
    readonly typeInitializer?: ConstructorInfo | null;
    readonly typeHandle: RuntimeTypeHandle;
    readonly guid: string;
    readonly baseType?: Type | null;
    /**
     * @deprecated
     */
    readonly isSerializable: boolean;
    readonly containsGenericParameters: boolean;
    readonly isVisible: boolean;
};
