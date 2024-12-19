#version 330 core

layout (location = 0) in vec3 aPos;

out vec3 RayDirection;

void main()
{
    // Преобразуем координаты вершины
    gl_Position = vec4(aPos, 1.0);

    // Направление луча
    RayDirection = normalize(aPos);
}

#version 330 core

out vec4 FragColor;

in vec3 RayDirection;

uniform vec3 Sphere1Position;
uniform float SphereRadius;
uniform vec3 LightPosition;

void main()
{
    vec3 rayOrigin = vec3(0.0, 0.0, 0.0);

    // Трассировка сферы
    vec3 oc = rayOrigin - Sphere1Position;
    float b = 2.0 * dot(oc, RayDirection);
    float c = dot(oc, oc) - SphereRadius * SphereRadius;
    float discriminant = b * b - 4.0 * c;

    if (discriminant < 0.0)
    {
        // Промах
        FragColor = vec4(0.0, 0.0, 0.0, 1.0);
    }
    else
    {
        // Попадание
        float t = (-b - sqrt(discriminant)) / 2.0;
        vec3 hitPoint = rayOrigin + t * RayDirection;
        vec3 normal = normalize(hitPoint - Sphere1Position);

        vec3 lightDir = normalize(LightPosition - hitPoint);
        float diff = max(dot(normal, lightDir), 0.0);

        FragColor = vec4(vec3(diff), 1.0);
    }
}

protected override void OnRenderFrame(FrameEventArgs args)
{
    base.OnRenderFrame(args);

    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    _shader.Use();
    GL.BindVertexArray(_vao);

    GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

    SwapBuffers();
}
