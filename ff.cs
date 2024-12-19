public class Ray
{
    public Vector3 Origin { get; set; }
    public Vector3 Direction { get; set; }

    public Ray(Vector3 origin, Vector3 direction)
    {
        Origin = origin;
        Direction = direction;
    }
}

public class Sphere
{
    public Vector3 Center { get; set; }
    public float Radius { get; set; }

    public Sphere(Vector3 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    // Функция пересечения луча с сферой
    public bool Intersect(Ray ray, out float t)
    {
        t = 0;

        Vector3 oc = ray.Origin - Center;
        float a = Vector3.Dot(ray.Direction, ray.Direction);
        float b = 2.0f * Vector3.Dot(oc, ray.Direction);
        float c = Vector3.Dot(oc, oc) - Radius * Radius;
        float discriminant = b * b - 4.0f * a * c;

        if (discriminant > 0)
        {
            t = (-b - (float)Math.Sqrt(discriminant)) / (2.0f * a);
            return true;
        }

        return false;
    }
}


public class RayTracer
{
    public List<Sphere> Spheres { get; set; }

    public RayTracer()
    {
        Spheres = new List<Sphere>();
    }

    // Трассировка луча по сцене
    public Vector3 TraceRay(Ray ray)
    {
        float tMin = float.MaxValue;
        Sphere closestSphere = null;

        // Ищем ближайшую сферу
        foreach (var sphere in Spheres)
        {
            if (sphere.Intersect(ray, out float t) && t < tMin)
            {
                tMin = t;
                closestSphere = sphere;
            }
        }

        if (closestSphere != null)
        {
            // Если луч пересекает сферу, возвращаем цвет
            Vector3 intersectionPoint = ray.Origin + ray.Direction * tMin;
            Vector3 normal = Vector3.Normalize(intersectionPoint - closestSphere.Center);
            return CalculateColor(normal); // Просто пример цвета по нормали
        }

        return new Vector3(0.0f, 0.0f, 0.0f); // Фон
    }

    private Vector3 CalculateColor(Vector3 normal)
    {
        // Простейший цвет: чем больше угол между нормалью и направлением к свету, тем светлее
        float brightness = Math.Max(Vector3.Dot(normal, new Vector3(1.0f, 1.0f, 1.0f)), 0.0f);
        return new Vector3(brightness, brightness, brightness);
    }
}


protected override void OnRenderFrame(FrameEventArgs args)
{
    base.OnRenderFrame(args);

    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

    // Рендеринг через трассировку лучей
    var rayTracer = new RayTracer();
    rayTracer.Spheres.Add(new Sphere(new Vector3(0.0f, 0.0f, -5.0f), 1.0f));

    for (int y = 0; y < Size.Y; y++)
    {
        for (int x = 0; x < Size.X; x++)
        {
            // Рассчитываем направление луча для каждого пикселя
            float xNorm = (x / (float)Size.X) * 2.0f - 1.0f;
            float yNorm = (y / (float)Size.Y) * 2.0f - 1.0f;
            Vector3 rayDirection = new Vector3(xNorm, yNorm, -1.0f); // Просто пример

            Ray ray = new Ray(new Vector3(0.0f, 0.0f, 0.0f), rayDirection);
            Vector3 color = rayTracer.TraceRay(ray);

            // Устанавливаем цвет пикселя в OpenGL
            GL.Color3(color.X, color.Y, color.Z);
            GL.Begin(PrimitiveType.Points);
            GL.Vertex2(x, y);
            GL.End();
        }
    }

    SwapBuffers();
}
