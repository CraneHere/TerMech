private void InitializePyramid()
{
    float[] vertices = {
        // Base (square)
        -0.5f, 0.0f, -0.5f,  1.0f, 0.0f, 0.0f, // 0 (Red)
         0.5f, 0.0f, -0.5f,  1.0f, 0.0f, 0.0f, // 1 (Red)
         0.5f, 0.0f,  0.5f,  1.0f, 0.0f, 0.0f, // 2 (Red)
        -0.5f, 0.0f,  0.5f,  1.0f, 0.0f, 0.0f, // 3 (Red)
        
        // Apex (top of the pyramid)
         0.0f, 1.0f, 0.0f,  0.0f, 1.0f, 0.0f  // 4 (Green)
    };

    uint[] indices = {
        // Base square
        0, 1, 2, 2, 3, 0,

        // Four sides (triangles)
        0, 1, 4,
        1, 2, 4,
        2, 3, 4,
        3, 0, 4
    };

    // Create and bind VAO, VBO, and EBO for the pyramid
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

protected override void OnRenderFrame(FrameEventArgs args)
{
    base.OnRenderFrame(args);
    
    GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    // Use the shader program
    _shader.Use();

    // Set the projection matrix (perspective projection)
    Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Width / (float)Height, 0.1f, 100.0f);
    _shader.SetMatrix4("projection", projection);

    // Set the view matrix (camera)
    Matrix4 view = Matrix4.LookAt(new Vector3(0f, 0f, 5f), Vector3.Zero, Vector3.UnitY);
    _shader.SetMatrix4("view", view);

    // Set the model matrix for the cube and draw it
    Matrix4 cubeModel = Matrix4.CreateTranslation(-1.5f, 0f, 0f);
    _shader.SetMatrix4("model", cubeModel);
    GL.BindVertexArray(_vaoCube);
    GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);

    // Set the model matrix for the pyramid and draw it
    Matrix4 pyramidModel = Matrix4.CreateTranslation(1.5f, 0f, 0f);
    _shader.SetMatrix4("model", pyramidModel);
    GL.BindVertexArray(_vaoPyramid);
    GL.DrawElements(PrimitiveType.Triangles, 18, DrawElementsType.UnsignedInt, 0);

    GL.BindVertexArray(0);

    SwapBuffers();
}
