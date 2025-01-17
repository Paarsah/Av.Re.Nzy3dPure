using Mag3DView.Nzy3dAPI.Colors;
using Mag3DView.Nzy3dAPI.Events;
using Mag3DView.Nzy3dAPI.Factories;
using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Primitives;
using Mag3DView.Nzy3dAPI.Plot3D.Primitives.Axes;
using Mag3DView.Nzy3dAPI.Plot3D.Primitives.Axes.Layout;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Canvas;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Scenes;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views.Modes;
using Mag3DView.Nzy3dAPI.Plot3D.Transform;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views
{
    public class View
	{
		public static double STRETCH_RATIO = 0.25;

		// force to have all object maintained in screen, meaning axebox won't always keep the same size.
		internal bool MAINTAIN_ALL_OBJECTS_IN_VIEW = false;

		// display a magenta parallelepiped (debug)
#if DEBUG
		internal bool DISPLAY_AXE_WHOLE_BOUNDS = true;
#else
		internal bool DISPLAY_AXE_WHOLE_BOUNDS = false;
#endif

		internal bool _axeBoxDisplayed = true;
		internal bool _squared = true;
		internal Camera _cam;
		internal IAxe _axe;
		internal Quality _quality;
		// TODO : Implement overlay
		// Friend _overlay As Overlay 
		internal Scene _scene { get; private set; }
		internal ICanvas _canvas;
		private Coord3d _viewpoint;
		private Coord3d _center;
		private Coord3d _scaling;
		internal BoundingBox3d _viewbounds;
		internal CameraMode _cameraMode;
		internal ViewPositionMode _viewmode;
		internal ViewBoundMode _boundmode;
		internal ImageViewport _bgViewport;
		//internal System.Drawing.Bitmap _bgImg = null;
		internal BoundingBox3d _targetBox;
		internal Color _bgColor = Color.BLACK;
		internal Color _bgOverlay = new Color(0, 0, 0, 0);
		// TODO : Implement overlay
		//Friend _tooltips As List(Of ITooltipRenderer) 
		internal List<IBaseRenderer2D> _renderers;
		internal List<IViewPointChangedListener> _viewPointChangedListeners;
        private List<IViewIsVerticalEventListener> _viewOnTopListeners;
        internal bool _wasOnTopAtLastRendering;
		internal const double PI_div2 = Math.PI / 2;
		public static readonly Coord3d DEFAULT_VIEW = new Coord3d(Math.PI / 3, Math.PI / 3, 2000);
		internal bool _dimensionDirty = false;
		internal bool _viewDirty = false;

        // Scale properties for each axis
        public Scale XScale { get; set; }
        public Scale YScale { get; set; }
        public Scale ZScale { get; set; }


		// Constructor or initialization
		//internal Scene _scene { get; private set; }
		public View()
		{
			XScale = new Scale(0, 10); // Example scale for the X-axis
			YScale = new Scale(0, 10); // Example scale for the Y-axis
			ZScale = new Scale(0, 10); // Example scale for the Z-axis
			_scene = new Scene();
            _center = new Coord3d(0, 0, 0);
            _viewpoint = new Coord3d(0, 0, 10); // Default initialization
            _scaling = new Coord3d(1, 1, 1);
            _viewOnTopListeners = new List<IViewIsVerticalEventListener>();
            _cam = new Camera()
            {
                Position = new Coord3d(0, 0, 10),
                Target = new Coord3d(0, 0, 0),
                Up = new Coord3d(0, 1, 0)
            };
            // Initialize _axe with a bounding box
            //BoundingBox3d initialBox = new BoundingBox3d(0, 10, 0, 10, 0, 10); // Using min/max values
            BoundingBox3d initialBox = new BoundingBox3d(new Coord3d(5, 5, 5), 10); // Center at (5,5,5), edge length 10
            IAxeLayout layout = new AxeBoxLayout();
            _axe = new AxeBox(initialBox, layout); // Use the default AxeBoxLayout
        }

        public void SetBoundManual(BoundingBox3d bounds)
        {
            BoundManual = bounds;
            Camera.SetBound(bounds); // Update the camera's bounds if needed
        }

        private void AddLine(Coord3d start, Coord3d end, Color color)
        {
            GL.Begin(PrimitiveType.Lines);
            color.Apply();
            GL.Vertex3(start.X, start.Y, start.Z);
            GL.Vertex3(end.X, end.Y, end.Z);
            GL.End();
        }

        private void AddLabel(string text, Coord3d position, Color color)
        {
            // Placeholder: Implement text rendering here or integrate with a text library
            Debug.WriteLine($"Label '{text}' at {position} with color {color}");
        }

        // Adds the three main axes (X, Y, Z) to the scene
        private void RenderAxes()
        {
            // X Axis: Red
            AddLine(new Coord3d(0, 0, 0), new Coord3d(1, 0, 0), new Color(1.0f, 0.0f, 0.0f));
            AddLabel("X", new Coord3d(1.1, 0, 0), new Color(1.0f, 0.0f, 0.0f));

            // Y Axis: Green
            AddLine(new Coord3d(0, 0, 0), new Coord3d(0, 1, 0), new Color(0.0f, 1.0f, 0.0f));
            AddLabel("Y", new Coord3d(0, 1.1, 0), new Color(0.0f, 1.0f, 0.0f));

            // Z Axis: Blue
            AddLine(new Coord3d(0, 0, 0), new Coord3d(0, 0, 1), new Color(0.0f, 0.0f, 1.0f));
            AddLabel("Z", new Coord3d(0, 0, 1.1), new Color(0.0f, 0.0f, 1.0f));
        }

        private void RenderGrid(double size, int divisions)
        {
            double step = size / divisions;
            Color gridColor = new Color(0.8f, 0.8f, 0.8f);

            GL.Begin(PrimitiveType.Lines);
            gridColor.Apply(); // Set the grid line color

            for (int i = -divisions; i <= divisions; i++)
            {
                double pos = i * step;

                // Horizontal lines (parallel to X-axis)
                GL.Vertex3(-size, pos, 0);
                GL.Vertex3(size, pos, 0);

                // Vertical lines (parallel to Y-axis)
                GL.Vertex3(pos, -size, 0);
                GL.Vertex3(pos, size, 0);
            }

            GL.End();
        }

        private void RenderTickMarks(int divisions, double length)
        {
            double step = 1.0 / divisions;
            Color tickColor = new Color(0.5f, 0.5f, 0.5f);

            GL.Begin(PrimitiveType.Lines);
            tickColor.Apply(); // Set the tick mark color

            for (int i = 1; i <= divisions; i++)
            {
                double pos = i * step;

                // X-axis ticks
                GL.Vertex3(pos, -length, 0);
                GL.Vertex3(pos, length, 0);

                // Y-axis ticks
                GL.Vertex3(-length, pos, 0);
                GL.Vertex3(length, pos, 0);
            }

            GL.End();
        }

        // Methods to update each axis scale (these could handle additional logic)
        private void SetXScale(Scale scale, bool updateView)
        {
            XScale = scale;
            if (updateView)
            {
                // Update the view after setting the scale
            }
        }

        private void SetYScale(Scale scale, bool updateView)
        {
            YScale = scale;
            if (updateView)
            {
                // Update the view after setting the scale
            }
        }

        private void SetZScale(Scale scale, bool updateView)
        {
            ZScale = scale;
            if (updateView)
            {
                // Update the view after setting the scale
            }
        }

        static internal View Current;
        private Camera _camera;

        public View(Scene scene, ICanvas canvas, Quality quality)
        {
            _scene = scene ?? throw new ArgumentNullException(nameof(scene));
            _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            _quality = quality ?? new Quality(true);

            // Ensure scene.Graph and its Bounds are initialized
            BoundingBox3d sceneBounds = scene.Graph?.Bounds ?? new BoundingBox3d(0, 0, 0, 1, 1, 1);

            _viewpoint = DEFAULT_VIEW.Clone();
            _center = sceneBounds.GetCenter();
            _scaling = Coord3d.IDENTITY.Clone();
            _viewmode = ViewPositionMode.FREE;
            _boundmode = ViewBoundMode.AUTO_FIT;
            _cameraMode = CameraMode.ORTHOGONAL;
            _axe = (IAxe)AxeFactory.GetInstance(sceneBounds, this);
            _cam = CameraFactory.GetInstance(_center);
            _renderers = new List<IBaseRenderer2D>();
            _bgViewport = new ImageViewport();
            _viewOnTopListeners = new List<IViewIsVerticalEventListener>();
            _viewPointChangedListeners = new List<IViewPointChangedListener>();
            _wasOnTopAtLastRendering = false;
            View.Current = this;

            _camera = new Camera(this);
            _camera.Zoom = 1;

            // Set the camera position
            _camera.SetPosition(sceneBounds.GetCenter());
        }

        public void Dispose()
		{
			_axe.Dispose();
			_cam = null;
			_renderers.Clear();
			_viewOnTopListeners.Clear();
			_scene = null;
			_canvas = null;
			_quality = null;
		}

		public void Shoot()
		{
            if (_canvas != null)
            {
                _canvas.ForceRepaint();
            }
            else
            {
                // Log or handle the error appropriately
                Debug.WriteLine("Canvas is not initialized.");
            }
        }

        public void Project()
		{
			_scene.Graph.Project(_cam);
		}

		/// <summary>
		/// Projects the 2D mouse coordinates into the 3D chart, using 0 as Z value.
		/// </summary>
		/// <param name="x">The mouse X coordinate.</param>
		/// <param name="y">The mouse Y coordinate.</param>
		public Coord3d ProjectMouse(int x, int y)
		{
			return _cam.ScreenToModel(new Coord3d(x, y, 0));
		}

		/// <summary>
		/// Projects the 2D mouse coordinates into the 3D chart, using the center as Z value.
		/// <para>This gives better results than <see cref="ProjectMouse(int, int)"/>.</para>
		/// </summary>
		/// <param name="x">The mouse X coordinate.</param>
		/// <param name="y">The mouse Y coordinate.</param>
		public Coord3d ProjectMouseCenter(int x, int y)
		{
			if (Camera == null || Camera.Eye == null)
			{
				return null;
			}

			var c = Camera.ModelToScreen(Axe.GetBoxBounds().GetCenter());
			var proj = _cam.ScreenToModel(new Coord3d(x, y, c.Z));

			System.Diagnostics.Debug.WriteLine($"Renderer3D.OnMouseMove: Location (X={x}, Y={y}, Z={c.Z:0.000000}) - Projection=({proj})");

			return proj;
		}

		/// <summary>
		/// Projects the 2D mouse coordinates into the 3D chart, using the depth component as Z value.
		/// <para>This gives better results than <see cref="ProjectMouse(int, int)"/>.</para>
		/// </summary>
		/// <param name="x">The mouse X coordinate.</param>
		/// <param name="y">The mouse Y coordinate.</param>
		public Coord3d ProjectMouseDepth(int x, int y)
		{
			if (Camera == null || Camera.Eye == null)
			{
				return null;
			}

			if (!GL.GetBoolean(GetPName.DepthTest))
			{
				throw new InvalidOperationException("Quality should allow for DepthTest (i.e. depth buffer should be activated) in order to get ProjectMouseDepth(). COnsider using GL.Enable(EnableCap.DepthTest).");
			}

			float depth = 0;
			GL.ReadPixels(x, y, 1, 1, PixelFormat.DepthComponent, PixelType.Float, ref depth);

			//if (depth >= 1) return null;

			return _cam.ScreenToModel(new Coord3d(x, y, depth));
		}

		#region "GENERAL DISPLAY CONTROLS"
		public void Rotate(Coord2d move)
		{
			Rotate(move, true);
		}

		public void Rotate(Coord2d move, bool updateView)
		{
			Coord3d eye = this.ViewPoint;
			eye.X -= move.X;
			eye.Y += move.Y;
			SetViewPoint(eye, updateView);
		}

		public void Shift(float factor)
		{
			Shift(factor, true);
		}

		public void Shift(float factor, bool updateView)
		{
			Scale current = this.Scale;
			Scale newScale = current.Add(factor * current.Range);
			SetScale(newScale, updateView);
			//fireControllerEvent(ControllerType.SHIFT, newScale);
		}

		public void SetMousePosition(int x, int y)
		{
			this.MousePosition = ProjectMouseDepth(x, y);
		}

		#region Zoom
		public void Zoom(float factor)
		{
			Zoom(factor, true);
		}

		public void Zoom(float factor, bool updateView)
        {
            // Adjust the scale for all axes
            double xRange = XScale.Max - XScale.Min;
            double yRange = YScale.Max - YScale.Min;
            double zRange = ZScale.Max - ZScale.Min;

            if (xRange <= 0 || yRange <= 0 || zRange <= 0)
            {
                return;
            }

            // Compute centers
            double xCenter = (XScale.Max + XScale.Min) / 2;
            double yCenter = (YScale.Max + YScale.Min) / 2;
            double zCenter = (ZScale.Max + ZScale.Min) / 2;

            // Adjust min/max for each axis
            double xMin = xCenter + (XScale.Min - xCenter) * factor;
            double xMax = xCenter + (XScale.Max - xCenter) * factor;
            double yMin = yCenter + (YScale.Min - yCenter) * factor;
            double yMax = yCenter + (YScale.Max - yCenter) * factor;
            double zMin = zCenter + (ZScale.Min - zCenter) * factor;
            double zMax = zCenter + (ZScale.Max - zCenter) * factor;

            // Update scales
            SetXScale(new Scale(xMin, xMax), updateView);
            SetYScale(new Scale(yMin, yMax), updateView);
            SetZScale(new Scale(zMin, zMax), updateView);
        }


        public void ZoomX(float factor)
		{
			ZoomX(factor, true);
		}

		public void ZoomX(float factor, bool updateView)
		{
			double range = this.Bounds.XMax - this.Bounds.XMin;
			if (range <= 0)
			{
				return;
			}
			double center = (this.Bounds.XMax + this.Bounds.XMin) / 2;
			double min = center + (this.Bounds.XMin - center) * factor;
			double max = center + (this.Bounds.XMax - center) * factor;

			// set min/max according to bounds
			Scale scale = null;
			if (min < max)
			{
				scale = new Scale(min, max);
			}
			else
			{
				// forbid to have min = max if we zoom in
				if (factor < 1)
				{
					scale = new Scale(center, center);
				}
			}

			if (scale != null)
			{
				BoundingBox3d bounds = this.Bounds;
				bounds.XMin = scale.Min;
				bounds.XMax = scale.Max;
				this.BoundManual = bounds;
				if (updateView)
				{
					Shoot();
				}
				// fireControllerEvent(ControllerType.ZOOM, scale);
			}
		}

		public void ZoomY(float factor)
		{
			ZoomY(factor, true);
		}

		public void ZoomY(float factor, bool updateView)
		{
			double range = this.Bounds.YMax - this.Bounds.YMin;
			if (range <= 0)
			{
				return;
			}
			double center = (this.Bounds.YMax + this.Bounds.YMin) / 2;
			double min = center + (this.Bounds.YMin - center) * factor;
			double max = center + (this.Bounds.YMax - center) * factor;

			// set min/max according to bounds
			Scale scale = null;
			if (min < max)
			{
				scale = new Scale(min, max);
			}
			else
			{
				// forbid to have min = max if we zoom in
				if (factor < 1)
				{
					scale = new Scale(center, center);
				}
			}

			if (scale != null)
			{
				BoundingBox3d bounds = this.Bounds;
				bounds.YMin = scale.Min;
				bounds.YMax = scale.Max;
				this.BoundManual = bounds;
				if (updateView)
				{
					Shoot();
				}
				// fireControllerEvent(ControllerType.ZOOM, scale);
			}
		}

		public void ZoomZ(float factor)
		{
			ZoomZ(factor, true);
		}

		public void ZoomZ(float factor, bool updateView)
		{
			double range = this.Bounds.ZMax - this.Bounds.ZMin;
			if (range <= 0)
			{
				return;
			}
			double center = (this.Bounds.ZMax + this.Bounds.ZMin) / 2;
			double min = center + (this.Bounds.ZMin - center) * factor;
			double max = center + (this.Bounds.ZMax - center) * factor;

			// set min/max according to bounds
			Scale scale = null;
			if (min < max)
			{
				scale = new Scale(min, max);
			}
			else
			{
				// forbid to have min = max if we zoom in
				if (factor < 1)
				{
					scale = new Scale(center, center);
				}
			}

			if (scale != null)
			{
				BoundingBox3d bounds = this.Bounds;
				bounds.ZMin = scale.Min;
				bounds.ZMax = scale.Max;
				this.BoundManual = bounds;
				if (updateView)
				{
					Shoot();
				}
				// fireControllerEvent(ControllerType.ZOOM, scale);
			}
		}

		public void ZoomXYZ(double factor)
		{
			ZoomXYZ(factor, true);
		}

		public void ZoomXYZ(double factor, bool updateView)
		{
			Scale scaleX = null;
			Scale scaleY = null;
			Scale scaleZ = null;

			// X
			double rangeX = this.Bounds.XMax - this.Bounds.XMin;
			if (rangeX > 0)
			{
				double centerX = (this.Bounds.XMax + this.Bounds.XMin) / 2;
				double minX = centerX + (this.Bounds.XMin - centerX) * factor;
				double maxX = centerX + (this.Bounds.XMax - centerX) * factor;

				// set min/max according to bounds
				if (minX < maxX)
				{
					scaleX = new Scale(minX, maxX);
				}
				else
				{
					// forbid to have min = max if we zoom in
					if (factor < 1)
					{
						scaleX = new Scale(centerX, centerX);
					}
				}
			}

			// Y
			double rangeY = this.Bounds.YMax - this.Bounds.YMin;
			if (rangeY > 0)
			{
				double centerY = (this.Bounds.YMax + this.Bounds.YMin) / 2;
				double minY = centerY + (this.Bounds.YMin - centerY) * factor;
				double maxY = centerY + (this.Bounds.YMax - centerY) * factor;

				// set min/max according to bounds
				if (minY < maxY)
				{
					scaleY = new Scale(minY, maxY);
				}
				else
				{
					// forbid to have min = max if we zoom in
					if (factor < 1)
					{
						scaleY = new Scale(centerY, centerY);
					}
				}
			}

			// Z
			double rangeZ = this.Bounds.ZMax - this.Bounds.ZMin;
			if (rangeZ > 0)
			{
				double centerZ = (this.Bounds.ZMax + this.Bounds.ZMin) / 2;
				double minZ = centerZ + (this.Bounds.ZMin - centerZ) * factor;
				double maxZ = centerZ + (this.Bounds.ZMax - centerZ) * factor;

				// set min/max according to bounds
				if (minZ < maxZ)
				{
					scaleZ = new Scale(minZ, maxZ);
				}
				else
				{
					// forbid to have min = max if we zoom in
					if (factor < 1)
					{
						scaleZ = new Scale(centerZ, centerZ);
					}
				}
			}

			// Apply
			if (scaleX == null && scaleY == null && scaleZ == null) return;

			BoundingBox3d bounds = this.Bounds;
			if (scaleX != null)
			{
				bounds.XMin = scaleX.Min;
				bounds.XMax = scaleX.Max;
			}

			if (scaleY != null)
			{
				bounds.YMin = scaleY.Min;
				bounds.YMax = scaleY.Max;
			}

			if (scaleZ != null)
			{
				bounds.ZMin = scaleZ.Min;
				bounds.ZMax = scaleZ.Max;
			}

			this.BoundManual = bounds;
			if (updateView)
			{
				Shoot();
			}
		}
		#endregion

		public Coord3d MousePosition { get; private set; }

		public bool DimensionDirty
		{
			get { return _dimensionDirty; }
			set { _dimensionDirty = value; }
		}

		public Scale Scale
		{
			get { return new Scale(this.Bounds.ZMin, this.Bounds.ZMax); }
			set { SetScale(value, true); }
		}

		public void SetScale(Scale scale, bool notify)
		{
			BoundingBox3d bounds = this.Bounds;
			bounds.ZMin = scale.Min;
			bounds.ZMax = scale.Max;
			this.BoundManual = bounds;
			if (notify)
			{
				Shoot();
			}
		}

		/// <summary>
		/// Set the surrounding AxeBox dimensions and the Camera target, and the
		/// colorbar range.
		/// </summary>
		public void LookToBox(BoundingBox3d box)
		{
			_center = box.GetCenter();
			_axe.SetAxe(box);
			_targetBox = box;
		}

		/// <summary>
		/// True to always include text labels. False for default.
		/// </summary>
		/// <remarks>Quite experimental!</remarks>
		public bool IncludingTextLabels { get; set; }

		/// <summary>
		/// Get the <see cref="AxeBox"/>'s bounds
		/// </summary>
		public BoundingBox3d Bounds
		{
			get { return _axe.GetBoxBounds(); }
		}

		public ViewBoundMode BoundsMode
		{
			get { return _boundmode; }
		}

		/// <summary>
		/// Set the ViewPositionMode applied to this view.
		/// </summary>
		public ViewPositionMode ViewMode
		{
			get { return _viewmode; }
			set { _viewmode = value; }
		}

		public Coord3d ViewPoint
		{
			get { return _viewpoint; }
			set { SetViewPoint(value, true); }
		}

		public void SetViewPoint(Coord3d polar, bool updateView)
		{
			_viewpoint = polar;
			_viewpoint.Y = (_viewpoint.Y < -PI_div2 ? -PI_div2 : _viewpoint.Y);
			_viewpoint.Y = (_viewpoint.Y > PI_div2 ? PI_div2 : _viewpoint.Y);
			if (updateView)
			{
				Shoot();
			}
			FireViewPointChangedEvent(new ViewPointChangedEventArgs(this, polar));
		}

		public Coord3d GetLastViewScaling()
		{
			return _scaling;
		}

		public IAxe Axe
		{
			get { return _axe; }
			set
			{
				_axe = value;
				UpdateBounds();
			}
		}

		public bool Squared
		{
			get { return _squared; }
			set { _squared = value; }
		}

		public bool AxeBoxDisplayed
		{
			get { return _axeBoxDisplayed; }
			set { _axeBoxDisplayed = value; }
		}

		public Color BackgroundColor
		{
			get { return _bgColor; }
			set { _bgColor = value; }
		}

		//public System.Drawing.Bitmap BackgroundImage {
		//	get { return _bgImg; }
		//	set {
		//		_bgImg = value;
		//		_bgViewport.Image = _bgImg;
		//	}
		//}

		public Camera Camera
		{
			get { return _cam; }
		}

		/// <summary>
		/// Get the projection of this view, either CameraMode.ORTHOGONAL or CameraMode.PERSPECTIVE.
		/// </summary>
		public CameraMode CameraMode
		{
			get { return _cameraMode; }
			set { _cameraMode = value; }
		}

		public bool Maximized
		{
			get { return _cam.StretchToFill; }
			set { _cam.StretchToFill = value; }
		}

		public Scene Scene
		{
			get { return _scene; }
		}

		public System.Drawing.Rectangle SceneViewportRectangle
		{
			get { return _cam.Rectange; }
		}

		public ICanvas Canvas
		{
			get { return _canvas; }
		}

		public void AddRenderer2d(IBaseRenderer2D renderer)
		{
			_renderers.Add(renderer);
		}

		public void RemoveRenderer2d(IBaseRenderer2D renderer)
		{
			_renderers.Remove(renderer);
		}

		public void AddViewOnTopEventListener(IViewIsVerticalEventListener listener)
		{
			_viewOnTopListeners.Add(listener);
		}

		public void RemoveViewOnTopEventListener(IViewIsVerticalEventListener listener)
		{
			_viewOnTopListeners.Remove(listener);
		}

		internal void FireViewOnTopEvent(bool isOnTop)
		{
			var e = new ViewIsVerticalEventArgs(this);
			if (isOnTop)
			{
				foreach (IViewIsVerticalEventListener listener in _viewOnTopListeners)
				{
					listener.ViewVerticalReached(e);
				}
			}
			else
			{
				foreach (IViewIsVerticalEventListener listener in _viewOnTopListeners)
				{
					listener.ViewVerticalLeft(e);
				}
			}
		}

		public void AddViewPointChangedListener(IViewPointChangedListener listener)
		{
			_viewPointChangedListeners.Add(listener);
		}

		public void RemoveViewPointChangedListener(IViewPointChangedListener listener)
		{
			_viewPointChangedListeners.Remove(listener);
		}

		internal void FireViewPointChangedEvent(ViewPointChangedEventArgs e)
		{
			foreach (IViewPointChangedListener vp in _viewPointChangedListeners)
			{
				vp.ViewPointChanged(e);
			}
		}

		/// <summary>
		/// Select between an automatic bounding (that allows fitting the entire scene graph), or a custom bounding.
		/// </summary>
		public ViewBoundMode BoundMode
		{
			set
			{
				_boundmode = value;
				UpdateBounds();
			}
		}

		/// <summary>
		/// Set the bounds of the view according to the current BoundMode, and orders a Camera.shoot().
		/// </summary>
		public void UpdateBounds()
		{
			switch (_boundmode)
			{
				case ViewBoundMode.AUTO_FIT:
					LookToBox(Scene.Graph.Bounds);
					// set axe and camera
					break;
				case ViewBoundMode.MANUAL:
					LookToBox(_viewbounds);
					// set axe and camera
					break;
				default:
					throw new Exception("Unsupported bound mode : " + _boundmode);
			}
			Shoot();
		}

		/// <summary>
		/// Update the bounds according to the scene graph whatever is the current
		/// BoundMode, and orders a shoot() if refresh is True
		/// </summary>
		/// <param name="refresh">Wether to order a shoot() or not.</param>
		public void UpdateBoundsForceUpdate(bool refresh)
		{
			LookToBox(Scene.Graph.Bounds);
			if (refresh)
			{
				Shoot();
			}
		}

		/// <summary>
		/// Set a manual bounding box and switch the bounding mode to
		/// ViewBoundMode.MANUAL, meaning that any call to updateBounds()
		/// will update view bounds to the current bounds.
		/// </summary>
		/// <remarks>The camera.shoot is not called in this case</remarks>
		public BoundingBox3d BoundManual
		{
			set
			{
				_viewbounds = value;
				_boundmode = ViewBoundMode.MANUAL;
				LookToBox(_viewbounds);
			}
		}

        /// <summary>
        /// <para>
        /// Return a 3d scaling factor that allows scaling the scene into a square
        /// box, according to the current ViewBoundMode.
        /// <p/>
        /// If the scene bounds are Infinite, NaN or zero, for a given dimension, the
        /// scaler will be set to 1 on the given dimension.
        /// </para>
        /// <para>@return a scaling factor for each dimension.</para>
        /// </summary>
        internal Coord3d Squarify()
        {
            // Get the view bounds
            BoundingBox3d bounds;

            if (_scene.Graph.Bounds == null)
            {
                _scene.Graph.Bounds.Add(new Coord3d(-1, -1, -1));
                _scene.Graph.Bounds.Add(new Coord3d(1, 1, 1));
            }

            switch (_boundmode)
            {
                case ViewBoundMode.AUTO_FIT:
                    if (Scene?.Graph?.Bounds == null)
                    {
                        throw new NullReferenceException("Scene.Graph.Bounds is null.");
                    }
                    bounds = Scene.Graph.Bounds;
                    break;

                case ViewBoundMode.MANUAL:
                    if (_viewbounds == null)
                    {
                        throw new NullReferenceException("_viewbounds is null.");
                    }
                    bounds = _viewbounds;
                    break;

                default:
                    throw new Exception("Unsupported bound mode : " + _boundmode);
            }

            // Compute factors
            double xLen = Math.Max(bounds.XMax - bounds.XMin, 1);
            double yLen = Math.Max(bounds.YMax - bounds.YMin, 1);
            double zLen = Math.Max(bounds.ZMax - bounds.ZMin, 1);
            double lmax = Math.Max(Math.Max(xLen, yLen), zLen);

            return new Coord3d(lmax / xLen, lmax / yLen, lmax / zLen);
        }
        #endregion

        #region "GL2"
        /// <summary>
        /// The init function specifies general GL settings that impact the rendering
        /// quality and performance (computation speed).
        /// <p/>
        /// The rendering settings are set by the Quality instance given in
        /// the constructor parameters.
        /// </summary>
        public void Init()
		{
			InitQuality();
			InitLights();
		}

		public void InitQuality()
		{
			// Activate Depth buffer
			if (_quality.DepthActivated)
			{
				GL.Enable(EnableCap.DepthTest);
				GL.DepthFunc(DepthFunction.Lequal);
			}
			else
			{
				GL.Disable(EnableCap.DepthTest);
			}

			// Blending
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			// on/off is handled by each viewport (camera or image)

			// Activate transparency
			if (_quality.AlphaActivated)
			{
				GL.Enable(EnableCap.AlphaTest);
				if (_quality.DisableDepthBufferWhenAlpha)
				{
					GL.Disable(EnableCap.DepthTest);
				}
			}
			else
			{
				GL.Disable(EnableCap.AlphaTest);
			}

			// Make smooth colors for polygons (interpolate color between points)
			if (_quality.SmoothColor)
			{
				GL.ShadeModel(ShadingModel.Smooth);
			}
			else
			{
				GL.ShadeModel(ShadingModel.Flat);
			}

			// Make smoothing setting
			if (_quality.SmoothLine)
			{
				GL.Enable(EnableCap.LineSmooth);
				GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
			}
			else
			{
				GL.Disable(EnableCap.LineSmooth);
			}

			if (_quality.SmoothPoint)
			{
				GL.Enable(EnableCap.PointSmooth);
				GL.Hint(HintTarget.PointSmoothHint, HintMode.Fastest);
			}
			else
			{
				GL.Disable(EnableCap.PointSmooth);
			}
		}

		public void InitLights()
		{
			// Init light
			Scene.LightSet.Init();
			Scene.LightSet.Enable();
		}

		/// <summary>
		/// Clear color and depth buffer (same as <see cref="ClearColorAndDepth"/>).
		/// </summary>
		public void Clear()
		{
			ClearColorAndDepth();
		}

		/// <summary>
		/// Clear color and depth buffer (same as Clear).
		/// </summary>
		public void ClearColorAndDepth()
		{
			GL.ClearColor((float)_bgColor.R, (float)_bgColor.G, (float)_bgColor.B, (float)_bgColor.A);
			// clear with background
			GL.ClearDepth(1);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		public virtual void Render()
		{
			RenderBackground(0, 1);
			RenderScene();
			RenderOverlay();
			if (_dimensionDirty)
			{
				_dimensionDirty = false;
			}
		}

		public void RenderBackground(float left, float right)
		{
			//if ((_bgImg != null)) {
			//	_bgViewport.SetViewPort(_canvas.RendererWidth, _canvas.RendererHeight, left, right);
			//	_bgViewport.Render();
			//}
		}

		public void RenderBackground(ViewPort viewport)
		{
			//if ((_bgImg != null)) {
			//	_bgViewport.SetViewPort(viewport);
			//	_bgViewport.Render();
			//}
		}

		public void RenderScene()
		{
			RenderScene(new ViewPort(_canvas.RendererWidth, _canvas.RendererHeight));
		}

		public void RenderScene(float left, float right)
		{
			RenderScene(new ViewPort(_canvas.RendererWidth, _canvas.RendererHeight, (int)left, (int)right));
		}

        private void RenderTriangle()
        {
            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(1.0, 0.0, 0.0); GL.Vertex2(-0.5, -0.5); // Bottom Left
            GL.Color3(0.0, 1.0, 0.0); GL.Vertex2(0.5, -0.5);  // Bottom Right
            GL.Color3(0.0, 0.0, 1.0); GL.Vertex2(0.0, 0.5);   // Top Center
            GL.End();
        }

        public void RenderScene(ViewPort viewport)
        {
            UpdateQuality();
            UpdateCamera(viewport, ComputeScaling());
            RenderAxeBox();
            RenderSceneGraph();

            if (DISPLAY_AXE_WHOLE_BOUNDS)
            {
                RenderMousePointer();
            }

            GL.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // Clear screen

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            RenderTriangle(); // Delegate the triangle rendering
        }

        public void UpdateQuality()
		{
            if (_quality == null)
            {
                return; // Or handle the null case appropriately
            }

            if (_quality.AlphaActivated)
			{
				GL.Enable(EnableCap.Blend);
			}
			else
			{
				GL.Disable(EnableCap.Blend);
			}
		}

		public BoundingBox3d ComputeScaling()
		{
			//-- Scale the scene's view -------------------
			if (Squared)
			{
				_scaling = Squarify();
			}
			else
			{
				_scaling = Coord3d.IDENTITY.Clone();
			}

			// Compute the bounds for computing cam distance, clipping planes, etc ...
			if (_targetBox == null)
			{
				_targetBox = new BoundingBox3d(0, 1, 0, 1, 0, 1);
			}

			var boundsScaled = new BoundingBox3d();
			boundsScaled.Add(_targetBox.Scale(_scaling));
			if (MAINTAIN_ALL_OBJECTS_IN_VIEW)
			{
				boundsScaled.Add(Scene.Graph.Bounds.Scale(_scaling));
			}
			return boundsScaled;
		}

		public void UpdateCamera(ViewPort viewport, BoundingBox3d boundsScaled)
		{
			UpdateCamera(viewport, boundsScaled, boundsScaled.GetRadius());
		}

		public void UpdateCamera(ViewPort viewport, BoundingBox3d boundsScaled, double sceneRadiusScaled)
		{
			Coord3d target = _center.Multiply(_scaling);
			_viewpoint.Z = sceneRadiusScaled * 2;

			Coord3d eye;
			// maintain a reasonnable distance to the scene for viewing it
			switch (_viewmode)
			{
				case ViewPositionMode.FREE:
					eye = _viewpoint.Cartesian().Add(target);
					break;

				case ViewPositionMode.TOP:
					eye = _viewpoint;
					eye.X = -PI_div2;
					// on x
					eye.Y = PI_div2;
					// on top
					eye = eye.Cartesian().Add(target);
					break;

				case ViewPositionMode.PROFILE:
					eye = _viewpoint;
					eye.Y = 0;
					eye = eye.Cartesian().Add(target);
					break;

				default:
					throw new Exception("Unsupported viewmode : " + _viewmode);
			}

			Coord3d up;
			if (Math.Abs(_viewpoint.Y) == PI_div2)
			{
				// handle up vector
				Coord2d direction = new Coord2d(_viewpoint.X, _viewpoint.Y).Cartesian();
				if (_viewpoint.Y > 0)
				{
					// on top
					up = new Coord3d(-direction.X, -direction.Y, 0);
				}
				else
				{
					up = new Coord3d(direction.X, direction.Y, 0);
				}

				// handle "on-top" events
				if (!_wasOnTopAtLastRendering)
				{
					_wasOnTopAtLastRendering = true;
					FireViewOnTopEvent(true);
				}
			}
			else
			{
				// handle up vector
				up = new Coord3d(0, 0, 1);
				// handle "on-top" events
				if (_wasOnTopAtLastRendering)
				{
					_wasOnTopAtLastRendering = false;
					FireViewOnTopEvent(false);
				}
			}

			// Apply camera settings
			_cam.Target = target;
			_cam.Up = up;
			_cam.Eye = eye;

			// Set rendering volume
			if (_viewmode == ViewPositionMode.TOP)
			{
				if (IncludingTextLabels)
				{
					CorrectCameraPositionForIncludingTextLabels(viewport); // ' quite experimental !
				}
				else
				{
					_cam.RenderingSphereRadius = Math.Max(boundsScaled.XMax - boundsScaled.XMin, boundsScaled.YMax - boundsScaled.YMin) / 2;
				}
			}
			else
			{
				if (IncludingTextLabels)
				{
					CorrectCameraPositionForIncludingTextLabels(viewport); // ' quite experimental !
				}
				else
				{
					_cam.RenderingSphereRadius = sceneRadiusScaled;
				}
			}

			// Setup camera (i.e. projection matrix)
			//cam.setViewPort(canvas.getRendererWidth(),
			// canvas.getRendererHeight(), left, right);
			_cam.SetViewPort(viewport);
			_cam.Shoot(_cameraMode);
		}

		public void RenderAxeBox()
		{
			if (_axeBoxDisplayed)
			{
				GL.MatrixMode(MatrixMode.Modelview);
				_scene.LightSet.Disable();
				_axe.SetScale(_scaling);
				_axe.Draw(_cam);

				// for debug
				if (DISPLAY_AXE_WHOLE_BOUNDS)
				{
					var box = _axe.GetWholeBounds();
					var p = new Parallelepiped(box)
					{
						FaceDisplayed = false,
						WireframeColor = Color.MAGENTA,
						WireframeDisplayed = true
					};
					p.Draw(_cam);
				}
				_scene.LightSet.Enable();
			}
		}

		public void RenderSceneGraph()
		{
			RenderSceneGraph(true);
		}

		public void RenderSceneGraph(bool light)
		{
			if (light)
			{
				Scene.LightSet.Apply(_scaling);
				// gl.glEnable(GL2.GL_LIGHTING);
				// gl.glEnable(GL2.GL_LIGHT0);
				// gl.glDisable(GL2.GL_LIGHTING);
			}

			Scene.Graph.Transform = new Transform.Transform(new Transform.TransformScale(_scaling));
			Scene.Graph.Draw(_cam);
		}

		private void RenderMousePointer()
		{
			if (MousePosition != null)
			{
				var magenta = Color.MAGENTA;
				GL.Color4(magenta.R, magenta.G, magenta.B, magenta.A);

				// Mouse pointer
				GL.PointSize(10);
				GL.Begin(PrimitiveType.Points);
				GL.Vertex3(MousePosition.X, MousePosition.Y, MousePosition.Z);
				GL.End();

				// Bounds
				var bbox = Axe.GetBoxBounds();

				// X
				// Point
				GL.PointSize(5);
				GL.Begin(PrimitiveType.Points);
				GL.Vertex3(bbox.XMin, MousePosition.Y, MousePosition.Z);
				GL.End();

				// Line
				GL.Begin(PrimitiveType.Lines);
				GL.Vertex3(bbox.XMin, MousePosition.Y, MousePosition.Z);
				GL.Vertex3(MousePosition.X, MousePosition.Y, MousePosition.Z);
				GL.End();

				// Y
				// Point
				GL.PointSize(5);
				GL.Begin(PrimitiveType.Points);
				GL.Vertex3(MousePosition.X, bbox.YMin, MousePosition.Z);
				GL.End();

				// Line
				GL.Begin(PrimitiveType.Lines);
				GL.Vertex3(MousePosition.X, bbox.YMin, MousePosition.Z);
				GL.Vertex3(MousePosition.X, MousePosition.Y, MousePosition.Z);
				GL.End();

				// Z
				// Point
				GL.PointSize(5);
				GL.Begin(PrimitiveType.Points);
				GL.Vertex3(MousePosition.X, MousePosition.Y, bbox.ZMax);
				GL.End();

				// Line
				GL.Begin(PrimitiveType.Lines);
				GL.Vertex3(MousePosition.X, MousePosition.Y, bbox.ZMax);
				GL.Vertex3(MousePosition.X, MousePosition.Y, MousePosition.Z);
				GL.End();
			}
		}

		public void RenderOverlay()
		{
			RenderOverlay(new ViewPort(0, 0, _canvas.RendererWidth, _canvas.RendererHeight));
		}

		/// <summary>
		/// <para>
		/// Renders all provided Tooltips and Renderer2ds on top of
		/// the scene.
		/// </para>
		/// <para>
		/// Due to the behaviour of the Overlay implementation, Java2d
		/// geometries must be drawn relative to the Chart's
		/// IScreenCanvas, BUT will then be stretched to fit in the
		/// Camera's viewport. This bug is very important to consider, since
		/// the Camera's viewport may not occupy the full IScreenCanvas.
		/// Indeed, when View is not maximized (like the default behaviour), the
		/// viewport remains square and centered in the canvas, meaning the Overlay
		/// won't cover the full canvas area.
		/// </para>
		/// <para>
		/// In other words, the following piece of code draws a border around the
		/// View, and not around the complete chart canvas, although queried
		/// to occupy chart canvas dimensions:
		/// </para>
		/// <para>
		/// g2d.drawRect(1, 1, chart.getCanvas().getRendererWidth()-2,
		/// chart.getCanvas().getRendererHeight()-2);
		/// </para>
		/// <para>
		/// renderOverlay() must be called while the OpenGL2 context for the
		/// drawable is current, and after the OpenGL2 scene has been rendered.
		/// </para>
		/// </summary>
		/// <param name="viewport"></param>
		public void RenderOverlay(ViewPort viewport)
		{
			// NOT Implemented so far
		}

		internal void CorrectCameraPositionForIncludingTextLabels(ViewPort viewport)
		{
			_cam.SetViewPort(viewport);
			_cam.Shoot(_cameraMode);
			_axe.Draw(_cam);
			Clear();

			BoundingBox3d newBounds = _axe.GetWholeBounds().Scale(_scaling);

			if (_viewmode == ViewPositionMode.TOP)
			{
				double radius = Math.Max(newBounds.XMax - newBounds.XMin, newBounds.YMax - newBounds.YMin);
				radius += radius * STRETCH_RATIO;
				_cam.RenderingSphereRadius = radius;
			}
			else
			{
				Coord3d target = newBounds.GetCenter();
				_cam.Target = target;
				_cam.Eye = _viewpoint.Cartesian().Add(target);
				_cam.RenderingSphereRadius = newBounds.GetRadius();
			}
		}
        #endregion
    }
}
