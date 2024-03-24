export type seedResponse = {
  jobsAdded: number;
  NumberOfPendingJobs: number;
  ContentCount: number;
  DictionaryCount: number;
  MediaCount: number;
};

export type customSeedNodes = {
  contentNodes: customSeedNode[];
  mediaNodes: customSeedNode[];
  dictionaryNodes: customSeedNode[];
};

export type customSeedNode = {
  id: number;
  includeDescendants: boolean;
};
