using System;
using System.IO;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Dashboards;

namespace Enterspeed.Source.UmbracoCms.V9Plus.Dashboard
{
    [Weight(30)]
    public class EnterspeedSettingsDashboard : IDashboard
    {
        public string Alias => "enterspeedSettingsDashboard";
        public string View => Path.Combine("/", "App_Plugins", "Enterspeed.Dashboard", "settings-dashboard.html");
        public string[] Sections => new[] { "Settings" };

        public IAccessRule[] AccessRules => Array.Empty<IAccessRule>();
    }
}
