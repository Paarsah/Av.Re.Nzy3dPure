using Mag3DView.Nzy3dAPI.Colors;
using System;

namespace Mag3DView.Nzy3dAPI.Colors.ColorMaps
{
    public class ColorMapJet : IColorMap
    {
        public bool Direction { get; set; } = true;

        public Color GetColor(IColorMappable mappable, double value)
        {
            double min = mappable.GetMin();
            double max = mappable.GetMax();
            return GetColor(mappable, value, min, max);
        }

        public Color GetColor(IColorMappable mappable, double value, double min, double max)
        {
            float ratio = (float)((value - min) / (max - min));
            float r = Math.Max(0, Math.Min(1, 1.5f - Math.Abs(4 * ratio - 3)));
            float g = Math.Max(0, Math.Min(1, 1.5f - Math.Abs(4 * ratio - 2)));
            float b = Math.Max(0, Math.Min(1, 1.5f - Math.Abs(4 * ratio - 1)));
            return new Color(r, g, b);
        }
    }
}
