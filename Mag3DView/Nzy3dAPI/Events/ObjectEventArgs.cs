using System;

namespace Mag3DView.Nzy3dAPI.Events
{
    public class ObjectEventArgs : EventArgs
    {
        public ObjectEventArgs(object objectChanged) : base()
        {
            ObjectChanged = objectChanged;
        }

        public object ObjectChanged { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"ObjectEventArgs(ObjectChanged): {ObjectChanged}";
        }
    }
}
