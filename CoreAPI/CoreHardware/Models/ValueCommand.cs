namespace CoreHardware.Models
{
    public class ValueCommand
    {
        const byte DC1 = 17;
        const byte ETB = 23;

        public byte Channel;
        public byte Value;

        public ValueCommand()
        {
            Channel = 0;
            Value = 0;
        }

        public ValueCommand(byte channel, byte value)
        {
            Channel = channel;
            Value = value;
        }

        public ValueCommand(byte channel, bool value)
        {
            Channel = channel;
            Value = (byte)(value ? 255 : 0);
        }

        public byte[] ToByteArray()
        {
            byte[] cmd = { DC1, Channel, Value, ETB };
            return cmd;
        }

        public override string ToString()
        {
            string sdata = string.Empty;
            foreach (byte b in ToByteArray()) { sdata += "[" + b + "]"; }
            return sdata;
        }
    }
}