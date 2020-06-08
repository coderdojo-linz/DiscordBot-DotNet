using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LibMCRcon.Nbt;

namespace LibMCRcon.WorldData
{
    public class RegionMCAEx : WorldVoxelEx
    {
      
        private Int32[] chunkhdr { get; set; }  = new Int32[1024];
        private Int32[] chunksect { get; set; } = new Int32[1024];
        private DateTime[] timehdr { get; set; } = new DateTime[1024];

        ChunkMCAEx[] chunks { get; set; } = new ChunkMCAEx[1024];

        int lastX = -1;
        int lastZ = -1;

        public int Count { get; set; }

        string mcaFilePath { get; set; } = string.Empty;

        public DateTime LastModified { get; set; }

        public RegionMCAEx()
        {

            lastX = int.MaxValue;
            lastZ = int.MaxValue;



        }

        public RegionMCAEx(string regionPath)
            : this()
        {
            mcaFilePath = regionPath;
        }

        public void SaveRegion()
        {
            if (lastX == -1 && lastZ == -1)
                return;


        }

        public void LoadRegion() { LoadRegion(Xs,Zs); }
        public void LoadRegion(int RegionX, int RegionZ)
        {
            R = 2;
            Xs = RegionX; Zs = RegionZ;

            if (lastX != RegionX || lastZ != RegionZ)
            {
                lastX = RegionX;
                lastZ = RegionZ;

                FileInfo f = new FileInfo(Path.Combine(mcaFilePath, $@"r.{lastX}.{lastZ}.mca"));

                LastModified = DateTime.MaxValue;

                if (f.Exists)
                {
                    LastModified = f.LastWriteTime;

                    for (int c = 0; c < 1024; c++)
                        chunks[c] = null;

                    Count = 0;


                    using (FileStream fs = f.OpenRead())
                    {

                        for (int c = 0; c < 1024; c++)
                        {
                            chunkhdr[c] = NbtReader.TagInt24(fs) * 4096;
                            chunksect[c] = NbtReader.TagByte(fs) * 4096;
                        }

                        for (int c = 0; c < 1024; c++)
                            timehdr[c] = DateTime.FromBinary(NbtReader.TagInt(fs));


                        for (int c = 0; c < 1024; c++)
                        {

                            try
                            {
                                fs.Seek(chunkhdr[c], SeekOrigin.Begin);
                                chunks[c] = new ChunkMCAEx(chunkhdr[c], chunksect[c], fs);
                                Count += 1;
                            }
                            catch (Exception)
                            {
                                break;
                            }

                        }

                        fs.Close();
                    }

                }
            }

        }


        public int ChunkIdx()
        {
            throw (new Exception("Not Implemented"));
        }



        public ChunkMCAEx this[int index]
        {
            get
            {
                if (Count == 0 || Count < index)
                    return null;
                return chunks[index];
            }
        }
        public ChunkMCAEx this[int ChunkX, int ChunkZ]
        {
            get
            {
                return this[(ChunkZ * 32) + ChunkX];
            }
        }

        NbtChunk nbtChunk;
        NbtChunkSection[] nbtChunkSection = new NbtChunkSection[16];



        public bool IsLoaded { get { return Count > 0; } }


    }
}
