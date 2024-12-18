using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Numerics;

public class OpenGLWindow : GameWindow
{
    private float rotation = 0f;

    public OpenGLWindow() : base(GameWindowSettings.Default, new NativeWindowSettings { Size = new OpenTK.Mathematics.Vector2i(800, 600), Title = "3D Scene with Lighting" })
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        // Инициализация OpenGL
        GL.ClearColor(0.39f, 0.58f, 0.93f, 1.0f);  // Цвет фона (cornflower blue)
        GL.Enable(EnableCap.DepthTest);  // Включаем тест глубины
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        // Обновляем размер окна
        GL.Viewport(0, 0, e.Width, e.Height);

        // Настройка перспективной проекции
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45), // угол обзора
            Width / (float)Height,            // соотношение сторон
            0.1f,                            // ближайшая плоскость отсечения
            100f                             // дальняя плоскость отсечения
        );

        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadMatrix(ref projection);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        // Очистка экрана
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Модельный обзор (сдвиг сцены, вращение объектов)
        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();

        // Камера
        GL.Translate(0f, 0f, -6f);

        // Рисуем объекты

        // Куб
        GL.PushMatrix();
        GL.Rotate(rotation, 1.0f, 0.0f, 0.0f);
        GL.Rotate(rotation, 0.0f, 1.0f, 0.0f);
        DrawCube();
        GL.PopMatrix();

        // Пирамида
        GL.PushMatrix();
        GL.Translate(-2f, 0f, 0f);
        GL.Rotate(rotation, 0.0f, 1.0f, 0.0f);
        DrawPyramid();
        GL.PopMatrix();

        // Цилиндр
        GL.PushMatrix();
        GL.Translate(2f, 0f, 0f);
        GL.Rotate(rotation, 0.0f, 1.0f, 0.0f);
        DrawCylinder();
        GL.PopMatrix();

        // Источники света
        DrawLightSource(-3f, 3f, 3f); // Источник света 1
        DrawLightSource(3f, 3f, 3f);  // Источник света 2
        DrawLightSource(0f, -3f, 3f); // Источник света 3

        // Обновление экрана
        SwapBuffers();

        rotation += 0.5f;
    }

    private void DrawCube()
    {
        GL.Begin(PrimitiveType.Quads);

        // Передняя грань
        GL.Color3(1f, 0f, 0f); // Красный
        GL.Vertex3(-1f, -1f, 1f);
        GL.Vertex3(1f, -1f, 1f);
        GL.Vertex3(1f, 1f, 1f);
        GL.Vertex3(-1f, 1f, 1f);

        // Задняя грань
        GL.Color3(0f, 1f, 0f); // Зеленый
        GL.Vertex3(-1f, -1f, -1f);
        GL.Vertex3(-1f, 1f, -1f);
        GL.Vertex3(1f, 1f, -1f);
        GL.Vertex3(1f, -1f, -1f);

        // Левые и правые грани
        GL.Color3(0f, 0f, 1f); // Синий
        GL.Vertex3(-1f, -1f, 1f);
        GL.Vertex3(-1f, -1f, -1f);
        GL.Vertex3(-1f, 1f, -1f);
        GL.Vertex3(-1f, 1f, 1f);

        GL.Vertex3(1f, -1f, 1f);
        GL.Vertex3(1f, 1f, 1f);
        GL.Vertex3(1f, 1f, -1f);
        GL.Vertex3(1f, -1f, -1f);

        // Верхняя и нижняя грань
        GL.Color3(1f, 1f, 0f); // Желтый
        GL.Vertex3(-1f, 1f, 1f);
        GL.Vertex3(1f, 1f, 1f);
        GL.Vertex3(1f, 1f, -1f);
        GL.Vertex3(-1f, 1f, -1f);

        GL.Vertex3(-1f, -1f, 1f);
        GL.Vertex3(-1f, -1f, -1f);
        GL.Vertex3(1f, -1f, -1f);
        GL.Vertex3(1f, -1f, 1f);

        GL.End();
    }

    private void DrawPyramid()
    {
        GL.Begin(PrimitiveType.Triangles);

        GL.Color3(1f, 0f, 0f); // Красный
        GL.Vertex3(0f, 1f, 0f);
        GL.Vertex3(-1f, -1f, 1f);
        GL.Vertex3(1f, -1f, 1f);

        GL.Color3(0f, 1f, 0f); // Зеленый
        GL.Vertex3(0f, 1f, 0f);
        GL.Vertex3(1f, -1f, 1f);
        GL.Vertex3(1f, -1f, -1f);

        GL.Color3(0f, 0f, 1f); // Синий
        GL.Vertex3(0f, 1f, 0f);
        GL.Vertex3(1f, -1f, -1f);
        GL.Vertex3(-1f, -1f, -1f);

        GL.Color3(1f, 1f, 0f); // Желтый
        GL.Vertex3(0f, 1f, 0f);
        GL.Vertex3(-1f, -1f, -1f);
        GL.Vertex3(-1f, -1f, 1f);

        GL.End();
    }

    private void DrawCylinder()
    {
        float radius = 1f;
        float height = 2f;
        int slices = 36;

        GL.Begin(PrimitiveType.TriangleFan);

        GL.Color3(0f, 0f, 1f); // Синий
        GL.Vertex3(0f, -height / 2, 0f); // Центральная точка основания
        for (int i = 0; i <= slices; i++)
        {
            float angle = MathHelper.TwoPi * i / slices;
            float x = radius * (float)Math.Cos(angle);
            float z = radius * (float)Math.Sin(angle);
            GL.Vertex3(x, -height / 2, z);
        }

        GL.End();

        GL.Begin(PrimitiveType.TriangleFan);

        GL.Color3(1f, 0f, 0f); // Красный
        GL.Vertex3(0f, height / 2, 0f); // Центральная точка верхней части
        for (int i = 0; i <= slices; i++)
        {
            float angle = MathHelper.TwoPi * i / slices;
            float x = radius * (float)Math.Cos(angle);
            float z = radius * (float)Math.Sin(angle);
            GL.Vertex3(x, height / 2, z);
        }

        GL.End();

        // Боковые грани
        GL.Begin(PrimitiveType.QuadStrip);
        for (int i = 0; i <= slices; i++)
        {
            float angle = MathHelper.TwoPi * i / slices;
            float x = radius * (float)Math.Cos(angle);
            float z = radius * (float)Math.Sin(angle);

            GL.Color3(0f, 1f, 0f); // Зеленый
            GL.Vertex3(x, -height / 2, z);
            GL.Vertex3(x, height / 2, z);
        }

        GL.End();
    }

    private void DrawLightSource(float x, float y, float z)
    {
        GL.PushMatrix();
        GL.Translate(x, y, z);
        GL.Color3(1f, 1f, 1f);
        GL.Begin(PrimitiveType.Spheres);
        GL.Vertex3(0f, 0f, 0f);
        GL.End();
        GL.PopMatrix();
    }

    public static void Main()
    {
        using (var window = new OpenGLWindow())
        {
            window.Run();
        }
    }
}
