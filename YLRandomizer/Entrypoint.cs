using System.IO;

namespace Doorstop
{
    public class Entrypoint
    {
        public static void Start()
        {
            File.WriteAllText("doorstop_hello.log", "Hello world!");
        }
    }
}
