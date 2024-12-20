private int CreatePlaneVAO()
{
    // Текстурированные плоскости
    float[] vertices = {
        // Позиции            // Текстурные координаты
        -1.0f,  0.0f, -1.0f,  0.0f, 0.0f, // Левый нижний угол
         1.0f,  0.0f, -1.0f,  1.0f, 0.0f, // Правый нижний угол
         1.0f,  0.0f,  1.0f,  1.0f, 1.0f, // Правый верхний угол
        -1.0f,  0.0f,  1.0f,  0.0f, 1.0f  // Левый верхний угол
    };

    uint[] indices = {
        0, 1, 2,
        0, 2, 3
    };

    int vao = GL.GenVertexArray();
    int vbo = GL.GenBuffer();
    int ebo = GL.GenBuffer();

    GL.BindVertexArray(vao);

    GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
    GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
    GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
    GL.EnableVertexAttribArray(0);

    GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
    GL.EnableVertexAttribArray(1);

    GL.BindVertexArray(0);

    return vao;
}

private int CreateSphereVAO()
{
    // Моделирование сферы с использованием библиотеки или вручную
    // Используем стандартный способ создания сферы
    // (в данном случае для примера можно использовать готовую сферу или генерировать данные вручную)

    // Для создания сферы используйте UV-сферу (пример):

    List<float> vertices = new List<float>();
    List<uint> indices = new List<uint>();

    int sectorCount = 36; // Количество секторов
    int stackCount = 18;  // Количество стеков

    float radius = 1.0f;
    float PI = 3.14159265358979323846f;

    for (int i = 0; i <= stackCount; ++i) {
        float stackAngle = PI / 2 - i * PI / stackCount;
        float xy = radius * Mathf.Cos(stackAngle);
        float z = radius * Mathf.Sin(stackAngle);

        for (int j = 0; j <= sectorCount; ++j) {
            float sectorAngle = j * 2 * PI / sectorCount;
            float x = xy * Mathf.Cos(sectorAngle);
            float y = xy * Mathf.Sin(sectorAngle);

            vertices.Add(x);
            vertices.Add(y);
            vertices.Add(z);

            float u = (float)j / sectorCount;
            float v = (float)i / stackCount;

            vertices.Add(u);
            vertices.Add(v);
        }
    }

    for (int i = 0; i < stackCount; ++i) {
        for (int j = 0; j < sectorCount; ++j) {
            uint first = (uint)(i * (sectorCount + 1) + j);
            uint second = (uint)((i + 1) * (sectorCount + 1) + j);

            indices.Add(first);
            indices.Add(second);
            indices.Add(first + 1);

            indices.Add(second);
            indices.Add(second + 1);
            indices.Add(first + 1);
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

    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
    GL.EnableVertexAttribArray(0);

    GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
    GL.EnableVertexAttribArray(1);

    GL.BindVertexArray(0);

    return vao;
}
