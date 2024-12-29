using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Builder.Delaunay.Jdt;
using Mag3DView.Nzy3dAPI.Plot3D.Primitives;
using System.Collections.Generic;

namespace Mag3DView.Nzy3dAPI.Plot3D.Builder.Delaunay
{
	public class DelaunayTessellator : Tessellator
	{
		public AbstractComposite Build(List<Coord3d> Coordinates)
		{
			return this.Build(new Coordinates(Coordinates));
		}

		public AbstractComposite Build(Coordinates coord)
		{
			ICoordinateValidator cv = new DelaunayCoordinateValidator(coord);
			Delaunay_Triangulation dt = new Delaunay_Triangulation();
			DelaunayTriangulationManager tesselator = new DelaunayTriangulationManager(cv, dt);
			return (Shape)tesselator.BuildDrawable();
		}

		public override AbstractComposite Build(float[] x, float[] y, float[] z)
		{
			return this.Build(new Coordinates(x, y, z));
		}
	}
}
