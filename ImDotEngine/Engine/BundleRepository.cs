using System.Collections.Generic;
using System.IO.Compression;
using System.IO;

class BundleInfo
{
    public string Name;
    public Dictionary<string, byte[]> Contents;
}

// for now i'll just store the dictionaries in memory but later on
// I want to load them one by one trigger some kind of load asset event
// then get rid of the unloaded bundle from memory
class BundleRepository
{
    Dictionary<string, BundleInfo> Bundles = new Dictionary<string, BundleInfo>();

    public void Initialize()
    {
        DebugLogger.Log("Bundles", "Loading bundles..");

        foreach (var bundlePath in Directory.GetFiles("Data"))
        {
            FileInfo bundle = new FileInfo(bundlePath);

            var bundleName = bundle.Name.Split('.')[0];

            Bundles[bundleName] = LoadBundle(bundleName);
            DebugLogger.Log("Bundles", $"Loaded {bundleName}");
        }

        DebugLogger.Log("Bundles", "Loaded bundles");
    }

    public BundleInfo GetBundle(string bundleName) => Bundles[bundleName];

    public BundleInfo LoadBundle(string bundleName)
    {
        var assets = new Dictionary<string, byte[]>();

        byte[] rawBundle = File.ReadAllBytes($"Data\\{bundleName}.bundle");

        using (MemoryStream zipStream = new MemoryStream(rawBundle))
        using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                using (var entryStream = entry.Open())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        entryStream.CopyTo(memoryStream);
                        assets[entry.FullName] = memoryStream.ToArray();
                    }
                }
            }
        }

        // return unpacked bundle
        return new BundleInfo()
        {
            Name = bundleName,
            Contents = assets
        };
    }
}