private int CreatePyramidVAO()
{
    float[] vertices = {
        // Позиции          // Нормали
        0.0f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, // верхняя точка (вверх)
        -0.5f, -0.5f, -0.5f, -1.0f, -1.0f, -1.0f, // задний левый
        0.5f, -0.5f, -0.5f,  1.0f, -1.0f, -1.0f, // задний правый
        0.5f, -0.5f,  0.5f,  1.0f, -1.0f,  1.0f, // передний правый
        -0.5f, -0.5f,  0.5f, -1.0f, -1.0f,  1.0f  // передний левый
    };

    uint[] indices = {
        0, 1, 2,
        0, 2, 3,
        0, 3, 4,
        0, 4, 1,
        1, 2, 3,
        1, 3, 4
    };

    int vao = GL.GenVertexArray();
    int vbo = GL.GenBuffer();
    int ebo = GL.GenBuffer();

    GL.BindVertexArray(vao);

    GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
    GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
    GL.EnableVertexAttribArray(0);

    GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
    GL.EnableVertexAttribArray(1);

    GL.BindVertexArray(0);

    return vao;
}
