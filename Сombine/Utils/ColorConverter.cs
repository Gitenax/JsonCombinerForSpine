using System.Windows.Media;

namespace Сombine.Utils
{
    internal static class ColorConverter
    {
        public static string ToHexString(this Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") + color.A.ToString("X2");
        }
    }
}