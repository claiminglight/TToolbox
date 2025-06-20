// ==================== VoxelGrid.cs ====================
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TToolbox
{
    public class VoxelGrid
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _depth;
        private readonly int _minZ;
        private readonly Voxel[,,] _voxels;

        public int GridWidth => _width;
        public int GridHeight => _height;
        public int MinZ { get; } = -3;
        public int Depth = 7;

        public VoxelGrid(int width, int height, int depth, int minZ)
        {
            _width = width;
            _height = height;
            _depth = depth;
            _minZ = minZ;
            _voxels = new Voxel[width, height, depth];
        }

        public bool IsValidCoordinate(int x, int y, int z)
        {
            return x >= 0 && y >= 0 && z >= MinZ && x < _width && y < _height && z <= (MinZ + _depth - 1);
        }

        public Voxel GetVoxel(int x, int y, int z)
        {
            return IsValidCoordinate(x, y, z) ? _voxels[x, y, z - MinZ] : default;
        }

        public void SetVoxel(int x, int y, int z, Voxel voxel)
        {
            if (IsValidCoordinate(x, y, z))
                _voxels[x, y, z - MinZ] = voxel;
        }

        public IEnumerable<Point3> GetAdjacentPositions(int x, int y, int z)
        {
            var offsets = new[] {
                new Point3(1, 0, 0), new Point3(-1, 0, 0),
                new Point3(0, 1, 0), new Point3(0, -1, 0),
                new Point3(0, 0, 1), new Point3(0, 0, -1)
            };

            foreach (var o in offsets)
            {
                int nx = x + o.X, ny = y + o.Y, nz = z + o.Z;
                if (IsValidCoordinate(nx, ny, nz))
                    yield return new Point3(nx, ny, nz);
            }
        }
    }

    public struct Point3
    {
        public int X, Y, Z;

        public Point3(int x, int y, int z)
        {
            X = x; Y = y; Z = z;
        }
    }
}
