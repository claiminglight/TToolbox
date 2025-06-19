// ==================== TileGrid.cs ====================
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TToolbox
{
    /// <summary>
    /// Manages the visual tile grid and painted tile data. Handles grid rendering and painting.
    /// </summary>
    public class TileGrid
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _tileSize;
        private Texture2D _tileTexture;
        private readonly Color _tileColor = Color.DarkSlateGray;
        private bool _showGridLines = true;
        private HashSet<Point> _painted = new HashSet<Point>();

        public TileGrid(GridConfig config)
        {
            _width = config.GridWidth;
            _height = config.GridHeight;
            _tileSize = config.TileSize;
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            _tileTexture = new Texture2D(graphicsDevice, 1, 1);
            _tileTexture.SetData(new[] { Color.White });
        }

        public void ToggleGridLines()
        {
            _showGridLines = !_showGridLines;
        }

        public void PaintTile(Point point)
        {
            if (point.X >= 0 && point.X < _width && point.Y >= 0 && point.Y < _height)
            {
                _painted.Add(point);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    int xPos = x * _tileSize;
                    int yPos = y * _tileSize;
                    var rect = new Rectangle(xPos, yPos, _tileSize, _tileSize);

                    Color fill = _painted.Contains(new Point(x, y)) ? Color.Orange : _tileColor;
                    spriteBatch.Draw(_tileTexture, rect, fill);

                    if (_showGridLines)
                    {
                        var lineColor = Color.Black * 0.4f;
                        int lineThickness = 3;
                        spriteBatch.Draw(_tileTexture, new Rectangle((int)MathF.Round(xPos), (int)MathF.Round(yPos), _tileSize, lineThickness), lineColor);
                        spriteBatch.Draw(_tileTexture, new Rectangle((int)MathF.Round(xPos), (int)MathF.Round(yPos), lineThickness, _tileSize), lineColor);
                    }
                }
            }
        }

        public int TileSize => _tileSize;
        public int GridWidth => _width;
        public int GridHeight => _height;
    }
}
