using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;
using System.Collections.Generic;

namespace Mag3DView.Nzy3dAPI.Plot3D.Primitives
{
	public interface ISelectable
	{
		void Project(Camera cam);

		List<Coord3d> LastProjection { get; }
	}
}
