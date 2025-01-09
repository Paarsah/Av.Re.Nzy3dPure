using Mag3DView.Nzy3dAPI.Chart.Controllers.Camera;
using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Primitives;
using Mag3DView.Nzy3dAPI.Plot3D.Primitives.Axes.Layout;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Canvas;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views.Modes;
using System.Collections.Generic;
using System;
using Mag3DView.Nzy3dAPI.Events.Mouse;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Scenes;
using System.Diagnostics;
using Mag3DView.Controls;

namespace Mag3DView.Nzy3dAPI.Chart
{
    public class Chart
    {
        protected ChartScene _scene { get; private set; }
        protected View _view;
        protected ICanvas _canvas;
        protected Coord3d _previousViewPointFree;
        protected Coord3d _previousViewPointTop;
        protected Coord3d _previousViewPointProfile;
        protected List<AbstractCameraController> _controllers;
        private Coord3d _previousMousePosition;
        public static readonly Quality DEFAULT_QUALITY = Quality.Intermediate;
        public Camera Camera { get; private set; }

        public Chart()
        {
            // Initialize the scene
            _scene = new ChartScene();
        }

        public Chart(ICanvas canvas, Scene scene) : this(canvas, DEFAULT_QUALITY)
        {
            Camera = new Camera();
        }

        public Chart(ICanvas canvas, Quality quality)
        {
            // Store canvas
            this._canvas = canvas;

            // Set up controllers
            _controllers = new List<AbstractCameraController>();

            // Set up scene 
            _scene = InitializeScene(quality.AlphaActivated);

            // Set up view
            _view = _scene.NewView(canvas, quality);

            // create view with links in scene and canvas
            _view.BackgroundColor = Colors.Color.WHITE;
        }

        /// <summary>
        /// Provides a concrete scene. This method shoud be overriden to inject a custom scene,
        /// which may rely on several views, and could enhance manipulation of scene graph.
        /// </summary>
        protected virtual ChartScene InitializeScene(bool graphsort)
        {
            return Factories.SceneFactory.GetInstance(graphsort);
        }

        public void Clear()
        {
            _scene.Clear();
            _view.Shoot();
        }

        public void Dispose()
        {
            ClearControllerList();
            _scene.Dispose();
            _canvas = null;
            _scene = null;
        }

        public void Render()
        {
            if (_view == null || _canvas == null)
            {
                Debug.WriteLine("View or Canvas is not initialized. Skipping render.");
                return;
            }

            Debug.WriteLine("Starting render...");
            if (_canvas is AvaloniaCanvas avaloniaCanvas)
            {
                avaloniaCanvas.MakeCurrent();
                avaloniaCanvas.Clear();
            }

            _view.Shoot(); // Render the scene

            if (_canvas is AvaloniaCanvas avaloniaCanvasSwap)
            {
                avaloniaCanvasSwap.SwapBuffers();
            }

            Debug.WriteLine("Render completed.");
        }

        //public System.Drawing.Bitmap Screenshot()
        //{
        //          return _canvas.Screenshot();
        //}

        public void UpdateProjectionsAndRender()
        {
            _view.Shoot();
            _view.Project();
            Render();
        }

        public void Render(Camera camera)
        {
            // Render the scene using the given camera
            _scene.Draw(camera);
        }

        /// <summary>
        /// Add a <see cref="AbstractCameraController"/> to this <see cref="Chart"/>.
        /// Warning: the <see cref="Chart"/> is not the owner of the controller. Disposing
        /// the chart thus just unregisters the controllers, but does not handle
        /// stopping and disposing controllers.
        /// </summary>
        public void AddController(AbstractCameraController controller)
        {
            controller.Register(this);
            _controllers.Add(controller);
        }

        public void RemoveController(AbstractCameraController controller)
        {
            controller.Unregister(this);
            _controllers.Remove(controller);
        }

        public void ClearControllerList()
        {
            foreach (AbstractCameraController controller in _controllers)
            {
                controller.Unregister(this);
            }
            _controllers.Clear();
        }

        public IEnumerable<AbstractCameraController> GetControllers()
        {
            return _controllers;
        }

        public void AddDrawable(AbstractDrawable drawable)
        {
            _scene.Graph.Add(drawable);
        }

        public void AddDrawable(AbstractDrawable drawable, bool updateViews)
        {
            _scene.Graph.Add(drawable, updateViews);
        }

        public void AddDrawable(IEnumerable<AbstractDrawable> drawables, bool updateViews)
        {
            _scene.Graph.Add(drawables, updateViews);
        }

        public void AddDrawable(IEnumerable<AbstractDrawable> drawables)
        {
            _scene.Graph.Add(drawables);
        }

        public void RemoveDrawable(AbstractDrawable drawable)
        {
            _scene.Graph.Remove(drawable);
        }

        public void RemoveDrawable(AbstractDrawable drawable, bool updateViews)
        {
            _scene.Graph.Remove(drawable, updateViews);
        }

        public void AddRenderer(IBaseRenderer2D renderer2d)
        {
            _view.AddRenderer2d(renderer2d);
        }

        public void RemoveRenderer(IBaseRenderer2D renderer2d)
        {
            _view.RemoveRenderer2d(renderer2d);
        }

        public View View
        {
            get { return _view; }
        }

        public ChartScene Scene
        {
            get { return _scene; }
        }

        public ICanvas Canvas
        {
            get { return _canvas; }
        }

        public IAxeLayout AxeLayout
        {
            get { return _view.Axe.Layout; }
        }

        public bool AxeDisplayed
        {
            set
            {
                _view.AxeBoxDisplayed = value;
                _view.Shoot();
            }
        }

        public Coord3d Viewpoint
        {
            get { return _view.ViewPoint; }
            set
            {
                _view.ViewPoint = value;
                _view.Shoot();
            }
        }

        public ViewPositionMode ViewMode
        {
            get { return _view.ViewMode; }
            set
            {
                // Store current view mode and view point in memory
                ViewPositionMode previous = View.ViewMode;
                switch (previous)
                {
                    case ViewPositionMode.FREE:
                        _previousViewPointFree = View.ViewPoint;
                        break;

                    case ViewPositionMode.PROFILE:
                        _previousViewPointTop = View.ViewPoint;
                        break;

                    case ViewPositionMode.TOP:
                        _previousViewPointProfile = View.ViewPoint;
                        break;

                    default:
                        throw new Exception("Unsupported ViewPositionMode :" + previous);
                }

                // Set new view mode and former view point
                _view.ViewMode = value;
                switch (previous)
                {
                    case ViewPositionMode.FREE:
                        _view.ViewPoint = _previousViewPointFree ?? View.DEFAULT_VIEW.Clone();
                        break;

                    case ViewPositionMode.PROFILE:
                        _view.ViewPoint = _previousViewPointTop ?? View.DEFAULT_VIEW.Clone();
                        break;

                    case ViewPositionMode.TOP:
                        _view.ViewPoint = _previousViewPointProfile ?? View.DEFAULT_VIEW.Clone();
                        break;

                    default:
                        throw new Exception("Unsupported ViewPositionMode :" + previous);
                }
                _view.Shoot();
            }
        }

        public Scale Scale
        {
            get { return new Scale(_view.Bounds.ZMin, _view.Bounds.ZMax); }
            set { _view.SetScale(value, true); }
        }

        public void SetScale(Scale scale, bool notify)
        {
            _view.SetScale(scale, notify);
        }

        public float Flip(float y)
        {
            return _canvas.RendererHeight - y;
        }

        public void StartMouseDrag(Coord3d startPosition)
        {
            // Store initial mouse position and/or initialize camera drag state
            _previousMousePosition = startPosition;
        }

        public void MouseDrag(Coord3d currentPosition)
        {
            // Calculate difference in mouse movement
            var deltaX = (float)(currentPosition.X - _previousMousePosition.X);
            var deltaY = (float)(currentPosition.Y - _previousMousePosition.Y);

            // Create Coord2d for rotation based on mouse movement
            Coord2d move = new Coord2d(deltaX, deltaY);

            // Use delta to rotate the camera (via AbstractCameraController)
            foreach (var controller in _controllers)
            {
                if (controller is AbstractCameraController cameraController)
                {
                    cameraController.RotateCamera(move);  // This calls the Rotate method defined in AbstractCameraController
                }
            }

            // Update previous mouse position for next frame
            _previousMousePosition = currentPosition;
        }

        public void EndMouseDrag()
        {
            // Finalize drag operation, if necessary (e.g., reset camera state or finish a smooth animation)
        }

        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Zoom in or out based on the mouse wheel
            _view.Zoom((float)e.Delta);
        }

    }
}
