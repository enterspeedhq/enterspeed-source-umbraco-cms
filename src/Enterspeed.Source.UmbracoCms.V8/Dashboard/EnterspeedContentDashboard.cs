using System;
using Umbraco.Core.Composing;
using Umbraco.Core.Dashboards;

namespace Enterspeed.Source.UmbracoCms.V8.Dashboard
{
    [Weight(30)]
    public class EnterspeedContentDashboard : IDashboard
    {
        public string Alias => "enterspeedDashboard";
        public string View => "/App_Plugins/Enterspeed.Dashboard/dashboard.html";
        public string[] Sections => new[] { "Content" };

        public IAccessRule[] AccessRules => Array.Empty<IAccessRule>();
    }
}
