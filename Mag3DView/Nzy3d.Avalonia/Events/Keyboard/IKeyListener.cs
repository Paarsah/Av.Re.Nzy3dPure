using Avalonia.Input;
using System;

namespace Mag3DView.Nzy3d.Avalonia.Events.Keyboard
{
    public interface IKeyListener
    {
        /// <summary>
        /// Invoked when a key has been pressed (key down in Avalonia).
        /// </summary>
        void KeyPressed(object? sender, KeyEventArgs e);

        /// <summary>
        /// Invoked when a key has been released (key up in Avalonia).
        /// </summary>
        void KeyReleased(object? sender, KeyEventArgs e);
    }
}
