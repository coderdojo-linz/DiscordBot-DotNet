namespace LibMCRcon.WorldData
{
    public class AxisVoxel
    {

        public AxisVoxel()
        {
            Y = new Axis();
            X = new Axis();
            Z = new Axis();
        }
        public AxisVoxel(int XZSegmentSize = 512,int YSegmentSize = int.MaxValue)
        {
            Y = new Axis(YSegmentSize);
            X = new Axis(XZSegmentSize);
            Z = new Axis(XZSegmentSize);
        }
        public AxisVoxel(int Y, int X, int Z, int XZSegmentSize = 512, int YSegmentSize = int.MaxValue):this(XZSegmentSize, YSegmentSize)
        {
            this.Y.Value = Y;
            this.X.Value = X;
            this.Z.Value = Z;
        }
        
        public Axis X { get; internal set; }
        public Axis Y { get; internal set; }
        public Axis Z { get; internal set; }

        public void SegmentAlign()
        {
            
            Y.SegmentAlign();
            X.SegmentAlign();
            Z.SegmentAlign();

        }

    }
}