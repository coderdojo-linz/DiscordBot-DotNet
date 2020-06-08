using System.Drawing;

namespace LibMCRcon.Rendering
{
    public static class ColorStepExtension
    {

        public static ColorStep ColorStep(this Color Color, int Steps)
        {
            return new ColorStep(Color, Steps);
        }

    }

   

}
