using Mag3DView.Nzy3dAPI.Plot3D.Builder.Delaunay.Jdt;
using System.Collections.Generic;

namespace Mag3DView.Nzy3dAPI.Plot3D.Builder.Delaunay
{
    public interface ITriangulation
	{
		void InsertPoint(Point_dt p);

		IEnumerator<Triangle_dt> TrianglesIterator();

		IEnumerator<Point_dt> VerticesIterator();

		int TrianglesSize();
	}
}