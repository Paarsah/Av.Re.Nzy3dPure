using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views.Modes;

namespace Mag3DView.Nzy3dAPI.Events
{
	public class ViewModeChangedEventArgs : ObjectEventArgs
	{
		public ViewModeChangedEventArgs(object objectChanged, ViewPositionMode mode) : base(objectChanged)
		{
			Mode = mode;
		}

		public ViewPositionMode Mode { get; }
	}
}
