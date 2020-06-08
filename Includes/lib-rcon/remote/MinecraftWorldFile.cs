using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using LibMCRcon.WorldData;

namespace LibMCRcon.Remote
{
    //public class MinecraftWorldFile : MinecraftFile
    //{

    //    public List<MinecraftFile> Regions { get; private set; }
    //    public List<MinecraftWorldFile> WorldMaps { get; private set; }

    //    public void InitRegionList()
    //    {
    //        Regions = new List<MinecraftFile>();
    //    }
    //    public void InitWorldMapList()
    //    {
    //        WorldMaps = new List<MinecraftWorldFile>();
    //    }

    //    public void InitRegionList(MinecraftFile AddMinecraftFile)
    //    {
    //        Regions = new List<MinecraftFile>
    //        {
    //            AddMinecraftFile
    //        };
    //    }

    //    public MinecraftWorldFile()
    //    {
    //        RegionsPerWorld = 8;
    //        SetVoxel(0, 0, 0, int.MaxValue, BlocksPerWorld);
    //        MCKind = MineCraftRegionFileKind.TILE;

    //    }

    //    public MinecraftWorldFile(int RegionsPerWorld)
    //    {

    //        this.RegionsPerWorld = RegionsPerWorld;
    //        SetVoxel(0, 0, 0, int.MaxValue, BlocksPerWorld);
    //        MCKind = MineCraftRegionFileKind.TILE;
    //    }

    //    public MinecraftWorldFile(string FileName) : base(FileName) { }


    //    public void ForEachRegionInWorldMaps(Action<MinecraftFile> action)
    //    {
    //        if (WorldMaps != null)
    //            foreach (var w in WorldMaps)
    //                if (w.Regions != null)
    //                    w.Regions.ForEach(action);
    //    }


    //    public void CollectWorlds(List<MinecraftWorldFile> WorldList)
    //    {

    //        InitWorldMapList();
    //        var w = new MinecraftWorldFile(RegionsPerWorld);

    //        void actCollect(MinecraftFile x)
    //        {

    //            if (x.IsValid)
    //            {

    //                w.SetVoxel(0, x.WorldX, x.WorldZ);
    //                var fw = WorldMaps.Find((f) => f.Xs == w.Xs && f.Zs == w.Zs);

    //                if (fw == null)
    //                {

    //                    WorldMaps.Add(w);
    //                    w.InitRegionList(x);
    //                    w = new MinecraftWorldFile(RegionsPerWorld);

    //                }
    //                else
    //                {
    //                    //found a worldfile, so check to see if worldfile has minecraft file

    //                    var mf = x.IsOfMCKind(MineCraftRegionFileKind.POI) ? fw.Regions.Find(f => f.WorldX == x.WorldX && f.WorldZ == x.WorldZ) : fw.Regions.Find(f => f.Xs == x.Xs && f.Zs == x.Zs);
    //                    if (mf == null)
    //                        fw.Regions.Add(x);

    //                }

    //            }
    //        }


    //        WorldList.ForEach(actCollect);
    //    }

    //    public void CollectWorldFiles(bool FromRemote = false, List<MinecraftFile> Source = null, MineCraftRegionFileKind MCKind = MineCraftRegionFileKind.MCA)
    //    {

    //        InitWorldMapList();

    //        var w = new MinecraftWorldFile(RegionsPerWorld);

    //        void actCollect(MinecraftFile x)
    //        {

    //            if (x.IsValid)
    //            {

    //                w.SetVoxel(0, x.WorldX, x.WorldZ);
    //                var fw = WorldMaps.Find((f) => f.Xs == w.Xs && f.Zs == w.Zs);

    //                if (fw == null)
    //                {

    //                    WorldMaps.Add(w);
    //                    w.InitRegionList(x);
    //                    w = new MinecraftWorldFile(RegionsPerWorld);

    //                }
    //                else
    //                {
    //                    //found a worldfile, so check to see if worldfile has minecraft file

    //                    var mf = x.IsOfMCKind(MineCraftRegionFileKind.POI) ? fw.Regions.Find(f => f.WorldX == x.WorldX && f.WorldZ == x.WorldZ) : fw.Regions.Find(f => f.Xs == x.Xs && f.Zs == x.Zs);
    //                    if (mf == null)
    //                        fw.Regions.Add(x);

    //                }

    //            }
    //        }

    //        if (Source == null)
    //            Source = (FromRemote == false) ? localMClist : remoteMClist;

    //        var SourceList = Source.FindAll(x => x.IsOfMCKind(MCKind));

    //        if (SourceList != null)
    //            SourceList.ForEach(actCollect);

    //    }

    //    public void RecreateWorldFile(string localpath, MinecraftTransfer UP_R, MinecraftTransfer UP_W)
    //    {


    //        var MF = new MinecraftFile(this, RegionsPerWorld >> 1);

    //        var Q1 = new MinecraftFile(MF.Xs, MF.Zs, 0, 0, MF.RegionsPerWorld, MineCraftRegionFileKind.MCA);
    //        var Q2 = new MinecraftFile(MF.Xs + 1, MF.Zs, 0, 0, MF.RegionsPerWorld, MineCraftRegionFileKind.MCA);
    //        var Q3 = new MinecraftFile(MF.Xs, MF.Zs + 1, 0, 0, MF.RegionsPerWorld, MineCraftRegionFileKind.MCA);
    //        var Q4 = new MinecraftFile(MF.Xs + 1, MF.Zs + 1, 0, 0, MF.RegionsPerWorld, MineCraftRegionFileKind.MCA);

    //        CollectWorldFiles(Source: new List<MinecraftFile>() { Q1, Q2, Q3, Q4 });
    //        WorldMaps.ForEach(x => x.CreateMapFile(UP_R, UP_W, localpath, x.Regions[0].RegionsPerWorld));

    //    }

    //    public void CollectWorldFilesAndCreateMapFile(IEnumerable<FileData> Files, string localpath, MinecraftTransfer UP_R, MinecraftTransfer UP_W)
    //    {
    //        InitWorldMapList();

    //        var w = new MinecraftWorldFile(RegionsPerWorld);

    //        void actCollect(MinecraftFile x)
    //        {

    //            if (x.IsValid)
    //            {

    //                w.SetVoxel(0, x.WorldX, x.WorldZ);
    //                var fw = WorldMaps.Find((f) => f.Xs == w.Xs && f.Zs == w.Zs);

    //                if (fw == null)
    //                {

    //                    WorldMaps.Add(w);
    //                    w.InitRegionList(x);
    //                    w = new MinecraftWorldFile(RegionsPerWorld);

    //                }
    //                else
    //                {
    //                    //found a worldfile, so check to see if worldfile has minecraft file

    //                    var mf = x.IsOfMCKind(MineCraftRegionFileKind.POI) ? fw.Regions.Find(f => f.WorldX == x.WorldX && f.WorldZ == x.WorldZ) : fw.Regions.Find(f => f.Xs == x.Xs && f.Zs == x.Zs);
    //                    if (mf == null)
    //                        fw.Regions.Add(x);

    //                }

    //            }
    //        }

    //        foreach (var f in Files)
    //        {
    //            actCollect(new MinecraftFile(f.filename) { Xo = 0, Zo = 0 });

    //        }

    //        WorldMaps.ForEach(x => x.CreateMapFile(UP_R, UP_W, localpath, x.Regions[0].RegionsPerWorld));

    //    }

    //    public void CreateMapFiles(string localpath)
    //    {

    //        if (WorldMaps == null) return;

    //        if (WorldMaps.Count > 0)
    //        {

    //            int minX = int.MaxValue;
    //            int minZ = int.MaxValue;
    //            int maxX = int.MinValue;
    //            int maxZ = int.MinValue;
    //            int X = 0;
    //            int Z = 0;

    //            foreach (MinecraftWorldFile WF in WorldMaps)
    //            {
    //                if (WF.Xs < minX)
    //                    minX = WF.Xs;

    //                if (WF.Xs > maxX)
    //                    maxX = WF.Xs;

    //                if (WF.Zs < minZ)
    //                    minZ = WF.Zs;

    //                if (WF.Zs > maxZ)
    //                    maxZ = WF.Zs;

    //            }

    //            X = Math.Abs(minX - maxX);
    //            Z = Math.Abs(minZ - maxZ);

    //            WorldMaps.ForEach(x => x.CreateMapFile(localpath));

    //        }

    //    }

    //    public void CreateMapFile(string localpath)
    //    {

    //        if (Regions == null) return;

    //        Brush SolidBrush = new SolidBrush(Color.Black);

    //        Bitmap topo;
    //        Bitmap tile;

    //        if (Regions.Count > 0)
    //        {

    //            var v = new Voxel(int.MaxValue, BlocksPerWorld) { WorldY = 0 };
    //            var p = new Voxel(int.MaxValue, WorldRegionPixelSize) { WorldY = 0 }; ;

    //            topo = new Bitmap(WorldRegionPixelSize, WorldRegionPixelSize);
    //            tile = new Bitmap(WorldRegionPixelSize, WorldRegionPixelSize);

    //            Image img = null;

    //            Graphics gTopo = Graphics.FromImage(topo);
    //            Graphics gTile = Graphics.FromImage(tile);

    //            gTopo.FillRectangle(SolidBrush, 0, 0, WorldRegionPixelSize, WorldRegionPixelSize);
    //            gTile.FillRectangle(SolidBrush, 0, 0, WorldRegionPixelSize, WorldRegionPixelSize);


    //            foreach (MinecraftFile mf in Regions)
    //            {

    //                v.SetVoxel(0, mf.WorldX, mf.WorldZ);
    //                p.SetVoxel(0, v.Xo, v.Zo);


    //                img = Image.FromFile(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TOPOX, mf.Xs, mf.Zs)));
    //                gTopo.DrawImage(img, (p.Xs * PixelsPerRegion), (p.Zs * PixelsPerRegion), PixelsPerRegion, PixelsPerRegion);
    //                img.Dispose();

    //                img = Image.FromFile(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TILEX, mf.Xs, mf.Zs)));
    //                gTile.DrawImage(img, (p.Xs * PixelsPerRegion), (p.Zs * PixelsPerRegion), PixelsPerRegion, PixelsPerRegion);
    //                img.Dispose();
    //            }

    //            string TopoFile = Path.Combine(localpath, FileName(MineCraftRegionFileKind.TOPO));
    //            string TileFile = Path.Combine(localpath, FileName(MineCraftRegionFileKind.TILE));

    //            topo.Save(TopoFile, System.Drawing.Imaging.ImageFormat.Png);
    //            tile.Save(TileFile, System.Drawing.Imaging.ImageFormat.Png);

    //            gTopo.Dispose();
    //            gTile.Dispose();
    //            topo.Dispose();
    //            tile.Dispose();

    //        }




    //    }
    //    public void CreateMapFile(string localpath, int RegionsPerWorld)
    //    {

    //        if (Regions == null) return;

    //        Brush SolidBrush = new SolidBrush(Color.Black);

    //        Bitmap topo;
    //        Bitmap tile;

    //        if (Regions.Count > 0)
    //        {

    //            var PixelsPerRegion = 512 * RegionsPerWorld;
    //            var PixelsPerWorldRegion = 512 / (this.RegionsPerWorld / RegionsPerWorld);


    //            var v = new Voxel(int.MaxValue, BlocksPerWorld) { WorldY = 0 };
    //            var p = new Voxel(int.MaxValue, PixelsPerRegion) { WorldY = 0 };

    //            topo = new Bitmap(this.WorldRegionPixelSize, this.WorldRegionPixelSize);
    //            tile = new Bitmap(this.WorldRegionPixelSize, this.WorldRegionPixelSize);

    //            Image img = null;

    //            Graphics gTopo = Graphics.FromImage(topo);
    //            Graphics gTile = Graphics.FromImage(tile);

    //            gTopo.FillRectangle(SolidBrush, 0, 0, this.WorldRegionPixelSize, this.WorldRegionPixelSize);
    //            gTile.FillRectangle(SolidBrush, 0, 0, this.WorldRegionPixelSize, this.WorldRegionPixelSize);


    //            foreach (MinecraftFile mf in Regions)
    //            {

    //                v.SetVoxel(0, mf.WorldX, mf.WorldZ);
    //                p.SetVoxel(0, v.Xo, v.Zo);

    //                if (RegionsPerWorld > 1)
    //                    img = Image.FromFile(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TOPO, mf.Xs, mf.Zs, RegionsPerWorld)));
    //                else
    //                    img = Image.FromFile(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TOPOX, mf.Xs, mf.Zs, RegionsPerWorld)));

    //                gTopo.DrawImage(img, (p.Xs * PixelsPerWorldRegion), (p.Zs * PixelsPerWorldRegion), PixelsPerWorldRegion, PixelsPerWorldRegion);
    //                img.Dispose();

    //                if (RegionsPerWorld > 1)
    //                    img = Image.FromFile(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TILE, mf.Xs, mf.Zs, RegionsPerWorld)));
    //                else
    //                    img = Image.FromFile(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TILEX, mf.Xs, mf.Zs, RegionsPerWorld)));


    //                gTile.DrawImage(img, (p.Xs * PixelsPerWorldRegion), (p.Zs * PixelsPerWorldRegion), PixelsPerWorldRegion, PixelsPerWorldRegion);
    //                img.Dispose();

    //                gTopo.FillRectangle(SolidBrush, 0, 0, this.WorldRegionPixelSize, this.WorldRegionPixelSize);
    //                gTile.FillRectangle(SolidBrush, 0, 0, this.WorldRegionPixelSize, this.WorldRegionPixelSize);
    //            }

    //            string TopoFile = Path.Combine(localpath, FileName(MineCraftRegionFileKind.TOPO));
    //            string TileFile = Path.Combine(localpath, FileName(MineCraftRegionFileKind.TILE));

    //            topo.Save(TopoFile, System.Drawing.Imaging.ImageFormat.Png);
    //            tile.Save(TileFile, System.Drawing.Imaging.ImageFormat.Png);

    //            gTopo.Dispose();
    //            gTile.Dispose();
    //            topo.Dispose();
    //            tile.Dispose();

    //        }




    //    }
    //    public void RecreateMapFile(string localpath, int RegionsPerWorld)
    //    {
    //        if (Regions == null) return;

    //        Brush SolidBrush = new SolidBrush(Color.Black);

    //        Bitmap topo = null;
    //        Bitmap tile = null;

    //        Graphics gTopo = null;
    //        Graphics gTile = null;


    //        if (Regions.Count > 0)
    //        {

    //            var PixelsPerRegion = 512 * RegionsPerWorld;
    //            var PixelsPerWorldRegion = 512 / (this.RegionsPerWorld / RegionsPerWorld);


    //            var v = new Voxel(int.MaxValue, BlocksPerWorld) { WorldY = 0 };
    //            var p = new Voxel(int.MaxValue, PixelsPerRegion) { WorldY = 0 }; ;


    //            if (this.RegionsPerWorld == 1)
    //            {

    //                topo = MinecraftFileCentered.RetrieveBitmap(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TOPOX)));
    //                tile = MinecraftFileCentered.RetrieveBitmap(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TILEX)));


    //            }
    //            else
    //            {

    //                topo = MinecraftFileCentered.RetrieveBitmap(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TOPO)));
    //                tile = MinecraftFileCentered.RetrieveBitmap(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TILE)));
    //            }

    //            if (topo == null)
    //            {
    //                topo = new Bitmap(this.WorldRegionPixelSize, this.WorldRegionPixelSize);
    //                gTopo = Graphics.FromImage(topo);
    //                gTopo.FillRectangle(SolidBrush, 0, 0, PixelsPerRegion, PixelsPerRegion);
    //            }
    //            else
    //                gTopo = Graphics.FromImage(topo);

    //            if (tile == null)
    //            {
    //                tile = new Bitmap(this.WorldRegionPixelSize, this.WorldRegionPixelSize);
    //                gTile = Graphics.FromImage(tile);
    //                gTile.FillRectangle(SolidBrush, 0, 0, PixelsPerRegion, PixelsPerRegion);
    //            }
    //            else
    //                gTile = Graphics.FromImage(tile);


    //            Image img = null;

    //            foreach (MinecraftFile mf in Regions)
    //            {

    //                v.SetVoxel(0, mf.WorldX, mf.WorldZ);
    //                p.SetVoxel(0, v.Xo, v.Zo);


    //                if (RegionsPerWorld > 1)
    //                    img = MinecraftFileCentered.RetrieveImage(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TOPO, mf.Xs, mf.Zs, RegionsPerWorld)));
    //                else
    //                    img = MinecraftFileCentered.RetrieveImage(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TOPOX, mf.Xs, mf.Zs, RegionsPerWorld)));

    //                if (img != null)
    //                {
    //                    gTopo.DrawImage(img, (p.Xs * PixelsPerWorldRegion), (p.Zs * PixelsPerWorldRegion), PixelsPerWorldRegion, PixelsPerWorldRegion);
    //                    img.Dispose();
    //                }

    //                if (RegionsPerWorld > 1)
    //                    img = MinecraftFileCentered.RetrieveImage(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TILE, mf.Xs, mf.Zs, RegionsPerWorld)));
    //                else
    //                    img = MinecraftFileCentered.RetrieveImage(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TILEX, mf.Xs, mf.Zs, RegionsPerWorld)));

    //                if (img != null)
    //                {
    //                    gTile.DrawImage(img, (p.Xs * PixelsPerWorldRegion), (p.Zs * PixelsPerWorldRegion), PixelsPerWorldRegion, PixelsPerWorldRegion);
    //                    img.Dispose();
    //                }

    //            }

    //            string TopoFile = Path.Combine(localpath, FileName(MineCraftRegionFileKind.TOPO));
    //            string TileFile = Path.Combine(localpath, FileName(MineCraftRegionFileKind.TILE));

    //            topo.Save(TopoFile, System.Drawing.Imaging.ImageFormat.Png);
    //            tile.Save(TileFile, System.Drawing.Imaging.ImageFormat.Png);


    //            gTopo.Dispose();
    //            gTile.Dispose();

    //            topo.Dispose();
    //            tile.Dispose();

    //        }
    //    }





    //    public void CreateMapFiles(MinecraftTransfer UP, string localpath)
    //    {
    //        if (WorldMaps == null) return;

    //        if (WorldMaps.Count > 0)
    //        {

    //            int minX = int.MaxValue;
    //            int minZ = int.MaxValue;
    //            int maxX = int.MinValue;
    //            int maxZ = int.MinValue;
    //            int X = 0;
    //            int Z = 0;

    //            foreach (MinecraftWorldFile WF in WorldMaps)
    //            {
    //                if (WF.Xs < minX)
    //                    minX = WF.Xs;

    //                if (WF.Xs > maxX)
    //                    maxX = WF.Xs;

    //                if (WF.Zs < minZ)
    //                    minZ = WF.Zs;

    //                if (WF.Zs > maxZ)
    //                    maxZ = WF.Zs;

    //            }

    //            X = Math.Abs(minX - maxX);
    //            Z = Math.Abs(minZ - maxZ);

    //            WorldMaps.ForEach(x => x.CreateMapFile(UP, localpath));

    //        }
    //    }
    //    public void CreateMapFile(MinecraftTransfer UP, string localpath)
    //    {
    //        if (Regions == null) return;

    //        Brush SolidBrush = new SolidBrush(Color.Black);

    //        Bitmap topo;
    //        Bitmap tile;

    //        if (Regions.Count > 0)
    //        {

    //            var v = new Voxel(int.MaxValue, BlocksPerWorld) { WorldY = 0 };
    //            var p = new Voxel(int.MaxValue, WorldRegionPixelSize) { WorldY = 0 }; ;



    //            topo = MinecraftFileCentered.RetrieveBitmap(UP, FileName(MineCraftRegionFileKind.TOPO)) ?? new Bitmap(WorldRegionPixelSize, WorldRegionPixelSize);
    //            tile = MinecraftFileCentered.RetrieveBitmap(UP, FileName(MineCraftRegionFileKind.TILE)) ?? new Bitmap(WorldRegionPixelSize, WorldRegionPixelSize);



    //            Image img = null;

    //            Graphics gTopo = Graphics.FromImage(topo);
    //            Graphics gTile = Graphics.FromImage(tile);

    //            // gTopo.FillRectangle(SolidBrush, 0, 0, PixelsPerRegion, PixelsPerRegion);
    //            // gTile.FillRectangle(SolidBrush, 0, 0, PixelsPerRegion, PixelsPerRegion);


    //            foreach (MinecraftFile mf in Regions)
    //            {

    //                v.SetVoxel(0, mf.WorldX, mf.WorldZ);
    //                p.SetVoxel(0, v.Xo, v.Zo);


    //                img = Image.FromFile(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TOPOX, mf.Xs, mf.Zs)));
    //                gTopo.DrawImage(img, (p.Xs * PixelsPerRegion), (p.Zs * PixelsPerRegion), PixelsPerRegion, PixelsPerRegion);
    //                img.Dispose();

    //                img = Image.FromFile(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TILEX, mf.Xs, mf.Zs)));
    //                gTile.DrawImage(img, (p.Xs * PixelsPerRegion), (p.Zs * PixelsPerRegion), PixelsPerRegion, PixelsPerRegion);
    //                img.Dispose();
    //            }


    //            MinecraftFileCentered.SaveBitmap(UP, FileName(MineCraftRegionFileKind.TOPO), topo);
    //            MinecraftFileCentered.SaveBitmap(UP, FileName(MineCraftRegionFileKind.TILE), tile);

    //            gTopo.Dispose();
    //            gTile.Dispose();

    //            topo.Dispose();
    //            tile.Dispose();

    //        }
    //    }
    //    public void CreateMapFile(MinecraftTransfer UP_R, MinecraftTransfer UP_W, string localpath, int RegionsPerWorld)
    //    {
    //        if (Regions == null) return;

    //        Brush SolidBrush = new SolidBrush(Color.Black);

    //        Bitmap topo = null;
    //        Bitmap tile = null;

    //        Graphics gTopo = null;
    //        Graphics gTile = null;



    //        if (Regions.Count > 0)
    //        {

    //            var PixelsPerRegion = 512 * RegionsPerWorld;
    //            var PixelsPerWorldRegion = 512 / (this.RegionsPerWorld / RegionsPerWorld);


    //            var v = new Voxel(int.MaxValue, BlocksPerWorld) { WorldY = 0 };
    //            var p = new Voxel(int.MaxValue, PixelsPerRegion) { WorldY = 0 }; ;


    //            if (this.RegionsPerWorld == 1)
    //            {

    //                topo = MinecraftFileCentered.RetrieveBitmap(UP_R, FileName(MineCraftRegionFileKind.TOPOX));
    //                tile = MinecraftFileCentered.RetrieveBitmap(UP_R, FileName(MineCraftRegionFileKind.TILEX));

    //            }
    //            else
    //            {

    //                topo = MinecraftFileCentered.RetrieveBitmap(UP_W, FileName(MineCraftRegionFileKind.TOPO));
    //                tile = MinecraftFileCentered.RetrieveBitmap(UP_W, FileName(MineCraftRegionFileKind.TILE));
    //            }

    //            if (topo == null)
    //            {
    //                topo = new Bitmap(this.WorldRegionPixelSize, this.WorldRegionPixelSize);
    //                gTopo = Graphics.FromImage(topo);
    //                gTopo.FillRectangle(SolidBrush, 0, 0, PixelsPerRegion, PixelsPerRegion);
    //            }
    //            else
    //                gTopo = Graphics.FromImage(topo);

    //            if (tile == null)
    //            {
    //                tile = new Bitmap(this.WorldRegionPixelSize, this.WorldRegionPixelSize);
    //                gTile = Graphics.FromImage(tile);
    //                gTile.FillRectangle(SolidBrush, 0, 0, PixelsPerRegion, PixelsPerRegion);
    //            }
    //            else
    //                gTile = Graphics.FromImage(tile);


    //            Image img = null;


    //            foreach (MinecraftFile mf in Regions)
    //            {

    //                v.SetVoxel(0, mf.WorldX, mf.WorldZ);
    //                p.SetVoxel(0, v.Xo, v.Zo);


    //                if (RegionsPerWorld > 1)
    //                    img = MinecraftFileCentered.RetrieveImage(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TOPO, mf.Xs, mf.Zs, RegionsPerWorld)));
    //                else
    //                    img = MinecraftFileCentered.RetrieveImage(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TOPOX, mf.Xs, mf.Zs, RegionsPerWorld)));

    //                if (img != null)
    //                {
    //                    gTopo.DrawImage(img, (p.Xs * PixelsPerWorldRegion), (p.Zs * PixelsPerWorldRegion), PixelsPerWorldRegion, PixelsPerWorldRegion);
    //                    img.Dispose();
    //                }

    //                if (RegionsPerWorld > 1)
    //                    img = MinecraftFileCentered.RetrieveImage(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TILE, mf.Xs, mf.Zs, RegionsPerWorld)));
    //                else
    //                    img = MinecraftFileCentered.RetrieveImage(Path.Combine(localpath, FileName(MineCraftRegionFileKind.TILEX, mf.Xs, mf.Zs, RegionsPerWorld)));

    //                if (img != null)
    //                {
    //                    gTile.DrawImage(img, (p.Xs * PixelsPerWorldRegion), (p.Zs * PixelsPerWorldRegion), PixelsPerWorldRegion, PixelsPerWorldRegion);
    //                    img.Dispose();
    //                }


    //            }


    //            MinecraftFileCentered.SaveBitmap(UP_W, FileName(MineCraftRegionFileKind.TOPO), topo);
    //            MinecraftFileCentered.SaveBitmap(UP_W, FileName(MineCraftRegionFileKind.TILE), tile);

    //            string TopoFile = Path.Combine(localpath, FileName(MineCraftRegionFileKind.TOPO));
    //            string TileFile = Path.Combine(localpath, FileName(MineCraftRegionFileKind.TILE));

    //            topo.Save(TopoFile, System.Drawing.Imaging.ImageFormat.Png);
    //            tile.Save(TileFile, System.Drawing.Imaging.ImageFormat.Png);


    //            gTopo.Dispose();
    //            gTile.Dispose();

    //            topo.Dispose();
    //            tile.Dispose();

    //        }
    //    }

    //}
}