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
        private int _vaoCube, _vaoSphere, _vaoPyramid;
        private Shader _shader;
        private Matrix4 _view, _projection;

        private Vector3 _lightDirection = new Vector3(1.0f, -1.0f, 0.0f);
        private Vector3 _pointLightPosition = new Vector3(2.0f, 2.0f, 2.0f);

        public Program() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            Size = new Vector2i(800, 600);
            Title = "Lighting Scene";
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            // Инициализация шейдеров
            _shader = new Shader("path/to/shader.vert", "path/to/shader.frag");

            // Создание VAO для каждого объекта
            _vaoCube = CreateCubeVAO();
            _vaoSphere = CreateSphereVAO();
            _vaoPyramid = CreatePyramidVAO();

            // Матрицы камеры
            _view = Matrix4.LookAt(new Vector3(0, 3, 10), Vector3.Zero, Vector3.UnitY);
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100f);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteVertexArray(_vaoCube);
            GL.DeleteVertexArray(_vaoSphere);
            GL.DeleteVertexArray(_vaoPyramid);
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

            // Установка матриц
            _shader.SetMatrix4("view", _view);
            _shader.SetMatrix4("projection", _projection);

            // Установка освещения
            _shader.SetVector3("lightDirection", _lightDirection);
            _shader.SetVector3("pointLightPosition", _pointLightPosition);

            // Рисуем объекты
            DrawObject(_vaoCube, Matrix4.CreateTranslation(-2, 0, 0));
            DrawObject(_vaoSphere, Matrix4.CreateTranslation(0, 0, 0));
            DrawObject(_vaoPyramid, Matrix4.CreateTranslation(2, 0, 0));

            SwapBuffers();
        }

        private void DrawObject(int vao, Matrix4 model)
        {
            _shader.SetMatrix4("model", model);
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        private int CreateCubeVAO()
        {
            // Упрощённый код для создания куба
            float[] vertices = { ... };

            int vao = GL.GenVertexArray();
            int vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            return vao;
        }

        private int CreateSphereVAO()
        {
            // Реализация создания VAO сферы
            return 0;
        }

        private int CreatePyramidVAO()
        {
            // Реализация создания VAO пирамиды
            return 0;
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
}
