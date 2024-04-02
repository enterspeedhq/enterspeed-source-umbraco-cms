export interface getNumberOfPendingJobsResponse {
  numberOfPendingJobs: number;
}

export interface seedResponse {
  jobsAdded: number;
  numberOfPendingJobs: number;
  contentCount: number;
  dictionaryCount: number;
  mediaCount: number;
}

export class customSeedNodes {
  contentNodes: string[] | undefined;
  mediaNodes: string[] | undefined;
  dictionaryNodes: string[] | undefined;
}

export interface apiResponseBase {
  isSuccess: boolean;
  errorCode: string;
}

export interface apiResponse<T> extends apiResponseBase {
  data: T;
}

export type tab = {
  alias: string;
  label: string;
  active?: boolean;
};

export enum serverRole {
  Unknown = 0,
  Single = 1,
  Subscriber = 2,
  SchedulingPublisher = 3,
}

export interface enterspeedUmbracoConfigurationResponse {
  serverRole: serverRole;
  runJobsOnServer: boolean;
  configuration : configuration
}

export interface configuration {
  mediaDomain: string | null;
  isConfigured: boolean;
  configuredFromSettingsFile: boolean;
  previewApiKey: string | null;
  apiKey: string;
  baseUrl: string;
}

enum enterspeedJobType {
  Publish = 0,
  Delete = 1,
}

enum enterspeedJobEntityType {
  Content,
  MasterContent,
  Media,
  Dictionary,
}

export interface enterspeedJob {
  id: number;
  entityId: string;
  culture: string;
  jobType: enterspeedJobType;
  exception: string;
  entityType: enterspeedJobEntityType;
  createdAt: Date;
  updatedAt: Date;
  selected: boolean;
}

export class jobIdsToDelete {
  constructor(jobIds: number[]) {
    this.ids = jobIds;
  }

  ids: number[];
}
