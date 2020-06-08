using System;
using System.Collections.Generic;
using System.Drawing;

namespace LibMCRcon.Rendering
{
    public class ColorStep
    {
        public int Steps { get; set; }
        public Color Color { get; set; }

        public ColorStep() { Color = Color.Black; Steps = 1; }
        public ColorStep(Color Color) { this.Color = Color; Steps = 1; }
        public ColorStep(Color Color, int Steps) { this.Color = Color; this.Steps = Steps; }

        public static Color[] CreatePallet(List<ColorStep> cList)
        {
            Color[] p = new Color[256];

            int z = 0;

            ColorStep A;
            ColorStep B;

            for (int y = 0; y < cList.Count; y++)
            {

                A = cList[y];

                if ((y + 1) < cList.Count)
                    B = cList[y + 1];
                else
                    B = Color.White.ColorStep(0);



                for (int x = 0; x < A.Steps; x++)
                {


                    Single aL = (1f / A.Steps) * x;


                    byte R1 = 0;
                    byte G1 = 0;
                    byte B1 = 0;

                    R1 = (byte)(((A.Color.R * (1 - aL)) + (B.Color.R * aL)) / 2);
                    G1 = (byte)(((A.Color.G * (1 - aL)) + (B.Color.G * aL)) / 2);
                    B1 = (byte)(((A.Color.B * (1 - aL)) + (B.Color.B * aL)) / 2);



                    p[z] = Color.FromArgb(R1, G1, B1);
                    z++;

                    if (z > 255)
                        return p;

                }
            }

            return p;
        }
        public static Color MixColors(Single Percentage, Color A, Color B, int WhiteBalance = 0)
        {
            int A1 = 0;
            int R1 = 0;
            int G1 = 0;
            int B1 = 0;
            Single aL = Percentage / 100;

            A1 = A.A;



            R1 = (int)((A.R * B.R  ) / 255) + WhiteBalance;
            G1 = (int)((A.G * B.G  ) / 255) + WhiteBalance;
            B1 = (int)((A.B * B.B  ) / 255) + WhiteBalance;

            //R1 = (int)((((A.R * aL) * (B.R * (1 - aL))) / 255) + WhiteBalance);
            //G1 = (int)((((A.G * aL) * (B.G * (1 - aL))) / 255) + WhiteBalance);
            //B1 = (int)((((A.B * aL) * (B.B * (1 - aL))) / 255) + WhiteBalance);

            if (R1 > 255) R1 = 255;
            if (G1 > 255) G1 = 255;
            if (B1 > 255) B1 = 255;


            return Color.FromArgb(A1,R1, G1, B1);
        }
    }

   

}
