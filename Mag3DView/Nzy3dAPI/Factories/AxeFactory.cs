using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Primitives.Axes;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;

namespace Mag3DView.Nzy3dAPI.Factories
{
	public class AxeFactory
	{
        public static object GetInstance(BoundingBox3d box, View view)
        {
            return new AxeBox(box)
            {
                View = view
            };
        }
    }
}
