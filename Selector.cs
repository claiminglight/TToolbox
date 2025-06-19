// ==================== Selector.cs ====================
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TToolbox
{
    /// <summary>
    /// Handles cursor movement and positioning logic within the tile grid.
    /// </summary>
    public class Selector
    {
        private Point _position;
        private readonly TileGrid _grid;
        private Texture2D _highlightTexture;
        private readonly InputConfig _inputConfig;

        private double _moveCooldown = 0;
        private double _repeatDelay = 100;

        public Point Position => _position;

        public Selector(TileGrid grid, InputConfig config)
        {
            _grid = grid;
            _inputConfig = config;
            _position = new Point(0, 0);
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            _highlightTexture = new Texture2D(graphicsDevice, 1, 1);
            _highlightTexture.SetData(new[] { Color.White });
        }

        public void CenterOnGrid()
        {
            _position.X = _grid.GridWidth / 2;
            _position.Y = _grid.GridHeight / 2;
        }

        public void Update(GameTime gameTime)
        {
            var kb = Keyboard.GetState();
            var gp = GamePad.GetState(PlayerIndex.One);

            double elapsed = gameTime.ElapsedGameTime.TotalMilliseconds;
            _moveCooldown -= elapsed;

            Point direction = Point.Zero;

            if (kb.IsKeyDown(_inputConfig.MoveRight)) direction.X = 1;
            if (kb.IsKeyDown(_inputConfig.MoveLeft)) direction.X = -1;
            if (kb.IsKeyDown(_inputConfig.MoveDown)) direction.Y = 1;
            if (kb.IsKeyDown(_inputConfig.MoveUp)) direction.Y = -1;

            if (_inputConfig.GamepadMovement == GamepadMode.DPad)
            {
                if (gp.DPad.Right == ButtonState.Pressed) direction.X = 1;
                if (gp.DPad.Left == ButtonState.Pressed) direction.X = -1;
                if (gp.DPad.Down == ButtonState.Pressed) direction.Y = 1;
                if (gp.DPad.Up == ButtonState.Pressed) direction.Y = -1;
            }
            else if (_inputConfig.GamepadMovement == GamepadMode.LeftStick)
            {
                if (gp.ThumbSticks.Left.X > 0.5f) direction.X = 1;
                else if (gp.ThumbSticks.Left.X < -0.5f) direction.X = -1;
                if (gp.ThumbSticks.Left.Y < -0.5f) direction.Y = 1;
                else if (gp.ThumbSticks.Left.Y > 0.5f) direction.Y = -1;
            }

            if (direction != Point.Zero && _moveCooldown <= 0)
            {
                Point oldPosition = _position;
                _position.X = MathHelper.Clamp(_position.X + direction.X, 0, _grid.GridWidth - 1);
                _position.Y = MathHelper.Clamp(_position.Y + direction.Y, 0, _grid.GridHeight - 1);
                if (_position != oldPosition)
                    _moveCooldown = _repeatDelay;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var rect = new Rectangle(
                _position.X * _grid.TileSize,
                _position.Y * _grid.TileSize,
                _grid.TileSize,
                _grid.TileSize);
            spriteBatch.Draw(_highlightTexture, rect, Color.Yellow * 0.5f);
        }
    }
}
