/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { ConstructorInfo } from './ConstructorInfo';
import type { CustomAttributeNamedArgument } from './CustomAttributeNamedArgument';
import type { CustomAttributeTypedArgument } from './CustomAttributeTypedArgument';
import type { Type } from './Type';

export type CustomAttributeData = {
    readonly attributeType: Type;
    readonly constructor: ConstructorInfo;
    readonly constructorArguments: Array<CustomAttributeTypedArgument>;
    readonly namedArguments: Array<CustomAttributeNamedArgument>;
};
