using Mag3DView.Nzy3dAPI.Maths;
using System;
using System.Collections.Generic;

namespace Mag3DView.Nzy3dAPI.Plot3D.Builder.Concrete
{
    public class SphereScatterGenerator
	{
		public static object Generate(Coord3d center, double radius, int steps, bool half)
		{
			var coords = new List<Coord3d>();
			double inc = Math.PI / steps;
			double i = 0;
			int jrat = (half ? 1 : 2);
			while (i < (2 * Math.PI))
			{
				double j = 0;
				while (j < (jrat * Math.PI))
				{
					var c = new Coord3d(i, j, radius).Cartesian();
					if (center != null)
					{
						c.X += center.X;
						c.Y += center.Y;
						c.Z += center.Z;
					}
					coords.Add(c);
					j += inc;
				}
				i += inc;
			}
			return coords;
		}

		public static object Generate(Coord3d center, double radius, int steps)
		{
			return Generate(center, radius, steps, false);
		}

		public static object Generate(double radius, int steps)
		{
			return Generate(null, radius, steps, false);
		}
	}
}

//=======================================================
//Service provided by Telerik (www.telerik.com)
//Conversion powered by NRefactory.
//Twitter: @telerik
//Facebook: facebook.com/telerik
//=======================================================
