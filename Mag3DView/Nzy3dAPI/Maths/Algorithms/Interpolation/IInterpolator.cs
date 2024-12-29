using System.Collections.Generic;

namespace Mag3DView.Nzy3dAPI.Maths.Algorithms.Interpolation
{
	public interface IInterpolator
	{
		List<Coord3d> Interpolate(List<Coord3d> controlpoints, int resolution);
	}
}
