using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using OpenTK.Graphics.OpenGL;
using System;
using Mag3DView.Nzy3dAPI.Events.Keyboard;
using Mag3DView.Nzy3dAPI.Events.Mouse;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Canvas;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;
using Mag3DView.Nzy3d.Avalonia.Events.Mouse;
using Mag3DView.Nzy3d.Avalonia.Events.Keyboard;
using Mag3DView.Nzy3dAPI.Events;
using MouseButton = OpenTK.Windowing.GraphicsLibraryFramework.MouseButton;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using KeyModifiers = OpenTK.Windowing.GraphicsLibraryFramework.KeyModifiers;

namespace Mag3DView.Nzy3d.Avalonia
{
    public class Renderer3D : Control, ICanvas, IControllerEventListener
    {
        internal View _view;
        internal int _width;
        internal int _height;
        internal bool _doScreenshotAtNextDisplay = false;
        internal bool _traceGL;
        internal bool _debugGL;
        private EventHandler<PointerPressedEventArgs> _pointerPressedHandler;
        private EventHandler<PointerReleasedEventArgs> _pointerReleasedHandler;


        private MouseButtonEventArgs ConvertToOpenTKMouseButtonEventArgs(PointerEventArgs e)
        {
            MouseButton button = e.GetCurrentPoint(this).Properties switch
            {
                { IsLeftButtonPressed: true } => MouseButton.Left,
                { IsRightButtonPressed: true } => MouseButton.Right,
                { IsMiddleButtonPressed: true } => MouseButton.Middle,
                _ => throw new ArgumentOutOfRangeException()  // Handle invalid button cases
            };

            // Determine whether the action is a press or release
            InputAction action = e is PointerPressedEventArgs ? InputAction.Press : InputAction.Release;

            // Use the KeyModifiers property from the event args
            KeyModifiers modifiers = (KeyModifiers)e.KeyModifiers;

            return new MouseButtonEventArgs(button, action, modifiers);
        }

        private PointerButton GetButton(PointerEventArgs e)
        {
            var properties = e.GetCurrentPoint(this).Properties;

            if (properties.IsLeftButtonPressed)
                return PointerButton.Left;
            if (properties.IsRightButtonPressed)
                return PointerButton.Right;
            if (properties.IsMiddleButtonPressed)
                return PointerButton.Middle;

            return PointerButton.None; // Default if no button is pressed
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (_view != null)
            {
                _view.Clear();
                _view.Render();
            }
        }

        public string GetGpuInfo()
        {
            return $"{GL.GetString(StringName.Renderer)} - {GL.GetString(StringName.Vendor)}";
        }

        private void Renderer3D_Render(object sender, EventArgs e)
        {
            if (_view != null)
            {
                _view.Clear();
                _view.Render();

                if (_doScreenshotAtNextDisplay)
                {
                    _doScreenshotAtNextDisplay = false;
                }
            }
        }

        private void Renderer3D_Resize(object? sender, EventArgs e)
        {
            _width = (int)this.Bounds.Width;
            _height = (int)this.Bounds.Height;

            if (_view != null)
            {
                _view.DimensionDirty = true;
            }
        }

        public void AddKeyListener(IBaseKeyListener baseListener)
        {
            if (baseListener is not IKeyListener listener)
            {
                throw new ArgumentException("", nameof(baseListener));
            }

            this.KeyUp += (s, e) => listener.KeyReleased(s, e);
            this.KeyDown += (s, e) => listener.KeyPressed(s, e);
        }

        public void AddMouseListener(IBaseMouseListener baseListener)
        {
            if (baseListener is not IMouseListener listener)
            {
                throw new ArgumentException("", nameof(baseListener));
            }

            this.PointerPressed += (s, e) =>
            {
                var openTKEventArgs = ConvertToOpenTKMouseButtonEventArgs(e); // Use conversion method
                listener.MousePressed(s, openTKEventArgs);
            };

            this.PointerReleased += (s, e) =>
            {
                var openTKEventArgs = ConvertToOpenTKMouseButtonEventArgs(e); // Use conversion method
                listener.MouseReleased(s, openTKEventArgs);
            };
        }

        public void AddMouseMotionListener(IBaseMouseMotionListener baseListener)
        {
            if (baseListener is not IMouseMotionListener listener)
            {
                throw new ArgumentException("", nameof(baseListener));
            }

            this.PointerMoved += (s, e) =>
                listener.MouseMoved(s, new MouseEventArgs(e.GetPosition(this).X, e.GetPosition(this).Y, GetButton(e)));
        }

        public void AddMouseWheelListener(IBaseMouseWheelListener baseListener)
        {
            if (baseListener is not IMouseWheelListener listener)
            {
                throw new ArgumentException("", nameof(baseListener));
            }

            this.PointerWheelChanged += (s, e) =>
            {
                var mouseWheelArgs = new Nzy3dAPI.Events.Mouse.MouseWheelEventArgs(e.Delta.X, e.GetPosition(this).X, e.GetPosition(this).Y);
                listener.MouseWheelMoved(s, mouseWheelArgs);
            };
        }

        public void Dispose()
        {
            _view.Dispose();
        }

        public void ForceRepaint()
        {
            this.InvalidateVisual();
        }

        public void RemoveKeyListener(IBaseKeyListener baseListener)
        {
            if (baseListener is not IKeyListener listener)
            {
                throw new ArgumentException("", nameof(baseListener));
            }

            this.KeyUp -= (s, e) => listener.KeyReleased(s, e);
            this.KeyDown -= (s, e) => listener.KeyPressed(s, e);
        }

        public void RemoveMouseListener(IBaseMouseListener baseListener)
        {
            if (baseListener is not IMouseListener listener)
            {
                throw new ArgumentException("", nameof(baseListener));
            }

            // Remove the event handlers
            this.PointerPressed -= _pointerPressedHandler;
            this.PointerReleased -= _pointerReleasedHandler;
        }

        public void RemoveMouseMotionListener(IBaseMouseMotionListener baseListener)
        {
            if (baseListener is not IMouseMotionListener listener)
            {
                throw new ArgumentException("", nameof(baseListener));
            }

            this.PointerMoved += (s, e) =>
                listener.MouseMoved(s, new MouseEventArgs(e.GetPosition(this).X, e.GetPosition(this).Y, GetButton(e)));
        }

        public void RemoveMouseWheelListener(IBaseMouseWheelListener baseListener)
        {
            if (baseListener is not IMouseWheelListener listener)
            {
                throw new ArgumentException("", nameof(baseListener));
            }

            this.PointerWheelChanged -= (s, e) => listener.MouseWheelMoved(s, new Nzy3dAPI.Events.Mouse.MouseWheelEventArgs(e.Delta.X, e.GetPosition(this).X, e.GetPosition(this).Y));
        }

        public object Screenshot()
        {
            throw new NotImplementedException();
        }

        public void SetView(View value)
        {
            _view = value;
            _view.Init();
            _view.Scene.Graph.MountAllGLBindedResources();
            _view.BoundManual = _view.Scene.Graph.Bounds;
        }

        public int RendererWidth => (int)this.Bounds.Width;

        public int RendererHeight => (int)this.Bounds.Height;

        public View View => _view;

        public Renderer3D()
        {
            this.SizeChanged += Renderer3D_Resize;
        }


        #region IControllerEventListener
        public void ControllerEventFired(ControllerEventArgs e)
        {
            this.ForceRepaint();
        }
        #endregion
    }
}
