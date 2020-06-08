using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LibMCRcon.Remote
{
    public class MCInfoList
    {
        public DateTime MinListDate { get; set; }
        public DateTime MaxListDate { get; set; }
        public DateTime FilterDate { get; set; }
        public List<MCInfo> RemoteList { get; set; }
        public List<MCInfo> FilteredList { get; set; }

        public MCInfoList()
        {
            RemoteList = new List<MCInfo>();
            FilteredList = new List<MCInfo>();

            MinListDate = DateTime.MaxValue;
            MaxListDate = DateTime.MinValue;
        }

        public MCInfoList(List<MCInfo> RemoteList, TimeSpan Age, List<string> Files = null)
        {
            this.RemoteList = RemoteList ?? new List<MCInfo>();

            MinListDate = DateTime.MaxValue;
            MaxListDate = DateTime.MinValue;
            FilterDate = DateTime.Now - Age;

            RemoteList.ForEach(x => CalculateAge(x));
            FilterList(FilterDate,Files);
        }
        
        private void CalculateAge(MCInfo mcf)
        {
            if (mcf.RemoteLastWrite < MinListDate)
                MinListDate = mcf.RemoteLastWrite;

            if (mcf.RemoteLastWrite > MaxListDate)
                MaxListDate = mcf.RemoteLastWrite;
        }

        public void FilterList(DateTime Cuttoff, List<string> Files = null)
        {
            FilteredList = new List<MCInfo>();
            RemoteList.ForEach(x => 
            {
                if (x.RemoteLastWrite >= Cuttoff)
                    FilteredList.Add(x);
                else if (Files.Find(f => f.Equals(x.FileName,StringComparison.OrdinalIgnoreCase)) != null)
                    FilteredList.Add(x);

            });
        }
    }
    public class MCInfo
    {
        public MineCraftRegionFileKind MCKind { get; set; } = MineCraftRegionFileKind.NOTPARSED;
        public bool IsValid { get; set; } = false;

        public int Y { get; set; }
        public int X { get; set; }
        public int Z { get; set; }
        public int Xs { get; set; }
        public int Zs { get; set; }
        public int Regions { get; set; }
        
        public long PoiTimestamp { get; set; } = long.MinValue;
        public DateTime RemoteLastWrite { get; set; }
        public DateTime LocalLastWrite { get; set; }

        public string FileName { get; set; }
        public string MimeType()
        {

            switch (MCKind)
            {
                case MineCraftRegionFileKind.TILE:
                case MineCraftRegionFileKind.TILEX:
                case MineCraftRegionFileKind.TOPOX:
                case MineCraftRegionFileKind.TOPO:
                case MineCraftRegionFileKind.IMG:
                    return "image/png";

                default:
                    return "application/octet-stream";

            }
        }

        public MCInfo() { }
        public MCInfo(string FileName)
        {
            SetByFileName(FileName);
        }

        public void SetByFileName(string FileName)
        {
            this.FileName = FileName;

            string[] v = FileName.ToUpper().Split('.');

            if (v.Length > 3)
            {

                string e = v.Length > 5 ? v[6] : v.Length > 4 ? v[4] : v[3];
                if (int.TryParse(v[1], out int x) == true)
                {
                    if (int.TryParse(v[2], out int z) == true)
                    {

                        Xs = x;
                        Zs = z;

                        IsValid = true;
                        int r = 1;

                        switch (e)
                        {
                            case "MCA":
                                MCKind = MineCraftRegionFileKind.MCA;
                                break;

                            case "PNG":
                        


                                if (v.Length == 5)
                                    int.TryParse(v[3], out r);

                                if (r > 1)
                                {
                                    switch (v[0])
                                    {
                                        case "TOPO":
                                            MCKind = MineCraftRegionFileKind.TOPO;
                                            break;
                                        case "TILE":
                                            MCKind = MineCraftRegionFileKind.TILE;
                                            break;


                                        default:
                                            MCKind = MineCraftRegionFileKind.NOTPARSED;
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (v[0])
                                    {
                                        case "TOPO":
                                            MCKind = MineCraftRegionFileKind.TOPOX;
                                            break;

                                        case "TILE":
                                            MCKind = MineCraftRegionFileKind.TILEX;
                                            break;
                                   
           
                                        case "POI":
                                            MCKind = MineCraftRegionFileKind.POI;

                                            if (int.TryParse(v[3], out int wx) == true)
                                                if (int.TryParse(v[4], out int wz) == true)
                                                {

                                                    if (long.TryParse(v[5], out long poi) == true)
                                                    {
                                                        PoiTimestamp = poi;
                                                        X = wx;
                                                        Z = wz;
                                                    }
                                                }

                                            break;

                                        default:
                                            MCKind = MineCraftRegionFileKind.NOTPARSED;
                                            break;
                                    }

                                }

                                break;

                            case "HDT":
                                MCKind = MineCraftRegionFileKind.HDT;
                                break;
                            default:
                                MCKind = MineCraftRegionFileKind.NOTPARSED;
                                break;
                        }


                        if (IsValid == true)
                        {

                            Regions = r;

                        }
                        else
                        {
                            IsValid = false;
                            MCKind = MineCraftRegionFileKind.NOTPARSED;
                        }
                    }
                }
            }


        }

        public string POIName => $"poi.{Xs}.{Zs}.{X}.{Z}.{PoiTimestamp}.png";
        public string MCAName => $"r.{Xs}.{Zs}.mca";
        public string HDTName => $"r.{Xs}.{Zs}.hdt";
        public string TOPOName => Regions > 1 ? $"topo.{Xs}.{Zs}.{Regions}.png" : $"topo.{Xs}.{Zs}.png";
        public string TILEName => Regions > 1 ? $"tile.{Xs}.{Zs}.{Regions}.png" : $"tile.{Xs}.{Zs}.png";

      
    }
    public class MCTransferJson<T> where T : class, new()
    {
        public T Data { get; set; }
        public bool LastError { get; set; } = false;
        public bool AquireLeaseError { get; set; } = false;

        public MCTransferJson()
        {
            Data = new T();
            LastError = true;
        }
        public MCTransferJson(T Data)
        {
            this.Data = Data;
            LastError = false;
        }

    }

    public abstract class MCTransfer 
    { 
        public string RemotePath { get; set; }
        public bool StopTransfer { get; set; }
        public bool LastTransferSuccess { get; set; }
        public string LastError { get; set; }
   
        public string FullRemotePath(string RootPath, string FileName)
        {
            if (RemotePath == string.Empty)
                return string.Format("{0}/{1}", RootPath, FileName);
            else
                return string.Format("{0}/{1}/{2}", RootPath, RemotePath, FileName);


        }
        public TxRx Direction { get; set; }

        public abstract bool ValidateTransfer(FileInfo item, TxRx Direction);
        public abstract int TransferItemAge(FileInfo item, TxRx Direction);

        public abstract bool TransferNext(FileInfo item, TxRx Direction);
        public abstract bool TransferNext(string FileName, Stream item, TxRx Direction);
             
        public abstract bool Exists(string FileName);
        public abstract int Age(string FileName);

        public abstract List<MCInfo> GetRemoteData();
        public abstract List<MCInfo> GetRemoteData(string RemotePath);
        public abstract List<MCInfo> GetRemoteData(string RemotePath, string Filter);

        public bool LockOut { get; set; }
        public abstract void Open();
        public abstract void Close();

        public bool IsOpen { get; set; }

        public void TransferRun(TransferQueue<FileInfo> Items, TxRx Direction, bool Continous = false)
        {
            TransferRun(Items, null, Direction, Continous);
        }
        public void TransferRun(TransferQueue<FileInfo> Items, TransferQueue<FileInfo> Finished, TxRx Direction, bool Continous = false)
        {
            if (IsOpen == false)
                Open();


            Action<FileInfo> Finish;

            if (Finished != null)
                Finish = (x) => { Finished.Enqueue(x); };
            else
                Finish = (x) => { };

            if (IsOpen)
            {

                FileInfo item;

                while (Items.Count > 0 || Continous)
                {

                    item = Items.Dequeue();

                    if (item != null)
                        if (ValidateTransfer(item, Direction))
                            TransferNext(item, Direction);

                    Finish(item);


                    if (StopTransfer) break;
                }
            }

        }

        public bool Upload(FileInfo Item)
        {
            return TransferNext(Item, TxRx.SEND);
        }
        public bool Upload(Stream Item, string FileName)
        {
            return TransferNext(FileName, Item, TxRx.SEND);
        }
        public bool Download(FileInfo Item)
        {
            return TransferNext(Item, TxRx.RECEIVE);
        }
        public bool Download(Stream Item, string FileName)
        {
            return TransferNext(FileName, Item, TxRx.RECEIVE);
        }

        public bool UploadJsonObject<T>(T Item, string FileName) where T : new()
        {

            bool ok = false;
            var data = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Item));
            using (var mem = new MemoryStream(data))
            {
                ok = TransferNext(FileName, mem, TxRx.SEND);
            }
            return ok;


        }
        public bool UploadJsonObject<T>(T[] Item, string FileName) where T : new()
        {

            bool ok = false;
            var data = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Item));
            using (var mem = new MemoryStream(data))
            {
                ok = TransferNext(FileName, mem, TxRx.SEND);
            }
            return ok;


        }

        public MCTransferJson<T> DownloadJsonObject<T>(string FileName) where T : class, new()
        {

            using (var mem = new MemoryStream())
            {
                if (TransferNext(FileName, mem, TxRx.RECEIVE) == true)
                {
                    try
                    {
                        var js = System.Text.Encoding.UTF8.GetString(mem.ToArray());
                        return new MCTransferJson<T>(JsonConvert.DeserializeObject<T>(js));
                    }
                    catch
                    {
                        LastTransferSuccess = false;
                        throw;
                    }
                    
                }
                else
                    return new MCTransferJson<T>();

            }

        }
       
    }
    public abstract class MCTransferAsync
    {

        public string RemotePath { get; set; }
        public bool StopTransfer { get; set; }
        public bool LastTransferSuccess { get; set; }
        public string LastError { get; set; }

        public string FullRemotePath(string RootPath, string FileName)
        {
            if (RemotePath == string.Empty)
                return string.Format("{0}/{1}", RootPath, FileName);
            else
                return string.Format("{0}/{1}/{2}", RootPath, RemotePath, FileName);


        }
        public TxRx Direction { get; set; }


        public abstract Task<bool> ValidateTransfer(FileInfo item, TxRx Direction);
        public abstract Task<int> TransferItemAge(FileInfo item, TxRx Direction);
        public abstract Task<bool> TransferNext(FileInfo item, TxRx Direction);
        public abstract Task<bool> TransferNext(string FileName, Stream item, TxRx Direction);

        public abstract Task<bool> Exists(string FileName);
        public abstract Task<int> Age(string FileName);

        public abstract Task<List<MCInfo>> GetRemoteData();
        public abstract Task<List<MCInfo>> GetRemoteData(string RemotePath);
        public abstract Task<List<MCInfo>> GetRemoteData(string RemotePath, string Filter);

        public bool LockOut { get; set; }
        public abstract Task Open();
        public abstract void Close();

        public bool IsOpen { get; set; }

        public async Task TransferRun(TransferQueue<FileInfo> Items, TxRx Direction, bool Continous = false)
        {
           await TransferRun(Items, null, Direction, Continous);
        }
        public async Task TransferRun(TransferQueue<FileInfo> Items, TransferQueue<FileInfo> Finished, TxRx Direction, bool Continous = false)
        {
            if (IsOpen == false)
               await Open();

            if (IsOpen)
            {

                FileInfo item;

                while (Items.Count > 0 || Continous)
                {

                    item = Items.Dequeue();

                    if (item != null)
                        if (await ValidateTransfer(item, Direction))
                            await TransferNext(item, Direction);

                    if (Finished != null)
                        Finished.Enqueue(item);

                    if (StopTransfer) break;
                }
            }

        }

        public async Task<bool> Upload(FileInfo Item)
        {
            return await TransferNext(Item, TxRx.SEND);
        }

        public async Task<bool> Upload(Stream Item, string FileName)
        {
            return await TransferNext(FileName, Item, TxRx.SEND);
        }
        public async Task<bool> Download(FileInfo Item)
        {
            return await TransferNext(Item, TxRx.RECEIVE);
        }
        public async Task<bool> Download(Stream Item, string FileName)
        {
            return await TransferNext(FileName, Item, TxRx.RECEIVE);
        }


        public async Task<bool> UploadJsonObject<T>(T Item, string FileName) where T : new()
        {

            bool ok = false;
            var data = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Item));
            using (var mem = new MemoryStream(data))
            {
                ok = await TransferNext(FileName, mem, TxRx.SEND);
            }
            return ok;


        }

        public async Task<bool> UploadJsonObject<T>(T[] Item, string FileName) where T : new()
        {

            bool ok = false;
            var data = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Item));
            using (var mem = new MemoryStream(data))
            {
                ok = await TransferNext(FileName, mem, TxRx.SEND);
            }
            return ok;


        }
        public async Task<MCTransferJson<T>> DownloadJsonObject<T>(string FileName) where T : class, new()
        {

            using (var mem = new MemoryStream())
            {
                if (await TransferNext(FileName, mem, TxRx.RECEIVE) == true)
                {
                    try
                    {
                        var js = System.Text.Encoding.UTF8.GetString(mem.ToArray());
                        return new MCTransferJson<T>(JsonConvert.DeserializeObject<T>(js));
                    }
                    catch(Exception)
                    {
                        LastTransferSuccess = false;
                        throw;
                    }

                }
                else
                    return new MCTransferJson<T>();

            }

        }


    }
   

}