// ==================== CameraController.cs ====================
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TToolbox
{
    /// <summary>
    /// Handles camera scrolling logic based on the selector's position and the viewport.
    /// </summary>
    public class CameraController
    {
        private readonly VoxelGrid _grid;
        private readonly Selector _selector;

        private Vector2 _offset = Vector2.Zero;
        public Vector2 Offset => _offset;

        public CameraController(VoxelGrid grid, Selector selector)
        {
            _grid = grid;
            _selector = selector;
        }

        public void Update(GraphicsDevice graphicsDevice)
        {
            Point pos = _selector.Position;
            int tileSize = 144;
            int viewTilesX = graphicsDevice.Viewport.Width / tileSize;
            int viewTilesY = graphicsDevice.Viewport.Height / tileSize;

            float marginPercent = 0.1f; // Later move to user settings
            int marginX = (int)(viewTilesX * marginPercent);
            int marginY = (int)(viewTilesY * marginPercent);

            int viewStartX = (int)(_offset.X / tileSize);
            int viewStartY = (int)(_offset.Y / tileSize);

            int viewEndX = viewStartX + viewTilesX;
            int viewEndY = viewStartY + viewTilesY;

            if (pos.X - viewStartX < marginX && viewStartX > 0)
                _offset.X = Math.Max(0, _offset.X - tileSize);
            else if (viewEndX - pos.X <= marginX && viewEndX < _grid.GridWidth)
                _offset.X = Math.Min((_grid.GridWidth - viewTilesX) * tileSize, _offset.X + tileSize);

            if (pos.Y - viewStartY < marginY && viewStartY > 0)
                _offset.Y = Math.Max(0, _offset.Y - tileSize);
            else if (viewEndY - pos.Y <= marginY && viewEndY < _grid.GridHeight)
                _offset.Y = Math.Min((_grid.GridHeight - viewTilesY) * tileSize, _offset.Y + tileSize);
        }
    }
}
