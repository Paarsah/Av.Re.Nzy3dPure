using Mag3DView.Nzy3dAPI.Plot3D.Primitives;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;
using Mag3DView.Nzy3dAPI.Plot3D.Transform;
using System.Collections.Generic;

namespace Mag3DView.Nzy3dAPI.Plot3D.Rendering.Ordering
{
	public abstract class AbstractOrderingStrategy : IComparer<AbstractDrawable>
	{
		internal Camera _camera;

		internal Transform.Transform _transform;

		public abstract int Compare(AbstractDrawable x, AbstractDrawable y);

		public void Sort(List<AbstractDrawable> monotypes, Camera cam)
		{
			_camera = cam;
			monotypes.Sort(this);
		}

		internal int Comparison(double dist1, double dist2)
		{
			if (dist1 == dist2)
			{
				return 0;
			}
			else if (dist1 < dist2)
			{
				return 1;
				//*Math.max((int)Math.abs(dist1-dist2),1)
			}
			else
			{
				return -1;
				//*Math.max((int)Math.abs(dist1-dist2),1);
			}
		}

		public void SetAll(Camera camera, Transform.Transform transform)
		{
			this.Camera = camera;
			this.Transform = transform;
		}

		public Camera Camera
		{
			get { return _camera; }
			set { _camera = value; }
		}

		public Transform.Transform Transform
		{
			get { return _transform; }
			set { _transform = value; }
		}
	}
}
