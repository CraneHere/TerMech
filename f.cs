public void SetUniform(string name, Matrix3 matrix)
{
    if (!this.GetShaderUniform(name, out ShaderUniform uniform))
    {
        throw new ArgumentException("Name was not found.");
    }

    if (uniform.Type != ActiveUniformType.FloatMat3)
    {
        throw new ArgumentException("Uniform type is not FloatMat3.");
    }

    GL.UseProgram(this.ShaderProgramHandle);
    GL.UniformMatrix3(uniform.Location, false, ref matrix);
    GL.UseProgram(0);
}
