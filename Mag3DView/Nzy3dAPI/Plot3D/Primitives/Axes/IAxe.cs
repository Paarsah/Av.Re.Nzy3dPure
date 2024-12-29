using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Primitives.Axes.Layout;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;

namespace Mag3DView.Nzy3dAPI.Plot3D.Primitives.Axes
{
	public interface IAxe
	{
		void Dispose();

		void SetAxe(BoundingBox3d box);

		void Draw(Camera camera);

		void SetScale(Coord3d scale);

		BoundingBox3d GetBoxBounds();

		BoundingBox3d GetWholeBounds();

		Coord3d GetCenter();

		IAxeLayout Layout { get; }
	}
}
