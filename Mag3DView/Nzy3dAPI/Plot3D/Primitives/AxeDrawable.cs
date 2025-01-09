using Mag3DView.Nzy3dAPI.Colors;
using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;

namespace Mag3DView.Nzy3dAPI.Plot3D.Primitives
{
    public class AxeDrawable : AbstractDrawable
    {
        private BoundingBox3d _bbox;
        public Color[] Colors { get; set; } = { Color.RED, Color.GREEN, Color.BLUE };

        public AxeDrawable(BoundingBox3d bbox)
        {
            _bbox = bbox;
        }

        public override void Draw(Camera camera)
        {
            Debug.WriteLine("AxeDrawable.Draw() invoked.");

            GL.LineWidth(2.0f);

            // X-axis
            GL.Color4(Colors[0].OpenTKColor4);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(_bbox.Xmin, 0, 0);
            GL.Vertex3(_bbox.Xmax, 0, 0);
            GL.End();

            // Y-axis
            GL.Color4(Colors[1].OpenTKColor4);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0, _bbox.Ymin, 0);
            GL.Vertex3(0, _bbox.Ymax, 0);
            GL.End();

            // Z-axis
            GL.Color4(Colors[2].OpenTKColor4);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(0, 0, _bbox.Zmin);
            GL.Vertex3(0, 0, _bbox.Zmax);
            GL.End();

            var error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Debug.WriteLine($"OpenGL Error in AxeDrawable: {error}");
            }
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
