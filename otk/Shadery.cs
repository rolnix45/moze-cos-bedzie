using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace otk;

public class Shadery
{
    private readonly int Handle;
    private readonly Dictionary<string, int>? _uniformLocations;
    
    public Shadery(string vertexPath, string fragmentPath)
    {
        // load and compile shaders
        var vertexShaderSource = File.ReadAllText(vertexPath);
        var fragmentShaderSource = File.ReadAllText(fragmentPath);

        var vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);

        var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);

        CompileShader(vertexShader);
        CompileShader(fragmentShader);

        Handle = GL.CreateProgram();

        // attach and link shaders
        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);

        LinkProgram(Handle);

        // cleanup
        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);

        // number of active uniforms
        GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
        _uniformLocations = new Dictionary<string, int>();

        for (var i = 0; i < numberOfUniforms; i++)
        {
            // get the name of this uniform,
            var key = GL.GetActiveUniform(Handle, i, out _, out _);

            // get the location,
            var location = GL.GetUniformLocation(Handle, key);

            // and then add it to the dictionary.
            _uniformLocations.Add(key, location);
        }
    }

    private static void CompileShader(int shader)
    {
        // Try to compile the shader
        GL.CompileShader(shader);

        // Check for compilation errors
        GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
        if (code != (int)All.True)
        {
            // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
            var infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
        }
    }

    private static void LinkProgram(int program)
    {
        // We link the program
        GL.LinkProgram(program);

        // Check for linking errors
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
        if (code != (int)All.True)
        {
            // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
            throw new Exception($"Error occurred whilst linking Program({program})");
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
        GL.UseProgram(Handle);
        if (_uniformLocations == null) throw new ArgumentNullException(nameof(_uniformLocations));
        GL.Uniform1(_uniformLocations![name], value);
    }

    public void SetFloat(string name, float value)
    {
        GL.UseProgram(Handle);
        if (_uniformLocations == null) throw new ArgumentNullException(nameof(_uniformLocations));
        GL.Uniform1(_uniformLocations![name], value);
    }

    public void SetMatrix4(string name, Matrix4 data)
    {
        GL.UseProgram(Handle);
        if (_uniformLocations == null) throw new ArgumentNullException(nameof(_uniformLocations));
        GL.UniformMatrix4(_uniformLocations![name], true, ref data);
    }

    public void SetVector3(string name, Vector3 data)
    {
        GL.UseProgram(Handle);
        if (_uniformLocations == null) throw new ArgumentNullException(nameof(_uniformLocations));
        GL.Uniform3(_uniformLocations![name], data);
    }
}