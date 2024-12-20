private Vector3 cameraPosition = new Vector3(0.0f, 2.0f, -8.0f); // Попробуйте изменить на что-то другое, например:
private Vector3 cameraPosition = new Vector3(0.0f, 2.0f, 0.0f);

Vector3 color = RayTrace(cameraPosition, rayDir, 0);
Console.WriteLine($"Color: {color}"); // Выводит значения цвета для каждого пикселя

return new Vector3(0.0f, 1.0f, 0.0f);

private Vector3 GetSphereTextureColor(Vector3 hitPoint, Sphere sphere)
{
    // Возвращаем фиксированный цвет для отладки (например, серый)
    return new Vector3(0.5f, 0.5f, 0.5f); 
}

