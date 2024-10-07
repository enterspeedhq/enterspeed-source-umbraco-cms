/* generated using openapi-typescript-codegen -- do no edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */

import type { EnterspeedContentState } from './EnterspeedContentState';
import type { EnterspeedJobEntityType } from './EnterspeedJobEntityType';
import type { EnterspeedJobState } from './EnterspeedJobState';
import type { EnterspeedJobType } from './EnterspeedJobType';

export type EnterspeedJob = {
    id: number;
    entityId?: string | null;
    culture?: string | null;
    jobType: EnterspeedJobType;
    createdAt: string;
    state: EnterspeedJobState;
    exception?: string | null;
    updatedAt: string;
    entityType: EnterspeedJobEntityType;
    contentState: EnterspeedContentState;
};
