using System;
using OpenTK;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CompGraph
{
    public class Game : GameWindow
    {
        private Shader shaderProgram;
        private int cubeVao;
        private int pyramidVao;
        private int cylinderVao;

        public Game(int width = 1280, int height = 768, string title = "3D Scene")
            : base(
                  GameWindowSettings.Default,
                  new NativeWindowSettings()
                  {
                      Title = title,
                      Size = new Vector2i(width, height),
                      WindowBorder = WindowBorder.Fixed,
                      StartVisible = true,
                      StartFocused = true,
                      API = ContextAPI.OpenGL,
                      Profile = ContextProfile.Core,
                      APIVersion = new Version(3, 3)
                  })
        {
            this.CenterWindow();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1f);

            string vertexShaderCode =
                @"
                #version 330 core

                layout (location = 0) in vec3 aPosition;
                layout (location = 1) in vec3 aColor;

                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;

                out vec3 vColor;

                void main()
                {
                    gl_Position = projection * view * model * vec4(aPosition, 1.0);
                    vColor = aColor;
                }
                ";

            string fragmentShaderCode =
                @"
                #version 330 core

                in vec3 vColor;
                out vec4 pixelColor;

                void main()
                {
                    pixelColor = vec4(vColor, 1.0);
                }
                ";

            shaderProgram = new Shader(vertexShaderCode, fragmentShaderCode);

            cubeVao = CreateCube();
            pyramidVao = CreatePyramid();
            cylinderVao = CreateCylinder();

            GL.Enable(EnableCap.DepthTest);

            base.OnLoad();
        }

        private int CreateCube()
        {
            float[] vertices = {
                // Positions         // Colors
                -0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 0.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 0.0f,
                 0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 1.0f,
                -0.5f,  0.5f, -0.5f,  1.0f, 1.0f, 0.0f,

                -0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, 1.0f, 1.0f,
                 0.5f,  0.5f,  0.5f,  1.0f, 1.0f, 1.0f,
                -0.5f,  0.5f,  0.5f,  0.5f, 0.5f, 0.5f
            };

            uint[] indices = {
                0, 1, 2, 2, 3, 0,
                4, 5, 6, 6, 7, 4,
                0, 1, 5, 5, 4, 0,
                2, 3, 7, 7, 6, 2,
                0, 3, 7, 7, 4, 0,
                1, 2, 6, 6, 5, 1
            };

            return CreateVao(vertices, indices);
        }

        private int CreatePyramid()
        {
            float[] vertices = {
                // Positions         // Colors
                 0.0f,  0.5f,  0.0f,  1.0f, 0.0f, 0.0f,
                -0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 0.0f,
                 0.5f, -0.5f, -0.5f,  0.0f, 0.0f, 1.0f,
                 0.5f, -0.5f,  0.5f,  1.0f, 1.0f, 0.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 1.0f, 1.0f
            };

            uint[] indices = {
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 1,
                1, 2, 3, 3, 4, 1
            };

            return CreateVao(vertices, indices);
        }

        private int CreateCylinder()
        {
            const int segments = 36;
            const float radius = 0.5f;
            const float height = 1.0f;

            float[] vertices = new float[segments * 12];
            uint[] indices = new uint[segments * 6];

            for (int i = 0; i < segments; i++)
            {
                float angle = MathHelper.TwoPi / segments * i;
                float nextAngle = MathHelper.TwoPi / segments * (i + 1);

                int v = i * 12;
                vertices[v + 0] = MathF.Cos(angle) * radius; // Bottom circle
                vertices[v + 1] = -height / 2;
                vertices[v + 2] = MathF.Sin(angle) * radius;
                vertices[v + 3] = 1.0f; // Color
                vertices[v + 4] = 0.0f;
                vertices[v + 5] = 0.0f;

                vertices[v + 6] = MathF.Cos(angle) * radius; // Top circle
                vertices[v + 7] = height / 2;
                vertices[v + 8] = MathF.Sin(angle) * radius;
                vertices[v + 9] = 0.0f;
                vertices[v + 10] = 1.0f;
                vertices[v + 11] = 0.0f;

                int idx = i * 6;
                indices[idx + 0] = (uint)(i * 2);
                indices[idx + 1] = (uint)(i * 2 + 1);
                indices[idx + 2] = (uint)((i * 2 + 2) % (segments * 2));
                indices[idx + 3] = (uint)((i * 2 + 2) % (segments * 2));
                indices[idx + 4] = (uint)(i * 2 + 1);
                indices[idx + 5] = (uint)((i * 2 + 3) % (segments * 2));
            }

            return CreateVao(vertices, indices);
        }

        private int CreateVao(float[] vertices, uint[] indices)
        {
            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            int ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            return vao;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 view = Matrix4.LookAt(new Vector3(2, 2, 2), Vector3.Zero, Vector3.UnitY);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Size.X / (float)Size.Y, 0.1f, 100f);

            shaderProgram.Use();

            shaderProgram
