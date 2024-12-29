using Mag3DView.Nzy3dAPI.Events.Mouse;

namespace Mag3DView.Nzy3d.Avalonia.Events.Mouse
{
    public interface IMouseWheelListener : IBaseMouseWheelListener
	{
		/// <summary>
		/// Invoked when a mouse button is pressed on a component and then dragged.
		/// </summary>
		void MouseWheelMoved(object? sender, MouseWheelEventArgs e);
	}
}
