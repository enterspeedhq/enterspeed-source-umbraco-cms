import { UmbContextBase } from "@umbraco-cms/backoffice/class-api";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";
import { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";

export class EnterspeedContext extends UmbContextBase<EnterspeedContext> {
  #api = new Object();

  constructor(host: UmbControllerHost) {
    super(host, ENTERSPEED_CONTEXT);
  }
}

export const ENTERSPEED_CONTEXT = new UmbContextToken<EnterspeedContext>(
  EnterspeedContext.name
);
