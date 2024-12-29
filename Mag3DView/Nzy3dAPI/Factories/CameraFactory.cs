using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;

namespace Mag3DView.Nzy3dAPI.Factories
{
	public class CameraFactory
	{
		public static Camera GetInstance(Coord3d center)
		{
			return new Camera(center);
		}
	}
}
