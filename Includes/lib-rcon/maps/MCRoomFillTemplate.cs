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
    /// Template class to help with creating 'rooms', various fill patterns.
    /// 
    ///
    /// </summary>
    public class MCRoomFillTemplate
    {


        private string emptyBlock;
        private string fillBlock;
        private int[] offsetVoxel = new int[3];

        private bool autoOffset = false;

        /// <summary>
        /// Any fill objects will be permamently offset changed using the values passed.  Set an activate
        /// offset render mode.
        /// </summary>
        /// <param name="x">X Axis offset.</param>
        /// <param name="y">Y Axis offset.</param>
        /// <param name="z">Z Axis offset.</param>
        public void ActivateOffset(int x, int y, int z)
        {
            offsetVoxel[0] = x; offsetVoxel[1] = y; offsetVoxel[2] = z;
            autoOffset = true;
        }
        /// <summary>
        /// Toggle offset adjustments on or off.
        /// </summary>
        public void ActivateOffset()
        {
            autoOffset = true;
        }

        /// <summary>
        /// Turn off offset option.
        /// </summary>
        public void DeactivateOffset() { autoOffset = false; }

        private MCRoomFill applyOffset(MCRoomFill Room)
        {

            if (autoOffset == true)
                Room.OffsetMCFill(offsetVoxel);

            return Room;


        }

        /// <summary>
        /// Minecraft block entity string used for an Emptied State
        /// </summary>
        public string EmptyFillBlock { get { return emptyBlock; } set { emptyBlock = value; } }
        /// <summary>
        /// Minecraft block entity string used for Filled State
        /// </summary>
        public string FillBlock { get { return fillBlock; } set { fillBlock = value; } }


        public MCFill Fill(string BlockType) { return new MCFill(6, 6, 6, 0, 0, 1, BlockType); }

        public MCFill FillCenter(string BlockType) { return new MCFill(4, 6, 4, 1, 0, 1, BlockType); }
        public MCFill FillCenterPillar(string BlockType) { return new MCFill(2, 6, 2, 2, 0, 3, BlockType); }

        public MCFill FillLeftWall(string BlockType) { return new MCFill(1, 6, 6, 0, 0, 1, BlockType); }
        public MCFill FillRightWall(string BlockType) { return new MCFill(1, 6, 6, 5, 0, 1, BlockType); }
        public MCFill FillFrontWall(string BlockType) { return new MCFill(6, 6, 1, 0, 0, 1, BlockType); }
        public MCFill FillBackWall(string BlockType) { return new MCFill(6, 6, 1, 0, 0, 6, BlockType); }

        public MCFill FillCenterLeftWall(string BlockType) { return new MCFill(1, 6, 4, 1, 0, 1, BlockType); }
        public MCFill FillCenterRightWall(string BlockType) { return new MCFill(1, 6, 4, 4, 0, 1, BlockType); }
        public MCFill FillCenterFrontWall(string BlockType) { return new MCFill(4, 6, 1, 1, 0, 1, BlockType); }
        public MCFill FillCenterBackWall(string BlockType) { return new MCFill(4, 6, 1, 1, 0, 5, BlockType); }

        public MCFill FillCenterPillarLeft(string BlockType) { return new MCFill(1, 6, 2, 2, 0, 3, BlockType); }
        public MCFill FillCenterPillarRight(string BlockType) { return new MCFill(1, 6, 2, 3, 0, 3, BlockType); }
        public MCFill FillCenterPillarFront(string BlockType) { return new MCFill(2, 6, 1, 2, 0, 3, BlockType); }
        public MCFill FillCenterPillarBack(string BlockType) { return new MCFill(2, 6, 1, 2, 0, 4, BlockType); }

        public MCFill FillFloor(string BlockType) { return new MCFill(6, 1, 6, 0, 0, 1, BlockType); }
        public MCFill FillCeiling(string BlockType) { return new MCFill(6, 1, 6, 0, 5, 1, BlockType); }

        public MCFill FillBackLeftCorner(string BlockType) { return new MCFill(2, 6, 2, 0, 0, 5, BlockType); }
        public MCFill FillBackRightCorner(string BlockType) { return new MCFill(2, 6, 2, 4, 0, 5, BlockType); }
        public MCFill FillFrontLeftCorner(string BlockType) { return new MCFill(2, 6, 2, 0, 0, 1, BlockType); }
        public MCFill FillFrontRightCorner(string BlockType) { return new MCFill(2, 6, 2, 4, 0, 1, BlockType); }

        public MCFill FillLRHall(string BlockType) { return new MCFill(6, 6, 2, 0, 0, 3, BlockType); }
        public MCFill FillFBHall(string BlockType) { return new MCFill(2, 6, 6, 2, 0, 1, BlockType); }

        public MCFill FillLeftWallSolid(string BlockType) { return new MCFill(3, 6, 6, 0, 0, 1, BlockType); }
        public MCFill FillRightWallSolid(string BlockType) { return new MCFill(3, 6, 6, 3, 0, 1, BlockType); }
        public MCFill FillBackWallSolid(string BlockType) { return new MCFill(6, 6, 3, 0, 0, 4, BlockType); }
        public MCFill FillFrontWallSolid(string BlockType) { return new MCFill(6, 6, 3, 0, 0, 1, BlockType); }



        public MCRoomFillTemplate()
        {

            fillBlock = "minecraft:stone";
            emptyBlock = "minecraft:air";

        }
        public MCRoomFillTemplate(string FillBlock, string EmptyBlock)
        {
            fillBlock = FillBlock;
            emptyBlock = EmptyBlock;



        }



        public MCRoomFill NSEWRoom { get { return applyOffset(new MCRoomFill(Fill(fillBlock), FillCenter(emptyBlock), FillLRHall(emptyBlock), FillFBHall(emptyBlock))); } }
        public MCRoomFill NSEWRoomPillar { get { return applyOffset(new MCRoomFill(NSEWRoom, FillCenterPillar(fillBlock))); } }

        public MCRoomFill NSEWRoomC { get { return applyOffset(new MCRoomFill(FillCenterFrontWall(fillBlock), FillCenterBackWall(fillBlock), FillCenterLeftWall(fillBlock), FillCenterRightWall(fillBlock), FillLRHall(emptyBlock), FillFBHall(emptyBlock))); } }


        public MCRoomFill EmptyRoom { get { return applyOffset(new MCRoomFill(Fill(emptyBlock))); } }
        public MCRoomFill SolidRoom { get { return applyOffset(new MCRoomFill(Fill(fillBlock))); } }

        public MCRoomFill EmptyCenter { get { return applyOffset(new MCRoomFill(FillCenter(emptyBlock))); } }
        public MCRoomFill SolidCenter { get { return applyOffset(new MCRoomFill(FillCenter(fillBlock))); } }

        public MCRoomFill EmptyCenterPillar { get { return applyOffset(new MCRoomFill(FillCenterPillar(emptyBlock))); } }
        public MCRoomFill SolidCenterPillar { get { return applyOffset(new MCRoomFill(FillCenterPillar(fillBlock))); } }

        public MCRoomFill BLRWall { get { return applyOffset(new MCRoomFill(FillLeftWall(fillBlock), FillBackWall(fillBlock), FillRightWall(fillBlock))); } }
        public MCRoomFill BRFWall { get { return applyOffset(new MCRoomFill(FillRightWall(fillBlock), FillFrontWall(fillBlock), FillBackWall(fillBlock))); } }
        public MCRoomFill FLRWall { get { return applyOffset(new MCRoomFill(FillLeftWall(fillBlock), FillFrontWall(fillBlock), FillRightWall(fillBlock))); } }
        public MCRoomFill FLBWall { get { return applyOffset(new MCRoomFill(FillLeftWall(fillBlock), FillFrontWall(fillBlock), FillBackWall(fillBlock))); } }

        public MCRoomFill BLCorner { get { return applyOffset(new MCRoomFill(FillBackLeftCorner(fillBlock))); } }
        public MCRoomFill BRCorner { get { return applyOffset(new MCRoomFill(FillBackRightCorner(fillBlock))); } }
        public MCRoomFill FLCorner { get { return applyOffset(new MCRoomFill(FillFrontLeftCorner(fillBlock))); } }
        public MCRoomFill FRCorner { get { return applyOffset(new MCRoomFill(FillFrontRightCorner(fillBlock))); } }

        public MCRoomFill BLWall { get { return applyOffset(new MCRoomFill(FillLeftWall(fillBlock), FillBackWall(fillBlock))); } }
        public MCRoomFill BRWall { get { return applyOffset(new MCRoomFill(FillRightWall(fillBlock), FillBackWall(fillBlock))); } }
        public MCRoomFill FLWall { get { return applyOffset(new MCRoomFill(FillLeftWall(fillBlock), FillFrontWall(fillBlock))); } }
        public MCRoomFill FRWall { get { return applyOffset(new MCRoomFill(FillRightWall(fillBlock), FillFrontWall(fillBlock))); } }

        public MCRoomFill LRWall { get { return applyOffset(new MCRoomFill(FillLeftWall(fillBlock), FillRightWall(fillBlock))); } }
        public MCRoomFill FBWall { get { return applyOffset(new MCRoomFill(FillFrontWall(fillBlock), FillBackWall(fillBlock))); } }

        public MCRoomFill LRWallC { get { return applyOffset(new MCRoomFill(FillCenterLeftWall(fillBlock), FillCenterRightWall(fillBlock))); } }
        public MCRoomFill FBWallC { get { return applyOffset(new MCRoomFill(FillCenterFrontWall(fillBlock), FillCenterBackWall(fillBlock))); } }

        public MCRoomFill LeftWall { get { return applyOffset(new MCRoomFill(FillLeftWall(fillBlock))); } }
        public MCRoomFill RightWall { get { return applyOffset(new MCRoomFill(FillRightWall(fillBlock))); } }
        public MCRoomFill BackWall { get { return applyOffset(new MCRoomFill(FillBackWall(fillBlock))); } }
        public MCRoomFill FrontWall { get { return applyOffset(new MCRoomFill(FillFrontWall(fillBlock))); } }

        public MCRoomFill LeftWallC { get { return applyOffset(new MCRoomFill(FillCenterLeftWall(fillBlock))); } }
        public MCRoomFill RightWallC { get { return applyOffset(new MCRoomFill(FillCenterRightWall(fillBlock))); } }
        public MCRoomFill BackWallC { get { return applyOffset(new MCRoomFill(FillCenterBackWall(fillBlock))); } }
        public MCRoomFill FrontWallC { get { return applyOffset(new MCRoomFill(FillCenterFrontWall(fillBlock))); } }

        public MCRoomFill LeftWallSolid { get { return applyOffset(new MCRoomFill(FillLeftWallSolid(fillBlock))); } }
        public MCRoomFill RightWallSolid { get { return applyOffset(new MCRoomFill(FillRightWallSolid(fillBlock))); } }
        public MCRoomFill BackWallSolid { get { return applyOffset(new MCRoomFill(FillBackWallSolid(fillBlock))); } }
        public MCRoomFill FrontWallSolid { get { return applyOffset(new MCRoomFill(FillFrontWallSolid(fillBlock))); } }



    }
}
