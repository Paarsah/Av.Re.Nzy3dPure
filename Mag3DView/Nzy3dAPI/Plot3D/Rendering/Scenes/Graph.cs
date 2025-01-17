using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Primitives;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Legends;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Ordering;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;
using Mag3DView.Nzy3dAPI.Plot3D.Transform;
using OpenTK.Graphics.OpenGL;
using SkiaSharp;
using System.Collections.Generic;

namespace Mag3DView.Nzy3dAPI.Plot3D.Rendering.Scenes
{
    /// <summary>
    /// <para>The scene's <see cref="Graph"/> basically stores the scene content and facilitate objects control</para>
    /// <para>
    /// The graph may decompose all <see cref="AbstractComposite"/> into a list of their <see cref="AbstractDrawable"/>s primitives
    /// if constructor is called with parameters enabling sorting.
    /// </para>
    /// <para>
    /// The list of primitives is ordered using either the provided <see cref="DefaultOrderingStrategy"/>
    /// or an other specified <see cref="AbstractOrderingStrategy"/>. Sorting is usefull for handling transparency
    /// properly.
    /// </para>
    /// <para>
    /// The <see cref="Graph"/> maintains a reference to its mother <see cref="Scene"/> in order to
    /// inform the <see cref="View"/>s when its content has change and that repainting is required.
    /// </para>
    /// <para>
    /// The add() method allows adding a <see cref="AbstractDrawable"/> to the scene Graph and updates
    /// all views' viewpoint in order to target the center of the scene.
    /// </para>
    /// <para>@author Martin Pernollet</para>
    /// </summary>
    public class Graph
	{
		internal List<AbstractDrawable> _components;
		internal Scene _scene;
		internal Transform.Transform _transform;
		internal AbstractOrderingStrategy _strategy;
        public BoundingBox3d Bounds { get; private set; } = new BoundingBox3d(-1, 1, -1, 1, -1, 1);
        public List<AbstractDrawable> Objects { get; private set; } = new List<AbstractDrawable>();
        private List<AbstractDrawable> drawables = new List<AbstractDrawable>();

        internal bool _sort = true;
        public Graph()
        {
            _scene = null; // Or provide a default Scene if applicable
            _strategy = new DefaultOrderingStrategy();
            _sort = true;
            _components = new List<AbstractDrawable>();
            Bounds = new BoundingBox3d();
            Objects = new List<AbstractDrawable>();
            drawables = new List<AbstractDrawable>();
        }

        /// <summary>
        /// Gets the collection of drawables in the graph.
        /// </summary>
        public IEnumerable<AbstractDrawable> Drawables => drawables;

        public Graph(Scene scene) : this(scene, new DefaultOrderingStrategy(), true)
		{
		}

		public Graph(Scene scene, bool sort) : this(scene, new DefaultOrderingStrategy(), sort)
		{
		}

		public Graph(Scene scene, AbstractOrderingStrategy strategy) : this(scene, strategy, true)
		{
		}

		public Graph(Scene scene, AbstractOrderingStrategy strategy, bool sort)
		{
			_scene = scene;
			_strategy = strategy;
			_sort = sort;
			_components = new List<AbstractDrawable>();
		}

        public IEnumerable<AbstractDrawable> GetAllComponents()
        {
            return _components;
        }

        public IEnumerable<AbstractDrawable> GetAllDrawables()
        {
            return drawables;
        }

        public void Dispose()
		{
			lock (_components)
			{
				foreach (AbstractDrawable c in _components)
				{
					c?.Dispose();
				}
				_components.Clear();
			}
			_scene = null;
		}

		public void Add(AbstractDrawable drawable, bool updateViews)
		{
			lock (_components)
			{
				_components.Add(drawable);
			}
			if (updateViews)
			{
				foreach (View view in _scene.Views)
				{
					view.UpdateBounds();
				}
			}
		}

		public void Add(AbstractDrawable drawable)
		{
            Add(drawable, true);
            Objects.Add(drawable);
            UpdateBounds(drawable);
            if (drawable != null)
            {
                drawables.Add(drawable);
            }
        }
        public void Clear()
        {
            drawables.Clear();
        }

        private void UpdateBounds(AbstractDrawable drawable)
        {
            if (drawable.Bounds != null)
            {
                Bounds.Add(drawable.Bounds);
            }
        }

        public BoundingBox3d ComputeBounds()
        {
            BoundingBox3d bounds = new BoundingBox3d();
            foreach (var drawable in Objects)
            {
                if (drawable.Bounds != null)
                {
                    bounds.Add(drawable.Bounds);
                }
            }
            return bounds;
        }

        public void Add(IEnumerable<AbstractDrawable> drawables, bool updateViews)
		{
			foreach (AbstractDrawable d in drawables)
			{
				Add(d, false);
			}
			if (updateViews)
			{
				foreach (View view in _scene.Views)
				{
					view.UpdateBounds();
				}
			}
		}

		public void Add(IEnumerable<AbstractDrawable> drawables)
		{
			Add(drawables, true);
		}

		public void Remove(AbstractDrawable drawable, bool updateViews)
		{
			lock (_components)
			{
				bool output = _components.Remove(drawable);
			}

			BoundingBox3d bbox = this.Bounds;
			foreach (View view in _scene.Views)
			{
				view.LookToBox(bbox);
				if (updateViews)
				{
					view.Shoot();
				}
			}
		}

		public void Remove(AbstractDrawable drawable)
		{
			Remove(drawable, true);
		}

		public IEnumerable<AbstractDrawable> All
		{
			get { return _components; }
		}

		public IEnumerable<IGLBindedResource> AllGLBindedResources
		{
			get
			{
				List<IGLBindedResource> @out = new List<IGLBindedResource>();
				lock (_components)
				{
					foreach (AbstractDrawable c in _components)
					{
						if (c is IGLBindedResource cIGL)
						{
							@out.Add(cIGL);
						}
					}
				}
				return @out;
			}
		}

		public void MountAllGLBindedResources()
		{
			foreach (IGLBindedResource r in this.AllGLBindedResources)
			{
				if (!r.HasMountedOnce())
				{
					r.Mount();
				}
			}
		}

		public void Draw(Camera camera)
		{
			Draw(camera, _components, _sort);
		}

		public void Draw(Camera camera, List<AbstractDrawable> components, bool sort)
		{
			GL.MatrixMode(MatrixMode.Modelview);
			lock (components)
			{
				if (!sort)
				{
					// render all items of the graph
					foreach (AbstractDrawable d in components)
					{
						if (d.Displayed)
						{
							d.Draw(camera);
						}
					}
				}
				else
				{
					// expand all composites into a list of monotypes
					List<AbstractDrawable> monotypes = Decomposition.GetDecomposition(components);
					//Compute order of monotypes for rendering
					_strategy.Sort(monotypes, camera);
					//Render sorted monotypes
					foreach (AbstractDrawable d in monotypes)
					{
						if (d.Displayed)
						{
							d.Draw(camera);
						}
					}
				}
			}
		}

		/// <summary>
		/// Update all interactive <see cref="AbstractDrawable"/> projections
		/// </summary>
		public void Project(Camera camera)
		{
			lock (_components)
			{
				foreach (AbstractDrawable d in _components)
				{
					if (d is ISelectable dS)
					{
						dS.Project(camera);
					}
				}
			}
		}

		public AbstractOrderingStrategy Strategy
		{
			get { return _strategy; }
			set { _strategy = value; }
		}

		/// <summary>
		/// Get/Set the transformation of this Graph
		/// When set, transforming is delegated iteratively to all Drawable of this graph.*/
		/// </summary>
		public Transform.Transform Transform
		{
			get { return _transform; }
			set
			{
				_transform = value;
				lock (_components)
				{
					foreach (AbstractDrawable c in _components)
					{
						if (c != null)
						{
							c.Transform = value;
						}
					}
				}
			}
		}

		//public BoundingBox3d Bounds
		//{
		//	get
		//	{
		//		if (_components.Count == 0)
		//		{
		//			return new BoundingBox3d(0, 0, 0, 0, 0, 0);
		//		}
		//		else
		//		{
		//			BoundingBox3d box = new BoundingBox3d();
		//			lock (_components)
		//			{
		//				foreach (AbstractDrawable a in _components)
		//				{
		//					if (a?.Bounds != null)
		//					{
		//						box.Add(a.Bounds);
		//					}
		//				}
		//			}
		//			return box;
		//		}
		//	}
		//}

		/// <summary>
		/// Return the list of available <see cref="AbstractDrawable"/>'s  displayed <see cref="BaseLegend"/>
		/// </summary>
		public List<Legend> Legends
		{
			get
			{
				List<Legend> list = new List<Legend>();
				lock (_components)
				{
					foreach (AbstractDrawable a in _components)
					{
						if (a?.HasLegend == true && a.LegendDisplayed)
						{
							list.Add(a.Legend);
						}
					}
				}
				return list;
			}
		}

		/// <summary>
		/// Returns the number of components with displayed legend
		/// </summary>
		public int HasLengends()
		{
			int k = 0;
			lock (_components)
			{
				foreach (AbstractDrawable a in _components)
				{
					if (a?.HasLegend == true && a.LegendDisplayed)
					{
						k++;
					}
				}
			}
			return k;
		}

		/// <summary>
		/// Print out information concerning all Drawable of this composite
		/// </summary>
		public override string ToString()
		{
			string output = "(Graph) #elements:" + _components.Count + ":\r\n";
			int k = 0;
			lock (_components)
			{
				foreach (AbstractDrawable a in _components)
				{
					if (a != null)
					{
						output += " Graph element [" + k + "]:" + a.ToString(1) + "\r\n";
					}
					else
					{
						output += " Graph element [" + k + "]:(null)\r\n";
					}
					k++;
				}
			}
			return output;
		}
	}
}
