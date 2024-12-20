private void InitializeCube()
{
    float[] vertices = {
        // Positions          // Colors
        -0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, // 0 (Red)
         0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, // 1 (Red)
         0.5f,  0.5f, -0.5f, 1.0f, 0.0f, 0.0f, // 2 (Red)
        -0.5f,  0.5f, -0.5f, 1.0f, 0.0f, 0.0f, // 3 (Red)
        -0.5f, -0.5f,  0.5f, 0.0f, 1.0f, 0.0f, // 4 (Green)
         0.5f, -0.5f,  0.5f, 0.0f, 1.0f, 0.0f, // 5 (Green)
         0.5f,  0.5f,  0.5f, 0.0f, 1.0f, 0.0f, // 6 (Green)
        -0.5f,  0.5f,  0.5f, 0.0f, 1.0f, 0.0f  // 7 (Green)
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
