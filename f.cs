public static class Vector3Extensions
{
    public static Vector3 Reflect(this Vector3 direction, Vector3 normal)
    {
        return direction - 2 * Vector3.Dot(direction, normal) * normal;
    }
}
