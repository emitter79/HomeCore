namespace CoreAPI.Models
{
    public class RGBValues
    {
        public RGBValues()
        {
            Red = 0;
            Green = 0;
            Blue = 0;
            Valid = true;
        }

        public RGBValues(byte[] data)
        {
            if (data.Length == 3) {
                Red = data[0];
                Green = data[1];
                Blue = data[2];
                Valid = true;              
            } else {
                Valid = false;
            }
        }

        public int Red;
        public int Green;
        public int Blue;
        public bool Valid;
    }
}