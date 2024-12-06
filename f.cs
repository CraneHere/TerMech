string vertexShaderCode = @"
#version 330 core

uniform mat4 ModelViewMatrix; // Матрица трансформации

layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec4 aColor;

out vec4 vColor;

void main()
{
    // Применение матрицы трансформации к координатам вершины
    vec4 transformedPosition = ModelViewMatrix * vec4(aPosition, 0.0, 1.0);
    gl_Position = transformedPosition;
    vColor = aColor;
}
";
