using Mag3DView.Nzy3dAPI.Chart;

namespace Mag3DView.Nzy3dAPI.Factories
{
	public class SceneFactory
	{
		public static ChartScene GetInstance(bool sort)
		{
			return new ChartScene(sort);
		}
	}
}
