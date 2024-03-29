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
