using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TToolbox
{
    public class TilePainter
    {
        private readonly TileGrid _grid;
        private readonly Selector _selector;
        private readonly InputConfig _inputConfig;
        private Texture2D _paintTexture;

        private bool _isSelecting = false;
        private bool _selectorHeld = false;
        private Point? _selectionStart = null;
        private Point _selectionEnd;
        private HashSet<Point> _selectedTiles = new HashSet<Point>();

        public TilePainter(TileGrid grid, Selector selector, InputConfig config)
        {
            _grid = grid;
            _selector = selector;
            _inputConfig = config;
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            _paintTexture = new Texture2D(graphicsDevice, 1, 1);
            _paintTexture.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            var kb = Keyboard.GetState();
            var gp = GamePad.GetState(PlayerIndex.One);

            bool paintPressed = kb.IsKeyDown(Keys.P) || gp.Buttons.A == ButtonState.Pressed;
            bool selectorPressed = kb.IsKeyDown(Keys.O) || gp.Buttons.X == ButtonState.Pressed;

            Point cursor = _selector.Position;

            if (selectorPressed && !_selectorHeld)
            {
                _selectorHeld = true;
                if (!_isSelecting)
                {
                    _isSelecting = true;
                    _selectionStart = cursor;
                    _selectionEnd = cursor;
                }
                else
                {
                    _isSelecting = false;
                    _selectionStart = null;
                    _selectedTiles.Clear();
                }
            }
            else if (!selectorPressed)
            {
                _selectorHeld = false;
            }

            if (_isSelecting && _selectionStart.HasValue)
            {
                _selectionEnd = cursor;
                _selectedTiles = GenerateSelection(_selectionStart.Value, _selectionEnd);
            }

            if (!_isSelecting && paintPressed)
            {
                _grid.PaintTile(cursor);
            }
            else if (_isSelecting && paintPressed)
            {
                foreach (var pt in _selectedTiles)
                    _grid.PaintTile(pt);
                _isSelecting = false;
                _selectionStart = null;
                _selectedTiles.Clear();
            }
        }

        private HashSet<Point> GenerateSelection(Point start, Point end)
        {
            var selected = new HashSet<Point>();
            int xMin = MathHelper.Clamp(Math.Min(start.X, end.X), 0, _grid.GridWidth - 1);
            int yMin = MathHelper.Clamp(Math.Min(start.Y, end.Y), 0, _grid.GridHeight - 1);
            int xMax = MathHelper.Clamp(Math.Max(start.X, end.X), 0, _grid.GridWidth - 1);
            int yMax = MathHelper.Clamp(Math.Max(start.Y, end.Y), 0, _grid.GridHeight - 1);

            for (int y = yMin; y <= yMax; y++)
                for (int x = xMin; x <= xMax; x++)
                    selected.Add(new Point(x, y));

            return selected;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_isSelecting)
            {
                foreach (var pt in _selectedTiles)
                {
                    var rect = new Rectangle(pt.X * _grid.TileSize, pt.Y * _grid.TileSize, _grid.TileSize, _grid.TileSize);
                    spriteBatch.Draw(_paintTexture, rect, Color.LightBlue * 0.3f);
                }
            }
        }
    }
}
