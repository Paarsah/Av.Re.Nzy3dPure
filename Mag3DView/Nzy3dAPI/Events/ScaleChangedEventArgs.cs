using Mag3DView.Nzy3dAPI.Maths;

namespace Mag3DView.Nzy3dAPI.Events
{
	public class ScaleChangedEventArgs : ObjectEventArgs
	{
		public ScaleChangedEventArgs(object objectChanged, Scale scaling, int scaleID) : base(objectChanged)
		{
			Scaling = scaling;
			ScaleId = scaleID;
		}

		public ScaleChangedEventArgs(object objectChanged, Scale scaling) : this(objectChanged, scaling, -1)
		{
		}

		public Scale Scaling { get; }

		public int ScaleId { get; }

		/// <inheritdoc/>
		public override string ToString()
		{
			return "ScaleChangeEventArgs:id" + ScaleId + ", scale=" + Scaling.ToString();
		}
	}
}
