

using LibMCRcon.Nbt;
using System.Collections.Generic;

namespace LibMCRcon.WorldData
{
    public class Region : Voxel
    {

        NbtChunk nbtChunk;
        NbtChunkSection[] nbtChunkSection = new NbtChunkSection[16];

        int lastChunkIdx = int.MaxValue;
        int lastYSect = int.MaxValue;


        public Voxel Chunk { get; private set; }

        public Region() : base() => Chunk = OffsetVoxel(16, 16); 
        public Region(Voxel Voxel) : base(Voxel) => Chunk = OffsetVoxel(16, 16); 

        public Region(int YSize, int XZSize) : base(YSize, XZSize) => Chunk = OffsetVoxel(16, 16);
        public Region(Voxel Voxel, int XZSize) : base(Voxel, XZSize) => Chunk = OffsetVoxel(16, 16);
       


        private void ChunkLoad(RegionMCA mca, Voxel Chunk)
        {
            int idx = Chunk.ChunkIdx();
            if (idx != lastChunkIdx)
            {
                nbtChunk = new NbtChunk(mca[idx].chunkNBT);
                lastYSect = int.MaxValue;
                lastChunkIdx = idx;
            }


        }

        private void ChunkYSectLoad(RegionMCA mca, Voxel Chunk)
        {
            int idx = Chunk.Ys;

            if (idx != lastYSect)
            {
                NbtCompound nbtComp = nbtChunk.Section(idx);
                nbtChunkSection[idx] = new NbtChunkSection(nbtComp);
                lastYSect = idx;
            }
        }

        public NbtChunk NbtChunk(RegionMCA mca, Voxel Chunk)
        {

            ChunkLoad(mca, Chunk);
            return nbtChunk;

        }
        public NbtChunk NbtChunk(RegionMCA mca)
        {

            ChunkLoad(mca, Chunk);
            return nbtChunk;
        }
        public NbtChunkSection NbtChunkSection(RegionMCA mca, Voxel Chunk)
        {

            ChunkLoad(mca, Chunk);
            ChunkYSectLoad(mca, Chunk);

            return nbtChunkSection[lastYSect];
        }
        public NbtChunkSection NbtChunkSection(RegionMCA mca)
        {

            ChunkLoad(mca, Chunk);
            ChunkYSectLoad(mca, Chunk);

            return nbtChunkSection[lastYSect];
        }
        public NbtChunkSection NbtChunkLastSection() => nbtChunkSection[lastYSect];

        public void RefreshChunk() => Chunk.SetVoxel(WorldY, Xo, Zo, 16, 16);
        public void MergeChunk() => SetOffset(Chunk);

        public void ResetData()
        {
            nbtChunk = null;
            nbtChunkSection = new NbtChunkSection[16];
            lastChunkIdx = int.MaxValue;
            lastYSect = int.MaxValue;


            Xo = 0;
            Zo = 0;
            WorldY = 0;

            Chunk = OffsetVoxel(16, 16);
        }

    }
}