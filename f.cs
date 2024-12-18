using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace OpenTKScene
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var settings = new NativeWindowSettings
            {
                Size = new Vector2i(800, 600),
                Title = "3D Scene with Cube, Pyramid, and Cylinder",
                Flags = ContextFlags.ForwardCompatible
            };

            using (var window = new SceneWindow(GameWindowSettings.Default, settings))
            {
                window.Run();
            }
        }
    }

    public class SceneWindow : GameWindow
    {
        private Vector3[] lightPositions = new[]
        {
            new Vector3(1.0f, 1.0f, 1.0f),
            new Vector3(-1.0f, 1.0f, -1.0f)
        };

        private float anglex = 70.0f;
        private float angley = 5.0f;

        public SceneWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            base.OnLoad();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (!IsFocused) return;

            // Keyboard controls for camera
            var keyboardState = KeyboardState;
            if (keyboardState.IsKeyDown(Keys.Right)) anglex += 1f;
            if (keyboardState.IsKeyDown(Keys.Left)) anglex -= 1f;
            if (keyboardState.IsKeyDown(Keys.Up)) angley += 0.5f;
            if (keyboardState.IsKeyDown(Keys.Down)) angley -= 0.5f;

            // Light movement controls
            if (keyboardState.IsKeyDown(Keys.W)) lightPositions[0].Y += 0.01f;
            if (keyboardState.IsKeyDown(Keys.S)) lightPositions[0].Y -= 0.01f;
            if (keyboardState.IsKeyDown(Keys.A)) lightPositions[0].X -= 0.01f;
            if (keyboardState.IsKeyDown(Keys.D)) lightPositions[0].X += 0.01f;
            if (keyboardState.IsKeyDown(Keys.Q)) lightPositions[0].Z -= 0.01f;
            if (keyboardState.IsKeyDown(Keys.E)) lightPositions[0].Z += 0.01f;

            if (keyboardState.IsKeyDown(Keys.I)) lightPositions[1].Y += 0.01f;
            if (keyboardState.IsKeyDown(Keys.K)) lightPositions[1].Y -= 0.01f;
            if (keyboardState.IsKeyDown(Keys.J)) lightPositions[1].X -= 0.01f;
            if (keyboardState.IsKeyDown(Keys.L)) lightPositions[1].X += 0.01f;
            if (keyboardState.IsKeyDown(Keys.U)) lightPositions[1].Z -= 0.01f;
            if (keyboardState.IsKeyDown(Keys.O)) lightPositions[1].Z += 0.01f;

            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Size.X / (float)Size.Y, 1.0f, 500.0f);
            GL.LoadMatrix(ref perspective);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Matrix4 lookAt = Matrix4.LookAt(new Vector3(0.0f, angley, 8.0f), Vector3.Zero, Vector3.UnitY);
            GL.LoadMatrix(ref lookAt);
            GL.Rotate(anglex, 0.0f, 1.0f, 0.0f);

            // Draw objects
            DrawCube(1.0f, new Vector3(-2.0f, 0.0f, 0.0f));
            DrawPyramid(1.0f, 1.5f, new Vector3(2.0f, 0.0f, 0.0f));
            DrawCylinder(0.5f, 1.5f, 32, new Vector3(0.0f, 0.0f, -2.0f));

            // Draw light sources
            DrawLightSource(lightPositions[0]);
            DrawLightSource(lightPositions[1]);

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

        private void DrawCube(float size, Vector3 position)
        {
            float halfSize = size / 2.0f;

            GL.PushMatrix();
            GL.Translate(position);

            GL.Begin(PrimitiveType.Quads);
            // Front face
            GL.Color3(0.2f, 0.0f, 0.1f);
            GL.Vertex3(-halfSize, -halfSize, halfSize);
            GL.Vertex3(halfSize, -halfSize, halfSize);
            GL.Vertex3(halfSize, halfSize, halfSize);
            GL.Vertex3(-halfSize, halfSize, halfSize);
            // Other faces (similar logic)
            GL.End();

            GL.PopMatrix();
        }

        private void DrawPyramid(float size, float height, Vector3 position)
        {
            GL.PushMatrix();
            GL.Translate(position);

            GL.Begin(PrimitiveType.Triangles);
            // Front face
            GL.Color3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(0.0f, height, 0.0f);
            GL.Vertex3(-size / 2, 0.0f, size / 2);
            GL.Vertex3(size / 2, 0.0f, size / 2);
            GL.End();

            GL.PopMatrix();
        }

        private void DrawCylinder(float radius, float height, int slices, Vector3 position)
        {
            float angleStep = 2 * MathF.PI / slices;

            GL.PushMatrix();
            GL.Translate(position);

            GL.Begin(PrimitiveType.QuadStrip);
            for (int i = 0; i <= slices; ++i)
            {
                float angle = i * angleStep;
                float x = radius * MathF.Cos(angle);
                float z = radius * MathF.Sin(angle);
                GL.Color3(0.0f, 0.0f, 0.5f);
                GL.Vertex3(x, 0.0f, z);
                GL.Vertex3(x, height, z);
            }
            GL.End();

            GL.PopMatrix();
        }

        private void DrawLightSource(Vector3 position)
        {
            GL.PushMatrix();
            GL.Translate(position);
            GL.Color3(1.0f, 1.0f, 1.0f);
            GL.Begin(PrimitiveType.Sphere); // Replace with OpenTK-based sphere drawing.
            GL.End();
            GL.PopMatrix();
        }
    }
}
