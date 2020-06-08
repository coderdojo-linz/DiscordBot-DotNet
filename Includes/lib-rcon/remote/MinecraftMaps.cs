using System.Collections.Generic;
using System.Drawing;
using System.IO;
using LibMCRcon.WorldData;
using System;
using System.Diagnostics;
using System.Threading.Tasks;



namespace LibMCRcon.Remote
{
    public static class MinecraftRender
    {
        public async static Task<byte[][]> hdt(MCTransferAsync data, int worldX, int worldZ)
        {
            var w = new WorldVoxelEx() { R = 1, X = worldX, Z = worldZ };

            byte[][] map = new byte[2][];

            var hd = new byte[512 * 512];

            var hdt = $"r.{w.Xs}.{w.Zs}.hdt";

            using (var mem = new MemoryStream())
            {

                if (await data.TransferNext(hdt, mem, TxRx.RECEIVE))
                {
                    mem.Position = 0;

                    mem.Read(hd, 0, 512 * 512);
                    map[0] = hd;

                    hd = new byte[512 * 512];
                    mem.Read(hd, 0, 512 * 512);
                    map[1] = hd;
                }
                else
                {
                    map[0] = new byte[512 * 512];
                    map[1] = new byte[512 * 512];
                }
            }

            return map;
        }

        public static Bitmap GetImg(string filename, Size Scale)
        {
            Bitmap fb;
            Bitmap b;

            if (File.Exists(filename))
            {

                fb = new Bitmap(filename);
                b = new Bitmap(fb, Scale);
                fb.Dispose();
            }
            else
                b = new Bitmap(Scale.Width, Scale.Height);

            return b;

        }
        public static Bitmap GetImg(MemoryStream ms, MCTransfer transfer, string ImgPath, string filename, Size Scale)
        {
            Bitmap fb;
            Bitmap b;

            var localFileName = Path.Combine(ImgPath, filename);

            if (File.Exists(localFileName))
            {
                using (var fs = File.Open(localFileName, FileMode.Open))
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;
                }

                
                fb = new Bitmap(ms);
                b = new Bitmap(fb, Scale);
                fb.Dispose();
                return b;
            }

            
                if (true == transfer.TransferNext(filename, ms, TxRx.RECEIVE))
                {
                    fb = new Bitmap(ms);
                    b = new Bitmap(fb, Scale);
                    
                    fb.Dispose();
                }
                else
                    b = new Bitmap(Scale.Width, Scale.Height);

            return b;
        }
        public static async Task<Bitmap> GetImgAsync(MemoryStream ms, MCTransferAsync transfer, string ImgPath, string filename, Size Scale)
        {
            Bitmap fb;
            Bitmap b;

            var localFileName = Path.Combine(ImgPath, filename);

            if (File.Exists(localFileName))
            {
                using (var fs = File.Open(localFileName, FileMode.Open))
                {
                    fs.CopyTo(ms);
                    ms.Position = 0;
                }


                fb = new Bitmap(ms);
                b = new Bitmap(fb, Scale);
                fb.Dispose();
                return b;
            }


            if (true == await transfer.TransferNext(filename, ms, TxRx.RECEIVE))
            {
                fb = new Bitmap(ms);
                b = new Bitmap(fb, Scale);

                fb.Dispose();
            }
            else
                b = new Bitmap(Scale.Width, Scale.Height);

            return b;
        }

        public static int WorldRegionIndex(int Region) => (int)(Math.Log(Region) / Math.Log(2));
        public static int IndexToWorldRegion(int RegionIndex) => (int)(Math.Pow(2, RegionIndex));

        public static bool RenderTopo(Rendering.BlockColors bc, WorldVoxelEx wv, string RegionsDirectory, string ImgsDirectory, Stream Data, DateTime DataModified)
        {

            if (wv.R > 1)
                return false;

            byte[][] MapData = new byte[][] { new byte[512 * 512], new byte[512 * 512] };
            Color[] BlockData = new Color[512 * 512];

            RegionMCA mca = new RegionMCA();


            mca.LoadRegion(Data, wv.Xs, wv.Zs, DataModified);

            try
            {
                Rendering.MCRegionMaps.RenderDataFromRegion(bc, mca, MapData, BlockData);
                Rendering.MCRegionMaps.RenderTopoPngFromRegion(MapData, ImgsDirectory, wv.Xs, wv.Zs);
                Rendering.MCRegionMaps.RenderBlockPngFromRegion(MapData, BlockData, ImgsDirectory, wv.Xs, wv.Zs);


                FileInfo mcaH = new FileInfo(Path.Combine(RegionsDirectory, $"r{wv.BaseFilename()}.hdt"));

                using (FileStream tempFS = mcaH.Create())
                {

                    tempFS.Write(MapData[0], 0, 512 * 512);
                    tempFS.Write(MapData[1], 0, 512 * 512);
                    tempFS.Flush();
                    tempFS.Close();

                }

                mcaH.LastWriteTime = mca.LastModified;

                FileInfo lwFS = new FileInfo(Path.Combine(ImgsDirectory, $"topo{wv.BaseFilename()}.png"));
                if (lwFS.Exists)
                    lwFS.LastWriteTime = mca.LastModified;

                lwFS = new FileInfo(Path.Combine(ImgsDirectory, $"tile{wv.BaseFilename()}.png"));
                if (lwFS.Exists)
                    lwFS.LastWriteTime = mca.LastModified;

                return true;

            }
            catch (Exception ex)
            {

                // Debug.Print(ex.Message);
                return false;
            }

        }

        public static bool RenderTopo(Rendering.BlockColors bc, WorldVoxelEx wv, string RegionDirectory, string ImgsDirectory)
        {
            
            if (wv.R > 1)
                return false;

            byte[][] MapData = new byte[][] { new byte[512 * 512], new byte[512 * 512] };
            Color[] BlockData = new Color[512 * 512];

            RegionMCA mca = new RegionMCA(RegionDirectory);


            mca.LoadRegion(wv.Xs, wv.Zs);

            try
            {
                Rendering.MCRegionMaps.RenderDataFromRegion(bc, mca, MapData, BlockData);
                Rendering.MCRegionMaps.RenderTopoPngFromRegion(MapData, ImgsDirectory, wv.Xs, wv.Zs);
                Rendering.MCRegionMaps.RenderBlockPngFromRegion(MapData, BlockData, ImgsDirectory, wv.Xs, wv.Zs);
               

                FileInfo mcaH = new FileInfo(Path.Combine(RegionDirectory, $"r{wv.BaseFilename()}.hdt"));

                using (FileStream tempFS = mcaH.Create())
                {

                    tempFS.Write(MapData[0], 0, 512 * 512);
                    tempFS.Write(MapData[1], 0, 512 * 512);
                    tempFS.Flush();
                    tempFS.Close();

                }

                mcaH.LastWriteTime = mca.LastModified;

                FileInfo lwFS = new FileInfo(Path.Combine(ImgsDirectory, $"topo{wv.BaseFilename()}.png"));
                if (lwFS.Exists)
                    lwFS.LastWriteTime = mca.LastModified;

                lwFS = new FileInfo(Path.Combine(ImgsDirectory, $"tile{wv.BaseFilename()}.png"));
                if (lwFS.Exists)
                    lwFS.LastWriteTime = mca.LastModified;

                return true;

            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }

        }


        public static void Stitch(WorldVoxelEx wv, string ImgPath, MCTransfer Tiles, MCTransfer Maps, int Regions = 2, bool TransferBack = true)
        {

            var map = wv.CopyToWorld(Regions);

            using (var ms1 = new MemoryStream())
            using (var ms2 = new MemoryStream())
            {

                void MakeImage(string ImgType)
                {

                    var FileName = $"{ImgType}{map.BaseFilename()}.png";
                    var LocalFileName = Path.Combine(ImgPath, FileName);

                    var b = GetImg(ms1, Maps, ImgPath, FileName, new Size(512, 512));
                    var g = Graphics.FromImage(b);

                    var img = wv.CopyToWorld(Regions);
                    var pix = WorldVoxelEx.PixelSize(img.R, wv.R);

                    var p = GetImg(ms2, (wv.R == 1) ? Tiles : Maps, ImgPath, $"{ImgType}{wv.BaseFilename()}.png", new Size(pix, pix));

                    g.DrawImage(p, img.Xo, img.Zo, pix, pix);
                    p.Dispose();

                    b.Save(LocalFileName);

                    if (TransferBack == true)
                    {
                        using (var mem = new MemoryStream())
                        {
                            b.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
                            mem.Position = 0;

                            Maps.TransferNext(FileName, mem, TxRx.SEND);
                        }
                    }

                    b.Dispose();
                    g.Dispose();

                }

                MakeImage("tile");
                MakeImage("topo");

            }

        }
        public static async Task StitchAsync(WorldVoxelEx wv, string ImgPath, MCTransferAsync Tiles, MCTransferAsync Maps, int Regions = 2, bool TransferBack = true)
        {

            var map = wv.CopyToWorld(Regions);

            async Task MakeImage(string ImgType)
            {
                try
                {
                    using (var ms1 = new MemoryStream())
                    using (var ms2 = new MemoryStream())
                    {
                        var FileName = $"{ImgType}{map.BaseFilename()}.png";
                        var LocalFileName = Path.Combine(ImgPath, FileName);

                        var b = await GetImgAsync(ms1, Maps, ImgPath, FileName, new Size(512, 512));
                        var g = Graphics.FromImage(b);

                        var img = wv.CopyToWorld(Regions);
                        var pix = WorldVoxelEx.PixelSize(img.R, wv.R);

                        var p = await GetImgAsync(ms2, (wv.R == 1) ? Tiles : Maps, ImgPath, $"{ImgType}{wv.BaseFilename()}.png", new Size(pix, pix));

                        g.DrawImage(p, img.Xo, img.Zo, pix, pix);
                        p.Dispose();

                        b.Save(LocalFileName);

                        if (TransferBack == true)
                        {
                            using (var mem = new MemoryStream())
                            {

                                b.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
                                mem.Position = 0;

                                await Maps.TransferNext(FileName, mem, TxRx.SEND);
                            }
                        }

                        b.Dispose();
                        g.Dispose();
                    }
                }
                catch (Exception ex)
                {

                }




            }

            await Task.WhenAll(MakeImage("tile"), MakeImage("topo"));


        }

        public static bool RenderTiles(string RegionDirectory, string ImgsDirectory, Process TogosJavaProc)
        {
            //TogosJavaProc should be set to run Togos TCMCR region tile generator

            TogosJavaProc.StartInfo.Arguments = $@"-jar {Path.Combine(TogosJavaProc.StartInfo.WorkingDirectory, "tmcmr.jar")} -o {ImgsDirectory} {RegionDirectory}\*.mca";
            var started = TogosJavaProc.Start();

            if (started)
                TogosJavaProc.WaitForExit();

            return !started;
        }
    }
    public class MinecraftWorldRender:WorldVoxelEx
    {
        public MinecraftWorldRender(int Xs, int Zs, int Region) : base(Xs, Zs, Region) { }
        public MinecraftWorldRender() : base() { }

        public bool Rendered { get; set; } = false;
        public MinecraftWorldRender ToWorldRender(int Regions) => new MinecraftWorldRender() { R = Regions, X = X, Z = Z, Y = Y };

        public DateTime Modified { get; set; }

    }
    public class MinecraftMapsTransferBatch
    {
        public List<WorldVoxelEx> Transfered { get; set; }
        public string LastStage { get; set; }
        public DateTime StateDT { get; set; }
    }

    public class MinecraftMapsUpload
    {
        public WorldVoxelEx W { get; set; }
        public DateTime RequestDT { get; set; }

    }

    public class MinecraftMaps
    {
        
        public List<WorldVoxelEx> SourceList { get; set; }
        public int MapR => SourceList[0].R;
               
        public bool DL { get; set; }
        public bool UP { get; set; }
        public bool Component { get; set; }
        public bool Completed { get; set; }

        public bool Rendered = false;
        public string RenderError = null;

        public void ScaleOut(int Regions)
        {
            var worldScale = SourceList[0].CopyToWorld(Regions);
            SourceList.Clear();
            SourceList.Add(worldScale);
        }

        #region "Image Processing"
        public int WorldScaleIndex()
        {
            return (int)(Math.Log(SourceList[0].R) / Math.Log(2));
        }
        public int PixelSizeScaleOut(int WorldR)
        {
            var currentIdx = (int)(Math.Log(MapR) / Math.Log(2));
            var scaleIdx = (int)(Math.Log(WorldR) / Math.Log(2));
            var upR = (int)(Math.Pow(2, scaleIdx - currentIdx));
            return 512 / upR;

        }

        public static bool RenderTiles(string RegionDirectory, string ImgsDirectory, Process TogosJavaProc)
        {

            TogosJavaProc.StartInfo.Arguments = $@"-jar {Path.Combine(TogosJavaProc.StartInfo.WorkingDirectory, "tmcmr.jar")} -o {ImgsDirectory} {RegionDirectory}\*.mca";
            var started = TogosJavaProc.Start();

            if (started)
                TogosJavaProc.WaitForExit();

            return started;
        }

        public bool SyncAnvilToRender(Rendering.BlockColors bc, string RegionDirectory, string ImgsDirectory)
        {
            int renderCount = 0;

            if (SourceList[0].R == 1)
            {
                foreach (var mf in SourceList)
                {

                    FileInfo mca = new FileInfo(Path.Combine(RegionDirectory, $"r{mf.BaseFilename()}.mca"));
                    FileInfo lwFStile = new FileInfo(Path.Combine(ImgsDirectory, $"tile{mf.BaseFilename()}.png"));
                    FileInfo lwFStopo = new FileInfo(Path.Combine(ImgsDirectory, $"topo{mf.BaseFilename()}.png"));

                    if (mca.Exists)
                    {
                        if (lwFStile.Exists)
                        {
                            lwFStile.LastWriteTime = mca.LastWriteTime;
                            if (lwFStopo.Exists ? true : RenderTopo(bc,mf, RegionDirectory, ImgsDirectory))
                                ++renderCount;

                        }
                        else if (!lwFStopo.Exists)
                        {
                            RenderTopo(bc,mf, RegionDirectory, ImgsDirectory);
                        }

                    }

                }
            }

            if (renderCount == SourceList.Count)
                return true;
            else
                return false;


        }

        public void Stitch(string ImgPath,int Regions = 2)
        {
 
            void MakeImage(string ImgType)
            {

                var map = SourceList[0].CopyToWorld(Regions);

                var FileName = Path.Combine(ImgPath, $"{ImgType}{map.BaseFilename()}.png");
                var b = GetImg(FileName, new Size(512, 512));
                var g = Graphics.FromImage(b);

                foreach (var src in SourceList)
                {
                    map = src.CopyToWorld(Regions);
                    
                    var p = GetImg(Path.Combine(ImgPath, $"{ImgType}{src.BaseFilename()}.png"), new Size(256, 256));
                    g.DrawImage(p, map.Xo, map.Zo, 256, 256);
                    p.Dispose();

                }

                b.Save(FileName);
                b.Dispose();
                g.Dispose();
            }


            MakeImage("tile");
            MakeImage("topo");
        }
        public void Stitch(string ImgPath,MCTransfer Tiles, MCTransfer Maps, int Regions = 2, bool TransferBack = true)
        {
           
            var map = SourceList[0].CopyToWorld(Regions);

            void MakeImage(string ImgType)
            {
                var FileName = $"{ImgType}{map.BaseFilename()}.png";
                var LocalFileName = Path.Combine(ImgPath, FileName);
                
                var b = GetImg(Maps, ImgPath, FileName, new Size(512, 512));
                var g = Graphics.FromImage(b);

                foreach (var src in SourceList)
                {
                    map = src.CopyToWorld(Regions);

                    var p = GetImg((src.R == 1) ? Tiles : Maps, ImgPath, $"{ImgType}{src.BaseFilename()}.png", new Size(256, 256));

                    g.DrawImage(p, map.Xo, map.Zo, 256, 256);
                    p.Dispose();
                }

                b.Save(LocalFileName);

                if (TransferBack == true)
                {
                    using (var mem = new MemoryStream())
                    {
                        b.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
                        mem.Position = 0;
                       
                        Maps.TransferNext(FileName, mem, TxRx.SEND);
                    }
                }


                b.Dispose();
                g.Dispose();
            }

            MakeImage("tile");
            MakeImage("topo");

        }

        public static bool RenderTopo(Rendering.BlockColors bc, WorldVoxelEx mr, string RegionDirectory, string ImgsDirectory)
        {

            if (mr.R > 1)
                return false;

            byte[][] MapData = new byte[][] { new byte[512 * 512], new byte[512 * 512] };
            Color[] BlockData = new Color[512 * 512];

            RegionMCA mca = new RegionMCA(RegionDirectory);


            mca.LoadRegion(mr.Xs, mr.Zs);

            Rendering.MCRegionMaps.RenderDataFromRegion(bc, mca, MapData, BlockData);
            Rendering.MCRegionMaps.RenderTopoPngFromRegion(MapData, ImgsDirectory, mr.Xs, mr.Zs);
            //LibMCRcon.Rendering.MCRegionMaps.RenderBlockPngFromRegion(MapData, BlockData, ImgsDir.FullName, RV);


            FileInfo mcaH = new FileInfo(Path.Combine(RegionDirectory, $"r{mr.BaseFilename()}.hdt"));

            using (FileStream tempFS = mcaH.Create())
            {

                tempFS.Write(MapData[0], 0, 512 * 512);
                tempFS.Write(MapData[1], 0, 512 * 512);
                tempFS.Flush();
                tempFS.Close();

            }

            mcaH.LastWriteTime = mca.LastModified;

            FileInfo lwFS = new FileInfo(Path.Combine(ImgsDirectory, $"topo{mr.BaseFilename()}.png"));
            if (lwFS.Exists)
                lwFS.LastWriteTime = mca.LastModified;

            return true;



        }

        private Bitmap GetImg(string filename, Size Scale)
        {
            Bitmap fb;
            Bitmap b;

            if (File.Exists(filename))
            {

                fb = new Bitmap(filename);
                b = new Bitmap(fb, Scale);
                fb.Dispose();
            }
            else
                b = new Bitmap(Scale.Width, Scale.Height);

            return b;

        }
        private Bitmap GetImg(MCTransfer transfer, string ImgPath,  string filename, Size Scale)
        {
            Bitmap fb;
            Bitmap b;

            var localFileName = Path.Combine(ImgPath, filename);

            if (File.Exists(localFileName))
            {

                fb = new Bitmap(localFileName);
                b = new Bitmap(fb, Scale);
                fb.Dispose();
                return b;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                if (true == transfer.TransferNext(filename, ms, TxRx.RECEIVE))
                {
                    fb = new Bitmap(ms);
                    b = new Bitmap(fb, Scale);
                    fb.Dispose();
                }
                else
                    b = new Bitmap(Scale.Width, Scale.Height);

            }
            return b;
        }
        #endregion
    }

     
   




}