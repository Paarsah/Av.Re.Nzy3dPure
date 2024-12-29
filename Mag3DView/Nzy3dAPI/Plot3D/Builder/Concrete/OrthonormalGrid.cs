using Mag3DView.Nzy3dAPI.Maths;
using System.Collections.Generic;
using Range = Mag3DView.Nzy3dAPI.Maths.Range;

namespace Mag3DView.Nzy3dAPI.Plot3D.Builder.Concrete
{
	public class OrthonormalGrid : Grid
	{
		public OrthonormalGrid(Range xrange, int xsteps, Range yrange, int ysteps) : base(xrange, xsteps, yrange, ysteps)
		{
		}

		public OrthonormalGrid(Range xyrange, int xysteps) : base(xyrange, xysteps)
		{
		}

		public override List<Coord3d> Apply(Mapper mapper)
		{
			double xstep = XRange.Range / (XSteps - 1);
			double ystep = YRange.Range / (YSteps - 1);
			var output = new List<Coord3d>();

			for (int xi = 0; xi <= XSteps - 1; xi++)
			{
				for (int yi = 0; yi <= YSteps - 1; yi++)
				{
					double x = XRange.Min + xi * xstep;
					double y = YRange.Min + yi * ystep;
					output.Add(new Coord3d(x, y, mapper.f(x, y)));
				}
			}

			return output;
		}
	}
}
