using Mag3DView.Nzy3dAPI.Maths;

namespace Mag3DView.Nzy3dAPI.Plot3D.Transform
{
	public interface ITransformer
	{
		// Execute the effective GL transformation held by this class.
		void Execute();

		Coord3d Compute(Coord3d input);
		// Apply the transformations to the input coordinates. (Warning: this method is a utility that may not be implemented.)
	}
}
