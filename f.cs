// Создание цилиндра (с верхней и нижней базой)
float[] cylinderVertices = new float[numSides * 6];
float radius = 1.0f;  // Радиус цилиндра
for (int i = 0; i < numSides; i++)
{
    float angle = i * 2 * MathF.PI / numSides;
    float x = MathF.Cos(angle) * radius;
    float z = MathF.Sin(angle) * radius;

    // Нижняя база (по оси Y = -1.0f)
    cylinderVertices[i * 6] = x;
    cylinderVertices[i * 6 + 1] = -1.0f;
    cylinderVertices[i * 6 + 2] = z;
    cylinderVertices[i * 6 + 3] = 1.0f;  // Цвет (красный)
    cylinderVertices[i * 6 + 4] = 0.0f;
    cylinderVertices[i * 6 + 5] = 0.0f;

    // Верхняя база (по оси Y = 1.0f)
    cylinderVertices[(i + numSides) * 6] = x;
    cylinderVertices[(i + numSides) * 6 + 1] = 1.0f;
    cylinderVertices[(i + numSides) * 6 + 2] = z;
    cylinderVertices[(i + numSides) * 6 + 3] = 0.0f;  // Цвет (синий)
    cylinderVertices[(i + numSides) * 6 + 4] = 0.0f;
    cylinderVertices[(i + numSides) * 6 + 5] = 1.0f;
}

// Боковые грани цилиндра
for (int i = 0; i < numSides; i++)
{
    int nextIndex = (i + 1) % numSides;
    
    // Нижняя грань (составляется из треугольников)
    cylinderVertices[(i + numSides * 2) * 6] = cylinderVertices[i * 6];
    cylinderVertices[(i + numSides * 2) * 6 + 1] = cylinderVertices[i * 6 + 1];
    cylinderVertices[(i + numSides * 2) * 6 + 2] = cylinderVertices[i * 6 + 2];
    cylinderVertices[(i + numSides * 2) * 6 + 3] = 0.0f;  // Цвет (зеленый)
    cylinderVertices[(i + numSides * 2) * 6 + 4] = 1.0f;
    cylinderVertices[(i + numSides * 2) * 6 + 5] = 0.0f;

    // Верхняя грань
    cylinderVertices[(i + numSides * 3) * 6] = cylinderVertices[nextIndex * 6];
    cylinderVertices[(i + numSides * 3) * 6 + 1] = cylinderVertices[nextIndex * 6 + 1];
    cylinderVertices[(i + numSides * 3) * 6 + 2] = cylinderVertices[nextIndex * 6 + 2];
    cylinderVertices[(i + numSides * 3) * 6 + 3] = 0.0f;  // Цвет (зеленый)
    cylinderVertices[(i + numSides * 3) * 6 + 4] = 1.0f;
    cylinderVertices[(i + numSides * 3) * 6 + 5] = 0.0f;
}

// Модифицированный способ построения боковых граней, чтобы они не выходили за пределы
