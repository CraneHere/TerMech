protected override void OnLoad()
{
    this.IsVisible = true;

    GL.ClearColor(0.8f, 0.8f, 0.8f, 1f);

    this.initialShape = CreateHexagon(200, 200, 100);
    this.targetShape = CreateHexagon(200, 200, 150); // Slightly larger hexagon
    this.currentShape = new Vector2[this.initialShape.Length];
    Array.Copy(this.initialShape, this.currentShape, this.initialShape.Length);

    VertexPositionColor[] vertices = new VertexPositionColor[this.currentShape.Length];
    for (int i = 0; i < this.currentShape.Length; i++)
    {
        vertices[i] = new VertexPositionColor(this.currentShape[i], new Color4(0.2f, 0.4f, 0.8f, 1f));
    }

    this.vertexCount = vertices.Length;

    int[] indices = new int[(this.vertexCount - 2) * 3];
    for (int i = 1; i < this.vertexCount - 1; i++)
    {
        indices[(i - 1) * 3] = 0;
        indices[(i - 1) * 3 + 1] = i;
        indices[(i - 1) * 3 + 2] = i + 1;
    }

    this.indexCount = indices.Length;

    this.vertexBuffer = new VertexBuffer(VertexPositionColor.VertexInfo, vertices.Length, true);
    this.vertexBuffer.SetData(vertices, vertices.Length);

    this.indexBuffer = new IndexBuffer(indices.Length, true);
    this.indexBuffer.SetData(indices, indices.Length);

    this.vertexArray = new VertexArray(this.vertexBuffer);

    string vertexShaderCode = @"
    #version 330 core
    uniform mat4 ModelViewMatrix; // Матрица трансформации
    layout (location = 0) in vec2 aPosition;
    layout (location = 1) in vec4 aColor;
    out vec4 vColor;
    void main()
    {
        vec4 transformedPosition = ModelViewMatrix * vec4(aPosition, 0.0, 1.0);
        gl_Position = transformedPosition;
        vColor = aColor;
    }";

    string pixelShaderCode = @"
    #version 330 core
    in vec4 vColor;
    out vec4 pixelColor;
    void main()
    {
        pixelColor = vColor;
    }";

    this.shaderProgram = new Shader(vertexShaderCode, pixelShaderCode);

    int[] viewport = new int[4];
    GL.GetInteger(GetPName.Viewport, viewport);
    this.shaderProgram.SetUniform("ViewportSize", (float)viewport[2], (float)viewport[3]);

    // Установка матрицы трансформации (например, смещение)
    Matrix4 translationMatrix = Matrix4.CreateTranslation(50, 50, 0);
    this.shaderProgram.SetUniform("ModelViewMatrix", translationMatrix);

    base.OnLoad();
}
