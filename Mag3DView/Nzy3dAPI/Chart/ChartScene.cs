using Mag3DView.Nzy3dAPI.Colors;
using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Primitives;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Canvas;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Scenes;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;
using System;

namespace Mag3DView.Nzy3dAPI.Chart
{
    public class ChartScene : Scene
	{
		internal int _nview;

		internal View _view;

        public ChartScene()
        {
            // Initialize the scene and add axes
            AddAxes();
        }

        public ChartScene(bool graphsort) : base(graphsort)
		{
			_nview = 0;
		}

        /// <summary>
        /// Draws the scene using the provided camera.
        /// </summary>
        public void Draw(Camera camera)
        {
            foreach (AbstractDrawable drawable in Graph.Drawables)
            {
                drawable.Draw(camera);
            }
        }

        public void AddAxes()
        {
            // Define bounding box for axes
            var bbox = new BoundingBox3d(-10, 10, -10, 10, -10, 10);

            // Create axes and customize their colors
            var axes = new AxeDrawable(bbox)
            {
                Colors = new[] { Color.RED, Color.GREEN, Color.BLUE }
            };

            // Add the axes to the scene
            Add(axes);
        }

        public void Refresh()
        {
            // Example: Reset the bounds to include all drawable elements
            UpdateBounds();

            // Notify the view to repaint
            _view?.Canvas.ForceRepaint();
        }

        public void UpdateBounds()
        {
            BoundingBox3d bounds = new BoundingBox3d();

            foreach (var drawable in Graph.GetAllDrawables())
            {
                var drawableBounds = drawable.GetBounds();
                if (drawableBounds != null && !drawableBounds.IsEmpty())
                {
                    bounds.Add(drawableBounds);
                }
            }

            _view?.SetBoundManual(bounds); // Update the view's manual bounds
        }

        public void Clear()
		{
			_view.BoundManual = new BoundingBox3d(0, 0, 0, 0, 0, 0);
		}

		public override View NewView(ICanvas canvas, Quality quality)
		{
			if (_nview > 0)
			{
				throw new Exception("A view has already been defined for this scene. Can not use several views.");
			}
			_nview++;
			_view = base.NewView(canvas, quality);
			return _view;
		}

		public override void ClearView(View view)
		{
			base.ClearView(view);
			_nview = 0;
		}
	}
}
