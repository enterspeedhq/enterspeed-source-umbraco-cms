import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, customElement, property } from "@umbraco-cms/backoffice/external/lit";

@customElement('enterspeed_source_umbracocms_v14_-dashboard')
export class Enterspeed_Source_UmbracoCms_V14_Dashboard extends UmbElementMixin(LitElement) {

    constructor() {
        super();
    }

    @property()
    title = 'Enterspeed.Source.UmbracoCms.V14+ dashboard'

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


export default Enterspeed_Source_UmbracoCms_V14_Dashboard;

declare global {
    interface HtmlElementTagNameMap {
        'Enterspeed.Source.UmbracoCms.V14+-dashboard': Enterspeed_Source_UmbracoCms_V14_Dashboard
    }
}