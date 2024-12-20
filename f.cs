#version 330 core

layout(location = 0) in vec3 aPosition; // Вершины
layout(location = 1) in vec3 aColor;    // Цвета

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 vertexColor;

void main()
{
    // Применяем модельную матрицу, затем видовую и перспективную проекцию
    gl_Position = projection * view * model * vec4(aPosition, 1.0f);
    
    // Передаем цвет во фрагментный шейдер
    vertexColor = aColor;
}

#version 330 core

in vec3 vertexColor; // Получаем цвет из вершинного шейдера

out vec4 FragColor;  // Итоговый цвет фрагмента

void main()
{
    FragColor = vec4(vertexColor, 1.0f); // Просто выводим цвет
}
