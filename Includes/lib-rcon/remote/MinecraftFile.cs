using System;
using System.Collections.Generic;
using System.Drawing;


using System.Text;
using System.IO;
using System.Diagnostics;
using LibMCRcon.WorldData;

namespace LibMCRcon.Remote
{
    public enum MCFileKind { NOTPARSED, MCA, HDT, POI, TOPO, TILE }
    public enum MineCraftRegionFileKind { NOTPARSED, MCA, TOPO, TILE, HDT, IMG, TOPOX, TILEX, POI }
    public enum TxRx { SEND, RECEIVE }
    public enum MinecraftFileCenteredOrientation { SINGLE, VERTICAL, HORIZONTAL, FULL }
    public enum MinecraftQueueProcess {UNDEFINED = 0, FTP = 1, REGIONIMGS = 2, WORLDIMGS = 3 }


    //public class MinecraftFile : Voxel
    //{

    //    public class FileData
    //    {
    //        public string filename { get; set; }
    //        public long utc { get; set; }
    //    }
    //    public class QueueReadyData
    //    {
    //        public DateTime lastUsedUtc { get; set; }
    //        public DateTime lastErrorUtc { get; set; }

    //        public MinecraftQueueProcess QueueReadyType { get; set; }

    //        public int RequestStage { get; set; }

    //        public QueueReadyData()
    //        {
    //            QueueReadyType = MinecraftQueueProcess.UNDEFINED;
    //        }
    //    }


    //    private MineCraftRegionFileKind _mckind = MineCraftRegionFileKind.MCA;
    //    public static readonly object SyncRoot = new object();

    //    private static string[] _fileNameFormat = {     "r.{0}.{1}.mca"          //0
    //                                                  , "r.{0}.{1}.hdt"          //1
    //                                                  , "topo.{0}.{1}.{2}.png"   //2
    //                                                  , "tile.{0}.{1}.{2}.png"   //3
    //                                                  , "x.{0}.{1}.{2}.bin"    //4
    //                                                  , "topo.{0}.{1}.png"   //5
    //                                                  , "tile.{0}.{1}.png"   //6
    //                                                  , "poi.{0}.{1}.{2}.{3}.{4}.png" //7
    //                                              };

    //    private static string[] _fileSearchPattern = {     "r.*.mca"            //0
    //                                                     , "r.*.hdt"            //1
    //                                                     , "topo.*.png"         //2
    //                                                     , "tile.*.png"         //3
    //                                                     , "t.*.png"         //4
    //                                                     , "topo.*.png"   //5
    //                                                     , "tile.*.png"   //6
    //                                                     , "poi.*.png" //7

    //                                                 };

    //    private int _fnfIdx = 0;
    //    private Voxel _V = MinecraftOrdinates.Region();

    //    public long PoiTimestamp { get; set; }

    //    protected static List<MinecraftFile> localMClist = new List<MinecraftFile>();
    //    protected static List<MinecraftFile> remoteMClist = new List<MinecraftFile>();

    //    public static int MCKindIndex(MineCraftRegionFileKind MCKind)
    //    {

    //        switch (MCKind)
    //        {

    //            case MineCraftRegionFileKind.MCA:
    //                return 0;
    //            case MineCraftRegionFileKind.HDT:
    //                return 1;
    //            case MineCraftRegionFileKind.TOPO:
    //                return 2;
    //            case MineCraftRegionFileKind.TILE:
    //                return 3;
    //            case MineCraftRegionFileKind.IMG:
    //                return 4;

    //            case MineCraftRegionFileKind.TOPOX:
    //                return 5;
    //            case MineCraftRegionFileKind.TILEX:
    //                return 6;
    //            case MineCraftRegionFileKind.POI:
    //                return 7;
    //        }

    //        return 0;
    //    }

    //    public MineCraftRegionFileKind MCKind
    //    {
    //        get
    //        {
    //            return _mckind;
    //        }
    //        set
    //        {
    //            _mckind = value;
    //            _fnfIdx = MCKindIndex(value);
    //        }
    //    }

    //    public void ConvertToWorldFile(int RegionsPerWorld)
    //    {
    //        this.RegionsPerWorld = RegionsPerWorld;
    //        Yz = int.MaxValue;

    //        SetVoxel(0, WorldX, WorldZ, BlocksPerWorld);
    //    }


    //    public int ScaleXo
    //    {
    //        get
    //        {
    //            return base.Xo;
    //        }
    //        set
    //        {
    //            base.Xo = value;
    //        }
    //    }
    //    public int ScaleZo
    //    {
    //        get
    //        {
    //            return base.Zo;
    //        }
    //        set
    //        {
    //            base.Zo = value;
    //        }
    //    }

    //    public override bool Parse(string CsvXYZ)
    //    {
    //        float tf = 0;
    //        Voxel pV = this;

    //        StringBuilder sb = new StringBuilder();

    //        sb.Append(CsvXYZ);
    //        sb.Replace(' ', ',');


    //        string[] tpdata = sb.ToString().Split(',');

    //        if (tpdata.Length > 2)
    //        {

    //            if (float.TryParse(tpdata[0], out tf))
    //            {
    //                pV.WorldY = (int)tf;
    //                tf = 0;
    //                if (float.TryParse(tpdata[1], out tf))
    //                {
    //                    pV.WorldX = (int)tf;
    //                    tf = 0;
    //                    if (float.TryParse(tpdata[2], out tf))
    //                    {
    //                        pV.WorldZ = (int)tf;
    //                        pV.IsValid = true;

    //                    }

    //                }
    //            }
    //        }
    //        else if (tpdata.Length > 1)
    //        {
    //            if (float.TryParse(tpdata[0], out tf))
    //            {
    //                pV.WorldX = (int)tf;
    //                tf = 0;
    //                if (float.TryParse(tpdata[1], out tf))
    //                {
    //                    pV.WorldZ = (int)tf;
    //                    pV.IsValid = true;
    //                }
    //            }
    //        }

    //        return pV.IsValid;
    //    }
    //    public override bool ParseSegment(string CsvYXZ)
    //    {
    //        float tf = 0;

    //        StringBuilder sb = new StringBuilder();

    //        sb.Append(CsvYXZ);
    //        sb.Replace(' ', ',');


    //        string[] tpdata = sb.ToString().Split(',');

    //        if (tpdata.Length > 2)
    //        {

    //            if (float.TryParse(tpdata[0], out tf))
    //            {
    //                WorldY = (int)tf;
    //                tf = 0;
    //                if (float.TryParse(tpdata[1], out tf))
    //                {
    //                    Xs = (int)tf;
    //                    tf = 0;
    //                    if (float.TryParse(tpdata[2], out tf))
    //                    {
    //                        Zs = (int)tf;
    //                        IsValid = true;

    //                    }

    //                }
    //            }
    //        }
    //        else if (tpdata.Length > 1)
    //        {
    //            if (float.TryParse(tpdata[0], out tf))
    //            {
    //                Xs = (int)tf;
    //                tf = 0;
    //                if (float.TryParse(tpdata[1], out tf))
    //                {
    //                    Zs = (int)tf;
    //                    IsValid = true;
    //                }
    //            }
    //        }

    //        return IsValid;
    //    }
    //    public override bool ParseSegment(string CsvYXZ, params int[] Offset)
    //    {
    //        if (ParseSegment(CsvYXZ) == true)
    //        {


    //            if (Offset.Length > 2)
    //            {
    //                SetOffset(Offset[0], Offset[1], Offset[2]);
    //            }
    //            else if (Offset.Length > 1)
    //            {
    //                Xo = Offset[0];
    //                Zo = Offset[1];
    //            }
    //        }

    //        return IsValid;

    //    }

    //    public override int Xo
    //    {
    //        get
    //        {
    //            return (base.Xo * 512) / BlocksPerWorld;
    //        }
    //        set
    //        {
    //            base.Xo = (value * BlocksPerWorld) / 512;
    //        }
    //    }
    //    public override int Zo
    //    {
    //        get
    //        {
    //            return (base.Zo * 512) / BlocksPerWorld;
    //        }
    //        set
    //        {
    //            base.Zo = (value * BlocksPerWorld) / 512;
    //        }
    //    }

    //    public Voxel Voxel
    //    {
    //        get
    //        {
    //            _V.SetVoxel(WorldY, WorldX, WorldZ);
    //            return _V;
    //        }
    //        set
    //        {
    //            SetVoxel(value.WorldY, value.WorldX, value.WorldZ);
    //            _V.SetVoxel(WorldY, WorldX, WorldZ);
    //        }
    //    }

    //    public bool LocalExists { get { return LocalLastWrite < DateTime.MaxValue; } }
    //    public bool RemoteExists { get { return RemoteLastWrite < DateTime.MaxValue; } }

    //    public DateTime LocalLastWrite { get; set; }
    //    public DateTime RemoteLastWrite { get; set; }
    //    public int Age { get { return (int)(RemoteLastWrite - LocalLastWrite).TotalSeconds; } }

    //    public bool ShouldRender { get; set; }
    //    public bool ShouldDownload { get; set; }
    //    public bool ShouldUpload { get; set; }

    //    public void ShouldFullProcess()
    //    {
    //        ShouldRender = true;
    //        ShouldUpload = true;
    //        ShouldDownload = true;
    //    }

    //    public bool IsOfMCKind(MineCraftRegionFileKind Check)
    //    {

    //        //if (Check == MineCraftRegionFileKind.TOPOX)
    //        //    return MCKind == MineCraftRegionFileKind.TOPOX || MCKind == MineCraftRegionFileKind.TOPO;

    //        //if (Check == MineCraftRegionFileKind.TILEX)
    //        //    return MCKind == MineCraftRegionFileKind.TILEX || MCKind == MineCraftRegionFileKind.TILE;

    //        if (Check == MineCraftRegionFileKind.IMG)
    //            return MCKind == MineCraftRegionFileKind.TOPO || MCKind == MineCraftRegionFileKind.TILE || MCKind == MineCraftRegionFileKind.IMG;

    //        return MCKind == Check;
    //    }

    //    public string PoiFileName()
    //    {

    //        return string.Format(_fileNameFormat[7], Xs, Zs, WorldX, WorldZ, PoiTimestamp);
    //    }

    //    public string FileName(MineCraftRegionFileKind MCKind)
    //    {
    //        return string.Format(_fileNameFormat[MCKindIndex(MCKind)], Xs, Zs, RegionsPerWorld);
    //    }

    //    public string FileName()
    //    {
    //        return FileName(MCKind);
    //    }

    //    public string FileName(int X, int Z, int worldsize = 1)
    //    {
    //        return string.Format(_fileNameFormat[_fnfIdx], X, Z, worldsize);
    //    }

    //    public static string FileName(MineCraftRegionFileKind MCKind, int X, int Z, int worldsize = 1)
    //    {
    //        return string.Format(_fileNameFormat[MCKindIndex(MCKind)], X, Z, worldsize);
    //    }

    //    public bool TryFind(IEnumerable<MinecraftFile> Search, MineCraftRegionFileKind MCFkind, out MinecraftFile Found)
    //    {

    //        foreach (MinecraftFile f in Search)
    //            if (f.Xs == Xs && f.Zs == Zs && f.MCKind == MCFkind && f.RegionsPerWorld == RegionsPerWorld)
    //            {
    //                Found = f;
    //                return true;
    //            }

    //        Found = null;
    //        return false;
    //    }

    //    public FileInfo FileInfo(string localpath) { return new FileInfo(Path.Combine(localpath, string.Format(_fileNameFormat[_fnfIdx], Xs, Zs, RegionsPerWorld))); }
    //    public FileInfo FileInfo(string localpath, MineCraftRegionFileKind MCKind) { return new FileInfo(Path.Combine(localpath, string.Format(_fileNameFormat[MCKindIndex(MCKind)], Xs, Zs, RegionsPerWorld))); }
    //    public FileInfo FileInfo(string localpath, int RegionsPerWorld) { return new FileInfo(Path.Combine(localpath, string.Format(_fileNameFormat[_fnfIdx], Xs, Zs, RegionsPerWorld))); }
    //    public FileInfo FileInfo(string localpath, int Xs, int Zs) { return new FileInfo(Path.Combine(localpath, string.Format(_fileNameFormat[_fnfIdx], Xs, Zs, RegionsPerWorld))); }
    //    public FileInfo FileInfo(string localpath, int Xs, int Zs, int RegionsPerWorld) { return new FileInfo(Path.Combine(localpath, string.Format(_fileNameFormat[_fnfIdx], Xs, Zs, RegionsPerWorld))); }

    //    public static FileInfo FileInfo(MineCraftRegionFileKind MCKind, int X, int Z, string localpath, int worldsize = 1)
    //    {
    //        return new FileInfo(Path.Combine(localpath, string.Format(_fileNameFormat[MCKindIndex(MCKind)], X, Z, worldsize)));
    //    }

    //    public FileInfo MCAFileInfo(string localpath) { return new FileInfo(Path.Combine(localpath, string.Format(_fileNameFormat[0], Xs, Zs, RegionsPerWorld))); }
    //    public FileInfo HDTFileInfo(string localpath) { return new FileInfo(Path.Combine(localpath, string.Format(_fileNameFormat[1], Xs, Zs, RegionsPerWorld))); }
    //    public FileInfo TOPOFileInfo(string localpath) { return new FileInfo(Path.Combine(localpath, string.Format(_fileNameFormat[RegionsPerWorld == 1 ? 5 : 2], Xs, Zs, RegionsPerWorld))); }
    //    public FileInfo TILEFileInfo(string localpath) { return new FileInfo(Path.Combine(localpath, string.Format(_fileNameFormat[RegionsPerWorld == 1 ? 6 : 3], Xs, Zs, RegionsPerWorld))); }



    //    private void Init()
    //    {
    //        MCKind = MineCraftRegionFileKind.NOTPARSED;
    //        LocalLastWrite = DateTime.MaxValue;
    //        RemoteLastWrite = DateTime.MaxValue;
    //        RegionsPerWorld = 1;
    //    }


    //    public MinecraftFile() : base(int.MaxValue, 512) { WorldY = 0; WorldX = 256; WorldZ = 256; Init(); }
    //    public MinecraftFile(int Xs, int Zs, int RegionsPerWorld, MineCraftRegionFileKind MCKind) : base(int.MaxValue, 512 * RegionsPerWorld)
    //    {
    //        IsValid = true;
    //        this.RegionsPerWorld = RegionsPerWorld;
    //        this.MCKind = MCKind;

    //        LocalLastWrite = DateTime.MaxValue;
    //        RemoteLastWrite = DateTime.MaxValue;


    //        WorldY = 255;
    //        this.Xs = Xs;
    //        this.Zs = Zs;

    //        Xo = 0;
    //        Zo = 0;
    //    }
    //    public MinecraftFile(int Xs, int Zs, int Xo, int Zo, int RegionsPerWorld, MineCraftRegionFileKind MCKind) : base(int.MaxValue, 512 * RegionsPerWorld)
    //    {
    //        IsValid = true;
    //        this.RegionsPerWorld = RegionsPerWorld;
    //        this.MCKind = MCKind;

    //        LocalLastWrite = DateTime.MaxValue;
    //        RemoteLastWrite = DateTime.MaxValue;


    //        WorldY = 255;

    //        this.Xs = Xs;
    //        this.Zs = Zs;

    //        this.Xo = Xo;
    //        this.Zo = Zo;
    //    }
    //    public MinecraftFile(MineCraftRegionFileKind mcKind) : this() { MCKind = mcKind; }
    //    public MinecraftFile(Voxel V) : base(V) { Init(); }
    //    public MinecraftFile(Voxel V, MineCraftRegionFileKind MCKind) : base(V, 512)
    //    {

    //        IsValid = true;
    //        RegionsPerWorld = 1;
    //        this.MCKind = MCKind;

    //        LocalLastWrite = DateTime.MaxValue;
    //        RemoteLastWrite = DateTime.MaxValue;

    //        WorldY = 255;
    //    }
    //    public MinecraftFile(Voxel V, int RegionsPerWorld) : base(V, 512 * RegionsPerWorld)
    //    {

    //        IsValid = true;
    //        this.RegionsPerWorld = RegionsPerWorld;

    //        if (RegionsPerWorld > 1)
    //            MCKind = MineCraftRegionFileKind.TILE;
    //        else
    //            MCKind = MineCraftRegionFileKind.TILEX;

    //        LocalLastWrite = DateTime.MaxValue;
    //        RemoteLastWrite = DateTime.MaxValue;

    //        WorldY = 255;
    //    }
    //    public MinecraftFile(Voxel V, int RegionsPerWorld, MineCraftRegionFileKind MCKind)
    //    {
    //        this.IsValid = true;
    //        this.RegionsPerWorld = RegionsPerWorld;

    //        if (RegionsPerWorld > 1)
    //        {
    //            if (MCKind == MineCraftRegionFileKind.TOPOX)
    //                this.MCKind = MineCraftRegionFileKind.TOPO;
    //            else if (MCKind == MineCraftRegionFileKind.TILEX)
    //                this.MCKind = MineCraftRegionFileKind.TILE;
    //            else
    //                this.MCKind = MCKind;
    //        }
    //        else
    //        {
    //            if (MCKind == MineCraftRegionFileKind.TOPO)
    //                this.MCKind = MineCraftRegionFileKind.TOPOX;
    //            else if (MCKind == MineCraftRegionFileKind.TILE)
    //                this.MCKind = MineCraftRegionFileKind.TILEX;
    //            else
    //                this.MCKind = MCKind;
    //        }

    //        LocalLastWrite = DateTime.MaxValue;
    //        RemoteLastWrite = DateTime.MaxValue;

    //        Yz = int.MaxValue;
    //        Sz = 512 * RegionsPerWorld;


    //        WorldY = 255;
    //        WorldX = V.WorldX;
    //        WorldZ = V.WorldZ;
    //    }
    //    public MinecraftFile(string FileName)
    //    {

    //        LocalLastWrite = DateTime.MaxValue;
    //        RemoteLastWrite = DateTime.MaxValue;
    //        RegionsPerWorld = 1;

    //        Yz = int.MaxValue;
    //        Sz = 512 * RegionsPerWorld;


    //        WorldY = 255;
    //        WorldX = 0;
    //        WorldZ = 0;

    //        SetByFileName(FileName);
    //    }
    //    public MinecraftFile(FileData FD) : this(FD.filename)
    //    {
    //        RemoteLastWrite = DateTime.FromFileTimeUtc(FD.utc).ToLocalTime();

    //    }
    //    public MinecraftFile(MinecraftFile MF, int RegionsPerWorld) : base(MF.SegmentAlignedVoxel(RegionsPerWorld))
    //    {

    //        this.RegionsPerWorld = RegionsPerWorld;

    //        MCKind = MF.MCKind;
    //        LocalLastWrite = DateTime.MaxValue;
    //        RemoteLastWrite = DateTime.MaxValue;

    //    }

    //    public void SetByFileName(string FileName)
    //    {

    //        MineCraftRegionFileKind MCKind = MineCraftRegionFileKind.NOTPARSED;
    //        bool IsValid = false;

    //        this.MCKind = MCKind;
    //        this.IsValid = IsValid;

    //        string[] v = FileName.ToUpper().Split('.');

    //        if (v.Length > 3)
    //        {

    //            string e = v.Length > 5 ? v[6] : v.Length > 4 ? v[4] : v[3];
    //            if (int.TryParse(v[1], out int x) == true)
    //            {
    //                if (int.TryParse(v[2], out int z) == true)
    //                {

    //                    IsValid = true;
    //                    int r = 1;

    //                    switch (e)
    //                    {
    //                        case "MCA":
    //                            MCKind = MineCraftRegionFileKind.MCA;
    //                            break;

    //                        case "PNG":


    //                            if (v.Length == 5)
    //                                int.TryParse(v[3], out r);

    //                            if (r > 1)
    //                            {
    //                                switch (v[0])
    //                                {
    //                                    case "TOPO":
    //                                        MCKind = MineCraftRegionFileKind.TOPO;
    //                                        break;
    //                                    case "TILE":
    //                                        MCKind = MineCraftRegionFileKind.TILE;
    //                                        break;

    //                                    default:
    //                                        MCKind = MineCraftRegionFileKind.NOTPARSED;
    //                                        break;
    //                                }
    //                            }
    //                            else
    //                            {
    //                                switch (v[0])
    //                                {
    //                                    case "TOPO":
    //                                        MCKind = MineCraftRegionFileKind.TOPOX;
    //                                        break;

    //                                    case "TILE":
    //                                        MCKind = MineCraftRegionFileKind.TILEX;
    //                                        break;

    //                                    case "POI":
    //                                        MCKind = MineCraftRegionFileKind.POI;

    //                                        x = 0;
    //                                        z = 0;

    //                                        int.TryParse(v[3], out x);
    //                                        int.TryParse(v[4], out z);

    //                                        long poi = 0;
    //                                        long.TryParse(v[5], out poi);

    //                                        PoiTimestamp = poi;


    //                                        break;

    //                                    default:
    //                                        MCKind = MineCraftRegionFileKind.NOTPARSED;
    //                                        break;
    //                                }

    //                            }

    //                            break;

    //                        case "HDT":
    //                            MCKind = MineCraftRegionFileKind.HDT;
    //                            break;
    //                        default:
    //                            MCKind = MineCraftRegionFileKind.NOTPARSED;
    //                            break;
    //                    }


    //                    if (IsValid == true)
    //                    {
    //                        this.IsValid = true;
    //                        this.RegionsPerWorld = r;
    //                        this.MCKind = MCKind;

    //                        Yz = int.MaxValue;
    //                        Sz = 512 * r;


    //                        WorldY = 255;
    //                        if (MCKind == MineCraftRegionFileKind.POI)
    //                        {
    //                            WorldX = x;
    //                            WorldZ = z;
    //                        }
    //                        else
    //                        {
    //                            Xs = x;
    //                            Zs = z;

    //                            Xo = BlocksPerWorld / 2;
    //                            Zo = BlocksPerWorld / 2;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        this.IsValid = false;
    //                        this.MCKind = MineCraftRegionFileKind.NOTPARSED;
    //                    }
    //                }
    //            }
    //        }

    //    }

    //    public string MimeType()
    //    {

    //        switch (MCKind)
    //        {
    //            case MineCraftRegionFileKind.TILE:
    //            case MineCraftRegionFileKind.TILEX:
    //            case MineCraftRegionFileKind.TOPOX:
    //            case MineCraftRegionFileKind.TOPO:
    //            case MineCraftRegionFileKind.IMG:
    //                return "image/png";

    //            default:
    //                return "application/octet-stream";

    //        }
    //    }

    //    public void Resize(int y, int x, int z, int RegionsPerWorld, MineCraftRegionFileKind MCKind)
    //    {
    //        this.RegionsPerWorld = RegionsPerWorld;
    //        this.MCKind = MCKind;

    //        Sz = 512 * RegionsPerWorld;


    //        WorldY = y;
    //        WorldX = x;
    //        WorldZ = z;
    //    }

    //    public void Verify(string path, bool islocal, int worldsize = 0)
    //    {
    //        FileInfo f = FileInfo(path, worldsize);

    //        if (f.Exists)
    //            if (islocal == true)
    //                LocalLastWrite = f.LastWriteTime;
    //            else
    //                RemoteLastWrite = f.LastWriteTime;

    //    }
    //    public void Verify(MineCraftRegionFileKind MCFKind)
    //    {

    //        bool fn(MinecraftFile x) => x.Xs == Xs && x.Zs == Zs && x.MCKind == MCFKind;

    //        MinecraftFile rRF = remoteMClist.Find(fn);
    //        MinecraftFile lRF = localMClist.Find(fn);



    //        if (rRF != null)
    //            RemoteLastWrite = rRF.RemoteLastWrite;
    //        else
    //            RemoteLastWrite = DateTime.MaxValue;


    //        if (lRF != null)
    //            LocalLastWrite = lRF.LocalLastWrite;
    //        else
    //            LocalLastWrite = DateTime.MaxValue;
    //    }
    //    public void Syncronize(int AgeCheck = 0)
    //    {
    //        if (LocalExists == false && RemoteExists == true)
    //            ShouldFullProcess();
    //        else if (LocalExists == true && RemoteExists == true)
    //        {
    //            if (Age > AgeCheck)
    //                ShouldFullProcess();
    //        }
    //    }


    //    public Action Start(Rendering.BlockColors bc, string RegionDirectory, string ImgsDirectory)
    //    {
    //        return delegate ()
    //        {
    //            Process(bc, RegionDirectory, ImgsDirectory);
    //        };
    //    }
    //    public Action DownloadMCA(MinecraftTransfer DL, String RegionDirectory)
    //    {
    //        return () => { DL.TransferNext(MCAFileInfo(RegionDirectory), TxRx.RECEIVE); };
    //    }
    //    public Action UploadImgs(MinecraftTransfer UP, String ImgDirectory)
    //    {
    //        return () =>
    //        {
    //            UP.TransferNext(TILEFileInfo(ImgDirectory), TxRx.SEND);
    //            UP.TransferNext(TOPOFileInfo(ImgDirectory), TxRx.SEND);
    //        };
    //    }
    //    public Action UploadHDT(MinecraftTransfer UP, String HdtDirectory)
    //    {
    //        return () =>
    //        {
    //            UP.TransferNext(HDTFileInfo(HdtDirectory), TxRx.SEND);
    //        };
    //    }

    //    public void Process(Rendering.BlockColors bc, string RegionDirectory, string ImgsDirectory)
    //    {



    //        byte[][] MapData = new byte[][] { new byte[512 * 512], new byte[512 * 512] };
    //        Color[] BlockData = new Color[512 * 512];

    //        RegionMCA mca = new RegionMCA(RegionDirectory);


    //        mca.LoadRegion(Xs, Zs);

    //        Rendering.MCRegionMaps.RenderDataFromRegion(bc, mca, MapData, BlockData);
    //        Rendering.MCRegionMaps.RenderTopoPngFromRegion(MapData, ImgsDirectory, Xs, Zs);
    //        Rendering.MCRegionMaps.RenderBlockPngFromRegion(MapData, BlockData, ImgsDirectory, Xs, Zs);


    //        FileInfo mcaH = HDTFileInfo(RegionDirectory);
    //        using (FileStream tempFS = mcaH.Create())
    //        {

    //            tempFS.Write(MapData[0], 0, 512 * 512);
    //            tempFS.Write(MapData[1], 0, 512 * 512);
    //            tempFS.Flush();
    //            tempFS.Close();

    //        }

    //        mcaH.LastWriteTime = mca.LastModified;




    //        FileInfo lwFS = null;

    //        lwFS = TOPOFileInfo(ImgsDirectory);
    //        if (lwFS.Exists)
    //            lwFS.LastWriteTime = mca.LastModified;

    //        lwFS = TILEFileInfo(ImgsDirectory);
    //        if (lwFS.Exists)
    //            lwFS.LastWriteTime = mca.LastModified;
    //    }





    //    public void FullProcess(Rendering.BlockColors bc, MinecraftTransfer DL, MinecraftTransfer IMG, MinecraftTransfer HDT, MinecraftTransfer MCA, bool FullRender, int Age, string RegionDirectory, string ImgsDirectory)
    //    {

    //        FileInfo FI = null;

    //        if (DL != null)
    //            DL.TransferNext(MCAFileInfo(RegionDirectory), TxRx.RECEIVE);

    //        Process(bc, RegionDirectory, ImgsDirectory);

    //        FI = TOPOFileInfo(ImgsDirectory);
    //        if (FI.Exists == true)
    //            IMG.TransferNext(FI, TxRx.SEND);

    //        FI = TILEFileInfo(ImgsDirectory);
    //        if (FI.Exists == true)
    //            IMG.TransferNext(FI, TxRx.SEND);

    //        FI = HDTFileInfo(ImgsDirectory);
    //        if (FI.Exists == true)
    //            HDT.TransferNext(FI, TxRx.SEND);


    //    }

    //    public static void RefreshRemote(IEnumerable<MinecraftFile> List, bool Append = false)
    //    {
    //        lock (SyncRoot)
    //        {

    //            if (Append == false) remoteMClist.Clear();
    //            remoteMClist.AddRange(List);
    //        }
    //    }
    //    public static void RefreshRemote(MinecraftTransfer RemoteFetch, bool Append = false)
    //    {
    //        lock (SyncRoot)
    //        {

    //            if (Append == false) remoteMClist.Clear();
    //            remoteMClist.AddRange(RemoteFetch.GetRemoteData(""));
    //        }
    //    }

    //    public static void RemoveRemoteLessThanDate(DateTime Cutoff)
    //    {
    //        lock (SyncRoot)
    //        {
    //            remoteMClist = remoteMClist.FindAll(x => x.RemoteLastWrite >= Cutoff);
    //        }
    //    }

    //    public static void RefreshRemote(FileInfo RemoteCSVData, bool Append = false)
    //    {
    //        using (var tx = RemoteCSVData.OpenText())
    //        {
    //            lock (SyncRoot)
    //            {
    //                if (Append == false) remoteMClist.Clear();

    //                while (tx.EndOfStream == false)
    //                {
    //                    string line = tx.ReadLine();
    //                    string[] data = line.Split(',');

    //                    if (data.Length > 1)
    //                    {
    //                        DateTime dt = DateTime.MaxValue;
    //                        long utc = DateTime.MaxValue.ToFileTimeUtc();


    //                        MinecraftFile x = new MinecraftFile(data[0]);

    //                        if (x.MCKind != MineCraftRegionFileKind.NOTPARSED && long.TryParse(data[1], out utc))
    //                        {
    //                            x.RemoteLastWrite = DateTime.FromFileTimeUtc(utc).ToLocalTime();
    //                            remoteMClist.Add(x);
    //                        }
    //                    }

    //                }

    //                tx.Close();
    //            }

    //        }
    //    }
    //    public static void RefreshRemote(IEnumerable<FileData> RemoteData, bool Append = false)
    //    {

    //        lock (SyncRoot)
    //        {
    //            if (Append == false) remoteMClist.Clear();
    //            foreach (var tx in RemoteData)
    //            {
    //                DateTime dt = DateTime.MaxValue;
    //                MinecraftFile x = new MinecraftFile(tx.filename);

    //                if (x.MCKind != MineCraftRegionFileKind.NOTPARSED)
    //                {
    //                    x.RemoteLastWrite = DateTime.FromFileTimeUtc(tx.utc).ToLocalTime();
    //                    remoteMClist.Add(x);
    //                }

    //            }

    //        }

    //    }

    //    public FileData Persist()
    //    {
    //        return new FileData() { filename = this.FileName(), utc = RemoteLastWrite.ToUniversalTime().ToFileTimeUtc() };
    //    }
    //    public List<MinecraftFile> RestoreFromFileData(IEnumerable<FileData> FD)
    //    {
    //        List<MinecraftFile> MFL = new List<MinecraftFile>();
    //        foreach (var f in FD)
    //            MFL.Add(new MinecraftFile(f));

    //        return MFL;
    //    }

    //    public static FileData[] PersistList(List<MinecraftFile> RemoteList)
    //    {
    //        FileData[] lst = new FileData[RemoteList.Count];

    //        lock (SyncRoot)
    //        {

    //            for (int idx = 0; idx < RemoteList.Count; idx++)
    //            {
    //                MinecraftFile x = RemoteList[idx];
    //                lst[idx] = new FileData() { filename = x.FileName(), utc = x.RemoteLastWrite.ToFileTimeUtc() };
    //            }
    //        }

    //        return lst;

    //    }

    //    public static void PersistRemoteList(FileInfo RemoteCSVData)
    //    {
    //        StringBuilder sb = new StringBuilder();

    //        foreach (var x in remoteMClist)
    //            sb.AppendLine(string.Format("{0},{1}", x.FileName(), x.RemoteLastWrite.ToFileTimeUtc()));

    //        File.WriteAllText(RemoteCSVData.FullName, sb.ToString());
    //    }
    //    public static FileData[] PersistRemoteList()
    //    {
    //        return PersistList(remoteMClist);
    //    }

    //    public static void PersistLocalList(FileInfo RemoteCSVData)
    //    {
    //        StringBuilder sb = new StringBuilder();

    //        foreach (var x in localMClist)
    //            sb.AppendLine(string.Format("{0},{1}", x.FileName(), x.LocalLastWrite.ToFileTimeUtc()));

    //        File.WriteAllText(RemoteCSVData.FullName, sb.ToString());
    //    }
    //    public static FileData[] PersistLocalList()
    //    {
    //        return PersistList(localMClist);
    //    }

    //    public static void RefreshLocal(IEnumerable<MinecraftFile> List, bool Append = false)
    //    {
    //        lock (SyncRoot)
    //        {

    //            if (Append == false) localMClist.Clear();
    //            localMClist.AddRange(List);

    //        }
    //    }
    //    public static void RefreshLocal(string localpath, MineCraftRegionFileKind MCKind, int worldsize = 0, bool Append = false)
    //    {

    //        DirectoryInfo di = new DirectoryInfo(localpath);
    //        lock (SyncRoot)
    //        {

    //            if (Append == false) localMClist.Clear();

    //            foreach (FileInfo fi in di.GetFiles(_fileSearchPattern[MCKindIndex(MCKind)]))
    //            {
    //                MinecraftFile RF = new MinecraftFile(fi.Name)
    //                { LocalLastWrite = fi.LastWriteTime };

    //                if (RF.IsValid)
    //                    localMClist.Add(RF);
    //            }

    //            //if (MCKind == MineCraftRegionFileKind.IMG)
    //            //{
    //            //    foreach (FileInfo fi in di.GetFiles(_fileSearchPattern[MCKindIndex(MineCraftRegionFileKind.TOPO)]))
    //            //    {
    //            //        MinecraftFile MF = new MinecraftFile(fi.Name);
    //            //        MF.LocalLastWrite = fi.LastWriteTime;
    //            //        localMClist.Add(MF);
    //            //    }

    //            //    foreach (FileInfo fi in di.GetFiles(_fileSearchPattern[MCKindIndex(MineCraftRegionFileKind.TILE)]))
    //            //    {

    //            //        MinecraftFile MF = new MinecraftFile(fi.Name);

    //            //        MF.LocalLastWrite = fi.LastWriteTime;
    //            //        localMClist.Add(MF);
    //            //    }

    //            //}
    //        }
    //    }
    //    public static void RefreshLocal(FileInfo RemoteCSVData, bool Append = false)
    //    {
    //        using (var tx = RemoteCSVData.OpenText())
    //        {
    //            lock (SyncRoot)
    //            {
    //                if (Append == false) localMClist.Clear();

    //                while (tx.EndOfStream == false)
    //                {
    //                    string line = tx.ReadLine();
    //                    string[] data = line.Split(',');

    //                    if (data.Length > 1)
    //                    {
    //                        DateTime dt = DateTime.MaxValue;
    //                        long utc = DateTime.MaxValue.ToFileTimeUtc();

    //                        MinecraftFile x = new MinecraftFile(data[0]);

    //                        if (x.MCKind != MineCraftRegionFileKind.NOTPARSED && long.TryParse(data[1], out utc))
    //                        {
    //                            x.LocalLastWrite = DateTime.FromFileTimeUtc(utc).ToLocalTime();
    //                            localMClist.Add(x);
    //                        }
    //                    }

    //                }

    //                tx.Close();
    //            }

    //        }
    //    }
    //    public static void RefreshLocal(IEnumerable<FileData> RemoteData, bool Append = false)
    //    {

    //        lock (SyncRoot)
    //        {
    //            if (Append == false) localMClist.Clear();
    //            foreach (var tx in RemoteData)
    //            {
    //                DateTime dt = DateTime.MaxValue;
    //                MinecraftFile x = new MinecraftFile(tx.filename);

    //                if (x.MCKind != MineCraftRegionFileKind.NOTPARSED)
    //                {
    //                    x.RemoteLastWrite = DateTime.FromFileTimeUtc(tx.utc).ToLocalTime();
    //                    localMClist.Add(x);
    //                }

    //            }

    //        }

    //    }
    //    public static void RefreshLocal(MinecraftTransfer RemoteFetch, bool Append = false)
    //    {
    //        lock (SyncRoot)
    //        {

    //            if (Append == false) localMClist.Clear();
    //            localMClist.AddRange(RemoteFetch.GetRemoteData());
    //        }
    //    }


    //    public static void SyncronizeMCA(int AgeCheck = 0)
    //    {

    //        List<MinecraftFile> remoteMCAlist = remoteMClist.FindAll(x => x.MCKind == MineCraftRegionFileKind.MCA);

    //        foreach (MinecraftFile rx in remoteMCAlist)
    //        {

    //            List<MinecraftFile> localFiles = localMClist.FindAll(lx => rx.Xs == lx.Xs && rx.Zs == lx.Zs);

    //            MinecraftFile localMCA = localFiles.Find(lx => rx.Xs == lx.Xs && rx.Zs == lx.Zs && lx.MCKind == MineCraftRegionFileKind.MCA);
    //            if (localMCA != null)
    //            {
    //                rx.LocalLastWrite = localMCA.LocalLastWrite;
    //                localMCA.RemoteLastWrite = rx.RemoteLastWrite;

    //                if (rx.Age > AgeCheck)
    //                    rx.ShouldFullProcess();



    //            }
    //            else
    //                rx.ShouldFullProcess();
    //        }





    //    }

    //    public static MinecraftFile Verify(Voxel V, MineCraftRegionFileKind MCKind)
    //    {
    //        MinecraftFile mf = new MinecraftFile(V, MCKind);

    //        bool fn(MinecraftFile x) => x.Xs == mf.Xs && x.Zs == mf.Zs && x.MCKind == mf.MCKind;

    //        MinecraftFile rRF = remoteMClist.Find(fn);
    //        MinecraftFile lRF = localMClist.Find(fn);



    //        if (rRF != null)
    //            mf.RemoteLastWrite = rRF.RemoteLastWrite;
    //        else
    //            mf.RemoteLastWrite = DateTime.MaxValue;


    //        if (lRF != null)
    //            mf.LocalLastWrite = lRF.LocalLastWrite;
    //        else
    //            mf.LocalLastWrite = DateTime.MaxValue;

    //        return mf;

    //    }

    //    public static TransferQueue<MinecraftFile> DownloadQueue(MineCraftRegionFileKind MCFKind = MineCraftRegionFileKind.MCA, int AgeCheck = 0)
    //    {
    //        return new TransferQueue<MinecraftFile>(remoteMClist.FindAll(x => x.ShouldDownload && x.MCKind == MCFKind && (x.Age > AgeCheck || x.Age < 0)));
    //    }

    //    public static TransferQueue<MinecraftFile> DownloadQueue(List<MinecraftFile> List, MineCraftRegionFileKind MCFKind = MineCraftRegionFileKind.MCA, int AgeCheck = 0)
    //    {
    //        TransferQueue<MinecraftFile> trans = new TransferQueue<MinecraftFile>();
    //        List.ForEach(x =>
    //        {
    //            x.Verify(MCFKind);
    //            x.Syncronize(AgeCheck);
    //        });

    //        trans.Enqueue(List.FindAll(x => x.ShouldDownload));

    //        return trans;
    //    }

    //    public static MinecraftFile MCA { get { return new MinecraftFile(MineCraftRegionFileKind.MCA); } }
    //    public static MinecraftFile HDT { get { return new MinecraftFile(MineCraftRegionFileKind.HDT); } }
    //    public static MinecraftFile TOPO { get { return new MinecraftFile(MineCraftRegionFileKind.TOPO); } }
    //    public static MinecraftFile TILE { get { return new MinecraftFile(MineCraftRegionFileKind.TILE); } }

    //    public static Process JavaTopoProc(string RegionPath, string JavaBinary = "java")
    //    {
    //        //TMCMR
    //        string JarName = Path.Combine(RegionPath, "tmcmr.jar");

    //        System.Diagnostics.Process proc = new System.Diagnostics.Process()
    //        { EnableRaisingEvents = false };

    //        proc.StartInfo.FileName = JavaBinary;
    //        proc.StartInfo.WorkingDirectory = RegionPath;

    //        proc.StartInfo.UseShellExecute = false;
    //        proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
    //        proc.StartInfo.CreateNoWindow = true;

    //        return proc;
    //    }
    //    public static string MimeType(MineCraftRegionFileKind MCKind)
    //    {

    //        switch (MCKind)
    //        {
    //            case MineCraftRegionFileKind.TILE:
    //            case MineCraftRegionFileKind.TOPO:
    //            case MineCraftRegionFileKind.IMG:
    //                return "image/png";

    //            default:
    //                return "application/octet-stream";

    //        }
    //    }


    //    private Bitmap GetImg(string filename)
    //    {
    //        Bitmap fb;
    //        Bitmap b;

    //        if (File.Exists(filename))
    //        {

    //            fb = new Bitmap(filename);
    //            b = new Bitmap(fb);
    //            fb.Dispose();
    //        }
    //        else
    //            b = new Bitmap(512, 512);

    //        return b;

    //    }
    //    private Bitmap GetImg(MinecraftTransfer transfer, string filename, int W, int H)
    //    {
    //        Bitmap fb;
    //        Bitmap b;


    //        using (MemoryStream ms = new MemoryStream())
    //        {
    //            if (transfer.TransferNext(filename, ms, TxRx.RECEIVE))
    //            {
    //                fb = new Bitmap(ms);
    //                b = new Bitmap(fb);
    //                fb.Dispose();
    //            }
    //            else
    //                b = new Bitmap(W, H);


    //        }
    //        return b;
    //    }

    //}






}