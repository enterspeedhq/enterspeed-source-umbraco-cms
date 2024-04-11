import { EnterspeedJob } from "./generated";

export interface EnterspeedFailedJob extends EnterspeedJob {
  selected: boolean;
}

export type tab = {
  alias: string;
  label: string;
  active?: boolean;
};