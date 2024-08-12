using System;
using System.IO;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Dashboards;

namespace Enterspeed.Source.UmbracoCms.V9Plus.Dashboard
{
    [Weight(30)]
    public class EnterspeedContentDashboard : IDashboard
    {
        public string Alias => "enterspeedDashboard";
        public string View => Path.Combine("/", "App_Plugins", "Enterspeed.Dashboard", "dashboard.html");
        public string[] Sections => new[] { "Content" };

        public IAccessRule[] AccessRules => Array.Empty<IAccessRule>();
    }
}
