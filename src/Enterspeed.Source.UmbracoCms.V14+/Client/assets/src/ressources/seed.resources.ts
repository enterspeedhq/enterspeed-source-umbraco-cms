import { customSeedNodes, seedResponse, apiResponse } from "../types";

export class SeedResource {
  public static async seed(): Promise<apiResponse<seedResponse>> {
    const request: RequestInit = {
      method: "get",
    };

    const response = await fetch("/umbraco/enterspeed/api/dashboard/seed", request);
    const data: apiResponse<seedResponse> = await response.json();

    return data;
  }

  public static async customSeed(
    customSeed: customSeedNodes
  ): Promise<apiResponse<seedResponse>> {
    const request: RequestInit = { 
      method: "get",
      body: JSON.stringify(customSeed),
    };

    const response = await fetch("/umbraco/enterspeed/api/dashboard/customSeed", request);
    const data: apiResponse<seedResponse> = await response.json();

    return data;
  }
}
