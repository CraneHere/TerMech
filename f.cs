using System;
using System.Collections.Generic;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace OpenTK4Scene
{
    class Program : GameWindow
    {
        private int _vaoCube, _vaoPyramid, _vaoCylinder;
        private int _vboCube, _vboPyramid, _vboCylinder;
        private Shader _shader;

        private Matrix4 _view;
        private Matrix4 _projection;
        private Vector3 _cameraPosition = new Vector3(0, 0, 10);

        private Vector3 _cubePosition = new Vector3(-2, 0, 0);
        private Vector3 _pyramidPosition = new Vector3(0, 0, 0);
        private Vector3 _cylinderPosition = new Vector3(2, 0, 0);

        public Program() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            Size = new Vector2i(800, 600);
            Title = "3D Graphics Lab";
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            _shader = new Shader();

            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100f);
            _view = Matrix4.LookAt(_cameraPosition, Vector3.Zero, Vector3.UnitY);

            InitializeObjects();
        }

        private void InitializeObjects()
        {
            float[] cubeVertices = {
                // Positions
                -0.5f, -0.5f, -0.5f,
                 0.5f, -0.5f, -0.5f,
                 0.5f,  0.5f, -0.5f,
                -0.5f,  0.5f, -0.5f,
                -0.5f, -0.5f,  0.5f,
                 0.5f, -0.5f,  0.5f,
                 0.5f,  0.5f,  0.5f,
                -0.5f,  0.5f,  0.5f
            };

            uint[] cubeIndices = {
                0, 1, 2, 2, 3, 0,
                4, 5, 6, 6, 7, 4,
                0, 1, 5, 5, 4, 0,
                2, 3, 7, 7, 6, 2,
                0, 3, 7, 7, 4, 0,
                1, 2, 6, 6, 5, 1
            };

            float[] pyramidVertices = {
                0.0f,  0.5f,  0.0f,
               -0.5f, -0.5f,  0.5f,
                0.5f, -0.5f,  0.5f,
                0.5f, -0.5f, -0.5f,
               -0.5f, -0.5f, -0.5f
            };

            uint[] pyramidIndices = {
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 1,
                1, 2, 3,
                3, 4, 1
            };

            CreateVAO(cubeVertices, cubeIndices, out _vaoCube, out _vboCube);
            CreateVAO(pyramidVertices, pyramidIndices, out _vaoPyramid, out _vboPyramid);
        }

        private void CreateVAO(float[] vertices, uint[] indices, out int vao, out int vbo)
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            int ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            GL.DeleteBuffer(_vboCube);
            GL.DeleteBuffer(_vboPyramid);
            GL.DeleteVertexArray(_vaoCube);
            GL.DeleteVertexArray(_vaoPyramid);
            _shader.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();
            _shader.SetMatrix4("view", _view);
            _shader.SetMatrix4("projection", _projection);

            RenderObject(_vaoCube, _cubePosition);
            RenderObject(_vaoPyramid, _pyramidPosition);

            SwapBuffers();
        }

        private void RenderObject(int vao, Vector3 position)
        {
            Matrix4 model = Matrix4.CreateTranslation(position);
            _shader.SetMatrix4("model", model);

            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);
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
