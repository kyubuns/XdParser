using System;
using XdParser;
using XdParser.Internal;

namespace Sample
{
    static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine($"- {args[0]}(Xd)");

            var xd = new XdFile(args[0]);
            foreach (var artwork in xd.Artworks)
            {
                Console.WriteLine($"    - {artwork.Name}(Artboard)");
                foreach (var artboard in artwork.Artboard.Children)
                {
                    Render("        ", artboard.Artboard.Children);
                }
            }
        }

        private static void Render(string indent, XdArtboardChildArtboardChildJson[] children)
        {
            foreach (var child in children)
            {
                Console.WriteLine($"{indent}- {child.Name}({child.Type}) / {child.Style?.Fill?.Pattern?.Meta?.Ux?.Uid}");
                if (child.Group != null)
                {
                    Render($"{indent}    ", child.Group.Children);
                }
            }
        }
    }
}