using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenTK4Scene
{
    class Program : GameWindow
    {
        private float _angle = 0f;
        private float _radius = 5f;
        
        private int _vao, _vbo, _ebo;
        private Shader _shader;

        // Преобразования
        private Matrix4 _view;
        private Matrix4 _projection;

        public Program() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            Size = new Vector2i(800, 600);
            Title = "Chill";
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f); // Фон

            // Включаем глубинный тест
            GL.Enable(EnableCap.DepthTest);

            _shader = new Shader("shader.vert", "shader.frag");

            // Задаем объекты для отрисовки
            InitializeObjects();

            // Настроим перспективную проекцию
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100f);
            _view = Matrix4.LookAt(new Vector3(0f, 0f, _radius), Vector3.Zero, Vector3.UnitY);
        }

        private void InitializeObjects()
        {
            // Здесь создаются объекты: куб, пирамида и цилиндр
            // Для примера мы создадим простую геометрию для куба.

            float[] vertices = {
                // Куб
                -0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f,  // 0
                 0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f,  // 1
                 0.5f,  0.5f, -0.5f, 1.0f, 0.0f, 0.0f,  // 2
                -0.5f,  0.5f, -0.5f, 1.0f, 0.0f, 0.0f,  // 3
                -0.5f, -0.5f,  0.5f, 0.0f, 1.0f, 0.0f,  // 4
                 0.5f, -0.5f,  0.5f, 0.0f, 1.0f, 0.0f,  // 5
                 0.5f,  0.5f,  0.5f, 0.0f, 1.0f, 0.0f,  // 6
                -0.5f,  0.5f,  0.5f, 0.0f, 1.0f, 0.0f   // 7
            };

            uint[] indices = {
                0, 1, 2, 2, 3, 0,  // передняя грань
                4, 5, 6, 6, 7, 4,  // задняя грань
                0, 1, 5, 5, 4, 0,  // нижняя грань
                2, 3, 7, 7, 6, 2,  // верхняя грань
                0, 3, 7, 7, 4, 0,  // левая грань
                1, 2, 6, 6, 5, 1   // правая грань
            };

            // Создаем VAO, VBO и EBO для куба
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            GL.DeleteVertexArray(_vao);
            _shader.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            // Модельное преобразование
            Matrix4 model = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_angle));
            _angle += 0.5f;

            // Обновляем матрицу вида
            _shader.SetMatrix4("view", _view);
            _shader.SetMatrix4("projection", _projection);
            _shader.SetMatrix4("model", model);

            // Рисуем куб
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);

            SwapBuffers();
        }

        [STAThread]
        static void Main(string[] args)
        {
            using (var program = new Program())
            {
                program.Run();
            }
        }
    }

    public class Shader : IDisposable
    {
        public int Handle { get; private set; }

        public Shader(string vertexPath = "shader.vert", string fragmentPath = "shader.frag")
        {
            string vertexSource = System.IO.File.ReadAllText(vertexPath);
            string fragmentSource = System.IO.File.ReadAllText(fragmentPath);

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            GL.CompileShader(vertexShader);
            CheckShaderCompileStatus(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(fragmentShader);
            CheckShaderCompileStatus(fragmentShader);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);
            CheckProgramLinkStatus(Handle);

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public void SetMatrix4(string name, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        private static void CheckShaderCompileStatus(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
            if (status != (int)All.True)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Shader compilation failed: {infoLog}");
            }
        }

        private static void CheckProgramLinkStatus(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int status);
            if (status != (int)All.True)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                throw new Exception($"Program linking failed: {infoLog}");
            }
        }

        public void Dispose()
        {
            GL.DeleteProgram(Handle);
        }
    }
}
