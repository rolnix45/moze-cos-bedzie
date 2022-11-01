using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace otk;

public class Okno : GameWindow
{
    public Okno(int width, int height, string title) : base(GameWindowSettings.Default,
        new NativeWindowSettings { Size = (width, height), Title = title }) { }

    private readonly float[] vertices =
    {
         //position          //texture coordinates
         0.5f,  0.5f, 0.0f,  1.0f, 1.0f, // top right
         0.5f, -0.5f, 0.0f,  1.0f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f,  0.0f, 0.0f, // bottom left
        -0.5f,  0.5f, 0.0f,  0.0f, 1.0f  // top left
    };

    private readonly uint[] indices = 
    { 
        0, 1, 3,   // first triangle
        1, 2, 3    // second triangle
    };
    
    private int VertexBufferObject;
    private int VertexArrayObject;
    private int ElementBufferObject;
    
    private Shadery? shader;
    private Textury? texture;
    
    private readonly Stopwatch _stopwatch = new Stopwatch();
    
    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(.0f, .8f, 1.0f, 1.0f);
        _stopwatch.Start();

        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);
        
        VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices,
            BufferUsageHint.StaticDraw);
        
        ElementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices,
            BufferUsageHint.StaticDraw);
     
        shader = new Shadery("../../shaders/vert.glsl","../../shaders/frag.glsl");
        shader.Use();
        
        var vertexLocation = shader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false
            ,5 * sizeof(float), 0);
        
        var texCoordLocation = shader.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false
            ,5 * sizeof(float), 3 * sizeof(float));
        
        texture = Textury.LoadFromFile("../../assets/texture.png");
        texture.Use(TextureUnit.Texture0);
    }
        
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        // clear color buffer
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        GL.BindVertexArray(VertexArrayObject);

        // activate shader and texture
        texture?.Use(TextureUnit.Texture0);
        shader?.Use();
        
        // update uniform color
        var timeValue = _stopwatch.Elapsed.TotalSeconds;
        var greenValue = (float)Math.Sin(timeValue) / (2.0f + 0.5f);
        var vertexColorLocation = GL.GetUniformLocation(shader!.Handle, "ourColor");
        GL.Uniform4(vertexColorLocation, 0.0f, greenValue, 0.0f, 1.0f);
        
        // render our triangle or rectangle
        GL.BindVertexArray(VertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

        // swap buffers
        SwapBuffers();
    }
        
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        
        // close app on escape key
        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }
}