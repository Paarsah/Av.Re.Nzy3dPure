using Avalonia.Input;
using Mag3DView.Nzy3dAPI.Events.Mouse;

namespace Mag3DView.Nzy3d.Avalonia.Events.Mouse
{
    public interface IMouseMotionListener : IBaseMouseMotionListener
    {
        /// <summary>
        /// Invoked when the mouse cursor has been moved onto a component but no buttons have been pushed.
        /// </summary>
        void MouseMoved(object? sender, MouseEventArgs e);
    }
}
