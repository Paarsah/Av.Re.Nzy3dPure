using Mag3DView.Nzy3dAPI.Maths;
using System.Collections.Generic;
using Range = Mag3DView.Nzy3dAPI.Maths.Range;

namespace Mag3DView.Nzy3dAPI.Plot3D.Builder.Concrete
{
    public class RingGrid : Grid
	{
		internal double sqradius;
		public RingGrid(double radius, int xysteps, int enlargeSteps) : base(new Range(-radius - (enlargeSteps * radius / xysteps), radius + (enlargeSteps * radius / xysteps)), xysteps)
		{
			sqradius = (radius + (enlargeSteps * radius / xysteps)) * (radius + (enlargeSteps * radius / xysteps));
		}

		public RingGrid(double radius, int xysteps) : this(radius, xysteps, 0)
		{
		}

		public override List<Coord3d> Apply(Mapper mapper)
		{
			double xstep = XRange.Range / XSteps;
			double ystep = YRange.Range / YSteps;
			var output = new List<Coord3d>();

			for (int xi = -(XSteps - 1) / 2; xi <= (XSteps - 1) / 2; xi++)
			{
				for (int yi = -(YSteps - 1) / 2; yi <= (YSteps - 1) / 2; yi++)
				{
					double x = xi * xstep;
					double y = yi * ystep;
					if (sqradius > x * x + y * y)
					{
						output.Add(new Coord3d(x, y, mapper.f(x, y)));
					}
				}
			}

			return output;
		}
	}
}

//=======================================================
//Service provided by Telerik (www.telerik.com)
//Conversion powered by NRefactory.
//Twitter: @telerik
//Facebook: facebook.com/telerik
//=======================================================
