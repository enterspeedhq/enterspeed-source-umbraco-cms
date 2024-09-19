using Newtonsoft.Json.Linq;

namespace Enterspeed.Source.UmbracoCms.Base.Models.Grid
{
    public class GridControl
    {
        public string Alias => Editor.Value<string>("alias");
        public JToken Value { get; }
        public JToken Editor { get; }
        public JToken Styles { get; }
        public JToken Config { get; }

        public GridControl(JToken control)
        {
            Value = control.Value<JToken>("value");
            Editor = control.Value<JToken>("editor");
            Styles = control.Value<JToken>("styles");
            Config = control.Value<JToken>("config");
        }
    }
}
