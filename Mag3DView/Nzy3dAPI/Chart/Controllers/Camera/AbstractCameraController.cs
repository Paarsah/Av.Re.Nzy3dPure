using Mag3DView.Nzy3dAPI.Chart.Controllers;
using Mag3DView.Nzy3dAPI.Maths;

namespace Mag3DView.Nzy3dAPI.Chart.Controllers.Camera
{
	public class AbstractCameraController : AbstractController
	{
		public static bool DEFAULT_UPDATE_VIEW = false;

		public AbstractCameraController() : base()
		{
		}

		public AbstractCameraController(Chart chart) : base(chart)
		{
		}

		protected void SetMousePosition(int x, int y)
		{
			foreach (Chart c in _targets)
			{
				c.View.SetMousePosition(x, y);
			}
			FireControllerEvent(ControllerType.MOUSE, new Coord2d(x, y));
		}

		protected void Rotate(Coord2d move)
		{
			Rotate(move, DEFAULT_UPDATE_VIEW);
		}

		protected void Rotate(Coord2d move, bool updateView)
		{
			foreach (Chart c in _targets)
			{
				c.View.Rotate(move, updateView);
			}
            FireControllerEvent(ControllerType.ROTATE, move);
		}

        public void RotateCamera(Coord2d move)
        {
            Rotate(move);
        }


        protected void Shift(float factor)
		{
			Shift(factor, DEFAULT_UPDATE_VIEW);
		}

		protected void Shift(float factor, bool updateView)
		{
			foreach (Chart c in _targets)
			{
				c.View.Shift(factor, updateView);
			}
			FireControllerEvent(ControllerType.SHIFT, factor);
		}

		protected void ZoomX(float factor)
		{
			ZoomX(factor, DEFAULT_UPDATE_VIEW);
		}

		protected void ZoomX(float factor, bool updateView)
		{
			foreach (Chart c in _targets)
			{
				c.View.ZoomX(factor, updateView);
			}
			FireControllerEvent(ControllerType.ZOOM, factor);
		}

		protected void ZoomY(float factor)
		{
			ZoomY(factor, DEFAULT_UPDATE_VIEW);
		}

		protected void ZoomY(float factor, bool updateView)
		{
			foreach (Chart c in _targets)
			{
				c.View.ZoomY(factor, updateView);
			}
			FireControllerEvent(ControllerType.ZOOM, factor);
		}

		protected void ZoomZ(float factor)
		{
			ZoomZ(factor, DEFAULT_UPDATE_VIEW);
		}

		protected void ZoomZ(float factor, bool updateView)
		{
			foreach (Chart c in _targets)
			{
				c.View.ZoomZ(factor, updateView);
			}
			FireControllerEvent(ControllerType.ZOOM, factor);
		}

		protected void ZoomXYZ(float factor)
		{
			ZoomXYZ(factor, DEFAULT_UPDATE_VIEW);
		}

		protected void ZoomXYZ(float factor, bool updateView)
		{
			foreach (Chart c in _targets)
			{
				c.View.ZoomXYZ(factor, updateView);
			}
			FireControllerEvent(ControllerType.ZOOM, factor);
		}
	}
}