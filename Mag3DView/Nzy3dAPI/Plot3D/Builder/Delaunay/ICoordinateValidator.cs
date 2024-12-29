namespace Mag3DView.Nzy3dAPI.Plot3D.Builder.Delaunay
{
	public interface ICoordinateValidator
	{
		float[] GetX();

		float[] GetY();

		/// <summary>
		/// Get_Z_as_fxy
		/// </summary>
		/// <returns></returns>
		float[,] GetZAsFxy();
	}
}
