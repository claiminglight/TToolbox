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

        private TileGrid _grid;
        private Selector _selector;
        private TilePainter _painter;
        private InputConfig _inputConfig;
        private GridConfig _gridConfig;
        private CameraController _camera;

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
            _gridConfig = new GridConfig();
            _grid = new TileGrid(_gridConfig);
            _selector = new Selector(_grid, _inputConfig);
            _painter = new TilePainter(_grid, _selector, _inputConfig);
            _camera = new CameraController(_grid, _selector);
            _selector.CenterOnGrid();
            ApplyFullscreen();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _grid.LoadContent(GraphicsDevice);
            _selector.LoadContent(GraphicsDevice);
            _painter.LoadContent(GraphicsDevice);
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
            _grid.Draw(_spriteBatch);
            _painter.Draw(_spriteBatch);
            _selector.Draw(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
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
                    _grid.ToggleGridLines();
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
    }
}