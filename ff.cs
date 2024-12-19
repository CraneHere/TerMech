#version 330 core

out vec4 FragColor;

in vec3 RayOrigin;
in vec3 RayDirection;

uniform vec3 Sphere1Position;
uniform vec3 Sphere2Position;
uniform vec3 Sphere3Position;
uniform vec3 PlaneNormal;
uniform float SphereRadius;
uniform vec3 LightPosition;
uniform vec3 CameraPosition;

// Maximum recursion depth for reflections and refractions
const int MAX_DEPTH = 5;

vec3 reflectRay(vec3 I, vec3 N) {
    return I - 2.0 * dot(I, N) * N;
}

bool intersectSphere(vec3 origin, vec3 dir, vec3 center, float radius, out float t) {
    vec3 oc = origin - center;
    float a = dot(dir, dir);
    float b = 2.0 * dot(oc, dir);
    float c = dot(oc, oc) - radius * radius;
    float discriminant = b * b - 4.0 * a * c;

    if (discriminant < 0.0) return false;
    t = (-b - sqrt(discriminant)) / (2.0 * a);
    return t > 0.0;
}

bool intersectPlane(vec3 origin, vec3 dir, vec3 normal, out float t) {
    float denom = dot(normal, dir);
    if (abs(denom) > 0.0001) {
        t = dot(-origin, normal) / denom;
        return t >= 0.0;
    }
    return false;
}

vec3 traceRay(vec3 origin, vec3 dir, int depth) {
    if (depth >= MAX_DEPTH) return vec3(0.0); // Terminate recursion

    float t;
    vec3 hitColor = vec3(0.0);

    vec3 closestHitNormal;
    vec3 closestHitPoint;
    float closestT = 1e20; // Large initial value
    bool hitSomething = false;

    // Check sphere intersections
    if (intersectSphere(origin, dir, Sphere1Position, SphereRadius, t) && t < closestT) {
        closestT = t;
        closestHitNormal = normalize((origin + t * dir) - Sphere1Position);
        closestHitPoint = origin + t * dir;
        hitSomething = true;
    }
    if (intersectSphere(origin, dir, Sphere2Position, SphereRadius, t) && t < closestT) {
        closestT = t;
        closestHitNormal = normalize((origin + t * dir) - Sphere2Position);
        closestHitPoint = origin + t * dir;
        hitSomething = true;
    }
    if (intersectSphere(origin, dir, Sphere3Position, SphereRadius, t) && t < closestT) {
        closestT = t;
        closestHitNormal = normalize((origin + t * dir) - Sphere3Position);
        closestHitPoint = origin + t * dir;
        hitSomething = true;
    }

    // Check plane intersection
    if (intersectPlane(origin, dir, PlaneNormal, t) && t < closestT) {
        closestT = t;
        closestHitNormal = PlaneNormal;
        closestHitPoint = origin + t * dir;
        hitSomething = true;
    }

    if (hitSomething) {
        vec3 reflectedDir = reflectRay(dir, closestHitNormal);
        vec3 reflectedColor = traceRay(closestHitPoint + 0.01 * closestHitNormal, reflectedDir, depth + 1);

        // Basic shading with light
        vec3 lightDir = normalize(LightPosition - closestHitPoint);
        float diff = max(dot(lightDir, closestHitNormal), 0.0);
        vec3 diffuse = diff * vec3(0.8, 0.8, 0.8);

        hitColor = diffuse + 0.5 * reflectedColor;
    }

    return hitColor;
}

void main() {
    vec3 color = traceRay(RayOrigin, RayDirection, 0);
    FragColor = vec4(color, 1.0);
}

     
-----------------------------------------------------------------------------------------------------

protected override void OnLoad()
{
    base.OnLoad();

    // Компиляция шейдера
    _shader = new Shader("path/to/vertex_shader.vert", "path/to/fragment_shader.frag");
    _shader.Use();

    // Настройки сцены
    _shader.SetVector3("Sphere1Position", new Vector3(-1.5f, 0.0f, -5.0f));
    _shader.SetVector3("Sphere2Position", new Vector3(1.5f, 0.0f, -5.0f));
    _shader.SetVector3("Sphere3Position", new Vector3(0.0f, 1.5f, -5.0f));
    _shader.SetVector3("PlaneNormal", new Vector3(0.0f, 1.0f, 0.0f));
    _shader.SetFloat("SphereRadius", 1.0f);
    _shader.SetVector3("LightPosition", new Vector3(0.0f, 5.0f, -3.0f));
    _shader.SetVector3("CameraPosition", new Vector3(0.0f, 0.0f, 0.0f));
}

protected override void OnRenderFrame(FrameEventArgs args)
{
    base.OnRenderFrame(args);

    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    _shader.Use();

    // Рисуем (рендеринг будет в шейдере)
    GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

    SwapBuffers();
}

-----------------------------------------------------------------------------------------------------


