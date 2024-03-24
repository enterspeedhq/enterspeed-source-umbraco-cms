export interface seedResponse {
  jobsAdded: number;
  NumberOfPendingJobs: number;
  ContentCount: number;
  DictionaryCount: number;
  MediaCount: number;
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
