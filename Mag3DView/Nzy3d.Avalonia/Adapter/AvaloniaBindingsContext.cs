using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.OpenGL;
using OpenTK;
using OpenTK.Graphics;

namespace Mag3DView.Nzy3d.Avalonia.Adapter
{
    public class AvaloniaBindingsContext : IBindingsContext
    {
        private readonly GlInterface glInterface;

        public AvaloniaBindingsContext(GlInterface glInterface)
        {
            this.glInterface = glInterface;
        }

        public IntPtr GetProcAddress(string procName)
        {
            // Use Avalonia's GlInterface to get the address of OpenGL functions
            return glInterface.GetProcAddress(procName);
        }
    }
}
