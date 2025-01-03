using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;

namespace Mag3DView.Nzy3dAPI.Plot3D.Primitives
{
    public class TestDrawable : AbstractDrawable
    {
        public TestDrawable()
        {
            var minCoord = new Coord3d(0, 0, 0);
            var maxCoord = new Coord3d(1, 1, 1);

            // Use the BoundingBox3d constructor that takes six double values
            _bbox = new BoundingBox3d(
                minCoord.X, maxCoord.X,
                minCoord.Y, maxCoord.Y,
                minCoord.Z, maxCoord.Z
            );
        }

        public override void Draw(Camera cam)
        {
            // Implement drawing logic here
        }

        public override BoundingBox3d GetBounds()
        {
            throw new System.NotImplementedException();
        }
    }
}
