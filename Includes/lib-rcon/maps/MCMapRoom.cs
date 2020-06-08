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
    public class MCMapRoom
    {

        MCRenderFacing defaultFacing = MCRenderFacing.North;
        int pitch = 0;

        public MCRoomFill room { get; set; }

        int mapX;
        int mapY;
        int mapZ;

        public MCMapRoom()
        {
            room = new MCRoomFill(new MCFill(6, 6, 6, 0, 0, 1, "minecraft:air"));
            mapX = 0;
            mapY = 0;
            mapZ = 0;
            defaultFacing = MCRenderFacing.North;
            pitch = 0;

        }
        public MCMapRoom(MCRoomFill room, int x, int y, int z)
        {
            this.room = room;
            mapX = x; mapY = y; mapZ = z;
            defaultFacing = MCRenderFacing.North;
            pitch = 0;

        }
        public MCMapRoom(MCRoomFill room, int x, int y, int z, MCRenderFacing Facing, int Pitch)
            : this(room, x, y, z)
        {
            defaultFacing = Facing;
            pitch = Pitch;
        }

        public string[] Render()
        {
            return room.Render(defaultFacing, pitch, false, mapX * 6, mapY * 6, mapZ * 6);
        }
        public string[] Render(MCRenderFacing Facing, int Pitch)
        {
            return room.Render(Facing, Pitch, false, mapX * 6, mapY * 6, mapZ * 6);
        }
        public string[] RenderAbsolute(int x, int y, int z)
        {
            return room.Render(defaultFacing, pitch, true, x + (mapX * 6), y + (mapY * 6), z + (mapZ * 6));

        }
        public string[] RenderAbsolute(MCRenderFacing Facing, int Pitch, int x, int y, int z)
        {
            return room.Render(Facing, Pitch, true, x + (mapX * 6), y + (mapY * 6), z + (mapZ * 6));
        }


    }
}
