if (string.IsNullOrWhiteSpace(vertexSource) || !vertexSource.EndsWith("}"))
{
    throw new Exception("Vertex shader source is empty or incomplete!");
}
if (string.IsNullOrWhiteSpace(fragmentSource) || !fragmentSource.EndsWith("}"))
{
    throw new Exception("Fragment shader source is empty or incomplete!");
}
