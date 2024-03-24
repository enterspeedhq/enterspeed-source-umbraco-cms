import { customSeedNodes, seedResponse } from "../types";

export class SeedResource {
  public static async seed(): Promise<seedResponse> {
    const request: RequestInit = {
      method: "get",
    };

    const response = await fetch("/umbraco/enterspeed/api/seed", request);
    const data: seedResponse = await response.json();

    return data;
  }

  public static async customSeed(
    customSeed: customSeedNodes
  ): Promise<seedResponse> {
    const request: RequestInit = {
      method: "get",
      body: JSON.stringify(customSeed),
    };

    const response = await fetch("/umbraco/enterspeed/api/customSeed", request);
    const data: seedResponse = await response.json();

    return data;
  }
}
