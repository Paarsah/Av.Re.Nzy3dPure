using Avalonia.OpenGL;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mag3DView.Controllers
{
    class AvaloniaTkContext : IBindingsContext
    {
        private readonly GlInterface _glInterface;
        public AvaloniaTkContext(GlInterface glInterface) => _glInterface = glInterface;
        public nint GetProcAddress(string procName) => _glInterface.GetProcAddress(procName);
    }
}
