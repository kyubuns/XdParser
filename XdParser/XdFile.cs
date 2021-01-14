using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using XdParser.Internal;
using Newtonsoft.Json;

namespace XdParser
{
    public class XdFile : IDisposable
    {
        private ZipFile _zipFile;
        public XdArtboard[] Artworks { get; }

        public XdFile(string xdFilePath)
        {
            _zipFile = new ZipFile(xdFilePath);
            var manifestJsonString = _zipFile.ReadString("manifest");
            var xdManifestJson = JsonConvert.DeserializeObject<XdManifestJson>(manifestJsonString);

            var artworks = new List<XdArtboard>();
            foreach (var xdManifestArtwork in xdManifestJson.Children.Single(x => x.Path == "artwork").Children)
            {
                var artworkJsonString = _zipFile.ReadString($"artwork/{xdManifestArtwork.Path}/graphics/graphicContent.agc");
                var artworkJson = JsonConvert.DeserializeObject<XdArtboardJson>(artworkJsonString);
                artworks.Add(new XdArtboard(xdManifestArtwork, artworkJson));
            }
            Artworks = artworks.ToArray();
        }


        public byte[] GetResource(XdStyleFillPatternMetaJson styleFillPatternMetaJson)
        {
            var uid = styleFillPatternMetaJson?.Ux?.Uid;
            if (string.IsNullOrWhiteSpace(uid)) return null;
            return _zipFile.ReadBytes($"resources/{uid}");
        }

        public void Dispose()
        {
            _zipFile?.Close();
            _zipFile = null;
        }
    }

    public class XdArtboard
    {
        public XdManifestChildJson Manifest { get; }
        public XdArtboardJson Artboard { get; }

        public string Name => Manifest.Name;

        public XdArtboard(XdManifestChildJson manifest, XdArtboardJson artboard)
        {
            Manifest = manifest;
            Artboard = artboard;
        }
    }
}

namespace XdParser.Internal
{
    public static class ZipExtensions
    {
        public static string ReadString(this ZipFile self, string filePath)
        {
            var manifestZipEntry = self.GetEntry(filePath);
            using var stream = self.GetInputStream(manifestZipEntry);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }


        public static byte[] ReadBytes(this ZipFile self, string filePath)
        {
            var manifestZipEntry = self.GetEntry(filePath);
            using var stream = self.GetInputStream(manifestZipEntry);
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }

    public class XdColorJson
    {
        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("r")]
        public int R { get; set; }

        [JsonProperty("g")]
        public int G { get; set; }

        [JsonProperty("b")]
        public int B { get; set; }
    }

    public class XdTransformJson
    {
        [JsonProperty("a")]
        public int A { get; set; }

        [JsonProperty("b")]
        public int B { get; set; }

        [JsonProperty("c")]
        public int C { get; set; }

        [JsonProperty("d")]
        public int D { get; set; }

        [JsonProperty("tx")]
        public int Tx { get; set; }

        [JsonProperty("ty")]
        public int Ty { get; set; }
    }

    public class XdStyleJson
    {
        [JsonProperty("fill")]
        public XdStyleFillJson Fill { get; set; }

        [JsonProperty("stroke")]
        public XdStyleStrokeJson Stroke { get; set; }
    }

    public class XdStyleFillJson
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("color")]
        public XdColorJson Color { get; set; }

        [JsonProperty("pattern")]
        public XdStyleFillPatternJson Pattern { get; set; }
    }

    public class XdStyleFillPatternJson
    {
        [JsonProperty("width")]
        public float Width { get; set; }

        [JsonProperty("height")]
        public float Height { get; set; }

        [JsonProperty("meta")]
        public XdStyleFillPatternMetaJson Meta { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }
    }

    public class XdStyleFillPatternMetaJson
    {
        [JsonProperty("ux")]
        public XdStyleFillPatternMetaUxJson Ux { get; set; }
    }

    public class XdStyleFillPatternMetaUxJson
    {
        [JsonProperty("scaleBehavior")]
        public string ScaleBehavior { get; set; }

        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("hrefLastModifiedDate")]
        public uint HrefLastModifiedDate { get; set; }
    }

    public class XdStyleStrokeJson
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("color")]
        public XdColorJson Color { get; set; }

        [JsonProperty("width")]
        public float Width { get; set; }

        [JsonProperty("align")]
        public string Align { get; set; }
    }

    public class XdShapeJson
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("x")]
        public float X { get; set; }

        [JsonProperty("y")]
        public float Y { get; set; }

        [JsonProperty("width")]
        public float Width { get; set; }

        [JsonProperty("height")]
        public float Height { get; set; }
    }

    public class XdManifestJson
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("manifest-format-version")]
        public string ManifestFormatVersion { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("components")]
        public XdManifestComponentJson[] Components { get; set; }

        [JsonProperty("children")]
        public XdManifestChildJson[] Children { get; set; }
    }

    public class XdManifestComponentJson
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("rel")]
        public string Rel { get; set; }

        [JsonProperty("width")]
        public float Width { get; set; }

        [JsonProperty("height")]
        public float Height { get; set; }
    }

    public class XdMetaJson
    {
        // not implemented
    }

    public class XdManifestChildJson
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("children")]
        public XdManifestChildJson[] Children { get; set; }

        [JsonProperty("components")]
        public XdManifestComponentJson[] Components { get; set; }
    }

    public class XdArtboardJson
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("children")]
        public XdArtboardChildJson[] Children { get; set; }

        [JsonProperty("resources")]
        public XdArtboardResourcesJson Resources { get; set; }

        [JsonProperty("artboards")]
        public XdArtboardArtboardsJson Artboards { get; set; }
    }

    public class XdArtboardChildJson
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("meta")]
        public XdMetaJson Meta { get; set; }

        [JsonProperty("style")]
        public XdStyleJson Style { get; set; }

        [JsonProperty("artboard")]
        public XdArtboardChildArtboardJson Artboard { get; set; }
    }

    public class XdArtboardChildArtboardJson
    {
        [JsonProperty("children")]
        public XdArtboardChildArtboardChildJson[] Children { get; set; }

        [JsonProperty("meta")]
        public XdMetaJson Meta { get; set; }

        [JsonProperty("ref")]
        public string Ref { get; set; }
    }

    public class XdArtboardChildArtboardChildJson
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("meta")]
        public XdMetaJson Meta { get; set; }

        [JsonProperty("transform")]
        public XdTransformJson Transform { get; set; }

        [JsonProperty("group")]
        public XdArtboardChildArtboardChildGroupJson Group { get; set; }

        [JsonProperty("style")]
        public XdStyleJson Style { get; set; }

        [JsonProperty("shape")]
        public XdShapeJson Shape { get; set; }
    }

    public class XdArtboardChildArtboardChildGroupJson
    {
        [JsonProperty("children")]
        public XdArtboardChildArtboardChildJson[] Children { get; set; }
    }

    public class XdArtboardResourcesJson
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }

    public class XdArtboardArtboardsJson
    {
        [JsonProperty("href")]
        public string Href { get; set; }
    }
}