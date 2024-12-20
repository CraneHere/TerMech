private int LoadTexture(string path)
{
    int textureId = GL.GenTexture();
    GL.BindTexture(TextureTarget.Texture2D, textureId);

    var image = new System.Drawing.Bitmap(path);
    var data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height), 
                              System.Drawing.Imaging.ImageLockMode.ReadOnly, 
                              System.Drawing.Imaging.PixelFormat.Format32bppArgb);

    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0,
                  OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
    image.UnlockBits(data);

    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

    return textureId;
}

private int wallTexture;
private int floorTexture;
private int sphereTexture;

protected override void OnLoad()
{
    base.OnLoad();
    GL.ClearColor(0.06f, 0.3f, 0.5f, 1.0f);
    GL.Enable(EnableCap.DepthTest);

    wallTexture = LoadTexture("path_to_wall_texture.png");
    floorTexture = LoadTexture("path_to_floor_texture.png");
    sphereTexture = LoadTexture("path_to_sphere_texture.png");

    // Инициализация материалов и объектов, как и раньше...
}

private Vector2 GetTextureCoordinates(Vector3 hitPoint, Plane plane)
{
    Vector3 vecToPoint = hitPoint - plane.Point;
    float u = Vector3.Dot(vecToPoint, new Vector3(1, 0, 0)) * 0.1f;  // Простой расчет U
    float v = Vector3.Dot(vecToPoint, new Vector3(0, 0, 1)) * 0.1f;  // Простой расчет V
    return new Vector2(u, v);
}

private Vector3 RayTrace(Vector3 rayOrigin, Vector3 rayDir, int depth)
{
    if (depth > MaxDepth) return new Vector3(0.06f, 0.3f, 0.5f);

    // Проверка на пересечение с шарами
    foreach (var sphere in spheres)
    {
        if (IntersectSphere(rayOrigin, rayDir, sphere, out float t))
        {
            Vector3 hitPoint = rayOrigin + t * rayDir;
            Vector3 normal = Vector3.Normalize(hitPoint - sphere.Center);

            Vector3 lightDir = Vector3.Normalize(lightPosition - hitPoint);
            float diffuseIntensity = Math.Max(Vector3.Dot(normal, lightDir), 0.0f);

            // Получаем координаты текстуры на сфере
            Vector3 sphereColor = GetSphereTextureColor(hitPoint, sphere);

            // Если объект в тени, уменьшить интенсивность
            if (IsInShadow(hitPoint, lightPosition))
            {
                diffuseIntensity *= 0.3f;
            }

            // Отражение
            Vector3 reflectionDir = Reflect(rayDir, normal);
            Vector3 reflectionColor = RayTrace(hitPoint + normal * 0.001f, reflectionDir, depth + 1);

            // Преломление
            Vector3 refractionColor = Vector3.Zero;
            if (sphere.Material.RefractionIndex > 1.0f)
            {
                refractionColor = Refract(rayDir, normal, sphere.Material.RefractionIndex);
                refractionColor = RayTrace(hitPoint + normal * 0.001f, refractionColor, depth + 1);
            }

            return sphere.Material.Reflectivity * reflectionColor + (1 - sphere.Material.Reflectivity) * refractionColor + sphereColor * diffuseIntensity;
        }
    }

    // Проверка на пересечение с плоскостью
    if (IntersectPlane(rayOrigin, rayDir, plane, out float tPlane))
    {
        Vector3 hitPoint = rayOrigin + tPlane * rayDir;
        Vector3 normal = plane.Normal;

        Vector3 lightDir = Vector3.Normalize(lightPosition - hitPoint);
        float diffuseIntensity = Math.Max(Vector3.Dot(normal, lightDir), 0.0f);

        // Получаем координаты текстуры на плоскости
        Vector2 uv = GetTextureCoordinates(hitPoint, plane);
        Vector3 planeColor = GetTextureColor(floorTexture, uv); // Для пола

        // Если объект в тени, уменьшить интенсивность
        if (IsInShadow(hitPoint, lightPosition))
        {
            diffuseIntensity *= 0.3f;
        }

        return planeColor * diffuseIntensity;
    }

    return new Vector3(0.06f, 0.3f, 0.5f); 
}


private Vector3 GetSphereTextureColor(Vector3 hitPoint, Sphere sphere)
{
    Vector3 sphereNormal = Vector3.Normalize(hitPoint - sphere.Center);
    float u = 0.5f + (float)(Math.Atan2(sphereNormal.Z, sphereNormal.X) / (2 * Math.PI));
    float v = 0.5f - (float)(Math.Asin(sphereNormal.Y) / Math.PI);

    Vector2 uv = new Vector2(u, v);
    return GetTextureColor(sphereTexture, uv); // Получаем цвет с текстуры
}

private Vector3 GetTextureColor(int textureId, Vector2 uv)
{
    GL.BindTexture(TextureTarget.Texture2D, textureId);
    int width = 512, height = 512; // Разрешение текстуры
    byte[] data = new byte[width * height * 4];
    GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data);
    
    int x = (int)(uv.X * width) % width;
    int y = (int)(uv.Y * height) % height;

    int index = (y * width + x) * 4;
    byte r = data[index + 2];
    byte g = data[index + 1];
    byte b = data[index];

    return new Vector3(r / 255.0f, g / 255.0f, b / 255.0f);
}

