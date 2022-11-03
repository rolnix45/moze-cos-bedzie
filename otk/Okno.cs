using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace otk;

public class Okno : GameWindow
{
#pragma warning disable CS8618
    public Okno(int width, int height, string title) : base(GameWindowSettings.Default,
        new NativeWindowSettings { Size = (width, height), Title = title })
    {
    }
#pragma warning restore CS8618

    private readonly float[] _vertices =
    {
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 0.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 0.0f,

        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 1.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 1.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 1.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,

        -0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
        -0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, 1.0f, 0.0f,

        0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f,

        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
        0.5f, -0.5f, -0.5f, 1.0f, 1.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
        0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
        -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,

        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f,
        0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
        0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
        -0.5f, 0.5f, 0.5f, 0.0f, 0.0f,
        -0.5f, 0.5f, -0.5f, 0.0f, 1.0f
    };

    private readonly uint[] _indices =
    {
        0, 1, 3, // first triangle
        1, 2, 3 // second triangle
    };

    // buffers and vertex arrays
    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private int _elementBufferObject;

    // textures and shaders
    private Shadery _shader;
    private Textury _texture0;
    private Textury _texture1;

    private Kamera _cameraObject;
    private bool _firstMove = true;
    private Vector2 _lastPosition;

    private double _elapsed;

    private const float mouseSensitivity = 0.2f;
    private const float cameraSpeed = 1.5f;
    
    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.0f, 0.85f, 0.9f, 1.0f);

        GL.Enable(EnableCap.DepthTest);

        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
            BufferUsageHint.StaticDraw);

        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * _indices.Length, _indices,
            BufferUsageHint.StaticDraw);

        _shader = new Shadery("../../shaders/vert.glsl", "../../shaders/frag.glsl");
        _shader.Use();

        var vertexLocation = _shader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false,
            5 * sizeof(float), 0);

        var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false,
            5 * sizeof(float), 3 * sizeof(float));

        _texture0 = Textury.LoadFromFile("../../assets/texture.png");
        _texture0.Use(TextureUnit.Texture0);

        _texture1 = Textury.LoadFromFile("../../assets/samohud.png");
        _texture1.Use(TextureUnit.Texture1);

        _shader.SetInt("texture0", 0);
        _shader.SetInt("texture1", 1);

        _cameraObject = new Kamera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);
        CursorState = CursorState.Grabbed;

    }
    
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        _elapsed += 4.0f * e.Time;

        // clear color and depth buffer
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.BindVertexArray(_vertexArrayObject);
        
        // activate shader and texture
        _texture0.Use(TextureUnit.Texture0);
        _texture1.Use(TextureUnit.Texture1);
        _shader.Use();

        // setting matrixes
        var model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_elapsed * 10f));
        _shader.SetMatrix4("model", model);
        _shader.SetMatrix4("view", _cameraObject.GetViewMatrix());
        _shader.SetMatrix4("projection", _cameraObject.GetProjectionMatrix());

        // render our triangle or rectangle or cube
        // GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

        // swap buffers
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (!IsFocused) return;

        var input = KeyboardState;

        // close app on escape key
        if (input.IsKeyDown(Keys.Escape)) Close();

        // camera controlls
        if (input.IsKeyDown(Keys.W)) _cameraObject.Position += _cameraObject.Front * cameraSpeed * (float)e.Time;

        if (input.IsKeyDown(Keys.S)) _cameraObject.Position -= _cameraObject.Front * cameraSpeed * (float)e.Time;

        if (input.IsKeyDown(Keys.A)) _cameraObject.Position -= _cameraObject.Right * cameraSpeed * (float)e.Time;

        if (input.IsKeyDown(Keys.D)) _cameraObject.Position += _cameraObject.Right * cameraSpeed * (float)e.Time;

        if (input.IsKeyDown(Keys.Space)) _cameraObject.Position += _cameraObject.Up * cameraSpeed * (float)e.Time;

        if (input.IsKeyDown(Keys.LeftControl)) _cameraObject.Position -= _cameraObject.Up * cameraSpeed * (float)e.Time;

        if (input.IsKeyDown(Keys.KeyPadAdd)) _cameraObject.Fov += 5f;

        if (input.IsKeyDown(Keys.KeyPadSubtract)) _cameraObject.Fov -= 5f;

        // get mouse state
        var mouseState = MouseState;

        if (_firstMove)
        {
            _lastPosition = new Vector2(mouseState.X, mouseState.Y);
            _firstMove = false;
        }
        else
        {
            // calculate camera offset
            var deltaX = mouseState.X - _lastPosition.X;
            var deltaY = mouseState.Y - _lastPosition.Y;
            _lastPosition = new Vector2(mouseState.X, mouseState.Y);

            // move camera
            _cameraObject.Yaw += deltaX * mouseSensitivity;
            _cameraObject.Pitch -= deltaY * mouseSensitivity;
        }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }
}
