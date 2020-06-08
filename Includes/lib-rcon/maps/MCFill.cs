using System;

/// <summary>
/// A way to programmatically control and track changes made by the /fill command in minecraft.
/// 
/// Either the player is the point of origin, or all coordinates are absolute.  A set of tri-ordinates (Voxel - volume pixel) x,y,z are 
/// used as points of reference.  All rendered fills are cubes of various size.  A collection of fills build a 'room' and
/// a collection of 'rooms' make a map.  The template system that supplies default fills use cube space that is 6x6x6 blocks.
/// All fill templates are based off a model where the fill cubes are rendered in front of the player.  So starting with
/// xyz of 0,0,1 is a block that is on the same level the player is standing, 1 unit in front.  The default render model renders
/// the X axis with + right, and - left.  Y axis with + up and - down.  Z axis + front of player, - behind player.
/// 
/// Most render functions request a facing and pitch.  Pitch is assumed to be level with ground, but can be set straight up or straight down.
/// Facing rotates the model as if the player was facing the direction in game.  In minecraft, North follows -Z, South follows
/// +Z, East follows +X, and West follows -X.  The fill voxels will be translated to the minecraft facing and coodinate system at rendering.
/// All fill primatives are expected to fit into a 6x6x6 cube, so the Mapping portion of the system will align rooms up correctly.
/// </summary>
namespace LibMCRcon.Maps
{
    /// <summary>
    /// Defines a fill command, including what tile block entity should be used to fill and 2 sets of X,Y,Z voxels (3-d ordinates)
    /// that create a cube.
    /// 
    /// Fills are cubes.  These cubes us a common coordinate system from the perspective of a player
    /// in Minecraft standing on origin.  For these fill primatives, the +Z axis goes out forward from
    /// the player, +X strafes right.  +Y goes above the player.  For creating these cubes, player
    /// facing is not considered. 
    /// -Z,-X, and -Y are the opposite. 
    /// 
    /// The voxels can either be offsets from player origin or absolute coordinates.
    /// </summary>
    public class MCFill
    {
        public string BlockType { get; set; }

        int[] p1 = new int[3];
        int[] p2 = new int[3];

        /// <summary>
        /// Permanently offsets the fill cube.
        /// </summary>
        /// <param name="offsetVoxel">An array representing 3 axis of offset. X,Y,Z [0..2]</param>
        public void OffsetFill(int[] offsetVoxel)
        {
            for (int x = 0; x < 3; x++)
            {

                p1[x] += offsetVoxel[x];
                p2[x] += offsetVoxel[x];

            }

        }

        /// <summary>
        /// Render the cube, using relative coodinates of the player executing the command.
        /// </summary>
        /// <param name="Facing">Which compass facing to render the cube, from the player location, level to ground.</param>
        /// <returns></returns>
        public string Render(MCRenderFacing Facing)
        {
            return Render(Facing, 0, false, 0, 0, 0);
        }
        /// <summary>
        /// Render the cube, using relative coodinates of the player executing the command.
        /// </summary>
        /// <param name="Facing">Compass direction to render centered on player location.</param>
        /// <param name="Pitch">Render the cube level (0), looking up (1), looking down (-1)</param>
        /// <param name="IsAbsolute">The coordinates stored for the cube are either relative to the player or
        /// absolute world coordinates.</param>
        /// <param name="offset">If there should be an offset computed first, does not change the orginal cube.</param>
        /// <returns></returns>
        public string Render(MCRenderFacing Facing, int Pitch, bool IsAbsolute, params int[] offset)
        {
            int rx = 0, ry = 1, rz = 0;
            int sx = 0, sy = 0, sz = 0;

            int[] r1 = new int[3];
            int[] r2 = new int[3];

            switch (Facing)
            {
                case MCRenderFacing.North:

                    switch (Pitch)
                    {

                        case -1:

                            rx = 0; ry = 2; rz = 1;
                            sx = 1; sy = -1; sz = -1;

                            break;
                        case 1:

                            rx = 0; ry = 2; rz = 1;
                            sx = -1; sy = 1; sz = 1;
                            break;

                        default:
                            rx = 0; ry = 1; rz = 2;
                            sx = 1; sy = 1; sz = -1;
                            break;
                    }
                    break;
                case MCRenderFacing.South:
                    switch (Pitch)
                    {
                        case -1:
                            rx = 0; ry = 2; rz = 1;
                            sx = -1; sy = 1; sz = -1;
                            break;
                        case 1:
                            rx = 0; ry = 2; rz = 1;
                            sx = 1; sy = -1; sz = 1;
                            break;
                        default:
                            rx = 0; ry = 1; rz = 2;
                            sx = -1; sy = 1; sz = 1;
                            break;
                    }
                    break;

                case MCRenderFacing.East:
                    switch (Pitch)
                    {
                        case -1:
                            rx = 2; ry = 0; rz = 1;
                            sx = 1; sy = 1; sz = -1;
                            break;
                        case 1:
                            rx = 2; ry = 0; rz = 1;
                            sx = -1; sy = -1; sz = 1;
                            break;
                        default:
                            rx = 2; ry = 1; rz = 0;
                            sx = 1; sy = 1; sz = 1;
                            break;
                    }
                    break;
                case MCRenderFacing.West:
                    switch (Pitch)
                    {
                        case -1:
                            rx = 2; ry = 0; rz = 1;
                            sx = -1; sy = -1; sz = -1;
                            break;
                        case 1:
                            rx = 2; ry = 0; rz = 1;
                            sx = 1; sy = 1; sz = 1;
                            break;
                        default:
                            rx = 2; ry = 1; rz = 0;
                            sx = -1; sy = 1; sz = -1;
                            break;
                    }
                    break;



            }

            r1[0] = sx * (offset[rx] + p1[rx]);
            r1[1] = sy * (offset[ry] + p1[ry]);
            r1[2] = sz * (offset[rz] + p1[rz]);
            r2[0] = sx * (offset[rx] + p2[rx]);
            r2[1] = sy * (offset[ry] + p2[ry]);
            r2[2] = sz * (offset[rz] + p2[rz]);

            if (IsAbsolute == true)
                return string.Format(@"fill {1} {2} {3} {4} {5} {6} {0}", BlockType, r1[0], r1[1], r1[2], r2[0], r2[1], r2[2]);
            else
                return string.Format(@"fill ~{1} ~{2} ~{3} ~{4} ~{5} ~{6} {0}", BlockType, r1[0], r1[1], r1[2], r2[0], r2[1], r2[2]);


        }


        /// <summary>
        /// Sets the first point in the fill cube.
        /// </summary>
        /// <param name="x">X axis relative or absolute.</param>
        /// <param name="y">Y axis relative or absolute.</param>
        /// <param name="z">Z axis relative or absolute.</param>
        public void SetPoint1(int x, int y, int z)
        {
            p1[0] = x;
            p1[1] = y;
            p1[2] = z;
        }
        /// <summary>
        /// Sets the second point in the fill cube.
        /// </summary>
        /// <param name="x">X axis relative or absolute.</param>
        /// <param name="y">Y axis relative or absolute.</param>
        /// <param name="z">Z axis relative or absolute.</param>
        public void SetPoint2(int x, int y, int z)
        {
            p2[0] = x;
            p2[1] = y;
            p2[2] = z;
        }

        /// <summary>
        /// Default fill type is stone.
        /// </summary>
        public MCFill()
        {
            BlockType = "minecraft:stone";
        }

        /// <summary>
        /// Set the entire fill cube up in one setting.
        /// </summary>
        /// <param name="BlockType">Minecraft verbose item in minecraft:[block entity id].</param>
        /// <param name="x1">1st X axis</param>
        /// <param name="y1">1st Y axis</param>
        /// <param name="z1">1st Z axis</param>
        /// <param name="xs">X axis length</param>
        /// <param name="ys">Y axis length</param>
        /// <param name="zs">Z axis length</param>
        public MCFill(int xs, int ys, int zs, int x1, int y1, int z1, string BlockType)
        {
            this.BlockType = BlockType;
            Reset(x1, y1, z1, xs, ys, zs);

        }

        public MCFill(int xs, int ys, int zs, string BlockType)
        {
            this.BlockType = BlockType;
            Reset(0, 0, 0, xs, ys, zs);
        }

        public int[] size
        {
            get
            {
                int[] sxyz = new int[3];

                for (int x = 0; x < 3; x++)
                    sxyz[x] = ((int)Math.Abs(p2[x] - p1[x])) + 1;

                return sxyz;
            }
        }

        public MCFill OffsetClone(int x, int y, int z, string BlockType)
        {
            int[] s = size;
            return new MCFill(s[0], s[1], s[2], x, y, z, BlockType);

        }

        public MCFill Clone(string BlockType)
        {
            int[] s = size;
            return new MCFill(s[0], s[1], s[2], p1[0], p1[1], p1[2], BlockType);
        }

        public void Reset(int x, int y, int z)
        {
            int[] s = size;
            Reset(x, y, z, s[0], s[1], s[2]);
        }

        public void Reset(int x1, int y1, int z1, int xs, int ys, int zs)
        {

            if (x1 < 0)
            {
                p1[0] = x1 - (xs - 1);
                p2[0] = x1;
            }
            else
            {
                p1[0] = x1;
                p2[0] = x1 + (xs - 1);
            }

            if (y1 < 0)
            {
                p1[1] = y1 - (ys - 1);
                p2[1] = y1;
            }
            else
            {
                p1[1] = y1;
                p2[1] = y1 + (ys - 1);
            }

            if (z1 < 0)
            {
                p1[2] = z1 - (zs - 1);
                p2[2] = z1;
            }
            else
            {
                p1[2] = z1;
                p2[2] = z1 + (zs - 1);
            }


        }



    }
}
