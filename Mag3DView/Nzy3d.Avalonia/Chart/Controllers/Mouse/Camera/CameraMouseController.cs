using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Canvas;
using System;

namespace Mag3DView.Nzy3d.Avalonia.Chart.Controllers.Mouse.Camera
{
    public class CameraMouseController
    {
        private Coord2d _prevMouse = new Coord2d();
        private float _prevZoomZ = 1.0f;

        /// <summary>
        /// Handles mouse interactions for controlling the camera.
        /// </summary>
        public void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var element = sender as Visual;

            if (element == null)
            {
                throw new ArgumentException("Sender must implement IVisual");
            }

            bool isDoubleClick = e.ClickCount == 2;

            // Handle mouse input (double click for toggle between rotation/auto-rotation)
            if (HandleSlaveThread(isDoubleClick))
            {
                return;
            }

            var p = e.GetPosition(element);

            _prevMouse.X = p.X;
            _prevMouse.Y = p.Y;
        }

        public void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            // Implement if needed for mouse release actions
        }

        public void OnPointerMoved(Visual? sender, PointerEventArgs e)
        {
            if (sender is not ICanvas canvas)
            {
                throw new InvalidOperationException("Sender must implement ICanvas.");
            }

            // Force a repaint of the canvas
            canvas.ForceRepaint();

            var p = e.GetPosition((Visual)sender); // Get position relative to IVisual

            Coord2d mouse = new Coord2d(p.X, p.Y);

            // Update the mouse position
            SetMousePosition((int)p.X, (int)(canvas.RendererHeight - p.Y));

            // Rotate on left mouse button press
            if (e.GetCurrentPoint(sender).Properties.IsLeftButtonPressed)
            {
                Coord2d move = mouse.Substract(_prevMouse).Divide(100);
                Rotate(move);
            }
            // Shift on right mouse button press
            else if (e.GetCurrentPoint(sender).Properties.IsRightButtonPressed)
            {
                Coord2d move = mouse.Substract(_prevMouse);
                if (move.Y != 0)
                {
                    Shift((float)(move.Y / 250));
                }
            }

            _prevMouse = mouse;
        }

        public void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            // Zoom based on mouse wheel direction
            if (e.Delta.Y > 0)
            {
                _prevZoomZ = 0.9f;
            }
            else
            {
                _prevZoomZ = 1.1f;
            }
            ZoomXYZ(_prevZoomZ);
        }

        // Placeholder methods to make the code compile
        private void Rotate(Coord2d move)
        {
            // Implement rotation logic here
        }

        private void Shift(float move)
        {
            // Implement shifting logic here
        }

        private float _minZoom = 0.1f;
        private float _maxZoom = 10f;

        private void ZoomXYZ(float zoomFactor)
        {
            _prevZoomZ = Math.Clamp(_prevZoomZ * zoomFactor, _minZoom, _maxZoom);
            // Implement zoom application logic
        }

        private bool HandleSlaveThread(bool isDoubleClick)
        {
            // Implement logic to handle thread if necessary, otherwise return false
            return false;
        }

        private void SetMousePosition(int x, int y)
        {
            // Implement logic to update mouse position if necessary
        }
    }
}
