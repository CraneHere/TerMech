using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Numerics;

public class OpenGLWindow : GameWindow
{
    private float rotation = 0f;
    private int shaderProgram;
    private int vaoCube, vaoPyramid, vaoCylinder;
    private int vboCube, vboPyramid, vboCylinder;
    private int projectionMatrixLocation, modelViewMatrixLocation;

    public OpenGLWindow() : base(GameWindowSettings.Default, new NativeWindowSettings { Size = new OpenTK.Mathematics.Vector2i(800, 600), Title = "3D Scene with Lighting" })
    {
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        // Инициализация OpenGL
        GL.ClearColor(0.39f, 0.58f, 0.93f, 1.0f);  // Цвет фона (cornflower blue)
        GL.Enable(EnableCap.DepthTest);  // Включаем тест глубины

        // Создание шейдерной программы
        shaderProgram = CreateShaderProgram();
        GL.UseProgram(shaderProgram);

        // Инициализация объектов
        InitializeCube();
        InitializePyramid();
        InitializeCylinder();

        // Матрица проекции
        projectionMatrixLocation = GL.GetUniformLocation(shaderProgram, "projection");
        modelViewMatrixLocation = GL.GetUniformLocation(shaderProgram, "modelView");

        // Устанавливаем матрицу проекции
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45), // угол обзора
            Width / (float)Height,            // соотношение сторон
            0.1f,                            // ближайшая плоскость отсечения
            100f                             // дальняя плоскость отсечения
        );
        GL.UniformMatrix4(projectionMatrixLocation, false, ref projection);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Камера
        Matrix4 modelView = Matrix4.LookAt(new Vector3(0f, 0f, 6f), Vector3.Zero, Vector3.UnitY);
        GL.UniformMatrix4(modelViewMatrixLocation, false, ref modelView);

        // Рисуем куб
        GL.BindVertexArray(vaoCube);
        Matrix4 modelCube = Matrix4.CreateRotationY(rotation);
        GL.UniformMatrix4(modelViewMatrixLocation, false, ref modelCube);
        GL.DrawArrays(PrimitiveType.Quads, 0, 24);

        // Рисуем пирамиду
        GL.BindVertexArray(vaoPyramid);
        Matrix4 modelPyramid = Matrix4.CreateTranslation(-2f, 0f, 0f) * Matrix4.CreateRotationY(rotation);
        GL.UniformMatrix4(modelViewMatrixLocation, false, ref modelPyramid);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 12);

        // Рисуем цилиндр
        GL.BindVertexArray(vaoCylinder);
        Matrix4 modelCylinder = Matrix4.CreateTranslation(2f, 0f, 0f) * Matrix4.CreateRotationY(rotation);
        GL.UniformMatrix4(modelViewMatrixLocation, false, ref modelCylinder);
        GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 144);

        // Источники света
        DrawLightSource(-3f, 3f, 3f);
        DrawLightSource(3f, 3f, 3f);
        DrawLightSource(0f, -3f, 3f);

        // Обновление экрана
        SwapBuffers();

        rotation += 0.5f;
    }

    private void InitializeCube()
    {
        float[] vertices = {
            // Front face
            -1f, -1f, 1f, 1f, 0f, 0f,
            1f, -1f, 1f, 1f, 0f, 0f,
            1f, 1f, 1f, 1f, 0f, 0f,
            -1f, 1f, 1f, 1f, 0f, 0f,

            // Back face
            -1f, -1f, -1f, 0f, 1f, 0f,
            -1f, 1f, -1f, 0f, 1f, 0f,
            1f, 1f, -1f, 0f, 1f, 0f,
            1f, -1f, -1f, 0f, 1f, 0f,

            // Left face
            -1f, -1f, 1f, 0f, 0f, 1f,
            -1f, -1f, -1f, 0f, 0f, 1f,
            -1f, 1f, -1f, 0f, 0f, 1f,
            -1f, 1f, 1f, 0f, 0f, 1f,

            // Right face
            1f, -1f, 1f, 1f, 1f, 0f,
            1f, 1f, 1f, 1f, 1f, 0f,
            1f, 1f, -1f, 1f, 1f, 0f,
            1f, -1f, -1f, 1f, 1f, 0f,

            // Top face
            -1f, 1f, 1f, 1f, 1f, 0f,
            1f, 1f, 1f, 1f, 1f, 0f,
            1f, 1f, -1f, 1f, 1f, 0f,
            -1f, 1f, -1f, 1f, 1f, 0f,

            // Bottom face
            -1f, -1f, 1f, 0f, 1f, 1f,
            -1f, -1f, -1f, 0f, 1f, 1f,
            1f, -1f, -1f, 0f, 1f, 1f,
            1f, -1f, 1f, 0f, 1f, 1f
        };

        // Вершины для куба
        vaoCube = GL.GenVertexArray();
        vboCube = GL.GenBuffer();
        GL.BindVertexArray(vaoCube);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboCube);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        // Связываем атрибуты вершин
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    private void InitializePyramid()
    {
        // Похожие шаги для пирамиды и цилиндра...
    }

    private void InitializeCylinder()
    {
        // Похожие шаги для пирамиды и цилиндра...
    }

    private void DrawLightSource(float x, float y, float z)
    {
        // Здесь можно нарисовать сферу или точку для источника света.
        // Для простоты мы можем использовать квадрат или другой объект.
    }

    private int CreateShaderProgram()
    {
        string vertexShaderSource = @"
            #version 330 core
            layout(location = 0) in vec3 position;
            layout(location = 1) in vec3 color;
            out vec3 fragColor;
            uniform mat4 modelView;
            uniform mat4 projection;
            void main()
            {
                gl_Position = projection * modelView * vec4(position, 1.0);
                fragColor = color;
            }
        ";

        string fragmentShaderSource = @"
            #version 330 core
            in vec3 fragColor;
            out vec4 color;
            void main()
            {
                color = vec4(fragColor, 1.0);
            }
        ";

        int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
        int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);

        int program = GL.CreateProgram();
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        GL.LinkProgram(program);

        GL.DetachShader(program, vertexShader);
        GL.DetachShader(program, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        return program;
    }

    private int CompileShader(ShaderType type, string source)
    {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);
        GL.GetShaderInfoLog(shader, out string infoLog);
        if (!string.IsNullOrEmpty(infoLog))
        {
            Console.WriteLine($"Shader Compile Error: {infoLog}");
        }
        return shader;
    }

    public static void Main()
    {
        using (var window = new OpenGLWindow())
        {
            window.Run();
        }
    }
}
