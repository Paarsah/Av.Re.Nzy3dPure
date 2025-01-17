using Mag3DView.Nzy3dAPI.Plot3D.Primitives;
using System.Collections.Generic;

namespace Mag3DView.Nzy3dAPI.Plot3D.Rendering.Scenes
{
	public class Decomposition
	{
		public static List<AbstractDrawable> GetDecomposition(List<AbstractDrawable> drawables)
		{
			List<AbstractDrawable> monotypes = new List<AbstractDrawable>();
			foreach (AbstractDrawable c in drawables)
			{
				if (c?.Displayed == true)
				{
                    if (c is AbstractComposite cAC)
                    {
                        monotypes.AddRange(GetDecomposition(cAC));
                    }
                    else if (c is AbstractDrawable cAD)
                    {
                        monotypes.Add(cAD);
                    }
                }
			}
			return monotypes;
		}

		/// <summary>
		/// Recursively expand all monotype Drawables from the given Composite
		/// </summary>
		public static List<AbstractDrawable> GetDecomposition(AbstractComposite input)
		{
			List<AbstractDrawable> selection = new List<AbstractDrawable>();
			foreach (AbstractDrawable c in input.Drawables)
			{
				if (c?.Displayed == true)
				{
                    if (c is AbstractComposite cAC)
                    {
                        selection.AddRange(GetDecomposition(cAC));
                    }
                    else if (c is AbstractDrawable cAD)
                    {
                        selection.Add(cAD);
                    }
                }
			}
			return selection;
		}
	}
}
