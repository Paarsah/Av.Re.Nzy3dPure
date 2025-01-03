using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Primitives;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;

namespace Mag3DView.Nzy3dAPI.Plot3D.Primitives.Axes
{
    public class AxeBoxDrawable : AbstractDrawable
    {
        private readonly AxeBox _axeBox;
        private BoundingBox3d _bbox;

        public AxeBoxDrawable(BoundingBox3d bbox)
        {
            _bbox = bbox;
        }

        public AxeBoxDrawable(AxeBox axeBox)
        {
            _axeBox = axeBox;
        }

        public override void Draw(Camera camera)
        {
            // Delegate drawing logic to the AxeBox
            _axeBox.Draw(camera);
        }

        public override BoundingBox3d GetBounds()
        {
            return _bbox;
        }

    }
}
