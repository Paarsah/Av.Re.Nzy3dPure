using System;

namespace Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views
{
	public class ViewPort
	{
		internal int _width;
		internal int _height;
		internal int _x;

		internal int _y;
		public ViewPort(int width, int height) : this(width, height, 0, 0)
		{
		}

		public ViewPort(int width, int height, int x, int y)
		{
			_width = width;
			_height = height;
			_x = x;
			_y = y;
		}
        public void UpdateDimensions(int x, int y, int width, int height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public static ViewPort Slice(int width, int height, float left, float right, float bottom, float top)
        {
            if (left >= right || bottom >= top)
                throw new ArgumentException("Invalid slice parameters: ensure left < right and bottom < top.");

            int thisWidth = Convert.ToInt32((right - left) * width);
            int thisHeight = Convert.ToInt32((top - bottom) * height);
            int thisX = Convert.ToInt32(left * width);
            int thisY = Convert.ToInt32(bottom * height);
            return new ViewPort(thisWidth, thisHeight, thisX, thisY);
        }

        public int Width
		{
			get { return _width; }
			set { _width = value; }
		}

		public int Height
		{
			get { return _height; }
			set { _height = value; }
		}

		public int X
		{
			get { return _x; }
			set { _x = value; }
		}

		public int Y
		{
			get { return _y; }
			set { _y = value; }
		}

		public override string ToString()
		{
			return "(ViewPort) width=" + Width + " height=" + Height + " x=" + X + " y=" + Y;
		}
	}
}

//=======================================================
//Service provided by Telerik (www.telerik.com)
//Conversion powered by NRefactory.
//Twitter: @telerik
//Facebook: facebook.com/telerik
//=======================================================
