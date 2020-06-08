namespace LibMCRcon.WorldData
{
    public class Axis
    {
        private int[] _value = {0};
        

        public Axis()
        {
            Size = 512;
        }
        
        public Axis(int SegmentSize = 512)
        {
            Size = SegmentSize;
        }

        public int Size { get; set; }

        
        public int Value { get { return _value[0]; } set { _value[0] = value; } }
        
        public int Segment
        {
            get
            {
                return _value[0] < 0 ? -(((_value[0] * -1) / Size) + 1) : _value[0] / Size;
            }

            set
            {
                _value[0] = value < 0 ? -(((value * -1) * Size) - Offset) : (value * Size) + Offset;
            }

        }
        public int Offset
        {
            get
            {
                return _value[0] < 0 ? Size - ((_value[0] * -1) - (((_value[0] * -1) / Size) * Size)) : _value[0] - ((_value[0] / Size) * Size);
            }

            set
            {
                _value[0] = value < 0 ? -(((Segment * -1) * Size) - value) : (Segment * Size) + value;
            }
        }


        public void SegmentAlign()
        {
            int s = Segment;
            _value[0] = s < 0 ? -((s * -1) * Size) : (s * Size);
        }
        public void SegmentAlign(int Segment)
        {
            _value[0] = Segment < 0 ? -((Segment * -1) * Size) : (Segment * Size);
        }

        public void SetSegmentOffset(int Segment, int Offset)
        {
            _value[0] = Segment < 0 ? -(((Segment * -1) * Size) - Offset) : (Segment * Size) + Offset;
        }
    }
}