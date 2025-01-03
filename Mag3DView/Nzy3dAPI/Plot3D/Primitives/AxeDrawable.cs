using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;
using System;

namespace Mag3DView.Nzy3dAPI.Plot3D.Primitives
{
    public class AxeDrawable : AbstractDrawable
    {
        private BoundingBox3d _bbox;

        public AxeDrawable(BoundingBox3d bbox)
        {
            _bbox = bbox;
        }

        // Implement the required Draw method
        public override void Draw(Camera camera)
        {
            // Custom drawing logic for axes using the camera and bounding box
            Console.WriteLine("Drawing axes with bounding box: " + _bbox);
        }

        // Override the Bounds property to return the bounding box
        public override BoundingBox3d Bounds
        {
            get { return _bbox; }
        }
    }
}
