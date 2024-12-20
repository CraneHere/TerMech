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
        private int _vaoCube, _vaoPyramid, _vaoCylinder;
        private int _vboCube, _vboPyramid, _vboCylinder;
        private int _eboCube, _eboPyramid, _eboCylinder;

        private Shader _shader;
        private Matrix4 _view;
        private Matrix4 _projection;
        private Matrix4 _modelCube, _modelPyramid, _modelCylinder;

        public Program() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            Size = new Vector2i(800, 600);
            Title = "3D Scene with Lighting";
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f); // Set background color

            GL.Enable(EnableCap.DepthTest); // Enable depth testing

            _shader = new Shader("shader.vert", "shader.frag");

            // Set up objects
            InitializeCube();
            InitializePyramid();
            InitializeCylinder();

            // Set up camera and perspective projection
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100f);
            _view = Matrix4.LookAt(new Vector3(3f, 3f, 3f), Vector3.Zero, Vector3.UnitY);

            _modelCube = Matrix4.CreateTranslation(-2f, 0f, 0f);
            _modelPyramid = Matrix4.CreateTranslation(0f, 0f, 0f);
            _modelCylinder = Matrix4.CreateTranslation(2f, 0f, 0f);
        }

        private void InitializeCube()
        {
            float[] vertices = {
                // Positions          // Colors
                -0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, // 0
                 0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, // 1
                 0.5f,  0.5f, -0.5f, 1.0f, 0.0f, 0.0f, // 2
                -0.5f,  0.5f, -0.5f, 1.0f, 0.0f, 0.0f, // 3
                -0.5f, -0.5f,  0.5f, 0.0f, 1.0f, 0.0f, // 4
                 0.5f, -0.5f,  0.5f, 0.0f, 1.0f, 0.0f, // 5
                 0.5f,  0.5f,  0.5f, 0.0f, 1.0f, 0.0f, // 6
                -0.5f,  0.5f,  0.5f, 0.0f, 1.0f, 0.0f  // 7
            };

            uint[] indices = {
                0, 1, 2, 2, 3, 0,  // front face
                4, 5, 6, 6, 7, 4,  // back face
                0, 1, 5, 5, 4, 0,  // bottom face
                2, 3, 7, 7, 6, 2,  // top face
                0, 3, 7, 7, 4, 0,  // left face
                1, 2, 6, 6, 5, 1   // right face
            };

            // Create and bind VAO, VBO, and EBO for the cube
            _vaoCube = GL.GenVertexArray();
            _vboCube = GL.GenBuffer();
            _eboCube = GL.GenBuffer();

            GL.BindVertexArray(_vaoCube);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboCube);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboCube);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        private void InitializePyramid()
        {
            // Define a simple pyramid
            float[] vertices = {
                // Positions          // Colors
                -0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, // Base
                 0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, // Base
                 0.5f, -0.5f,  0.5f, 1.0f, 0.0f, 0.0f, // Base
                -0.5f, -0.5f,  0.5f, 1.0f, 0.0f, 0.0f, // Base
                 0.0f,  0.5f,  0.0f, 0.0f, 1.0f, 0.0f  // Apex
            };

            uint[] indices = {
                0, 1, 4,  // Front face
                1, 2, 4,  // Right face
                2, 3, 4,  // Back face
                3, 0, 4   // Left face
            };

            _vaoPyramid = GL.GenVertexArray();
            _vboPyramid = GL.GenBuffer();
            _eboPyramid = GL.GenBuffer();

            GL.BindVertexArray(_vaoPyramid);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboPyramid);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboPyramid);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        private void InitializeCylinder()
        {
            // Create a simple cylinder
            // (Use a more detailed mesh generation algorithm in a real-world app)

            // For simplicity, let's use a placeholder for cylinder initialization
            // Similar to how cube and pyramid were initialized
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteBuffer(_vboCube);
            GL.DeleteBuffer(_eboCube);
            GL.DeleteVertexArray(_vaoCube);
            GL.DeleteBuffer(_vboPyramid);
            GL.DeleteBuffer(_eboPyramid);
            GL.DeleteVertexArray(_vaoPyramid);
            GL.DeleteBuffer(_vboCylinder);
            GL.DeleteBuffer(_eboCylinder);
            GL.DeleteVertexArray(_vaoCylinder);
            _shader.Dispose();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();

            // Handle any logic for updates (e.g. camera movement, light changes)
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            // Set uniform matrices for the shader
            _shader.SetMatrix4("model", _modelCube);
            _shader.SetMatrix4("view", _view);
            _shader.SetMatrix4("projection", _projection);

            // Set light properties
            _shader.SetVector3("lightPos", new Vector3(3f, 3f, 3f)); // Position of light source
            _shader.SetVector3("viewPos", new Vector3(3f, 3f, 3f)); // Camera position
            _shader.SetVector3("lightColor", new Vector3(1f, 1f, 1f)); // White light

            // Draw Cube
            GL.BindVertexArray(_vaoCube);
            GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);

            // Draw Pyramid
            GL.BindVertexArray(_vaoPyramid);
            GL.DrawElements(PrimitiveType.Triangles, 12, DrawElementsType.UnsignedInt, 0);

            // Draw Cylinder (placeholder)
            GL.BindVertexArray(_vaoCylinder);
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

        public void SetVector3(string name, Vector3 vector)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.Uniform3(location, vector);
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
