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

export interface customSeedNodes {
  contentNodes: customSeedNode[];
  mediaNodes: customSeedNode[];
  dictionaryNodes: customSeedNode[];
}

export interface customSeedNode {
  id: number;
  includeDescendants: boolean;
}

export interface apiResponseBase {
  isSuccess: boolean;
  errorCode: string;
}

export interface apiResponse<T> extends apiResponseBase {
  data: T;
}
