using OpenTK.Graphics.OpenGL;
using System;

namespace Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views
{
	public class ImageRenderer
	{
		public static void RenderImage(IntPtr image, int imageWidth, int imageHeight, int screenWidth, int screenHeight)
		{
			RenderImage(image, imageWidth, imageHeight, screenWidth, screenHeight, 0.75);
		}

		public static void RenderImage(IntPtr image, int imageWidth, int imageHeight, int screenWidth, int screenHeight, double z)
		{
			if (image == null)
			{
				return;
			}

			double xratio = 1;
			double yratio = 1;
			double xpict = 0;
			double ypict = 0;

			if (imageWidth < screenWidth)
			{
				xpict = Convert.ToInt32(screenWidth / 2 - imageWidth / 2);
			}
			else
			{
				xratio = screenWidth / imageWidth;
			}

			if (imageHeight < screenHeight)
			{
				xpict = Convert.ToInt32(screenHeight / 2 - imageWidth / 2);
			}
			else
			{
				xratio = screenHeight / imageHeight;
			}

			// Draw
			GL.PixelZoom((float)xratio, (float)yratio);
			GL.RasterPos3(xpict, ypict, z);
			GL.DrawPixels(imageWidth, imageHeight, PixelFormat.Rgba, PixelType.UnsignedByte, image);
		}
	}
}
