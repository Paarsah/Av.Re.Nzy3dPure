namespace Mag3DView.Nzy3dAPI.Events.Mouse
{
    public class MouseEventArgs
    {
        private PointerButton _button; // Change from MouseButton to PointerButton

        public MouseEventArgs(double x, double y, PointerButton button) // Change MouseButton to PointerButton here
        {
            X = x;
            Y = y;
            _button = button;
        }

        public double X { get; }

        public double Y { get; }

        public PointerButton Button  // Change MouseButton to PointerButton here
        {
            get { return _button; }
        }
    }

    // Define PointerButton enum (if not already defined)
    public enum PointerButton
    {
        None,
        Left,
        Right,
        Middle
    }
}
