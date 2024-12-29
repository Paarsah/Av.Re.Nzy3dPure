namespace Mag3DView.Nzy3dAPI.Events
{
	public interface IViewIsVerticalEventListener
	{
		void ViewVerticalReached(ViewIsVerticalEventArgs e);

		void ViewVerticalLeft(ViewIsVerticalEventArgs e);
	}
}
