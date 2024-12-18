using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;

class MyGame : GameWindow
{
    private Matrix4 _projectionMatrix;
    
    public MyGame()
        : base(800, 600, GraphicsMode.Default, "3D Scene with OpenTK")
    {
        VSync = VSyncMode.On;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        GL.ClearColor(Color4.CornflowerBlue);
        GL.Enable(EnableCap.DepthTest);

        _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 0.1f, 100.0f);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(ClientRectangle);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (Keyboard[Key.Escape])
        {
            Exit();
        }
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        Matrix4 modelView = Matrix4.LookAt(Vector3.UnitZ * 5, Vector3.Zero, Vector3.UnitY);
        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadMatrix(ref modelView);
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadMatrix(ref _projectionMatrix);

        // Рисуем объекты
        DrawCube();
        DrawPyramid();
        DrawCylinder();
        
        // Рисуем источники света как сферы
        DrawLightSources();

        SwapBuffers();
    }

    private void DrawCube()
    {
        GL.Begin(PrimitiveType.Quads);
        
        GL.Color3(1.0, 0.0, 0.0); // Красный цвет
        // Front face
        GL.Vertex3(-1.0, -1.0, 1.0);
        GL.Vertex3(1.0, -1.0, 1.0);
        GL.Vertex3(1.0, 1.0, 1.0);
        GL.Vertex3(-1.0, 1.0, 1.0);
        
        // Repeat for other faces...

        GL.End();
    }

    private void DrawPyramid()
    {
        GL.Begin(PrimitiveType.Triangles);

        GL.Color3(0.0, 1.0, 0.0); // Зеленый цвет
        // Base
        GL.Vertex3(-1.0, -1.0, -1.0);
        GL.Vertex3(1.0, -1.0, -1.0);
        GL.Vertex3(1.0, -1.0, 1.0);
        GL.Vertex3(-1.0, -1.0, 1.0);

        // Sides
        GL.Vertex3(0.0, 1.0, 0.0); // Apex
        GL.Vertex3(-1.0, -1.0, -1.0);
        GL.Vertex3(1.0, -1.0, -1.0);

        // Repeat for other sides...

        GL.End();
    }

    private void DrawCylinder()
    {
        GL.Begin(PrimitiveType.QuadStrip);

        GL.Color3(0.0, 0.0, 1.0); // Синий цвет
        float radius = 1.0f;
        float height = 2.0f;
        int segments = 32;

        for (int i = 0; i <= segments; i++)
        {
            float theta = (float)i / segments * MathHelper.TwoPi;
            float x = radius * (float)Math.Cos(theta);
            float z = radius * (float)Math.Sin(theta);

            GL.Vertex3(x, -height / 2, z);
            GL.Vertex3(x, height / 2, z);
        }
        
        GL.End();
    }

    private void DrawLightSources()
    {
        GL.Begin(PrimitiveType.Points);

        GL.Color3(1.0, 1.0, 0.0); // Желтый цвет для источников света
        GL.Vertex3(2.0, 2.0, 2.0);
        GL.Vertex3(-2.0, 2.0, 2.0);
        GL.Vertex3(0.0, 2.0, -2.0);

        GL.End();
    }

    [STAThread]
    static void Main()
    {
        using (MyGame game = new MyGame())
        {
            game.Run(60.0);
        }
    }
}
