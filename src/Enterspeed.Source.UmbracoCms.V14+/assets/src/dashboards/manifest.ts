import type { ManifestDashboard } from "@umbraco-cms/backoffice/extension-registry";

const dashboards: Array<ManifestDashboard> = [
    {
        type: 'dashboard',
        name: 'Enterspeed.Source.UmbracoCms.V14+',
        alias: 'Enterspeed.Source.UmbracoCms.V14+.dashboard',
        elementName: 'enterspeed_source_umbracocms_v14_-dashboard',
        js: ()=> import('./dashboard.element.js'),
        weight: -10,
        meta: {
            label: 'Enterspeed.Source.UmbracoCms.V14+',
            pathname: 'Enterspeed.Source.UmbracoCms.V14+'
        },
        conditions: [
            {
                alias: 'Umb.Condition.SectionAlias',
                match: 'Umb.Section.Content'
            }
        ]
    }
]

export const manifests = [...dashboards];