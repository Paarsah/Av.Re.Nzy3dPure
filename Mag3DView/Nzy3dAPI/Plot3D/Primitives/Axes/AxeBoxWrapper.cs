using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mag3DView.Nzy3dAPI.Plot3D.Primitives.Axes
{
    public class AxeBoxWrapper : AbstractDrawable
    {
        private readonly AxeBox _axes;

        public AxeBoxWrapper(AxeBox axes)
        {
            _axes = axes;
        }

        public override void Draw(Camera camera)
        {
            throw new NotImplementedException();
        }

        public override BoundingBox3d GetBounds()
        {
            throw new NotImplementedException();
        }

        // Override and forward required AbstractDrawable methods to _axes
    }
}
