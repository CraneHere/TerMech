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
        private int _vao;
        private int _vbo;
        private Shader _shader;

        private Vector3 _position = Vector3.Zero;
        private float _scale = 1.0f;
        private float _rotation = 0.0f;

        private readonly float[] _lineVertices =
        {
            -0.5f, 0.0f, 0.0f, // Start point
             0.5f, 0.0f, 0.0f  // End point
        };

        public Program() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            Size = new Vector2i(800, 600);
            Title = "Interactive Line";
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Compile and use shader
            _shader = new Shader();
            _shader.Use();

            // Generate VAO and VBO
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _lineVertices.Length * sizeof(float), _lineVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
            _shader.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
                Close();

            // Movement controls
            if (input.IsKeyDown(Keys.W))
                _position.Y += 1.0f * (float)args.Time;
            if (input.IsKeyDown(Keys.S))
                _position.Y -= 1.0f * (float)args.Time;
            if (input.IsKeyDown(Keys.A))
                _position.X -= 1.0f * (float)args.Time;
            if (input.IsKeyDown(Keys.D))
                _position.X += 1.0f * (float)args.Time;

            // Scaling controls
            if (input.IsKeyDown(Keys.Up))
                _scale += 1.0f * (float)args.Time;
            if (input.IsKeyDown(Keys.Down))
                _scale -= 1.0f * (float)args.Time;

            // Rotation controls
            if (input.IsKeyDown(Keys.Left))
                _rotation += 1.0f * (float)args.Time;
            if (input.IsKeyDown(Keys.Right))
                _rotation -= 1.0f * (float)args.Time;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();

            // Create transformation matrix
            var transform = Matrix4.CreateScale(_scale) *
                            Matrix4.CreateRotationZ(_rotation) *
                            Matrix4.CreateTranslation(_position);

            _shader.SetMatrix4("uTransform", transform);

            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
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
