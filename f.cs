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

        private int _vaoCube, _vaoPyramid, _vaoCylinder;
        private int _vboCube, _vboPyramid, _vboCylinder;
        private Shader _shader;

        public Program() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            Size = new Vector2i(800, 600);
            Title = "3D Scene with Cube, Pyramid and Cylinder";
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            _shader = new Shader();

            // 1. Cube
            float[] cubeVertices = {
                // Positions          // Colors
                -1.0f, -1.0f, -1.0f,  1.0f, 0.0f, 0.0f,  // Red
                 1.0f, -1.0f, -1.0f,  0.0f, 1.0f, 0.0f,  // Green
                 1.0f,  1.0f, -1.0f,  0.0f, 0.0f, 1.0f,  // Blue
                -1.0f,  1.0f, -1.0f,  1.0f, 1.0f, 0.0f,  // Yellow
                -1.0f, -1.0f,  1.0f,  1.0f, 0.0f, 1.0f,  // Magenta
                 1.0f, -1.0f,  1.0f,  0.0f, 1.0f, 1.0f,  // Cyan
                 1.0f,  1.0f,  1.0f,  1.0f, 0.0f, 1.0f,  // Purple
                -1.0f,  1.0f,  1.0f,  1.0f, 1.0f, 1.0f   // White
            };

            _vaoCube = GL.GenVertexArray();
            _vboCube = GL.GenBuffer();
            GL.BindVertexArray(_vaoCube);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCube);
            GL.BufferData(BufferTarget.ArrayBuffer, cubeVertices.Length * sizeof(float), cubeVertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // 2. Pyramid (with simple vertices)
            float[] pyramidVertices = {
                // Base
                -1.0f, -1.0f, -1.0f,  1.0f, 0.0f, 0.0f,
                 1.0f, -1.0f, -1.0f,  0.0f, 1.0f, 0.0f,
                 1.0f, -1.0f,  1.0f,  0.0f, 0.0f, 1.0f,
                -1.0f, -1.0f,  1.0f,  1.0f, 1.0f, 0.0f,
                // Sides
                0.0f,  1.0f,  0.0f,  1.0f, 0.0f, 0.0f, // Tip
                -1.0f, -1.0f, -1.0f,  1.0f, 1.0f, 0.0f,
                1.0f, -1.0f, -1.0f,  0.0f, 1.0f, 0.0f,
                1.0f, -1.0f,  1.0f,  0.0f, 0.0f, 1.0f,
                -1.0f, -1.0f,  1.0f,  1.0f, 0.0f, 1.0f
            };

            _vaoPyramid = GL.GenVertexArray();
            _vboPyramid = GL.GenBuffer();
            GL.BindVertexArray(_vaoPyramid);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboPyramid);
            GL.BufferData(BufferTarget.ArrayBuffer, pyramidVertices.Length * sizeof(float), pyramidVertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // 3. Cylinder (using a simple approximation)
            int numSides = 30;
            float[] cylinderVertices = new float[numSides * 6];
            for (int i = 0; i < numSides; i++)
            {
                float angle = i * 2 * MathF.PI / numSides;
                float x = MathF.Cos(angle);
                float z = MathF.Sin(angle);

                // Bottom circle
                cylinderVertices[i * 6] = x;
                cylinderVertices[i * 6 + 1] = -1.0f;
                cylinderVertices[i * 6 + 2] = z;
                cylinderVertices[i * 6 + 3] = 0.0f;
                cylinderVertices[i * 6 + 4] = 1.0f;
                cylinderVertices[i * 6 + 5] = 0.0f;

                // Top circle
                cylinderVertices[(i + numSides) * 6] = x;
                cylinderVertices[(i + numSides) * 6 + 1] = 1.0f;
                cylinderVertices[(i + numSides) * 6 + 2] = z;
                cylinderVertices[(i + numSides) * 6 + 3] = 0.0f;
                cylinderVertices[(i + numSides) * 6 + 4] = 0.0f;
                cylinderVertices[(i + numSides) * 6 + 5] = 1.0f;
            }

            _vaoCylinder = GL.GenVertexArray();
            _vboCylinder = GL.GenBuffer();
            GL.BindVertexArray(_vaoCylinder);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCylinder);
            GL.BufferData(BufferTarget.ArrayBuffer, cylinderVertices.Length * sizeof(float), cylinderVertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteBuffer(_vboCube);
            GL.DeleteVertexArray(_vaoCube);
            GL.DeleteBuffer(_vboPyramid);
            GL.DeleteVertexArray(_vaoPyramid);
            GL.DeleteBuffer(_vboCylinder);
            GL.DeleteVertexArray(_vaoCylinder);
            _shader.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyPressed(Keys.Escape))
                Close();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            // Projection Matrix (Perspective)
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), Width / (float)Height, 0.1f, 100f);
            _shader.SetMatrix4("projection", projection);

            // View Matrix (Camera)
            Matrix4 view = Matrix4.LookAt(new Vector3(_radius * MathF.Cos(_angle), 2.0f, _radius * MathF.Sin(_angle)), Vector3.Zero, Vector3.UnitY);
            _shader.SetMatrix4("view", view);

            // Model Matrix for Cube
            Matrix4 model = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_angle));
            _shader.SetMatrix4("model", model);
            GL.BindVertexArray(_vaoCube);
            GL.DrawArrays(PrimitiveType.Quads, 0, 24);

            // Model Matrix for Pyramid
            model = Matrix4.CreateTranslation(2.0f, 0.0f, 0.0f);
            _shader.SetMatrix4("model", model);
            GL.BindVertexArray(_vaoPyramid);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 18);

            // Model Matrix for Cylinder
            model = Matrix4.CreateTranslation(-2.0f, 0.0f, 0.0f);
            _shader.SetMatrix4("model", model);
            GL.BindVertexArray(_vaoCylinder);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 60);

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
