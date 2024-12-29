using System.Collections.Generic;

namespace Mag3DView.Nzy3dAPI.Maths.Algorithms.Interpolation.Bernstein
{
	public sealed class BernsteinInterpolator : IInterpolator
	{
		public List<Coord3d> Interpolate(List<Coord3d> controlpoints, int resolution)
		{
			Spline3D spline = new Spline3D(controlpoints);
			return spline.ComputeVertices(resolution);
		}
	}
}
