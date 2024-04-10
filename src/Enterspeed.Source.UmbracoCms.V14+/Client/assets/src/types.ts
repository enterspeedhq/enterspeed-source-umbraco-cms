import { EnterspeedJob } from "./generated";

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

export interface connectionResponse {
  success: boolean;
  message: string;
  exception: string;
  errors: { [key: string]: string };
  errorCode: string;
  content: string;
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
  configuration: configuration;
}

export interface configuration {
  mediaDomain: string | null;
  isConfigured: boolean;
  configuredFromSettingsFile: boolean;
  previewApiKey: string | null;
  apiKey: string;
  baseUrl: string;
}

export interface EnterspeedFailedJob extends EnterspeedJob {
  selected: boolean;
}

export class jobIdsToDelete {
  constructor(jobIds: number[]) {
    this.ids = jobIds;
  }

  ids: number[];
}
