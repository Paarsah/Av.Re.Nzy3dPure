using Mag3DView.Nzy3dAPI.Chart.Controllers;

namespace Mag3DView.Nzy3dAPI.Events
{
    public class ControllerEventArgs : ObjectEventArgs
    {
        public enum FieldChanged : byte
        {
            Data = 0,
            Transform = 1,
            Color = 2,
            Metadata = 3,
            Displayed = 4
        }

        public ControllerEventArgs(object objectChanged, ControllerType type, object value) : base(objectChanged)
        {
            Type = type;
            Value = value;
        }

        public ControllerType Type { get; }

        public object Value { get; }
    }
}
