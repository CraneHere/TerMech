protected override void OnRenderFrame(FrameEventArgs args)
{
    GL.Clear(ClearBufferMask.ColorBufferBit);

    GL.UseProgram(this.shaderProgram.ShaderProgramHandle);

    GL.BindVertexArray(this.vertexArray.VertexArrayHandle);
    GL.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBuffer.IndexBufferHandle);
    GL.DrawElements(PrimitiveType.Triangles, this.indexCount, DrawElementsType.UnsignedInt, 0);

    this.Context.SwapBuffers();
    base.OnRenderFrame(args);
}
