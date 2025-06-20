// ==================== Game1.cs ====================
// Entry point class, orchestrating the game lifecycle.
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TToolbox
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private VoxelGrid _voxelGrid;
        private Selector _selector;
        private VoxelPainter _painter;
        private InputConfig _inputConfig;
        private VoxelRenderer _renderer;
        private CameraController _camera;

        private SpriteFont _debugFont;
        private Texture2D _gridLineTexture;
        private bool _showGrid = true;

        private bool _isFullscreen = true;
        private Point _previousWindowSize = new Point(800, 600);

        private int _toggleDelayFrames = 0;
        private const int ToggleCooldownFrames = 15;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 600,
                IsFullScreen = false
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            _inputConfig = new InputConfig();
            _voxelGrid = new VoxelGrid(100, 100, 7, -3);
            FillBaseLayer();
            _selector = new Selector(_inputConfig);
            _painter = new VoxelPainter(_voxelGrid, _selector, _inputConfig);
            _camera = new CameraController(_voxelGrid, _selector);
            _selector.CenterOnGrid(_voxelGrid);
            ApplyFullscreen();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _renderer = new VoxelRenderer(_voxelGrid);
            _renderer.LoadContent(GraphicsDevice);
            _debugFont = Content.Load<SpriteFont>("DebugFont");
            _selector.LoadContent(GraphicsDevice);
            _painter.LoadContent(GraphicsDevice);

            _gridLineTexture = new Texture2D(GraphicsDevice, 1, 1);
            _gridLineTexture.SetData(new[] { Color.Black });
        }

        protected override void Update(GameTime gameTime)
        {
            HandleGlobalInput();
            _selector.Update(gameTime);
            _painter.Update(gameTime);
            _camera.Update(GraphicsDevice);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);
            _spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation((int)-_camera.Offset.X, (int)-_camera.Offset.Y, 0));
            _renderer.Draw(_spriteBatch, _selector.CameraZ, 144);
            _painter.Draw(_spriteBatch, 144);
            if (_showGrid) DrawGridLines();
            _selector.Draw(_spriteBatch, _debugFont);
            DrawDebugInfo();
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawGridLines()
        {
            int tileSize = 144;
            int width = _voxelGrid.GridWidth * tileSize;
            int height = _voxelGrid.GridHeight * tileSize;

            for (int x = 0; x <= _voxelGrid.GridWidth; x++)
            {
                _spriteBatch.Draw(_gridLineTexture, new Rectangle(x * tileSize, 0, 3, height), Color.Black);
            }

            for (int y = 0; y <= _voxelGrid.GridHeight; y++)
            {
                _spriteBatch.Draw(_gridLineTexture, new Rectangle(0, y * tileSize, width, 3), Color.Black);
            }
        }

        private void DrawDebugInfo()
        {
            int cursorX = _selector.Position.X;
            int cursorY = _selector.Position.Y;
            int paintZ = _selector.CameraZ - 1;
            int cameraZ = _selector.CameraZ;

            string debugText = $"Cursor: ({cursorX}, {cursorY}, {paintZ})\nCamera Z: {cameraZ} (Paint Z: {paintZ})";
            _spriteBatch.DrawString(_debugFont, debugText, new Vector2(10, 10) + _camera.Offset, Color.White);
        }

        private void HandleGlobalInput()
        {
            var kb = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || kb.IsKeyDown(Keys.Escape))
                Exit();

            if (_toggleDelayFrames == 0)
            {
                if (kb.IsKeyDown(Keys.NumPad1))
                {
                    ToggleFullscreen();
                    _toggleDelayFrames = ToggleCooldownFrames;
                }
                else if (kb.IsKeyDown(Keys.NumPad2))
                {
                    _showGrid = !_showGrid;
                    _toggleDelayFrames = ToggleCooldownFrames;
                }
            }
            else
            {
                _toggleDelayFrames--;
            }
        }

        private void ApplyFullscreen()
        {
            var display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            _graphics.PreferredBackBufferWidth = display.Width;
            _graphics.PreferredBackBufferHeight = display.Height;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            _isFullscreen = true;
        }

        private void ToggleFullscreen()
        {
            if (!_isFullscreen) ApplyFullscreen();
            else
            {
                _graphics.PreferredBackBufferWidth = _previousWindowSize.X;
                _graphics.PreferredBackBufferHeight = _previousWindowSize.Y;
                _graphics.IsFullScreen = false;
                _graphics.ApplyChanges();
                _isFullscreen = false;
            }
        }
        private void FillBaseLayer()
        {
            for (int y = 0; y < 100; y++)
            {
                for (int x = 0; x < 100; x++)
                {
                    for (int z = -3; z <= 0; z++)
                    {
                        _voxelGrid.SetVoxel(x, y, z, new Voxel
                        {
                            Type = VoxelType.Solid,
                            IsFloor = z == 0,
                            FluidDensityModifier = 0f,
                            FluidOpacity = 0f
                        });
                    }
                }
            }
        }
    }
}
