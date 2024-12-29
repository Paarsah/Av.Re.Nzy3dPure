namespace Mag3DView.Nzy3dAPI.Events.Mouse
{
    public class MouseWheelEventArgs
    {
        public double Delta { get; }
        public double X { get; }
        public double Y { get; }

        public MouseWheelEventArgs(double delta, double x, double y)
        {
            Delta = delta;
            X = x;
            Y = y;
        }
    }
}
