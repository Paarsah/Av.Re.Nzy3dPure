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

        public void Clear()
        {
            _coordinates = null;
            _bbox.Reset();
        }

        public override void Draw(Camera cam)
        {
            Debug.WriteLine("Executing Scatter.Draw()...");

            // Apply transformations if any
            if (_transform != null)
            {
                Debug.WriteLine("Applying transformation...");
                _transform.Execute();
            }

            // Set point size
            GL.PointSize(Width);
            GL.Begin(PrimitiveType.Points);

            if (_colors == null)
            {
                // Set default color if no colors array is provided
                Debug.WriteLine($"Using default color: R={Color.R}, G={Color.G}, B={Color.B}, A={Color.A}");
                GL.Color4(Color.R, Color.G, Color.B, Color.A);
            }

            if (_coordinates != null)
            {
                int k = 0;
                Debug.WriteLine($"Rendering {_coordinates.Length} points...");
                foreach (Coord3d coord in _coordinates)
                {
                    // Apply individual colors if available
                    if (_colors != null && k < _colors.Length)
                    {
                        var pointColor = _colors[k];
                        Debug.WriteLine($"Point {k}: R={pointColor.R}, G={pointColor.G}, B={pointColor.B}, A={pointColor.A} at X={coord.X}, Y={coord.Y}, Z={coord.Z}");
                        GL.Color4(pointColor.R, pointColor.G, pointColor.B, pointColor.A);
                        k++;
                    }

                    // Render the vertex
                    GL.Vertex3(coord.X, coord.Y, coord.Z);
                }
            }
            else
            {
                Debug.WriteLine("No coordinates to render.");
            }

            GL.End();

            // Check for OpenGL errors
            var error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Debug.WriteLine($"OpenGL Error in Scatter.Draw(): {error}");
            }

            Debug.WriteLine("Scatter.Draw() complete.");
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

        public override BoundingBox3d GetBounds()
        {
            throw new System.NotImplementedException();
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