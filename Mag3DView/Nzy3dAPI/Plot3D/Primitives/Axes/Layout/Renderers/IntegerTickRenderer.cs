using System;

namespace Mag3DView.Nzy3dAPI.Plot3D.Primitives.Axes.Layout.Renderers
{
	public class IntegerTickRenderer : ITickRenderer
	{
		public string Format(double value)
		{
			return Convert.ToInt32(value).ToString();
		}
	}
}
