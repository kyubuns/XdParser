using System;
using System.Linq;
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
                    Render("        ", artboard.Artboard.Children, artwork.Resources.Resources);
                }
            }
        }

        private static void Render(string indent, XdObjectJson[] children, XdResourcesResourcesJson resources)
        {
            foreach (var child in children)
            {
                Console.WriteLine($"{indent}- {child.Name}({child.Type}) / {child.Style?.Fill?.Pattern?.Meta?.Ux?.Uid}, {child.Meta?.Ux?.SymbolId}");
                if (child.Group != null)
                {
                    Render($"{indent}    ", child.Group.Children, resources);
                }
            }
        }
    }
}