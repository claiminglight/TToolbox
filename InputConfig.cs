using Microsoft.Xna.Framework.Input;

namespace TToolbox
{
    public enum GamepadMode
    {
        DPad,
        LeftStick
    }

    public class InputConfig
    {
        public Keys MoveUp = Keys.Up;
        public Keys MoveDown = Keys.Down;
        public Keys MoveLeft = Keys.Left;
        public Keys MoveRight = Keys.Right;

        public GamepadMode GamepadMovement = GamepadMode.DPad;
    }
}
