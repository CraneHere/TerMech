using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;

class Program
{
    private static float anglex = 70.0f;
    private static float angley = 5.0f;

    private static Vector3[] lightPositions = {
        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(-1.0f, 1.0f, -1.0f)
    };

    static void Main(string[] args)
    {
        using (var window = new GameWindow(800, 600, GraphicsMode.Default, "3D Scene with Cube, Pyramid, and Cylinder"))
        {
            window.VSync = VSyncMode.On;

            window.Load += (sender, e) =>
            {
                GL.Enable(EnableCap.DepthTest);
                GL.ClearColor(Color4.CornflowerBlue);
                SetupLights();
            };

            window.UpdateFrame += (sender, e) =>
            {
                if (Keyboard.GetState().IsKeyDown(Key.Escape))
                    window.Exit();

                // Обработка клавиатуры для изменения углов обзора
                if (Keyboard.GetState().IsKeyDown(Key.Right)) anglex += 1.0f;
                if (Keyboard.GetState().IsKeyDown(Key.Left)) anglex -= 1.0f;
                if (Keyboard.GetState().IsKeyDown(Key.Up)) angley += 0.5f;
                if (Keyboard.GetState().IsKeyDown(Key.Down)) angley -= 0.5f;

                // Обработка клавиш для перемещения источников света
                HandleLightMovement();
            };

            window.RenderFrame += (sender, e) =>
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                SetPerspective(45.0f, window.Width / (float)window.Height, 1.0f, 500.0f);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
                GLU.gluLookAt(0.0f, angley, 8.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);

                GL.Rotate(anglex, 0.0f, 1.0f, 0.0f);

                // Draw objects
                DrawCube(1.0f, new Vector3(-2.0f, 0.0f, 0.0f));
                DrawPyramid(1.0f, 1.5f, new Vector3(2.0f, 0.0f, 0.0f));
                DrawCylinder(0.5f, 1.5f, 32, new Vector3(0.0f, 0.0f, -2.0f));

                // Draw light sources
                DrawLightSource(lightPositions[0]);
                DrawLightSource(lightPositions[1]);

                window.SwapBuffers();
            };

            window.Run(60.0);
        }
    }

    private static void SetupLights()
    {
        // Установим начальное освещение
        GL.Light(LightName.Light0, LightParameter.Position, new float[] { lightPositions[0].X, lightPositions[0].Y, lightPositions[0].Z, 1.0f });
        GL.Light(LightName.Light1, LightParameter.Position, new float[] { lightPositions[1].X, lightPositions[1].Y, lightPositions[1].Z, 1.0f });
        GL.Enable(EnableCap.Light0);
        GL.Enable(EnableCap.Light1);
    }

    private static void HandleLightMovement()
    {
        if (Keyboard.GetState().IsKeyDown(Key.W)) lightPositions[0].Y += 0.01f;
        if (Keyboard.GetState().IsKeyDown(Key.S)) lightPositions[0].Y -= 0.01f;
        if (Keyboard.GetState().IsKeyDown(Key.A)) lightPositions[0].X -= 0.01f;
        if (Keyboard.GetState().IsKeyDown(Key.D)) lightPositions[0].X += 0.01f;
        if (Keyboard.GetState().IsKeyDown(Key.Q)) lightPositions[0].Z -= 0.01f;
        if (Keyboard.GetState().IsKeyDown(Key.E)) lightPositions[0].Z += 0.01f;

        if (Keyboard.GetState().IsKeyDown(Key.I)) lightPositions[1].Y += 0.01f;
        if (Keyboard.GetState().IsKeyDown(Key.K)) lightPositions[1].Y -= 0.01f;
        if (Keyboard.GetState().IsKeyDown(Key.J)) lightPositions[1].X -= 0.01f;
        if (Keyboard.GetState().IsKeyDown(Key.L)) lightPositions[1].X += 0.01f;
        if (Keyboard.GetState().IsKeyDown(Key.U)) lightPositions[1].Z -= 0.01f;
        if (Keyboard.GetState().IsKeyDown(Key.O)) lightPositions[1].Z += 0.01f;
    }

    private static void SetPerspective(float fov, float aspect, float nearPlane, float farPlane)
    {
        float f = (float)(1.0 / Math.Tan(fov * Math.PI / 360.0));
        float range = nearPlane - farPlane;

        float[] matrix = new float[16];

        matrix[0] = f / aspect;
        matrix[1] = 0.0f;
        matrix[2] = 0.0f;
        matrix[3] = 0.0f;

        matrix[4] = 0.0f;
        matrix[5] = f;
        matrix[6] = 0.0f;
        matrix[7] = 0.0f;

        matrix[8] = 0.0f;
        matrix[9] = 0.0f;
        matrix[10] = (farPlane + nearPlane) / range;
        matrix[11] = -1.0f;

        matrix[12] = 0.0f;
        matrix[13] = 0.0f;
        matrix[14] = (2.0f * farPlane * nearPlane) / range;
        matrix[15] = 0.0f;

        GL.MultMatrix(matrix);
    }

    private static void DrawCube(float size, Vector3 position)
    {
        GL.PushMatrix();
        GL.Translate(position);

        float halfSize = size / 2.0f;

        GL.Begin(PrimitiveType.Quads);

        // Front face
        GL.Color3(0.2f, 0.0f, 0.1f);
        GL.Vertex3(-halfSize, -halfSize, halfSize);
        GL.Vertex3(halfSize, -halfSize, halfSize);
        GL.Vertex3(halfSize, halfSize, halfSize);
        GL.Vertex3(-halfSize, halfSize, halfSize);

        // Back face
        GL.Color3(0.3f, 0.0f, 0.0f);
        GL.Vertex3(-halfSize, -halfSize, -halfSize);
        GL.Vertex3(-halfSize, halfSize, -halfSize);
        GL.Vertex3(halfSize, halfSize, -halfSize);
        GL.Vertex3(halfSize, -halfSize, -halfSize);

        // Top face
        GL.Color3(0.5f, 0.0f, 0.0f);
        GL.Vertex3(-halfSize, halfSize, -halfSize);
        GL.Vertex3(-halfSize, halfSize, halfSize);
        GL.Vertex3(halfSize, halfSize, halfSize);
        GL.Vertex3(halfSize, halfSize, -halfSize);

        // Bottom face
        GL.Color3(0.7f, 0.0f, 0.0f);
        GL.Vertex3(-halfSize, -halfSize, -halfSize);
        GL.Vertex3(halfSize, -halfSize, -halfSize);
        GL.Vertex3(halfSize, -halfSize, halfSize);
        GL.Vertex3(-halfSize, -halfSize, halfSize);

        // Right face
        GL.Color3(0.7f, 0.3f, 0.3f);
        GL.Vertex3(halfSize, -halfSize, -halfSize);
        GL.Vertex3(halfSize, halfSize, -halfSize);
        GL.Vertex3(halfSize, halfSize, halfSize);
        GL.Vertex3(halfSize, -halfSize, halfSize);

        // Left face
        GL.Color3(0.7f, 0.5f, 0.5f);
        GL.Vertex3(-halfSize, -halfSize, -halfSize);
        GL.Vertex3(-halfSize, -halfSize, halfSize);
        GL.Vertex3(-halfSize, halfSize, halfSize);
        GL.Vertex3(-halfSize, halfSize, -halfSize);

        GL.End();
        GL.PopMatrix();
    }

    private static void DrawPyramid(float size, float height, Vector3 position)
    {
        float halfSize = size / 2.0f;

        GL.PushMatrix();
        GL.Translate(position);

        GL.Begin(PrimitiveType.Triangles);

        // Front face
        GL.Color3(0.0f, 1.0f, 0.0f);
        GL.Vertex3(0.0f, height, 0.0f);
        GL.Vertex3(-halfSize, 0.0f, halfSize);
        GL.Vertex3(halfSize, 0.0f, halfSize);

        // Right face
        GL.Color3(0.0f, 0.7f, 0.0f);
        GL.Vertex3(0.0f, height, 0.0f);
        GL.Vertex3(halfSize, 0.0f, halfSize);
        GL.Vertex3(halfSize, 0.0f, -halfSize);

        // Back face
        GL.Color3(0.0f, 0.5f, 0.0f);
        GL.Vertex3(0.0f, height, 0.0f);
        GL.Vertex3(halfSize, 0.0f, -halfSize);
        GL.Vertex3(-halfSize, 0.0f, -halfSize);

        // Left face
        GL.Color3(0.0f, 0.3f, 0.0f);
        GL.Vertex3(0.0f, height, 0.0f);
        GL.Vertex3(-halfSize, 0.0f, -halfSize);
        GL.Vertex3(-halfSize, 0.0f, halfSize);

        GL.End();

        GL.Begin(PrimitiveType.Quads);
        GL.Color3(1.0f, 1.0f, 0.0f);

        // Base
        GL.Vertex3(-halfSize, 0.0f, halfSize);
        GL.Vertex3(halfSize, 0.0f, halfSize);
        GL.Vertex3(halfSize, 0.0f, -halfSize);
        GL.Vertex3(-halfSize, 0.0f, -halfSize);

        GL.End();
        GL.PopMatrix();
    }

    private static void DrawCylinder(float radius, float height, int slices, Vector3 position)
    {
        float angleStep = 2 * (float)Math.PI / slices;

        GL.PushMatrix();
        GL.Translate(position);

        GL.Begin(PrimitiveType.QuadStrip);
        for (int i = 0; i <= slices; ++i)
        {
            float angle = i * angleStep;
            float x = radius * (float)Math.Cos(angle);
            float z = radius * (float)Math.Sin(angle);

            GL.Color3(0.0f, 0.0f, 0.5f);
            GL.Vertex3(x, 0.0f, z);
            GL.Color3(0.5f, 0.0f, 0.0f);
            GL.Vertex3(x, height, z);
        }
        GL.End();

        GL.Begin(PrimitiveType.TriangleFan);
        GL.Vertex3(0.0f, 0.0f, 0.0f);
        for (int i = 0; i <= slices; ++i)
        {
            float angle = i * angleStep;
            float x = radius * (float)Math.Cos(angle);
            float z = radius * (float)Math.Sin(angle);
            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(x, 0.0f, z);
        }
        GL.End();

        GL.Begin(PrimitiveType.TriangleFan);
        GL.Vertex3(0.0f, height, 0.0f);
        for (int i = 0; i <= slices; ++i)
        {
            float angle = i * angleStep;
            float x = radius * (float)Math.Cos(angle);
            float z = radius * (float)Math.Sin(angle);
            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(x, height, z);
        }
        GL.End();

        GL.PopMatrix();
    }

    private static void DrawLightSource(Vector3 position)
    {
        GL.PushMatrix();
        GL.Translate(position);
        GL.Color3(1.0f, 1.0f, 1.0f);
        GLU.glutSolidSphere(0.1f, 10, 10);
        GL.PopMatrix();
    }
}
