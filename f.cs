private Vector3 GetTextureColor(int textureId, Vector2 uv)
{
    GL.BindTexture(TextureTarget.Texture2D, textureId);
    int width = 512, height = 512; // Texture resolution
    byte[] data = new byte[width * height * 4];
    GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data);

    // Clamp UV to [0, 1] to avoid out of bounds
    uv.X = MathF.Clamp(uv.X, 0.0f, 1.0f);
    uv.Y = MathF.Clamp(uv.Y, 0.0f, 1.0f);

    int x = (int)(uv.X * width);
    int y = (int)(uv.Y * height);

    // Ensure the indices are within bounds
    x = Math.Min(width - 1, Math.Max(0, x));
    y = Math.Min(height - 1, Math.Max(0, y));

    int index = (y * width + x) * 4;
    byte r = data[index + 2];
    byte g = data[index + 1];
    byte b = data[index];

    return new Vector3(r / 255.0f, g / 255.0f, b / 255.0f);
}
