using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TToolbox
{
    public class VoxelPainter
    {
        private readonly VoxelGrid _voxels;
        private readonly Selector _selector;
        private readonly InputConfig _inputConfig;
        private Texture2D _highlightTexture;

        private bool _isSelecting = false;
        private bool _selectorHeld = false;
        private Point? _selectionStart = null;
        private Point _selectionEnd;
        private HashSet<Point> _selectedTiles = new HashSet<Point>();
        private int _activeZ;

        public VoxelPainter(VoxelGrid grid, Selector selector, InputConfig config)
        {
            _voxels = grid;
            _selector = selector;
            _inputConfig = config;
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            _highlightTexture = new Texture2D(graphicsDevice, 1, 1);
            _highlightTexture.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            var kb = Keyboard.GetState();
            var gp = GamePad.GetState(PlayerIndex.One);

            bool paintPressed = kb.IsKeyDown(Keys.P) || gp.Buttons.A == ButtonState.Pressed;
            bool deletePressed = kb.IsKeyDown(Keys.Delete) || gp.Buttons.B == ButtonState.Pressed;
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
                _activeZ = _selector.CameraZ - 1;
            }

            if (!_isSelecting && (paintPressed || deletePressed))
            {
                _activeZ = _selector.CameraZ - 1;
                if (deletePressed) DeleteVoxel(cursor);
                else PaintVoxel(cursor);
            }
            else if (_isSelecting && (paintPressed || deletePressed))
            {
                foreach (var pt in _selectedTiles)
                {
                    if (deletePressed) DeleteVoxel(pt);
                    else PaintVoxel(pt);
                }
                _isSelecting = false;
                _selectionStart = null;
                _selectedTiles.Clear();
            }
        }

        private HashSet<Point> GenerateSelection(Point start, Point end)
        {
            var selected = new HashSet<Point>();
            int xMin = Math.Min(start.X, end.X);
            int yMin = Math.Min(start.Y, end.Y);
            int xMax = Math.Max(start.X, end.X);
            int yMax = Math.Max(start.Y, end.Y);

            for (int y = yMin; y <= yMax; y++)
                for (int x = xMin; x <= xMax; x++)
                    selected.Add(new Point(x, y));

            return selected;
        }

        private void PaintVoxel(Point position)
        {
            var voxel = new Voxel
            {
                Type = VoxelType.Solid,
                IsFloor = true,
                FluidDensityModifier = 1.0f,
                FluidOpacity = 0f
            };
            _voxels.SetVoxel(position.X, position.Y, _activeZ, voxel);
        }

        private void DeleteVoxel(Point position)
        {
            if (_voxels.GetVoxel(position.X, position.Y, _activeZ).Type != VoxelType.Void)
            {
                _voxels.SetVoxel(position.X, position.Y, _activeZ, new Voxel { Type = VoxelType.Void });
            }
        }

        public void Draw(SpriteBatch spriteBatch, int tileSize)
        {
            if (_isSelecting)
            {
                foreach (var pt in _selectedTiles)
                {
                    var color = (_selector.CameraZ - 1) == _activeZ ? Color.Red * 0.4f : Color.White * 0.05f;
                    var rect = new Rectangle(pt.X * tileSize, pt.Y * tileSize, tileSize, tileSize);
                    spriteBatch.Draw(_highlightTexture, rect, color);
                }
            }
        }
    }
}
