using Mag3DView.Nzy3dAPI.Maths;

namespace Mag3DView.Nzy3dAPI.Events
{
	public class ViewPointChangedEventArgs : ObjectEventArgs
	{
		public ViewPointChangedEventArgs(object objectChanged, Coord3d viewPoint) : base(objectChanged)
		{
			ViewPoint = viewPoint;
		}

		public Coord3d ViewPoint { get; }
	}
}
