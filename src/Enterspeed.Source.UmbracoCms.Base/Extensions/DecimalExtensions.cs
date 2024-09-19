using System.Globalization;

namespace Enterspeed.Source.UmbracoCms.Base.Extensions
{
    public static class DecimalExtensions
    {
        public static double ToDouble(this decimal value)
        {
            var number = 0d;

            if (double.TryParse(value.ToString(CultureInfo.InvariantCulture), out var n))
            {
                number = n;
            }

            return number;
        }
    }
}