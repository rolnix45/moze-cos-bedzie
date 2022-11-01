using OpenTK.Graphics.OpenGL4;

namespace otk;

public class Shadery
{
    public readonly int Handle;
    
    public Shadery(string vertexPath, string fragmentPath)
    {
        var VertexShaderSource = File.ReadAllText(vertexPath);
        var FragmentShaderSource = File.ReadAllText(fragmentPath);
        
        var VertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(VertexShader, VertexShaderSource);

        var FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(FragmentShader, FragmentShaderSource);
        
        GL.CompileShader(VertexShader);

        GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out var success0);
        if (success0 == 0)   
        {
            var infoLog = GL.GetShaderInfoLog(VertexShader);
            Console.WriteLine(infoLog);
        }

        GL.CompileShader(FragmentShader);

        GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out var success1);
        if (success1 == 0)
        {
            var infoLog = GL.GetShaderInfoLog(FragmentShader);
            Console.WriteLine(infoLog);
        }
        
        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, VertexShader);
        GL.AttachShader(Handle, FragmentShader);

        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out var success);
        if (success != 0) return;
        {
            var infoLog = GL.GetProgramInfoLog(Handle);
            Console.WriteLine(infoLog);
        }
    }
    
    public void Use()
    {
        GL.UseProgram(Handle);
    }
    
    public int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(Handle, attribName);
    }
    
    public void SetInt(string name, int value)
    {
        var location = GL.GetUniformLocation(Handle, name);

        GL.Uniform1(location, value);
    }
    
    // ReSharper disable once RedundantDefaultMemberInitializer
    private bool disposedValue = false;

    private void Dispose(bool disposing)
    {
        if (disposedValue) return;
        GL.DeleteProgram(Handle);

        disposedValue = true;
    }

    ~Shadery()
    {
        GL.DeleteProgram(Handle);
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}