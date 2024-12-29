using Mag3DView.Nzy3dAPI.Maths;
using OpenTK.Graphics.OpenGL;

namespace Mag3DView.Nzy3dAPI.Plot3D.Transform
{
	public sealed class TransformScale : ITransformer
	{
		private readonly Coord3d _scale;
		public TransformScale(Coord3d scale)
		{
			_scale = scale;
		}

		public Coord3d Compute(Coord3d input)
		{
			return input.Multiply(_scale);
		}

		public void Execute()
		{
			GL.Scale(_scale.X, _scale.Y, _scale.Z);
		}

		public override string ToString()
		{
			return "(Scale)" + _scale.ToString();
		}
	}
}
