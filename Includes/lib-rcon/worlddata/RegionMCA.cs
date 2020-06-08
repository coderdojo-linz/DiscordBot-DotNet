
using System;
using System.IO;
using LibMCRcon.Nbt;

namespace LibMCRcon.WorldData
{
    public class RegionMCA:Region
    {

        private Int32[] chunkhdr = new Int32[1024];
        private Int32[] chunksect = new Int32[1024];
        private DateTime[] timehdr = new DateTime[1024];

        ChunkMCA[] chunks = new ChunkMCA[1024];

        int lastX = -1;
        int lastZ = -1;

        public int Count { get; set; }

        string mcaFilePath = string.Empty;

        public DateTime LastModified { get; set; }

        public RegionMCA()
        {

            lastX = int.MaxValue;
            lastZ = int.MaxValue;
         


        }

        public RegionMCA(string regionPath)
            : this()
        {
            mcaFilePath = regionPath;
        }

        public void SaveRegion()
        {
            if (lastX == -1 && lastZ == -1)
                return;


        }

        public void LoadRegion() { LoadRegion(Xs, Zs); }

        public void LoadRegion(Stream Data, int RegionX, int RegionZ, DateTime Modified)
        {
            SetSegmentOffset(RegionX, RegionZ, 0, 0);

            if (lastX != RegionX || lastZ != RegionZ)
            {
                lastX = RegionX;
                lastZ = RegionZ;

                LastModified = Modified;


                for (int c = 0; c < 1024; c++)
                    chunks[c] = null;

                Count = 0;


                using (Stream fs = Data)
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
                            chunks[c] = new ChunkMCA(chunkhdr[c], chunksect[c], fs);
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

        public void LoadRegion(int RegionX, int RegionZ)
        {
            SetSegmentOffset(RegionX, RegionZ, 0, 0);

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
                                chunks[c] = new ChunkMCA(chunkhdr[c], chunksect[c], fs);
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

        public ChunkMCA this[int index]
        {
            get
            {
                if (Count == 0 || Count < index)
                    return null;
                return chunks[index];
            }
        }
        public ChunkMCA this[int ChunkX, int ChunkZ]
        {
            get
            {
                return this[(ChunkZ * 32) + ChunkX];
            }
        }

        public bool IsLoaded { get { return Count > 0; } }


    }
}