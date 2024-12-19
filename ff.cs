protected override void OnLoad()
{
    base.OnLoad();

    float[] vertices = {
        -1.0f, -1.0f, 0.0f,
         1.0f, -1.0f, 0.0f,
         1.0f,  1.0f, 0.0f,
        -1.0f,  1.0f, 0.0f,
    };

    uint[] indices = {
        0, 1, 2,
        2, 3, 0
    };

    _vao = GL.GenVertexArray();
    _vbo = GL.GenBuffer();
    int ebo = GL.GenBuffer();

    GL.BindVertexArray(_vao);

    GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
    GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
    GL.EnableVertexAttribArray(0);

    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    GL.BindVertexArray(0);

    // Загружаем шейдеры
    _shader = new Shader();
    _shader.Use();

    // Устанавливаем параметры
    _shader.SetVector3("Sphere1Position", new Vector3(-1.5f, 0.0f, -5.0f));
    _shader.SetVector3("Sphere2Position", new Vector3(1.5f, 0.0f, -5.0f));
    _shader.SetVector3("Sphere3Position", new Vector3(0.0f, 1.5f, -5.0f));
    _shader.SetVector3("PlaneNormal", new Vector3(0.0f, 1.0f, 0.0f));
    _shader.SetFloat("SphereRadius", 1.0f);
    _shader.SetVector3("LightPosition", new Vector3(0.0f, 5.0f, -3.0f));
    _shader.SetVector3("CameraPosition", new Vector3(0.0f, 0.0f, 0.0f));
}
