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
        private float _speed = 0.5f; // Camera speed

        private int _vao;
        private int _vbo;
        private Shader _shader;

        private float[] _vertices = new float[]
        {
            // Positions           // Normals
            0.0f,  1.0f,  0.0f,    0.0f,  1.0f,  0.0f, // Top vertex
            -1.0f, -1.0f,  1.0f,    0.0f, -1.0f,  1.0f, // Bottom-left front
            1.0f, -1.0f,  1.0f,     0.0f, -1.0f,  1.0f, // Bottom-right front
            1.0f, -1.0f, -1.0f,     0.0f, -1.0f, -1.0f, // Bottom-right back
            -1.0f, -1.0f, -1.0f,    0.0f, -1.0f, -1.0f, // Bottom-left back
        };

        private int[] _indices = new int[]
        {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 1,
            1, 2, 3,
            1, 3, 4
        };

        public Program() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            Size = new Vector2i(800, 600);
            Title = "Chill";
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            _shader = new Shader();

            // Create buffers
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            int ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            // Bind VBO and EBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(int), _indices, BufferUsageHint.StaticDraw);

            // Position attribute
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Normal attribute
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Up)) _speed += 0.1f;
            if (KeyboardState.IsKeyDown(Keys.Down)) _speed -= 0.1f;

            // Clamp the shininess value
            _speed = MathHelper.Clamp(_speed, 0.1f, 128.0f);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            // Set up the camera
            Matrix4 model = Matrix4.Identity;
            Matrix4 view = Matrix4.LookAt(new Vector3(3.0f, 3.0f, 3.0f), Vector3.Zero, Vector3.UnitY);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), (float)ClientSize.X / ClientSize.Y, 0.1f, 100f);
            _shader.SetMatrix4("model", model);
            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);

            // Set up light and camera positions
            Vector3 lightPos = new Vector3(1.0f, 1.0f, 1.0f);
            Vector3 viewPos = new Vector3(3.0f, 3.0f, 3.0f);
            _shader.SetVector3("lightPos", lightPos);
            _shader.SetVector3("viewPos", viewPos);

            // Set up material properties
            _shader.SetFloat("shininess", 32.0f); // Specular intensity

            _shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f)); // White light
            _shader.SetVector3("objectColor", new Vector3(0.7f, 0.3f, 0.3f)); // Red object

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

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

    public class Shader : IDisposable
    {
        public int Handle { get; private set; }

        public Shader(string vertexPath = "C:\\Users\\danil\\source\\repos\\CompGraph\\CompGraph\\Shaders\\shader.vert", 
                      string fragmentPath = "C:\\Users\\danil\\source\\repos\\CompGraph\\CompGraph\\Shaders\\shader.frag")
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

        public void SetFloat(string name, float value)
        {
            int location = GL.GetUniformLocation(Handle, name);
            if (location != -1)
            {
                GL.Uniform1(location, value);
            }
        }
        public void SetVector3(string name, Vector3 vector)
        {
            int location = GL.GetUniformLocation(Handle, name);
            if (location != -1)
            {
                GL.Uniform3(location, vector);
            }
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
