#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;

out vec3 FragPos;
out vec3 Normal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    FragPos = vec3(model * vec4(aPosition, 1.0));
    Normal = mat3(transpose(inverse(model))) * aNormal;

    gl_Position = projection * view * vec4(FragPos, 1.0);
}

#version 330 core

in vec3 FragPos;
in vec3 Normal;

out vec4 FragColor;

uniform vec3 lightDirection;
uniform vec3 pointLightPosition;
uniform vec3 viewPosition;

// Материальные параметры
uniform vec3 objectColor = vec3(1.0, 0.5, 0.31);
uniform vec3 lightColor = vec3(1.0, 1.0, 1.0);

// Функция расчёта диффузного освещения
vec3 calculateDirectionalLight(vec3 normal, vec3 lightDir)
{
    float diff = max(dot(normal, -lightDir), 0.0);
    return lightColor * diff;
}

// Функция расчёта точечного освещения
vec3 calculatePointLight(vec3 fragPos, vec3 normal)
{
    vec3 lightDir = normalize(pointLightPosition - fragPos);
    float diff = max(dot(normal, lightDir), 0.0);
    float distance = length(pointLightPosition - fragPos);
    float attenuation = 1.0 / (distance * distance);
    return lightColor * diff * attenuation;
}

void main()
{
    vec3 norm = normalize(Normal);
    vec3 directionalLight = calculateDirectionalLight(norm, normalize(lightDirection));
    vec3 pointLight = calculatePointLight(FragPos, norm);

    vec3 result = (directionalLight + pointLight) * objectColor;
    FragColor = vec4(result, 1.0);
}
