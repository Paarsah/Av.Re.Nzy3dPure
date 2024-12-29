using Avalonia.Input;
using Mag3DView.Nzy3dAPI.Events.Mouse;
using OpenTK.Windowing.Common;

namespace Mag3DView.Nzy3d.Avalonia.Events.Mouse
{
    public interface IMouseListener : IBaseMouseListener
    {
        void MousePressed(object sender, MouseButtonEventArgs e);
        void MouseReleased(object sender, MouseButtonEventArgs e);
    }
}
