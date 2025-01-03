using Mag3DView.Nzy3dAPI.Events.Keyboard;
using Mag3DView.Nzy3dAPI.Events.Mouse;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;

namespace Mag3DView.Nzy3dAPI.Plot3D.Rendering.Canvas
{
	public interface ICanvas
	{
		/// <summary>
		/// Returns a reference to the held view.
		/// </summary>
		View View { get; }

		/// <summary>
		/// Set the held view.
		/// </summary>
		/// <param name="value"></param>
		void SetView(View value);

		/// <summary>
		/// Returns the renderer's width, i.e. the display width.
		/// </summary>
		int RendererWidth { get; }

		/// <summary>
		/// Returns the renderer's height, i.e. the display height.
		/// </summary>
		int RendererHeight { get; }

		/// <summary>
		/// Invoked when a user requires the Canvas to be repainted (e.g. a non 3d layer has changed).
		/// </summary>
		void ForceRepaint();

		/// <summary>
		/// Returns an image with the current renderer's size.
		/// </summary>
		//System.Drawing.Bitmap Screenshot();
		object Screenshot();

		/// <summary>
		/// Performs all required cleanup when destroying a Canvas.
		/// </summary>
		void Dispose();

		void AddMouseListener(IBaseMouseListener listener);

		void RemoveMouseListener(IBaseMouseListener listener);

		void AddMouseWheelListener(IBaseMouseWheelListener listener);

		void RemoveMouseWheelListener(IBaseMouseWheelListener listener);

		void AddMouseMotionListener(IBaseMouseMotionListener listener);

		void RemoveMouseMotionListener(IBaseMouseMotionListener listener);

		void AddKeyListener(IBaseKeyListener listener);

		void RemoveKeyListener(IBaseKeyListener listener);
	}
}
