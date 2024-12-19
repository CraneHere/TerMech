#version 330 core

layout (location = 0) in vec3 aPos; // Входной атрибут позиции вершины

out vec3 RayDirection; // Передаем в фрагментный шейдер

void main()
{
    gl_Position = vec4(aPos, 1.0); // Преобразование координат вершины
    RayDirection = normalize(aPos); // Нормализуем направление луча
}

#version 330 core

out vec4 FragColor; // Цвет выходного пикселя

in vec3 RayDirection; // Получаем направление луча из вершинного шейдера

uniform vec3 Sphere1Position; // Позиция сферы
uniform float SphereRadius; // Радиус сферы
uniform vec3 LightPosition; // Позиция источника света

void main()
{
    vec3 rayOrigin = vec3(0.0, 0.0, 0.0); // Начальная точка луча

    // Рассчитываем пересечение луча со сферой
    vec3 oc = rayOrigin - Sphere1Position;
    float b = 2.0 * dot(oc, RayDirection);
    float c = dot(oc, oc) - SphereRadius * SphereRadius;
    float discriminant = b * b - 4.0 * c;

    if (discriminant < 0.0)
    {
        // Промах: возвращаем черный цвет
        FragColor = vec4(0.0, 0.0, 0.0, 1.0);
    }
    else
    {
        // Попадание: вычисляем цвет
        float t = (-b - sqrt(discriminant)) / 2.0;
        vec3 hitPoint = rayOrigin + t * RayDirection;
        vec3 normal = normalize(hitPoint - Sphere1Position);

        vec3 lightDir = normalize(LightPosition - hitPoint);
        float diff = max(dot(normal, lightDir), 0.0);

        FragColor = vec4(vec3(diff), 1.0); // Освещение с учетом диффузного отражения
    }
}
