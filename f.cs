using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;

class Program : GameWindow
{
    // Light positions
    private Vector3[] lightPositions = {
        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(-1.0f, 1.0f, -1.0f)
    };

    private float anglex = 70.0f;
    private float angley = 5.0f;

    // Cube drawing function
    private void DrawCube(float size)
    {
        float halfSize = size / 2.0f;
        float cubeShiftX = 3.0f;

        GL.Begin(PrimitiveType.Quads);
        // Front face
        GL.Color3(0.2f, 0.0f, 0.1f);
        GL.Vertex3(cubeShiftX + -halfSize, -halfSize, halfSize);
        GL.Vertex3(cubeShiftX + halfSize, -halfSize, halfSize);
        GL.Vertex3(cubeShiftX + halfSize, halfSize, halfSize);
        GL.Vertex3(cubeShiftX + -halfSize, halfSize, halfSize);

        // Back face
        GL.Color3(0.3f, 0.0f, 0.0f);
        GL.Vertex3(cubeShiftX + -halfSize, -halfSize, -halfSize);
        GL.Vertex3(cubeShiftX + -halfSize, halfSize, -halfSize);
        GL.Vertex3(cubeShiftX + halfSize, halfSize, -halfSize);
        GL.Vertex3(cubeShiftX + halfSize, -halfSize, -halfSize);

        // Top face
        GL.Color3(0.5f, 0.0f, 0.0f);
        GL.Vertex3(cubeShiftX + -halfSize, halfSize, -halfSize);
        GL.Vertex3(cubeShiftX + -halfSize, halfSize, halfSize);
        GL.Vertex3(cubeShiftX + halfSize, halfSize, halfSize);
        GL.Vertex3(cubeShiftX + halfSize, halfSize, -halfSize);

        // Bottom face
        GL.Color3(0.7f, 0.0f, 0.0f);
        GL.Vertex3(cubeShiftX + -halfSize, -halfSize, -halfSize);
        GL.Vertex3(cubeShiftX + halfSize, -halfSize, -halfSize);
        GL.Vertex3(cubeShiftX + halfSize, -halfSize, halfSize);
        GL.Vertex3(cubeShiftX + -halfSize, -halfSize, halfSize);

        // Right face
        GL.Color3(0.7f, 0.3f, 0.3f);
        GL.Vertex3(cubeShiftX + halfSize, -halfSize, -halfSize);
        GL.Vertex3(cubeShiftX + halfSize, halfSize, -halfSize);
        GL.Vertex3(cubeShiftX + halfSize, halfSize, halfSize);
        GL.Vertex3(cubeShiftX + halfSize, -halfSize, halfSize);

        // Left face
        GL.Color3(0.7f, 0.5f, 0.5f);
        GL.Vertex3(cubeShiftX + -halfSize, -halfSize, -halfSize);
        GL.Vertex3(cubeShiftX + -halfSize, -halfSize, halfSize);
        GL.Vertex3(cubeShiftX + -halfSize, halfSize, halfSize);
        GL.Vertex3(cubeShiftX + -halfSize, halfSize, -halfSize);
        GL.End();
    }

    // Pyramid drawing function
    private void DrawPyramid(float size, float height)
    {
        float halfSize = size / 2.0f;
        float pyramidShiftX = 2.0f;

        GL.Begin(PrimitiveType.Triangles);
        // Front face
        GL.Color3(0.0f, 1.0f, 0.0f);
        GL.Vertex3(pyramidShiftX + 0.0f, height, 0.0f);
        GL.Vertex3(pyramidShiftX + -halfSize, 0.0f, halfSize);
        GL.Vertex3(pyramidShiftX + halfSize, 0.0f, halfSize);

        // Right face
        GL.Color3(0.0f, 0.7f, 0.0f);
        GL.Vertex3(pyramidShiftX + 0.0f, height, 0.0f);
        GL.Vertex3(pyramidShiftX + halfSize, 0.0f, halfSize);
        GL.Vertex3(pyramidShiftX + halfSize, 0.0f, -halfSize);

        // Back face
        GL.Color3(0.0f, 0.5f, 0.0f);
        GL.Vertex3(pyramidShiftX + 0.0f, height, 0.0f);
        GL.Vertex3(pyramidShiftX + halfSize, 0.0f, -halfSize);
        GL.Vertex3(pyramidShiftX + -halfSize, 0.0f, -halfSize);

        // Left face
        GL.Color3(0.0f, 0.3f, 0.0f);
        GL.Vertex3(pyramidShiftX + 0.0f, height, 0.0f);
        GL.Vertex3(pyramidShiftX + -halfSize, 0.0f, -halfSize);
        GL.Vertex3(pyramidShiftX + -halfSize, 0.0f, halfSize);
        GL.End();

        GL.Begin(PrimitiveType.Quads);
        // Base
        GL.Color3(1.0f, 1.0f, 0.0f);
        GL.Vertex3(pyramidShiftX + -halfSize, 0.0f, halfSize);
        GL.Vertex3(pyramidShiftX + halfSize, 0.0f, halfSize);
        GL.Vertex3(pyramidShiftX + halfSize, 0.0f, -halfSize);
        GL.Vertex3(pyramidShiftX + -halfSize, 0.0f, -halfSize);
        GL.End();
    }

    // Cylinder drawing function
    private void DrawCylinder(float radius, float height, int slices)
    {
        float angleStep = 2 * MathF.PI / slices;
        float cylinderShiftX = 2.0f;

        GL.Begin(PrimitiveType.QuadStrip);
        for (int i = 0; i <= slices; ++i)
        {
            float angle = i * angleStep;
            float x = radius * MathF.Cos(angle);
            float z = radius * MathF.Sin(angle);
            GL.Color3(0.0f, 0.0f, 0.5f);
            GL.Vertex3(cylinderShiftX + x, 0.0f, z);
            GL.Color3(0.5f, 0.0f, 0.0f);
            GL.Vertex3(cylinderShiftX + x, height, z);
        }
        GL.End();

        GL.Begin(PrimitiveType.TriangleFan);
        GL.Vertex3(cylinderShiftX + 0.0f, 0.0f, 0.0f);
        for (int i = 0; i <= slices; ++i)
        {
            float angle = i * angleStep;
            float x = radius * MathF.Cos(angle);
            float z = radius * MathF.Sin(angle);
            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(cylinderShiftX + x, 0.0f, z);
        }
        GL.End();

        GL.Begin(PrimitiveType.TriangleFan);
        GL.Vertex3(cylinderShiftX + 0.0f, height, 0.0f);
        for (int i = 0; i <= slices; ++i)
        {
            float angle = i * angleStep;
            float x = radius * MathF.Cos(angle);
            float z = radius * MathF.Sin(angle);
            GL.Color3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(cylinderShiftX + x, height, z);
        }
        GL.End();
    }

    // Light source drawing function
    private void DrawLightSource(Vector3 position)
    {
        GL.PushMatrix();
        GL.Translate(position);
        GL.Color3(1.0f, 1.0f, 1.0f);
        GLUT.glutSolidSphere(0.1, 10, 10); // Drawing the light source as a sphere
        GL.PopMatrix();
    }

    // Handling the rendering loop
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        GL.Enable(EnableCap.DepthTest);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Width / (float)Height, 1.0f, 500.0f);
        GL.LoadMatrix(ref projection);

        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();
        Matrix4 lookAt = Matrix4.LookAt(0.0f, angley, 8.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
        GL.LoadMatrix(ref lookAt);
        
        GL.Rotate(anglex, 0.0f, 1.0f, 0.0f);

        // Draw Cube
        GL.PushMatrix();
        GL.Translate(-2.0f, 0.0f, 0.0f);
        DrawCube(1.0f);
        GL.PopMatrix();

        // Draw Pyramid
        GL.PushMatrix();
        GL.Translate(2.0f, 0.0f, 0.0f);
        DrawPyramid(1.0f, 1.5f);
        GL.PopMatrix();

        // Draw Cylinder
        GL.PushMatrix();
        GL.Translate(0.0f, 0.0f, -2.0f);
        DrawCylinder(0.5f, 1.5f, 32);
        GL.PopMatrix();

        // Draw light sources
        DrawLightSource(lightPositions[0]);
        DrawLightSource(lightPositions[1]);

        SwapBuffers();
    }

    // Handling the key input
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (Keyboard[Key.Right]) anglex += 1.0f;
        if (Keyboard[Key.Left]) anglex -= 1.0f;
        if (Keyboard[Key.Up]) angley += 0.5f;
        if (Keyboard[Key.Down]) angley -= 0.5f;

        if (Keyboard[Key.W]) lightPositions[0].Y += 0.01f;
        if (Keyboard[Key.S]) lightPositions[0].Y -= 0.01f;
        if (Keyboard[Key.A]) lightPositions[0].X -= 0.01f;
        if (Keyboard[Key.D]) lightPositions[0].X += 0.01f;
        if (Keyboard[Key.Q]) lightPositions[0].Z -= 0.01f;
        if (Keyboard[Key.E]) lightPositions[0].Z += 0.01f;

        if (Keyboard[Key.I]) lightPositions[1].Y += 0.01f;
        if (Keyboard[Key.K]) lightPositions[1].Y -= 0.01f;
        if (Keyboard[Key.J]) lightPositions[1].X -= 0.01f;
        if (Keyboard[Key.L]) lightPositions[1].X += 0.01f;
        if (Keyboard[Key.U]) lightPositions[1].Z -= 0.01f;
        if (Keyboard[Key.O]) lightPositions[1].Z += 0.01f;
    }

    static void Main(string[] args)
    {
        using (var game = new Program())
        {
            game.Run(60.0); // 60 FPS
        }
    }
}
