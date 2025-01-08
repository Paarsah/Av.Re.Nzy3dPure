using Avalonia;
using Mag3DView.Nzy3dAPI.Colors;
using Mag3DView.Nzy3dAPI.Events;
using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;
using Mag3DView.Nzy3dAPI.Plot3D.Transform;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Array = System.Array;

namespace Mag3DView.Nzy3dAPI.Plot3D.Primitives
{
    public class Scatter : AbstractDrawable, ISingleColorable
    {
        private Color[] _colors;
        private Coord3d[] _coordinates;
        private Color _color;
        private BoundingBox3d _bbox; // Declare the bounding box
        private float _pointSize;

        public Scatter()
        {
            var defaultCoordinates = new Coord3d[]
            {
                new Coord3d(-1, -1, -1),
                new Coord3d(1, 1, 1),
                new Coord3d(0, 0, 0)
            };

            InitializeScatter(defaultCoordinates, Color.BLACK, 1);

            // Update bounding box based on default coordinates
            _bbox = new BoundingBox3d();
            foreach (var point in defaultCoordinates)
            {
                _bbox.Add(point);
            }
        }

        public Scatter(Coord3d[] coordinates) : this(coordinates, Color.BLACK)
        {
        }

        public Scatter(Coord3d[] coordinates, Color color) : this(coordinates, color, 1)
        {
        }

        public Scatter(Coord3d[] coordinates, Color color, float pointSize)
        {
            InitializeScatter(coordinates, color, pointSize);
        }

        public Scatter(Coord3d[] coordinates, Color[] colors, float width = 1)
        {
            _colors = colors;
            InitializeScatter(coordinates, Color.BLACK, width);
        }

        private void InitializeScatter(Coord3d[] coordinates, Color color, float pointSize)
        {
            _coordinates = coordinates ?? Array.Empty<Coord3d>();
            _color = color;
            _pointSize = pointSize;

            _bbox = new BoundingBox3d(); // Initialize the bounding box
            UpdateBounds();

            Debug.WriteLine($"Scatter initialized with {_coordinates.Length} points.");
        }

        public override BoundingBox3d GetBounds()
        {
            if (_coordinates != null && _coordinates.Length > 0)
            {
                Debug.WriteLine($"Calculating bounds for {_coordinates.Length} points...");
                _bbox.Reset();
                foreach (var point in _coordinates)
                {
                    Debug.WriteLine($"Point: X={point.X}, Y={point.Y}, Z={point.Z}");
                    _bbox.Add(point);
                }
            }
            else
            {
                Debug.WriteLine("No coordinates found in Scatter object.");
            }
            return _bbox;
        }

        private void UpdateBounds()
        {
            if (_bbox == null)
            {
                _bbox = new BoundingBox3d();
            }

            _bbox.Reset();
            foreach (var point in _coordinates ?? Array.Empty<Coord3d>())
            {
                _bbox.Add(point);
            }
        }

        public void Clear()
        {
            _coordinates = null;
            _bbox?.Reset();
        }

        public override void Draw(Camera cam)
        {
            Debug.WriteLine("Scatter.Draw() invoked.");
            GL.PointSize(_pointSize);
            GL.Begin(PrimitiveType.Points);

            if (_coordinates != null)
            {
                for (int i = 0; i < _coordinates.Length; i++)
                {
                    var point = _coordinates[i];
                    if (_colors != null && i < _colors.Length)
                    {
                        GL.Color4(_colors[i].R, _colors[i].G, _colors[i].B, _colors[i].A);
                    }
                    else
                    {
                        GL.Color4(_color.R, _color.G, _color.B, _color.A);
                    }
                    GL.Vertex3(point.X, point.Y, point.Z);
                }
            }

            GL.End();

            var error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                Debug.WriteLine($"OpenGL Error: {error}");
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

        public Coord3d[] GetCoordinates()
        {
            return _coordinates;
        }

        private Coord3d[] Data
        {
            get => _coordinates;
            set
            {
                _coordinates = value ?? Array.Empty<Coord3d>();
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

        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                FireDrawableChanged(new DrawableChangedEventArgs(this, DrawableChangedEventArgs.FieldChanged.Color));
            }
        }
    }
}