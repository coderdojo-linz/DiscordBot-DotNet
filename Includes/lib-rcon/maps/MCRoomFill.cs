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
    /// A collection of MCFill objects that could resemble a room structure.
    /// </summary>
    public class MCRoomFill : List<MCFill>
    {


        public MCRoomFill() : base() { }
        public MCRoomFill(MCRoomFill Room) : base(Room) { }
        public MCRoomFill(params MCFill[] Fill)
            : base()
        {
            AddRange(Fill);
        }
        public MCRoomFill(MCRoomFill Room, params MCFill[] Fill)
            : base(Room)
        {
            AddRange(Fill);
        }
        public MCRoomFill(params MCRoomFill[] Rooms)
            : base()
        {
            foreach (MCRoomFill rm in Rooms)
                AddRange(rm);
        }


        public string[] Render()
        {
            string[] render = new string[this.Count];
            for (int x = 0; x < this.Count; x++)
                render[x] = this[x].Render(MCRenderFacing.North);

            return render;
        }
        public string[] Render(MCRenderFacing Facing, int Pitch, bool IsAbsolute, params int[] Offset)
        {
            string[] render = new string[this.Count];
            for (int x = 0; x < this.Count; x++)
                render[x] = this[x].Render(Facing, Pitch, IsAbsolute, Offset);

            return render;
        }

        public void OffsetMCFill(int[] voxelOffset)
        {
            foreach (MCFill mcf in this)
            {
                mcf.OffsetFill(voxelOffset);

            }


        }

    }
}
