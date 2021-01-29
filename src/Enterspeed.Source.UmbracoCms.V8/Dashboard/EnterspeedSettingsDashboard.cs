using System;
using Umbraco.Core.Composing;
using Umbraco.Core.Dashboards;

namespace Enterspeed.Source.UmbracoCms.V8.Dashboard
{
    [Weight(30)]
    public class EnterspeedSettingsDashboard : IDashboard
    {
        public string Alias => "enterspeedSettingsDashboard";
        public string View => "/App_Plugins/Enterspeed.Dashboard/settings-dashboard.html";
        public string[] Sections => new[] { "Settings" };

        public IAccessRule[] AccessRules => Array.Empty<IAccessRule>();
    }
}
