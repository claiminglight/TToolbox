using Microsoft.Xna.Framework;

namespace TToolbox
{
    /// <summary>
    /// Handles deletion of voxels based on visible Z level logic.
    /// </summary>
    public class VoxelDeleter
    {
        private readonly VoxelGrid _voxels;
        private readonly Selector _selector;

        public VoxelDeleter(VoxelGrid grid, Selector selector)
        {
            _voxels = grid;
            _selector = selector;
        }

        /// <summary>
        /// Deletes the voxel the user sees, either on CameraZ or CameraZ - 1.
        /// </summary>
        public void DeleteVoxel(Point position)
        {
            int cameraZ = _selector.CameraZ;

            // Check if there's a visible voxel at the camera Z level (wall)
            var topVoxel = _voxels.GetVoxel(position.X, position.Y, cameraZ);
            if (topVoxel.Type != VoxelType.Void)
            {
                _voxels.SetVoxel(position.X, position.Y, cameraZ, new Voxel { Type = VoxelType.Void });
                return;
            }

            // Otherwise delete at cameraZ - 1 (floor)
            var baseVoxel = _voxels.GetVoxel(position.X, position.Y, cameraZ - 1);
            if (baseVoxel.Type != VoxelType.Void)
            {
                _voxels.SetVoxel(position.X, position.Y, cameraZ - 1, new Voxel { Type = VoxelType.Void });
            }
        }
    }
}
