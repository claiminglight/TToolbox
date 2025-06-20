// ==================== VoxelRenderer.cs ====================
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TToolbox
{
    public class VoxelRenderer
    {
        private readonly VoxelGrid _grid;
        private Texture2D _solidTexture;
        private Texture2D _fluidTexture;

        public VoxelRenderer(VoxelGrid grid)
        {
            _grid = grid;
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            _solidTexture = new Texture2D(graphicsDevice, 1, 1);
            _solidTexture.SetData(new[] { Color.Green });

            _fluidTexture = new Texture2D(graphicsDevice, 1, 1);
            _fluidTexture.SetData(new[] { Color.Blue });
        }

        public void Draw(SpriteBatch spriteBatch, int cameraZ, int tileSize)
        {
            for (int y = 0; y < _grid.GridHeight; y++)
            {
                for (int x = 0; x < _grid.GridWidth; x++)
                {
                    for (int z = _grid.MinZ + _grid.Depth; z >= _grid.MinZ; z--)
                    {
                        Voxel voxel = _grid.GetVoxel(x, y, z);
                        if (voxel.Type == VoxelType.Void) continue;

                        if (z == cameraZ - 1)
                        {
                            DrawVoxel(spriteBatch, voxel, x, y, tileSize, Color.White);
                            break;
                        }
                        else if (z == cameraZ)
                        {
                            if (voxel.Type == VoxelType.Solid)
                            {
                                DrawVoxel(spriteBatch, voxel, x, y, tileSize, Color.SaddleBrown);
                            }
                            else if (voxel.Type == VoxelType.Fluid)
                            {
                                float opacity = MathHelper.Clamp(voxel.FluidOpacity, 0.2f, 1f);
                                DrawVoxel(spriteBatch, voxel, x, y, tileSize, Color.Blue * opacity);
                            }
                            break;
                        }
                        else if (z < cameraZ - 1)
                        {
                            float shading = MathHelper.Clamp(1f - ((cameraZ - 1 - z) * 0.2f), 0f, 1f);
                            DrawVoxel(spriteBatch, voxel, x, y, tileSize, Color.White * shading);
                            break;
                        }
                    }
                }
            }
        }

        private void DrawVoxel(SpriteBatch spriteBatch, Voxel voxel, int x, int y, int tileSize, Color color)
        {
            Texture2D texture = voxel.Type == VoxelType.Fluid ? _fluidTexture : _solidTexture;
            spriteBatch.Draw(texture, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), color);
        }
    }
}
