// ==================== Voxel.cs ====================
namespace TToolbox
{
    public enum VoxelType
    {
        Void,
        Solid,
        Fluid
    }

    public struct Voxel
    {
        public VoxelType Type;
        public bool IsFloor;
        public bool IsWall;
        public bool IsCeiling;
        public float FluidOpacity;
        public float FluidDensityModifier;
    }
}