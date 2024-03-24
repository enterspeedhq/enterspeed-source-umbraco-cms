import { customSeedNodes, seedResponse, apiResponse } from "../types";

export class SeedResource {
  public static async seed(): Promise<apiResponse<seedResponse>> {
    const request: RequestInit = {
      method: "get",
    };

    const response = await fetch("/umbraco/enterspeed/api/seed", request);
    const data: apiResponse<seedResponse> = await response.json();

    return data;
  }

  public static async customSeed(
    customSeed: customSeedNodes
  ): Promise<apiResponse<seedResponse>> {
    const request: RequestInit = {
      method: "post",
      body: JSON.stringify(customSeed),
    };

    const response = await fetch("/umbraco/enterspeed/api/customSeed", request);
    const data: apiResponse<seedResponse> = await response.json();

    return data;
  }
}
