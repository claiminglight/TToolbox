// ==================== GameServices.cs ====================
using Microsoft.Xna.Framework.Graphics;

namespace TToolbox
{
    /// <summary>
    /// Provides globally accessible services such as GraphicsDevice.
    /// </summary>
    public static class GameServices
    {
        private static GraphicsDevice _graphicsDevice;
        public static GraphicsDevice GraphicsDevice
        {
            get => _graphicsDevice;
            set => _graphicsDevice ??= value;
        }
    }
}
