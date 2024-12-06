protected override void OnRenderFrame(FrameEventArgs args)
{
    // Создаем матрицу трансформации (например, вращение)
    float rotationAngle = (float)(args.Time * MathHelper.TwoPi); // Вращение за время каждого кадра
    Matrix4 rotationMatrix = Matrix4.CreateRotationZ(rotationAngle);

    // Устанавливаем матрицу в шейдер при каждом кадре
    this.shaderProgram.SetUniform("ModelViewMatrix", rotationMatrix);

    GL.Clear(ClearBufferMask.ColorBufferBit);
    GL.UseProgram(this.shaderProgram.ShaderProgramHandle);
    GL.BindVertexArray(this.vertexArray.VertexArrayHandle);
    GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBuffer.IndexBufferHandle);
    GL.DrawElements(PrimitiveType.Triangles, this.indexCount, DrawElementsType.UnsignedInt, 0);

    this.Context.SwapBuffers();
    base.OnRenderFrame(args);
}
