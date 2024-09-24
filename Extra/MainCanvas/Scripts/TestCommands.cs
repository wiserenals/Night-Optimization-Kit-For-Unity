namespace Commands.Player
{
    public class Commands_Player
    {
        [Command] public static float health { get; set; } = 100;
        [Command] public float hungry { get; set; } = 100;
    }
}

namespace Commands.Console
{
    public class Commands_Console
    {
        [Command]
        public static string Log(string val)
        {
            return val;
        }
    }
}
