using System;
using System.Collections.Generic;
using System.Drawing;

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

        private int rbs_idx = 0;
        private int rbs_next = 0;

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
                    foreach (var kv in BlockPalettes)
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

        public static BlockColors Create()
        {
            var bc = new BlockColors(); bc.InitBlockColors(); return bc;
        }
    }
}