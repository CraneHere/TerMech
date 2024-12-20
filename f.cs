using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Generic;

namespace OpenTK4Scene
{
    class Program : GameWindow
    {
        private Shader _shader;

        // Вектор камеры и направления
        private Vector3 _cameraPosition = new Vector3(0, 0, 5);
        private Vector3 _cameraDirection = new Vector3(0, 0, -1);

        // Список объектов сцены
        private List<IObject> _sceneObjects;

        public Program() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            Size = new Vector2i(800, 600);
            Title = "Ray Tracing Scene";
            _sceneObjects = new List<IObject>();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            _shader = new Shader();

            // Добавление объектов на сцену
            _sceneObjects.Add(new Sphere(new Vector3(0, 0, -3), 1f));  // Сфера 1
            _sceneObjects.Add(new Sphere(new Vector3(2, 0, -3), 1f));  // Сфера 2
            _sceneObjects.Add(new Sphere(new Vector3(-2, 0, -3), 1f)); // Сфера 3
            _sceneObjects.Add(new Plane(new Vector3(0, -1, 0), 1f));   // Плоскость
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            // Здесь можно добавлять обновление состояния сцены, например, движение камеры
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            // Трассировка лучей для каждого пикселя экрана
            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    Ray ray = GenerateRay(x, y);
                    Vector3 color = TraceRay(ray, 0); // Рекурсивная трассировка
                    _shader.SetVector3("color", color);
                    // Здесь можно использовать цвет для пикселя, например, с помощью OpenGL
                }
            }

            SwapBuffers();
        }

        private Ray GenerateRay(int x, int y)
        {
            // Генерация луча из камеры
            float aspectRatio = Size.X / (float)Size.Y;
            float fov = 90f;
            float tanFov = MathF.Tan(MathF.PI / 2f * fov / 180f);

            float screenX = (2f * (x + 0.5f) / Size.X - 1f) * aspectRatio * tanFov;
            float screenY = (1f - 2f * (y + 0.5f) / Size.Y) * tanFov;

            Vector3 rayDirection = Vector3.Normalize(new Vector3(screenX, screenY, -1));
            return new Ray(_cameraPosition, rayDirection);
        }

        private Vector3 TraceRay(Ray ray, int depth)
        {
            if (depth > 3) return new Vector3(0, 0, 0); // Ограничение на глубину рекурсии

            float closestT = float.MaxValue;
            IObject closestObject = null;
            Vector3 intersectionPoint = Vector3.Zero;

            // Найдем ближайший объект, с которым луч пересекается
            foreach (var obj in _sceneObjects)
            {
                if (obj.Intersect(ray, out float t))
                {
                    if (t < closestT)
                    {
                        closestT = t;
                        closestObject = obj;
                        intersectionPoint = ray.Origin + ray.Direction * t;
                    }
                }
            }

            if (closestObject == null)
                return new Vector3(0, 0, 0); // Нет пересечений, черный цвет

            // Вычисление цвета на основе материала объекта (простое освещение)
            Vector3 normal = closestObject.GetNormal(intersectionPoint);
            Vector3 lightDir = Vector3.Normalize(new Vector3(1, 1, -1)); // Направление источника света
            float diffuse = MathF.Max(Vector3.Dot(normal, lightDir), 0.0f);

            Vector3 color = closestObject.GetColor() * diffuse;

            // Рассчитываем отражения и преломления
            if (depth < 3)
            {
                Vector3 reflectDir = Vector3.Reflect(ray.Direction, normal);
                Ray reflectRay = new Ray(intersectionPoint + normal * 0.001f, reflectDir);
                color += TraceRay(reflectRay, depth + 1) * 0.5f; // Отражение

                Vector3 refractDir = Vector3.Refract(ray.Direction, normal, 1.0f / 1.5f); // Преломление (с индексом преломления 1.5)
                Ray refractRay = new Ray(intersectionPoint + normal * 0.001f, refractDir);
                color += TraceRay(refractRay, depth + 1) * 0.5f; // Преломление
            }

            return color;
        }

        [STAThread]
        static void Main(string[] args)
        {
            using (var program = new Program())
            {
                program.Run();
            }
        }
    }

    // Структура луча
    public struct Ray
    {
        public Vector3 Origin;
        public Vector3 Direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }
    }

    // Интерфейс для объектов сцены
    public interface IObject
    {
        bool Intersect(Ray ray, out float t);
        Vector3 GetNormal(Vector3 point);
        Vector3 GetColor();
    }

    // Реализация сферы
    public class Sphere : IObject
    {
        private Vector3 _center;
        private float _radius;

        public Sphere(Vector3 center, float radius)
        {
            _center = center;
            _radius = radius;
        }

        public bool Intersect(Ray ray, out float t)
        {
            t = 0f;
            Vector3 oc = ray.Origin - _center;
            float a = Vector3.Dot(ray.Direction, ray.Direction);
            float b = 2.0f * Vector3.Dot(oc, ray.Direction);
            float c = Vector3.Dot(oc, oc) - _radius * _radius;
            float discriminant = b * b - 4 * a * c;

            if (discriminant < 0)
                return false;

            t = (-b - MathF.Sqrt(discriminant)) / (2.0f * a);
            if (t < 0) t = (-b + MathF.Sqrt(discriminant)) / (2.0f * a);

            return t >= 0;
        }

        public Vector3 GetNormal(Vector3 point)
        {
            return Vector3.Normalize(point - _center);
        }

        public Vector3 GetColor()
        {
            return new Vector3(0.8f, 0.3f, 0.3f); // Красный цвет для сферы
        }
    }

    // Реализация плоскости
    public class Plane : IObject
    {
        private Vector3 _normal;
        private float _distance;

        public Plane(Vector3 normal, float distance)
        {
            _normal = Vector3.Normalize(normal);
            _distance = distance;
        }

        public bool Intersect(Ray ray, out float t)
        {
            t = 0f;
            float denom = Vector3.Dot(ray.Direction, _normal);
            if (MathF.Abs(denom) > 1e-6f)
            {
                t = (Vector3.Dot(_normal, new Vector3(0, 0, 0)) - Vector3.Dot(ray.Origin, _normal)) / denom;
                return t >= 0;
            }
            return false;
        }

        public Vector3 GetNormal(Vector3 point)
        {
            return _normal;
        }

        public Vector3 GetColor()
        {
            return new Vector3(0.5f, 0.5f, 0.5f); // Серый цвет для плоскости
        }
    }
}
