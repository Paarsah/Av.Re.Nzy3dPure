using Mag3DView.Nzy3dAPI.Colors;
using Mag3DView.Nzy3dAPI.Events;
using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;
using Mag3DView.Nzy3dAPI.Plot3D.Transform;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using System.Linq;

namespace Mag3DView.Nzy3dAPI.Plot3D.Primitives
{
    public class Scatter : AbstractDrawable, ISingleColorable
    {
        private Color[] _colors;
        private Coord3d[] _coordinates;
        private Color _color;
        public Scatter()
        {
            _bbox = new BoundingBox3d();
            Width = 1;
            Color = Color.BLACK;
        }

        public Scatter(Coord3d[] coordinates) :
                this(coordinates, Color.BLACK)
        {
        }
        public Scatter(Coord3d[] coordinates, Color color)
        {
            _coordinates = coordinates;
            _color = color;
        }

        public Scatter(Coord3d[] coordinates, Color rgb, float width = 1)
        {
            _bbox = new BoundingBox3d();
            Data = coordinates;
            Width = width;
            Color = rgb;
        }

        public Scatter(Coord3d[] coordinates, Color[] colors, float width = 1)
        {
            _bbox = new BoundingBox3d();
            Data = coordinates;
            Width = width;
            Colors = colors;
        }

        public override BoundingBox3d GetBounds()
        {
            if (_coordinates == null || _coordinates.Length == 0)
            {
                return new BoundingBox3d(); // Return an empty bounding box
            }

            // Calculate min and max for X, Y, Z
            double xmin = _coordinates.Min(coord => coord.X);
            double xmax = _coordinates.Max(coord => coord.X);
            double ymin = _coordinates.Min(coord => coord.Y);
            double ymax = _coordinates.Max(coord => coord.Y);
            double zmin = _coordinates.Min(coord => coord.Z);
            double zmax = _coordinates.Max(coord => coord.Z);

            // Create and return the bounding box
            return new BoundingBox3d(xmin, xmax, ymin, ymax, zmin, zmax);
        }

        public void Clear()
        {
            _coordinates = null;
            _bbox.Reset();
        }

        public override void Draw(Camera cam)
        {
            Debug.WriteLine("Scatter.Draw() invoked.");

            _transform?.Execute();

            GL.PointSize(Width);
            GL.Begin(PrimitiveType.Points);
            if (_colors == null)
            {
                GL.Color4(Color.R, Color.G, Color.B, Color.A);
            }

            if (_coordinates != null)
            {
                Debug.WriteLine($"Rendering {_coordinates.Length} points...");
                int k = 0;
                foreach (Coord3d coord in _coordinates)
                {
                    Debug.WriteLine($"Point {k}: X={coord.X}, Y={coord.Y}, Z={coord.Z}");
                    if (_colors != null)
                    {
                        GL.Color4(_colors[k].R, _colors[k].G, _colors[k].B, _colors[k].A);
                        k++;
                    }
                    GL.Vertex3(coord.X, coord.Y, coord.Z);
                }
            }
            GL.End();

            // Check OpenGL errors
            var error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Debug.WriteLine($"OpenGL Error in Scatter.Draw(): {error}");
            }
        }

        public override Transform.Transform Transform
        {
            get => _transform;
            set
            {
                _transform = value;
                UpdateBounds();
            }
        }

        private void UpdateBounds()
        {
            _bbox.Reset();
            foreach (var c in _coordinates)
            {
                _bbox.Add(c);
            }
        }

        public Coord3d[] GetCoordinates()
        {
            return _coordinates;
        }

        private Coord3d[] Data
        {
            get => _coordinates;
            set
            {
                _coordinates = value;
                UpdateBounds();
            }
        }

        private Color[] Colors
        {
            get => _colors;
            set
            {
                _colors = value;
                FireDrawableChanged(new DrawableChangedEventArgs(this, DrawableChangedEventArgs.FieldChanged.Color));
            }
        }

        private float Width { get; }

        public Color Color { get; set; }
    }
}