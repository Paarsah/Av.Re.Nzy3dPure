using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;
using System;

namespace Mag3DView.Nzy3dAPI.Factories
{
	public class Renderer3dFactory
	{
		public static object GetInstance(View view, bool traceGL, bool debugGL)
		{
			throw new NotFiniteNumberException("Renderer3dFactory.GetInstance()");
			//return new Renderer3d(view, traceGL, debugGL);
		}
	}
}
