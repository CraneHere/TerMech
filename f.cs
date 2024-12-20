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
        private int _vao;
        private int _vbo;
        private Shader _shader;
        private List<float> _vertices;

        private Vector3 _directionalLight = new Vector3(1.0f, -1.0f, -0.5f).Normalized();
        private Vector3 _pointLightPosition = new Vector3(2.0f, 1.0f, 2.0f);

        public Program() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            Size = new Vector2i(800, 600);
            Title = "Lighting in OpenTK";
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            _vertices = new List<float>();
            GenerateScene();

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * sizeof(float), _vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            _shader = new Shader("vertex_shader.glsl", "fragment_shader.glsl");
        }

        private void GenerateScene()
        {
            AddCube(new Vector3(-1.0f, 0.0f, -3.0f));
            AddSphere(new Vector3(1.0f, 0.0f, -3.0f));
            AddPyramid(new Vector3(0.0f, 0.0f, -2.0f));
        }

        private void AddCube(Vector3 position)
        {
            float[] cubeVertices = {
                // Positions          // Normals
                -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
                 0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
                 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
                -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f
                // Other faces can be added here
            };

            for (int i = 0; i < cubeVertices.Length / 6; i++)
            {
                Vector3 pos = new Vector3(cubeVertices[i * 6], cubeVertices[i * 6 + 1], cubeVertices[i * 6 + 2]);
                Vector3 normal = new Vector3(cubeVertices[i * 6 + 3], cubeVertices[i * 6 + 4], cubeVertices[i * 6 + 5]);
                pos += position;

                _vertices.Add(pos.X);
                _vertices.Add(pos.Y);
                _vertices.Add(pos.Z);
                _vertices.Add(normal.X);
                _vertices.Add(normal.Y);
                _vertices.Add(normal.Z);
            }
        }

        private void AddSphere(Vector3 position)
        {
            // Sphere generation logic
        }

        private void AddPyramid(Vector3 position)
        {
            // Pyramid generation logic
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (IsKeyPressed(Keys.Escape))
                Close();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            _shader.SetVector3("directionalLight", _directionalLight);
            _shader.SetVector3("pointLightPosition", _pointLightPosition);

            Matrix4 model = Matrix4.Identity;
            Matrix4 view = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 5.0f), Vector3.Zero, Vector3.UnitY);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Size.X / (float)Size.Y, 0.1f, 100.0f);

            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);

            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count / 6);

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
            _shader.Dispose();

            base.OnUnload();
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
