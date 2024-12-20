private int CreateSphereVAO()
{
    int latitudeBands = 30;
    int longitudeBands = 30;
    float radius = 1.0f;

    List<float> vertices = new List<float>();

    for (int latNumber = 0; latNumber <= latitudeBands; latNumber++)
    {
        float theta = latNumber * MathF.PI / latitudeBands;
        float sinTheta = MathF.Sin(theta);
        float cosTheta = MathF.Cos(theta);

        for (int lonNumber = 0; lonNumber <= longitudeBands; lonNumber++)
        {
            float phi = lonNumber * 2 * MathF.PI / longitudeBands;
            float sinPhi = MathF.Sin(phi);
            float cosPhi = MathF.Cos(phi);

            // Сферы координаты
            float x = cosPhi * sinTheta;
            float y = cosTheta;
            float z = sinPhi * sinTheta;

            // Нормаль
            float nx = x;
            float ny = y;
            float nz = z;

            // Добавление вершины
            vertices.Add(x * radius);  // x
            vertices.Add(y * radius);  // y
            vertices.Add(z * radius);  // z

            // Нормали
            vertices.Add(nx);
            vertices.Add(ny);
            vertices.Add(nz);
        }
    }

    List<uint> indices = new List<uint>();
    for (int latNumber = 0; latNumber < latitudeBands; latNumber++)
    {
        for (int lonNumber = 0; lonNumber < longitudeBands; lonNumber++)
        {
            uint first = (uint)((latNumber * (longitudeBands + 1)) + lonNumber);
            uint second = (uint)((latNumber + 1) * (longitudeBands + 1) + lonNumber);
            uint third = (uint)((latNumber + 1) * (longitudeBands + 1) + (lonNumber + 1) % longitudeBands);
            uint fourth = (uint)((latNumber * (longitudeBands + 1)) + (lonNumber + 1) % longitudeBands);

            indices.Add(first);
            indices.Add(second);
            indices.Add(third);

            indices.Add(first);
            indices.Add(third);
            indices.Add(fourth);
        }
    }

    int vao = GL.GenVertexArray();
    int vbo = GL.GenBuffer();
    int ebo = GL.GenBuffer();

    GL.BindVertexArray(vao);

    GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
    GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);

    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
    GL.EnableVertexAttribArray(0);

    GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
    GL.EnableVertexAttribArray(1);

    GL.BindVertexArray(0);

    return vao;
}
