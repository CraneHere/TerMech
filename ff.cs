// vertexShader.vert
#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aColor;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 ourColor;

void main()
{
    gl_Position = projection * view * model * vec4(aPosition, 1.0f);
    ourColor = aColor;
}

// fragmentShader.frag
#version 330 core
out vec4 FragColor;

in vec3 ourColor;

void main()
{
    FragColor = vec4(ourColor, 1.0f);
}

---------------------------------------------------------------------------------

using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic;

namespace OpenTK4Scene
{
    class Program : GameWindow
    {
        private Shader _shader;
        private int _vaoPlane, _vboPlane;
        private int _vaoSphere, _vboSphere;

        private List<Vector3> _sphereVertices;
        
        public Program() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            Size = new Vector2i(800, 600);
            Title = "3D Scene with Spheres and Plane";
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            _shader = new Shader();
            CreatePlane();
            CreateSphere();
            GL.Enable(EnableCap.DepthTest); // Включаем тест глубины для 3D
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteVertexArray(_vaoPlane);
            GL.DeleteBuffer(_vboPlane);
            GL.DeleteVertexArray(_vaoSphere);
            GL.DeleteBuffer(_vboSphere);
            _shader.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            // Камера (view) и проекция
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), (float)Width / (float)Height, 0.1f, 100f);
            Matrix4 view = Matrix4.CreateLookAt(new Vector3(0f, 0f, 10f), Vector3.Zero, Vector3.UnitY); // Камера в позиции (0, 0, 10) смотрит в центр

            _shader.SetMatrix4("projection", projection);
            _shader.SetMatrix4("view", view);

            // Рендерим плоскость
            RenderPlane();

            // Рендерим три сферы
            RenderSphere(new Vector3(0f, 0f, 0f));  // Первая сфера в центре
            RenderSphere(new Vector3(2f, 0f, 0f));  // Вторая сфера по оси X
            RenderSphere(new Vector3(-2f, 0f, 0f)); // Третья сфера по оси X

            SwapBuffers();
        }

        private void CreatePlane()
        {
            // Плоскость
            float[] planeVertices = {
                // Positions          // Colors
                -5.0f, 0.0f, -5.0f,   1.0f, 0.0f, 0.0f,
                 5.0f, 0.0f, -5.0f,   0.0f, 1.0f, 0.0f,
                 5.0f, 0.0f,  5.0f,   0.0f, 0.0f, 1.0f,
                -5.0f, 0.0f,  5.0f,   1.0f, 1.0f, 0.0f
            };

            _vaoPlane = GL.GenVertexArray();
            _vboPlane = GL.GenBuffer();
            GL.BindVertexArray(_vaoPlane);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboPlane);
            GL.BufferData(BufferTarget.ArrayBuffer, planeVertices.Length * sizeof(float), planeVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        private void CreateSphere()
        {
            // Генерация вершины сферы
            _sphereVertices = new List<Vector3>();
            int sectors = 36;
            int stacks = 18;

            for (int i = 0; i <= stacks; i++)
            {
                double stackStep = Math.PI / stacks;
                double stackAngle = Math.PI / 2 - i * stackStep; // От 90° до -90°

                for (int j = 0; j <= sectors; j++)
                {
                    double sectorStep = 2 * Math.PI / sectors;
                    double sectorAngle = j * sectorStep;

                    float x = (float)(Math.Cos(stackAngle) * Math.Cos(sectorAngle));
                    float y = (float)(Math.Cos(stackAngle) * Math.Sin(sectorAngle));
                    float z = (float)(Math.Sin(stackAngle));

                    _sphereVertices.Add(new Vector3(x, y, z));
                }
            }

            _vaoSphere = GL.GenVertexArray();
            _vboSphere = GL.GenBuffer();
            GL.BindVertexArray(_vaoSphere);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboSphere);
            GL.BufferData(BufferTarget.ArrayBuffer, _sphereVertices.Count * Vector3.SizeInBytes, _sphereVertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        private void RenderPlane()
        {
            _shader.Use();
            GL.BindVertexArray(_vaoPlane);

            Matrix4 model = Matrix4.Identity; // Плоскость на оси XZ
            _shader.SetMatrix4("model", model);

            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
            GL.BindVertexArray(0);
        }

        private void RenderSphere(Vector3 position)
        {
            _shader.Use();
            GL.BindVertexArray(_vaoSphere);

            Matrix4 model = Matrix4.CreateTranslation(position); // Трансляция на заданную позицию
            _shader.SetMatrix4("model", model);

            GL.DrawArrays(PrimitiveType.TriangleFan, 0, _sphereVertices.Count); // Рендерим сферы
            GL.BindVertexArray(0);
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
