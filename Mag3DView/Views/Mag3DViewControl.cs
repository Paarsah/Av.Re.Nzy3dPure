using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;

namespace Mag3DView.Views
{
    public class Mag3DViewControl : OpenGlControlBase
    {
        private const int GL_COLOR_BUFFER_BIT = 0x00004000;
        private const int GL_DEPTH_BUFFER_BIT = 0x00000100;

        protected override void OnOpenGlInit(GlInterface gl)
        {
            // Perform OpenGL initialization here
            // For example, set up your rendering context, shaders, etc.
            gl.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);
        }

        protected override void OnOpenGlDeinit(GlInterface gl)
        {
            // Clean up any OpenGL resources here
        }

        protected override void OnOpenGlRender(GlInterface gl, int fb)
        {
            // Clear the screen
            gl.Clear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

            // Add your rendering logic here
        }
    }
}
