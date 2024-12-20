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

        private int _vao;
        private int _vbo;
        private Shader _shader;

        // Источник света
        private Vector3 _dirLightDirection = new Vector3(-0.2f, -1.0f, -0.3f);
        private Vector3 _pointLightPosition = new Vector3(2.0f, 2.0f, 2.0f);

        public Program() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            Size = new Vector2i(800, 600);
            Title = "Lighting with Directional and Point Lights";
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            // Инициализация объектов и шейдера
            InitializeObjects();
            _shader = new Shader();
        }

        private void InitializeObjects()
        {
            // Задание геометрии объектов (например, куб)
            float[] vertices = new float[]
            {
                // Позиции       // Нормали          // Цвета
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 1.0f,  1.0f, 0.0f, 0.0f,
                 0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 1.0f,  1.0f, 0.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 1.0f,  1.0f, 0.0f, 0.0f,
                 0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 1.0f,  1.0f, 0.0f, 0.0f,
                -0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 1.0f,  1.0f, 0.0f, 0.0f,
                -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 1.0f,  1.0f, 0.0f, 0.0f,

                // Продолжайте для других граней
            };

            // Создаем VAO, VBO и связываем их с шейдером
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);
            GL.BindVertexArray(0);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);
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
            _shader.SetVector3("dirLight.direction", _dirLightDirection);
            _shader.SetVector3("pointLight.position", _pointLightPosition);

            Matrix4 model = Matrix4.CreateRotationY(_angle);
            Matrix4 view = Matrix4.CreateLookAt(new Vector3(0.0f, 0.0f, _radius), Vector3.Zero, Vector3.UnitY);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), 800f / 600f, 0.1f, 100f);

            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);

            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36); // Рендерим куб
            GL.BindVertexArray(0);

            _angle += 0.01f;

            Context.SwapBuffers();
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

#version 330 core

layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec3 inNormal;
layout(location = 2) in vec3 inColor;

out vec3 fragPosition;
out vec3 fragNormal;
out vec3 fragColor;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    fragPosition = vec3(model * vec4(inPosition, 1.0));
    fragNormal = mat3(transpose(inverse(model))) * inNormal; // Для правильного расчета нормалей
    fragColor = inColor;
    
    gl_Position = projection * view * vec4(fragPosition, 1.0);
}

#version 330 core

in vec3 fragPosition;
in vec3 fragNormal;
in vec3 fragColor;

out vec4 fragColorOut;

uniform vec3 dirLight.direction;
uniform vec3 pointLight.position;

void main()
{
    // Направленный источник света
    float diff = max(dot(normalize(fragNormal), normalize(-dirLight.direction)), 0.0);

    // Точечный источник света
    float dist = length(pointLight.position - fragPosition);
    float attenuation = 1.0 / (dist * dist);  // Простая модель затухания
    float diffPoint = max(dot(normalize(fragNormal), normalize(pointLight.position - fragPosition)), 0.0);
    diffPoint *= attenuation;

    vec3 ambient = 0.1 * fragColor;
    vec3 diffuse = diff * fragColor;
    vec3 diffusePoint = diffPoint * fragColor;

    fragColorOut = vec4(ambient + diffuse + diffusePoint, 1.0);
}
