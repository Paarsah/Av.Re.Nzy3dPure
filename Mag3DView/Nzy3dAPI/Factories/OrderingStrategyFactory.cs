using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Ordering;

namespace Mag3DView.Nzy3dAPI.Factories
{
	public class OrderingStrategyFactory
	{
		public static AbstractOrderingStrategy GetInstance()
		{
			return DEFAULTORDERING;
		}

		public static readonly BarycentreOrderingStrategy DEFAULTORDERING = new BarycentreOrderingStrategy();
	}
}
