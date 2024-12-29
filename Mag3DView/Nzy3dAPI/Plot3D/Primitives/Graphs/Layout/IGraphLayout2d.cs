using Mag3DView.Nzy3dAPI.Maths;
using System.Collections.Generic;

namespace Mag3DView.Nzy3dAPI.Plot3D.Primitives.Graphs.Layout
{
	public interface IGraphLayout2d<V>
	{
		Coord2d VertexPosition { get; set; }

		Coord2d GetV(V v);

		List<Coord2d> Values();
	}
}
