protected override void OnRenderFrame(FrameEventArgs args)
{
    GL.Clear(ClearBufferMask.ColorBufferBit);

    // Обновим данные в буфере вершин, чтобы они содержали новые координаты
    VertexPositionColor[] vertices = new VertexPositionColor[this.currentShape.Length];
    for (int i = 0; i < this.currentShape.Length; i++)
    {
        vertices[i] = new VertexPositionColor(this.currentShape[i], new Color4(0.2f, 0.4f, 0.8f, 1f));
    }

    this.vertexBuffer.SetData(vertices, vertices.Length); // Обновляем данные в буфере

    GL.UseProgram(this.shaderProgram.ShaderProgramHandle);

    GL.BindVertexArray(this.vertexArray.VertexArrayHandle);
    GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBuffer.IndexBufferHandle);
    GL.DrawElements(PrimitiveType.Triangles, this.indexCount, DrawElementsType.UnsignedInt, 0);

    this.Context.SwapBuffers();
    base.OnRenderFrame(args);
}
