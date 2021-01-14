using System;
using XdParser;

namespace Sample
{
    static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine($"LoadFile: {args[0]}");
            var xd = new XdFile(args[0]);
        }
    }
}