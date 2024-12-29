using Mag3DView.Nzy3dAPI.Maths;
using System;

namespace Mag3DView.Nzy3dAPI.Plot3D.Primitives.Axes.Layout.Renderers
{
	/// <summary>
	/// Force number to be represented with a given number of decimals
	/// </summary>
	public class DateTickRenderer : ITickRenderer
	{
		internal string _format;
		public DateTickRenderer() : this("dd/MM/yyyy HH:mm:ss")
		{
		}

		public DateTickRenderer(string format)
		{
			_format = format;
		}

		public string Format(double value)
		{
			DateTime ldate = Utils.Num2date(value);
			return Utils.Dat2str(ldate, _format);
		}
	}
}