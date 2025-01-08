using Mag3DView.Nzy3dAPI.Colors;
using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;
using System;
using System.Diagnostics;

namespace Mag3DView.Nzy3dAPI.Plot3D.Primitives
{
    public class AxeDrawable : AbstractDrawable
    {
        private BoundingBox3d _bbox;

        public AxeDrawable(BoundingBox3d bbox)
        {
            _bbox = bbox;
        }

        public override void Draw(Camera camera)
        {
            // Draw axis lines
            DrawLine(camera, new Coord3d(_bbox.Xmin, 0, 0), new Coord3d(_bbox.Xmax, 0, 0), Color.RED); // X-axis
            DrawLine(camera, new Coord3d(0, _bbox.Ymin, 0), new Coord3d(0, _bbox.Ymax, 0), Color.GREEN); // Y-axis
            DrawLine(camera, new Coord3d(0, 0, _bbox.Zmin), new Coord3d(0, 0, _bbox.Zmax), Color.BLUE); // Z-axis

            // Optionally, add ticks or labels here
        }

        private void DrawLine(Camera camera, Coord3d start, Coord3d end, Color color)
        {
            // Implement actual line drawing logic with canvas or OpenGL
        }

        //public override BoundingBox3d GetBounds()
        //{
        //    return _bbox;
        //}

        public override BoundingBox3d GetBounds()
        {
            // Define axes bounds based on their conceptual range
            double xmin = -10, xmax = 10;
            double ymin = -10, ymax = 10;
            double zmin = -10, zmax = 10;

            Debug.WriteLine($"AxeDrawable.GetBounds: Xmin={xmin}, Xmax={xmax}, Ymin={ymin}, Ymax={ymax}, Zmin={zmin}, Zmax={zmax}");
            return new BoundingBox3d(xmin, xmax, ymin, ymax, zmin, zmax);
        }

    }
}
