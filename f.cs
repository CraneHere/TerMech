#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;

out vec3 FragPos;
out vec3 Normal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    FragPos = vec3(model * vec4(aPosition, 1.0));
    Normal = mat3(transpose(inverse(model))) * aNormal;  
    gl_Position = projection * view * vec4(FragPos, 1.0);
}
------------
#version 330 core

in vec3 FragPos;
in vec3 Normal;

out vec4 FragColor;

uniform vec3 lightPos;
uniform vec3 viewPos;
uniform vec3 lightColor;
uniform vec3 objectColor;
uniform float shininess; // Intensity of specular reflection

void main()
{
    // Ambient lighting
    float ambientStrength = 0.1;
    vec3 ambient = ambientStrength * lightColor;

    // Diffuse lighting
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff * lightColor;

    // Specular lighting
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
    vec3 specular = spec * lightColor;

    // Final color
    vec3 result = (ambient + diffuse + specular) * objectColor;
    FragColor = vec4(result, 1.0);
}

--------------------
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

--------------------
protected override void OnLoad()
{
    base.OnLoad();
    
    _shader = new Shader("shader.vert", "shader.frag");

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
---------------------------------------
protected override void OnRenderFrame(FrameEventArgs args)
{
    base.OnRenderFrame(args);

    GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    _shader.Use();

    // Set up the camera
    Matrix4 model = Matrix4.Identity;
    Matrix4 view = Matrix4.LookAt(new Vector3(3.0f, 3.0f, 3.0f), Vector3.Zero, Vector3.UnitY);
    Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), (float)Width / Height, 0.1f, 100f);
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
----------------------------------------
protected override void OnUpdateFrame(FrameEventArgs args)
{
    base.OnUpdateFrame(args);

    if (KeyboardState.IsKeyDown(Keys.Up)) _speed += 0.1f;
    if (KeyboardState.IsKeyDown(Keys.Down)) _speed -= 0.1f;

    // Clamp the shininess value
    _speed = MathHelper.Clamp(_speed, 0.1f, 128.0f);
}
