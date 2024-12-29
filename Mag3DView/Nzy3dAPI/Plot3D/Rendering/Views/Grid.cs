using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;
using Mag3DView.Nzy3dAPI.Plot3D.Primitives;

namespace Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views
{

    public class Grid : AbstractDrawable
    {
        private readonly float _size;
        private readonly int _divisions;

        public double[] X { get; set; }
        public double[] Y { get; set; }
        public double[,] Z { get; set; }

        public Grid(double[] x, double[] y, double[,] z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Grid(float size, int divisions)
        {
            _size = size;
            _divisions = divisions;
        }

        public override void Draw(Camera cam)
        {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(1.0f, 1.0f, 1.0f); // White lines

            float step = _size / _divisions;
            for (float i = -_size; i <= _size; i += step)
            {
                // Vertical lines
                GL.Vertex3(i, -_size, 0);
                GL.Vertex3(i, _size, 0);
                // Horizontal lines
                GL.Vertex3(-_size, i, 0);
                GL.Vertex3(_size, i, 0);
            }

            GL.End();
        }
    }
}
