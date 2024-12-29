using Mag3DView.Nzy3dAPI.Maths;

namespace Mag3DView.Nzy3dAPI.Plot3D.Builder
{
	public interface IObjectTopology<O>
	{
		Coord3d GetCoord(O obj);

		string GetXAxisLabel();

		string GetYAxisLabel();

		string GetZAxisLabel();
	}
}
