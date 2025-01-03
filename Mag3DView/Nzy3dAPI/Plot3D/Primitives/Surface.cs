using Mag3DView.Nzy3dAPI.Colors;
using Mag3DView.Nzy3dAPI.Maths;
using Mag3DView.Nzy3dAPI.Plot3D.Primitives;
using Mag3DView.Nzy3dAPI.Plot3D.Rendering.Views;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Mag3DView.Nzy3dAPI.Plot3D.Primitives
{
    public class Surface : AbstractDrawable
    {
        private List<Vector3> vertices;
        private List<int> indices;
        private int vertexArrayObject;
        private int vertexBufferObject;
        private int indexBufferObject;


        public bool FaceDisplayed { get; set; } = true;  // Default to true
        public bool WireframeDisplayed { get; set; } = true;  // Default to true
        public Color WireframeColor { get; set; } = new Color(1, 1, 1, 1); // Default to white
        public ColorMapper ColorMapper { get; set; }
        public Bounds Bounds { get; private set; }

        public Func<float, float, float> Function { get; }
        public int Resolution { get; }

        // Custom Constructor 
        public Surface(Func<float, float, float> function, int resolution)
        {
            Function = function;
            Resolution = resolution;
        }

        // Constructor (keep this without OpenGL initialization)
        public Surface(Func<float, float, float> surfaceFunction, int gridSize, float scale = 1f)
        {
            vertices = new List<Vector3>();
            indices = new List<int>();

            // Create the grid of vertices
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    float x = i * scale;
                    float y = j * scale;
                    float z = surfaceFunction(x, y);
                    vertices.Add(new Vector3(x, y, z));
                }
            }

            // Generate indices to connect the vertices
            for (int i = 0; i < gridSize - 1; i++)
            {
                for (int j = 0; j < gridSize - 1; j++)
                {
                    int topLeft = i * gridSize + j;
                    int topRight = topLeft + 1;
                    int bottomLeft = (i + 1) * gridSize + j;
                    int bottomRight = bottomLeft + 1;

                    indices.Add(topLeft);
                    indices.Add(bottomLeft);
                    indices.Add(topRight);
                    indices.Add(topRight);
                    indices.Add(bottomLeft);
                    indices.Add(bottomRight);
                }
            }
        }

        // Initialize OpenGL buffers (called after OpenGL context is set up)
        public void InitializeOpenGLBuffers()
        {
            vertexArrayObject = GL.GenVertexArray();
            vertexBufferObject = GL.GenBuffer();
            indexBufferObject = GL.GenBuffer();

            GL.BindVertexArray(vertexArrayObject);

            // Set up vertex buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, (nint)(vertices.Count * Vector3.SizeInBytes), vertices.ToArray(), BufferUsageHint.StaticDraw);

            // Set up index buffer
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (nint)(indices.Count * sizeof(int)), indices.ToArray(), BufferUsageHint.StaticDraw);

            // Enable vertex attribute
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
        }

        public override void Draw(Camera cam)
        {
            Render();
        }

        public override BoundingBox3d GetBounds()
        {
            // Compute and return the bounding box based on the function
            return new BoundingBox3d();
        }

        public void Render()
        {
            GL.BindVertexArray(vertexArrayObject);
            if (FaceDisplayed)
            {
                GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, nint.Zero);
            }

            if (WireframeDisplayed)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, nint.Zero);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }

            GL.BindVertexArray(0);
        }
    }

    // Helper classes
    public class Bounds
    {
        public float ZMin { get; }
        public float ZMax { get; }

        public Bounds(float zMin, float zMax)
        {
            ZMin = zMin;
            ZMax = zMax;
        }
    }
}
