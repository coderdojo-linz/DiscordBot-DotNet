using System.Collections.Generic;

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
    /// Enumeration to set the facing of the 'player' before rendering fill commands.
    /// </summary>
    public enum MCRenderFacing { North = 0, South = 1, East = 2, West = 3 };

    public class MCMap : List<MCMapRoom>
    {
        MCRenderFacing facing = MCRenderFacing.North;
        int pitch = 0;

        public MCMap() : base() { facing = MCRenderFacing.North; pitch = 0; }
        public MCMap(MCRenderFacing Facing, int Pitch) : base() { facing = Facing; pitch = Pitch; }



        public void InsertMapFill(string BlockType, int FloorWidth, int FloorHeight, int FloorDepth)
        {
            Add(new MCMapRoom(new MCRoomFill(new MCFill(FloorWidth * 6, FloorHeight * 6, FloorDepth * 6, 0, 0, 0, BlockType)), 0, 0, 0));
        }

        public string[] RenderMap()
        {
            List<string> cmds = new List<string>();
            foreach (MCMapRoom room in this)
            {
                cmds.AddRange(room.Render(facing, pitch));

            }

            return cmds.ToArray();
        }
        public string[] RenderMap(MCRenderFacing Facing, int Pitch)
        {

            List<string> cmds = new List<string>();
            foreach (MCMapRoom room in this)
            {
                cmds.AddRange(room.Render(Facing, Pitch));

            }

            return cmds.ToArray();

        }

        public string[] RenderMapAbsolute(int x, int y, int z)
        {

            List<string> cmds = new List<string>();
            foreach (MCMapRoom room in this)
            {
                cmds.AddRange(room.RenderAbsolute(x, y, z));

            }

            return cmds.ToArray();

        }
        public string[] RenderMapAbsolute(MCRenderFacing Facing, int Pitch, int x, int y, int z)
        {

            List<string> cmds = new List<string>();
            foreach (MCMapRoom room in this)
            {
                cmds.AddRange(room.RenderAbsolute(Facing, Pitch, x, y, z));

            }

            return cmds.ToArray();

        }

    }
}
