import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, customElement, property } from "@umbraco-cms/backoffice/external/lit";

@customElement('enterspeed-dashboard')
export class enterspeed_dashboard extends UmbElementMixin(LitElement) {

    constructor() {
        super();
    }

    @property()
    title = 'Enterspeed.Source.UmbracoCms dashboard'

    render() {
        return html`
            <uui-box headline="${this.title}">
                dashboard content goes here
            </uui-box>
        `
    }

    static styles = css`
        :host {
            display: block;
            padding: 20px;
        }
    `
}


export default enterspeed_dashboard;

declare global {
    interface HtmlElementTagNameMap {
        'enterspeed_dashboard': enterspeed_dashboard
    }
}