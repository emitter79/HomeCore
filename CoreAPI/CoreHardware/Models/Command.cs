namespace CoreHardware.Models
{
    public enum CommandCodes : int {
        ReadAll = 70,
        ReadRed,
        ReadGreen,
        ReadBlue
    }
    public class Command : ValueCommand
    {

        public Command(CommandCodes code)
        {
            Channel = (byte)code;
            Value = 0;
        }
    }
}