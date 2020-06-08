using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using LibMCRcon.WorldData;
using LibMCRcon.Remote;

namespace LibMCRcon.Rendering
{

    public class BitChunker : List<int>
    {

        public BitChunker(Int64[] Source)
        {

            BitBlitz(Source, (Source.Length * 64) / 4096);
        }
        public BitChunker(Int64[] Source, int BitSizeOverride)
        {
            BitBlitz(Source, BitSizeOverride);
        }

        public void BitBlitz(Int64[] Source, int BitSize)
        {

            int block_index = 0;
            int bits_remaining = 64;
            int bits_left_over = 0;
            int bitsource_idx = 1;
            int bitsource_max = Source.Length;

            ulong working = 0;
            ulong split_left_over = 0;


            Clear();

            if (BitSize < 4)
                BitSize = 4;

            ulong mask = (ulong)~(-1U << BitSize);
            working = (ulong)Source[0];

            while (true)
            {

                while (bits_remaining >= BitSize)
                {
                    block_index = (int)(working & mask);
                    working >>= BitSize;
                    bits_remaining -= BitSize;

                    Add(block_index);
                }

                if (bits_left_over > 0)
                {

                    working |= (split_left_over << bits_remaining);
                    bits_remaining += bits_left_over;
                    bits_left_over = 0;
                }
                else if (bitsource_idx < bitsource_max)
                {


                    split_left_over = (ulong)Source[bitsource_idx];

                    split_left_over <<= bits_remaining;
                    bits_left_over = bits_remaining;
                    working |= split_left_over;
                    split_left_over = (ulong)Source[bitsource_idx] >> (64 - bits_left_over);
                    bits_remaining = 64;
                    bitsource_idx++;


                }
                else
                    break;


            };



        }
        public int Max()
        {
            var idx_max = int.MinValue;

            foreach (var idx in this)
                if (idx > idx_max)
                    idx_max = idx;

            return idx_max;
        }

    }
    public class RegionMasterPaletteWithBlocks
    {
        public List<string> MasterPalette { get; private set; } = new List<string>();
        public int[] RegionBlockStates { get; private set; } = new int[512 * 512];

        int rbs_idx = 0;
        int rbs_next = 0;

        public void PrimeNextBlock(string block)
        {
            if (MasterPalette.Contains(block) == false)
            {
                MasterPalette.Add(block);
                rbs_idx = rbs_next;
                rbs_next++;
            }
            else
                rbs_idx = MasterPalette.FindIndex((x) => x.Equals(block));
        }
        public void SetBlock(int ridx, string block)
        {
            PrimeNextBlock(block);
            RegionBlockStates[ridx] = rbs_idx;
        }

        public IEnumerable<Tuple<string, int>> Range(int idx)
        {
            for (var id = idx; id < idx + 100; idx++)
            {
                yield return new Tuple<string, int>(MasterPalette[RegionBlockStates[id]], id);
            }
            yield break;
        }
    }
    public class BlockColors
    {
        public List<KeyValuePair<string, Color>> BlockPalettes { get; set; } = new List<KeyValuePair<string, Color>>();
        public List<string> Defaulted { get; set; } = new List<string>();

        public Color[] GetPalette(List<string> BlockList)
        {
            var lst_idx = 0;
            var lst_idxMax = BlockList.Count;
            Color[] Rainbow = new Color[lst_idxMax];

            for (; lst_idx < lst_idxMax; lst_idx++)
            {
                var block = BlockList[lst_idx].Split(':')[1];

                Color SelectColor()
                { 
                  
                    foreach(var kv in BlockPalettes)
                    {
                        if (block.Contains(kv.Key))
                            return kv.Value;
                    }

                    if (Defaulted.Find(x => x.Equals(block)) == null)
                        Defaulted.Add(block);

                    return Color.Gray;
                }

                Rainbow[lst_idx] = SelectColor();

            }

            return Rainbow;
        }

        public void InitBlockColors()
        {
            BlockPalettes.Add(new KeyValuePair<string, Color>("lilac", Color.Violet));
            BlockPalettes.Add(new KeyValuePair<string, Color>("rose_bush", Color.MistyRose));
            BlockPalettes.Add(new KeyValuePair<string, Color>("sunflower", Color.LightYellow));
            BlockPalettes.Add(new KeyValuePair<string, Color>("oxeye_daisy", Color.FloralWhite));
            BlockPalettes.Add(new KeyValuePair<string, Color>("azure_bluet", Color.Azure));
            BlockPalettes.Add(new KeyValuePair<string, Color>("peony", Color.Azure));
            BlockPalettes.Add(new KeyValuePair<string, Color>("pumpkin", Color.Orange));
            BlockPalettes.Add(new KeyValuePair<string, Color>("fern", Color.LawnGreen));
            BlockPalettes.Add(new KeyValuePair<string, Color>("large_fern", Color.LawnGreen));
            BlockPalettes.Add(new KeyValuePair<string, Color>("gray_wool", Color.Gray));
            BlockPalettes.Add(new KeyValuePair<string, Color>("obsidian", Color.BlueViolet));
            BlockPalettes.Add(new KeyValuePair<string, Color>("farmland", Color.SandyBrown));
            BlockPalettes.Add(new KeyValuePair<string, Color>("birch_sapling", Color.Beige));
            BlockPalettes.Add(new KeyValuePair<string, Color>("glass", Color.CornflowerBlue));
            BlockPalettes.Add(new KeyValuePair<string, Color>("potatoes", Color.Tan));
            BlockPalettes.Add(new KeyValuePair<string, Color>("attached_melon_stem", Color.GreenYellow));
            BlockPalettes.Add(new KeyValuePair<string, Color>("melon", Color.GreenYellow));
            BlockPalettes.Add(new KeyValuePair<string, Color>("melon_stem", Color.Brown));
            BlockPalettes.Add(new KeyValuePair<string, Color>("pumpkin_stem", Color.Brown));
            BlockPalettes.Add(new KeyValuePair<string, Color>("attached_pumpkin_stem", Color.Orange));
            BlockPalettes.Add(new KeyValuePair<string, Color>("birch_slab", Color.Beige));
            BlockPalettes.Add(new KeyValuePair<string, Color>("yellow_carpet", Color.Yellow));
            BlockPalettes.Add(new KeyValuePair<string, Color>("wall_sign", Color.Tan));
            BlockPalettes.Add(new KeyValuePair<string, Color>("podzol", Color.LightGray));
            BlockPalettes.Add(new KeyValuePair<string, Color>("purple_stained_glass", Color.Purple));
            BlockPalettes.Add(new KeyValuePair<string, Color>("cobweb", Color.Gray));
            BlockPalettes.Add(new KeyValuePair<string, Color>("mushroom_stem", Color.Beige));
            BlockPalettes.Add(new KeyValuePair<string, Color>("orange_tulip", Color.Orange));
            BlockPalettes.Add(new KeyValuePair<string, Color>("red_tulip", Color.Red));
            BlockPalettes.Add(new KeyValuePair<string, Color>("white_tulip", Color.White));
            BlockPalettes.Add(new KeyValuePair<string, Color>("pink_tulip", Color.Pink));
            BlockPalettes.Add(new KeyValuePair<string, Color>("white_bed", Color.White));
            BlockPalettes.Add(new KeyValuePair<string, Color>("carved_pumpkin", Color.Orange));

            BlockPalettes.Add(new KeyValuePair<string, Color>("fence", Color.Brown));
            BlockPalettes.Add(new KeyValuePair<string, Color>("kelp", Color.DarkGreen));
            BlockPalettes.Add(new KeyValuePair<string, Color>("pad", Color.DarkGreen));
            BlockPalettes.Add(new KeyValuePair<string, Color>("lava", Color.OrangeRed));
            BlockPalettes.Add(new KeyValuePair<string, Color>("water", Color.DarkBlue));
            BlockPalettes.Add(new KeyValuePair<string, Color>("clay", Color.DarkGray));
            BlockPalettes.Add(new KeyValuePair<string, Color>("gravel", Color.Gray));
            BlockPalettes.Add(new KeyValuePair<string, Color>("stone", Color.Gray));
            BlockPalettes.Add(new KeyValuePair<string, Color>("ore", Color.Gray));
            BlockPalettes.Add(new KeyValuePair<string, Color>("andesite", Color.Silver));
            BlockPalettes.Add(new KeyValuePair<string, Color>("diorite", Color.LightGray));
            BlockPalettes.Add(new KeyValuePair<string, Color>("granite", Color.IndianRed));
            BlockPalettes.Add(new KeyValuePair<string, Color>("coral", Color.LightPink));
            BlockPalettes.Add(new KeyValuePair<string, Color>("seagrass", Color.MediumSeaGreen));
            BlockPalettes.Add(new KeyValuePair<string, Color>("sea_pickle", Color.DarkOliveGreen));
            BlockPalettes.Add(new KeyValuePair<string, Color>("sand", Color.BlanchedAlmond));
            BlockPalettes.Add(new KeyValuePair<string, Color>("wood", Color.Brown));
            BlockPalettes.Add(new KeyValuePair<string, Color>("dirt", Color.SaddleBrown));
            BlockPalettes.Add(new KeyValuePair<string, Color>("grass", Color.Green));
            BlockPalettes.Add(new KeyValuePair<string, Color>("snow", Color.White));
            BlockPalettes.Add(new KeyValuePair<string, Color>("leaves", Color.DarkGreen));
            BlockPalettes.Add(new KeyValuePair<string, Color>("air", Color.Transparent));
            BlockPalettes.Add(new KeyValuePair<string, Color>("planks", Color.BurlyWood));
            BlockPalettes.Add(new KeyValuePair<string, Color>("spruce", Color.BurlyWood));
            BlockPalettes.Add(new KeyValuePair<string, Color>("oak", Color.Tan));
            BlockPalettes.Add(new KeyValuePair<string, Color>("jungle", Color.Tan));
            BlockPalettes.Add(new KeyValuePair<string, Color>("brown_mushroom", Color.Tan));
            BlockPalettes.Add(new KeyValuePair<string, Color>("vine", Color.DarkGreen));
            BlockPalettes.Add(new KeyValuePair<string, Color>("red_mushroom", Color.DarkRed));
            BlockPalettes.Add(new KeyValuePair<string, Color>("dandelion", Color.Gainsboro));
            BlockPalettes.Add(new KeyValuePair<string, Color>("poppy", Color.DarkRed));
            BlockPalettes.Add(new KeyValuePair<string, Color>("bubble", Color.OldLace));
            BlockPalettes.Add(new KeyValuePair<string, Color>("orchid", Color.DodgerBlue));
            BlockPalettes.Add(new KeyValuePair<string, Color>("sugar_cane", Color.LightGreen));
            BlockPalettes.Add(new KeyValuePair<string, Color>("cactus", Color.ForestGreen));
            BlockPalettes.Add(new KeyValuePair<string, Color>("dead", Color.Sienna));
            BlockPalettes.Add(new KeyValuePair<string, Color>("chest", Color.Brown));
            BlockPalettes.Add(new KeyValuePair<string, Color>("crafting", Color.Brown));
            BlockPalettes.Add(new KeyValuePair<string, Color>("terracotta", Color.RosyBrown));
            BlockPalettes.Add(new KeyValuePair<string, Color>("furnace", Color.Gray));
            BlockPalettes.Add(new KeyValuePair<string, Color>("acacia", Color.IndianRed));
            BlockPalettes.Add(new KeyValuePair<string, Color>("magma", Color.OrangeRed));
            BlockPalettes.Add(new KeyValuePair<string, Color>("wheat", Color.LightGreen));
            BlockPalettes.Add(new KeyValuePair<string, Color>("beetroots", Color.Firebrick));
            BlockPalettes.Add(new KeyValuePair<string, Color>("ladder", Color.Brown));
            BlockPalettes.Add(new KeyValuePair<string, Color>("carrots", Color.Orange));
            BlockPalettes.Add(new KeyValuePair<string, Color>("torch", Color.Tan));
            BlockPalettes.Add(new KeyValuePair<string, Color>("black_wool", Color.Black));
            BlockPalettes.Add(new KeyValuePair<string, Color>("ice", Color.DodgerBlue));
            BlockPalettes.Add(new KeyValuePair<string, Color>("log", Color.Tan));
        }
        public void ResetBlockColors()
        {
            BlockPalettes.Clear();
            InitBlockColors();
            
        }

        public static BlockColors Create() { var bc = new BlockColors(); bc.InitBlockColors(); return bc; }

    }

    public static class MCRegionMaps
    {
              
        public static Color[][] Palettes()
        {

            Color[] Water;
            Color[] Topo;

            List<ColorStep> cList = new List<ColorStep>
            {
                Color.Black.ColorStep(20),
                Color.Pink.ColorStep(20),
                Color.Blue.ColorStep(20),
                Color.FromArgb(0xDF, 0xC7, 0x00).ColorStep(20),
                Color.DarkGreen.ColorStep(20),
                Color.Orange.ColorStep(20),
                Color.Brown.ColorStep(20),
                Color.Plum.ColorStep(20),
                Color.Magenta.ColorStep(20),
                Color.Coral.ColorStep(20),
                Color.Aqua.ColorStep(20),
                Color.LightCyan.ColorStep(20),
                Color.Yellow.ColorStep(15)
            };
            Topo = ColorStep.CreatePallet(cList);


            cList.Clear();
            cList.Add(Color.Blue.ColorStep(50));
            cList.Add(Color.Aqua.ColorStep(50));
            cList.Add(Color.Teal.ColorStep(50));
            cList.Add(Color.Cyan.ColorStep(50));
            cList.Add(Color.SkyBlue.ColorStep(25));
            cList.Add(Color.Turquoise.ColorStep(25));

            Water = ColorStep.CreatePallet(cList);






            return new Color[][] { Topo, Water };
        }

        public static void RenderBlockPngFromRegion(byte[][] TopoData, Color[] BlockData, string ImgPath, int Xs, int Zs)
        {
            byte[] hMap = TopoData[0];
            byte[] wMap = TopoData[1];

            Bitmap bit = new Bitmap(512, 512);

            Color[][] pal = Palettes();
            Color[] tRGB = pal[0];
            Color[] wRGB = pal[1];

            for (int zz = 0; zz < 512; zz++)
            {

                for (int xx = 0; xx < 512; xx++)
                {

                    int gI = (zz * 512) + xx;

                    if (wMap[gI] < 255)
                    {

                        if (zz > 1)
                        {
                            int cI = ((zz - 1) * 512) + xx;

                            if (hMap[cI] > hMap[gI])
                                bit.SetPixel(xx, zz, ColorStep.MixColors(10, BlockData[gI], Color.FromArgb(25, 25, 175), 0));
                            else if (hMap[cI] < hMap[gI])
                                bit.SetPixel(xx, zz, ColorStep.MixColors(10, BlockData[gI], Color.FromArgb(75, 75, 255), 0));
                            else
                                bit.SetPixel(xx, zz, ColorStep.MixColors(10, BlockData[gI], Color.FromArgb(50, 50, 200), 0));
                        }
                        else
                        {
                            int cI = ((zz + 1) * 512) + xx;

                            if (hMap[cI] > hMap[gI])
                                bit.SetPixel(xx, zz, ColorStep.MixColors(10, BlockData[gI], Color.FromArgb(75, 75, 255), 0));
                            else if (hMap[cI] < hMap[gI])
                                bit.SetPixel(xx, zz, ColorStep.MixColors(10, BlockData[gI], Color.FromArgb(25, 25, 175), 0));
                            else
                                bit.SetPixel(xx, zz, ColorStep.MixColors(10, BlockData[gI], Color.FromArgb(50, 50, 200), 0));
                        }
                    }
                    //if (xx > 1 && xx < 511 && zz > 1 && zz < 511)
                    //{
                    //    int cI = ((zz - 1) * 512) + xx - 1;

                    //     if (hMap[cI] > hMap[gI])
                    //        bit.SetPixel(xx, zz, ColorStep.MixColors(10, BlockData[gI], Color.FromArgb(25, 25, 175), 0));
                    //    else if (hMap[cI] < hMap[gI])
                    //        bit.SetPixel(xx, zz, ColorStep.MixColors(10, BlockData[gI], Color.FromArgb(75, 75, 255), 0));
                    //    else
                    //        bit.SetPixel(xx, zz, ColorStep.MixColors(10, BlockData[gI], Color.FromArgb(50, 50, 200), 0));
                    //}
                    //else
                    //    bit.SetPixel(xx, zz, ColorStep.MixColors(10, BlockData[gI], Color.FromArgb(50, 50, 200), 0));
                    else
                    {

                        if (zz > 1)
                        {
                            int cI = ((zz - 1) * 512) + xx;

                            if (hMap[cI] > hMap[gI])
                                bit.SetPixel(xx, zz, ColorStep.MixColors(0, BlockData[gI], Color.FromArgb(80, 80, 80), 0));
                            else if (hMap[cI] < hMap[gI])
                                bit.SetPixel(xx, zz, ColorStep.MixColors(0, BlockData[gI], Color.FromArgb(200, 200, 200), 0));
                            else
                                bit.SetPixel(xx, zz, BlockData[gI]);
                        }
                        else
                        {
                            int cI = ((zz + 1) * 512) + xx;

                            if (hMap[cI] > hMap[gI])
                                bit.SetPixel(xx, zz, ColorStep.MixColors(0, BlockData[gI], Color.FromArgb(80, 80, 80), 0));
                            else if (hMap[cI] < hMap[gI])
                                bit.SetPixel(xx, zz, ColorStep.MixColors(0, BlockData[gI], Color.FromArgb(200, 200, 200), 0));
                            else
                                bit.SetPixel(xx, zz, BlockData[gI]);
                        }
                    }
                }

            }



            DirectoryInfo imgDir = new DirectoryInfo(ImgPath);
            string SaveBitMap  = string.Format(Path.Combine(imgDir.FullName, string.Format("tile.{0}.{1}.png", Xs, Zs)));
            bit.Save(SaveBitMap, System.Drawing.Imaging.ImageFormat.Png);
            bit.Dispose();

        }
        public static void RenderTopoPngFromRegion(byte[][] HeightData, string ImgPath, int Xs, int Zs)
        {
            byte[] hMap = HeightData[0];
            byte[] hWMap = HeightData[1];

            Bitmap bit = new Bitmap(512, 512);

            Color[][] pal = Palettes();
            Color[] tRGB = pal[0];
            Color[] wRGB = pal[1];

            for (int zz = 0; zz < 512; zz++)
            {

                for (int xx = 0; xx < 512; xx++)
                {

                    int gI = (zz * 512) + xx;

                    if (hWMap[gI] < 255)
                    {
                        if (zz > 1)
                        {
                            int cI = ((zz - 1) * 512) + xx;
                            if (hMap[cI] > hMap[gI])
                                bit.SetPixel(xx, zz, ColorStep.MixColors(0, wRGB[hMap[gI]], Color.White, 0));
                            else if (hMap[cI] < hMap[gI])
                                bit.SetPixel(xx, zz, ColorStep.MixColors(0, wRGB[hMap[gI]], Color.DarkGray, 0));
                            else
                                bit.SetPixel(xx, zz, wRGB[hMap[gI]]);
                        }
                        else
                        {
                            int cI = ((zz + 1) * 512) + xx;
                            if (hMap[cI] > hMap[gI])
                                bit.SetPixel(xx, zz, ColorStep.MixColors(0, wRGB[hMap[gI]], Color.White, 0));
                            else if (hMap[cI] < hMap[gI])
                                bit.SetPixel(xx, zz, ColorStep.MixColors(0, wRGB[hMap[gI]], Color.DarkGray, 0));
                            else
                                bit.SetPixel(xx, zz, wRGB[hMap[gI]]);
                        }
                    }
                    else
                    {

                        if (zz > 1)
                        {
                            int cI = ((zz - 1) * 512) + xx;

                            if (hMap[cI] > hMap[gI])
                                bit.SetPixel(xx, zz, ColorStep.MixColors(0, tRGB[hMap[gI]], Color.White, 0));
                            else if (hMap[cI] < hMap[gI])
                                bit.SetPixel(xx, zz, ColorStep.MixColors(0, tRGB[hMap[gI]], Color.DarkGray, 0));
                            else
                                bit.SetPixel(xx, zz, tRGB[hMap[gI]]);
                        }
                        else
                        {
                            int cI = ((zz + 1) * 512) + xx;

                            if (hMap[cI] > hMap[gI])
                                bit.SetPixel(xx, zz, ColorStep.MixColors(0, tRGB[hMap[gI]], Color.White, 0));
                            else if (hMap[cI] < hMap[gI])
                                bit.SetPixel(xx, zz, ColorStep.MixColors(0, tRGB[hMap[gI]], Color.DarkGray, 0));
                            else
                                bit.SetPixel(xx, zz, tRGB[hMap[gI]]);
                        }

                    }
                }
            }



            DirectoryInfo imgDir = new DirectoryInfo(ImgPath);
            string SaveBitMap = string.Format(Path.Combine(imgDir.FullName, string.Format("topo.{0}.{1}.png", Xs, Zs)));

            bit.Save(SaveBitMap, System.Drawing.Imaging.ImageFormat.Png);
            bit.Dispose();

        }
        public static void RenderLegend(string ImgPath)
        {

            FileInfo legend = new FileInfo(Path.Combine(ImgPath, "legend.png"));
            if (legend.Exists == false)
            {

                Bitmap bit = new Bitmap(20, 512);

                Color[][] pal = Palettes();

                Color[] tRGB = pal[0];
                Color[] wRGB = pal[1];


                Graphics gBit = Graphics.FromImage(bit);

                for (int z = 0; z < 256; z++)
                {
                    gBit.DrawLine(new Pen(tRGB[z]), 0, 255 - z, 15, 255 - z);
                    gBit.DrawLine(new Pen(wRGB[z]), 0, 511 - z, 15, 511 - z);

                    if (z % 10 == 0)
                    {
                        gBit.DrawLine(new Pen(Color.Black), 16, 255 - z, 19, 255 - z);
                        gBit.DrawLine(new Pen(Color.Black), 16, 511 - z, 19, 511 - z);
                    }
                }

                gBit.Dispose();

                DirectoryInfo imgDir = new DirectoryInfo(ImgPath);
                string SaveBitMap = string.Format(Path.Combine(imgDir.FullName, "legend.png"));
                bit.Save(SaveBitMap, System.Drawing.Imaging.ImageFormat.Png);
                bit.Dispose();
            }
        }

        public static void RenderDataFromRegion(BlockColors bc, RegionMCA mca, byte[][] TopoData, Color[] Blocks = null)
        {

            byte[] hMap = TopoData[0];
            byte[] hWMap = TopoData[1];
            
            Voxel Chunk;

            RegionMasterPaletteWithBlocks RegionBlocks = new RegionMasterPaletteWithBlocks();



            if (mca.IsLoaded)
            {

                for (int zz = 0; zz < 32; zz++)
                    for (int xx = 0; xx < 32; xx++)
                    {
                        var ridx = (zz * 16 * 512) + (xx * 16);


                        mca.SetOffset(65, xx * 16, zz * 16);
                        mca.RefreshChunk();

                        Chunk = mca.Chunk;

                        NbtChunk c = null;

                        c = mca.NbtChunk(mca);
                  
                        int[,] cGround = new int[16, 16];

                        if (c.IsLoaded && c.sections != null)
                        {
                            var sc = c.sections.tagvalue.Count;
                            var bb = new List<BitChunker>();
                            var bp = new List<List<string>>();


                            if (sc > 0)
                            {
                                foreach (var sect in c.sections.tagvalue)
                                {
                                    var sdata = ((Nbt.NbtLongArray)sect["BlockStates"]).tagvalue;
                                    var yidx = ((Nbt.NbtByte)sect["Y"]).tagvalue;
                                    if (sdata.Length > 256)
                                    {

                                    }
                                    var pal = ((Nbt.NbtList)sect["Palette"]).tagvalue;
                                    var bl = new BitChunker(sdata);

                                    bb.Add(bl);
                                    var bp_block = new List<string>();


                                    foreach (var p in pal)
                                        bp_block.Add(((Nbt.NbtString)p["Name"]).tagvalue);

                                    bp.Add(bp_block);
                                }


                            }
                            sc--;
                            var cidx = 0;
                            RenderBlocks();

                            void RenderBlocks()
                            {
                                var blocksleft = 256;

                                for (var iB = sc; iB > 0; iB--)
                                {
                                    var bl = bb[iB];
                                    var blp = bp[iB];

                                    for (var iC = 15; iC > 0; iC--)
                                    {
                                        cidx = iC * 256;
                                        var h = (iB * 16) + iC;



                                        for (var cz = 0; cz < 16; cz++)
                                            for (var cx = 0; cx < 16; cx++, cidx++)
                                            {

                                                if (cGround[cx, cz] == 0)
                                                {

                                                    var bidx = bl[cidx];
                                                    var midx = ridx + ((cz * 512) + cx);

                                                    if (bidx < blp.Count)
                                                    {


                                                        var block = blp[bidx];




                                                        if (block.Contains(":water"))
                                                        {
                                                            if (hWMap[midx] == 255)
                                                                hWMap[midx] = (byte)h;
                                                        }
                                                        else if (block.Contains(":air"))
                                                        {
                                                            hWMap[midx] = 255;
                                                            hMap[midx] = 0;
                                                        }
                                                        else
                                                        {

                                                            RegionBlocks.SetBlock(midx, block);

                                                            cGround[cx, cz] = h;
                                                            hMap[midx] = (byte)h;

                                                            if (hWMap[midx] == 0)
                                                                hWMap[midx] = 255;

                                                            --blocksleft;
                                                            if (blocksleft == 0)
                                                                return;

                                                        }

                                                    }
                                                    else
                                                    {
                                                        //can't do this... why have index not in palette?
                                                        cGround[cx, cz] = h;
                                                        hMap[midx] = (byte)h;

                                                        if (hWMap[midx] == 0)
                                                            hWMap[midx] = 255;

                                                        --blocksleft;
                                                        if (blocksleft == 0)
                                                            return;
                                                    }
                                                }

                                            }

                                    }

                                }
                            }
                        }
                        else
                        {
                            for (var cz = 0; cz < 16; cz++)
                                for (var cx = 0; cx < 16; cx++)
                                {
                                    var midx = ridx + ((cz * 512) + cx);
                                    hWMap[midx] = 255;
                                    hMap[midx] = 0;
                                    RegionBlocks.SetBlock(midx, "minecraft:void_air");
                                }
                        }
                    }

                if (Blocks != null)
                {
                    Color[] Pal = bc.GetPalette(RegionBlocks.MasterPalette);
                    var idx_max = 512 * 512;

                    for (var idx = 0; idx < idx_max; idx++)
                    {
                        Blocks[idx] = Pal[RegionBlocks.RegionBlockStates[idx]];
                    }
                }

            }
        }


        public static byte[][] RetrieveHDT(Voxel RV, string RegionPath)
        {
            byte[][] MapData = new byte[][] { new byte[512 * 512], new byte[512 * 512] };

            FileInfo mcaF = new FileInfo(Path.Combine(RegionPath, string.Format("r.{0}.{1}.hdt", RV.Xs, RV.Zs)));
            if (mcaF.Exists == true)
            {
                FileStream tempFS = mcaF.Open(FileMode.Open, FileAccess.Read);
                tempFS.Read(MapData[0], 0, 512 * 512);
                tempFS.Read(MapData[1], 0, 512 * 512);
                tempFS.Close();
            }

            return MapData;
        }




    }
   
   

}
