using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace otk;

public class Textury
{
    private readonly int Handle;

    public static Textury LoadFromFile(string path)
    {
        // generate handle
        var handle = GL.GenTexture();

        // bind the texture and handle
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, handle);

        // flip the texture
        StbImage.stbi_set_flip_vertically_on_load(1);

        // load the texture
        using Stream stream = File.OpenRead(path);
        var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

        // min mag ( scaling ) filter
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        // wrapping mode
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        // generate mipmap
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        return new Textury(handle);
    }

    private Textury(int glHandle)
    {
        Handle = glHandle;
    }

    public void Use(TextureUnit unit)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, Handle);
    }
}