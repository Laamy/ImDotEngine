using System;
using System.Text;

class CID
{
    // 475,920,314,814,253,376,475,136
    private static readonly char[] characterSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-+=)(*&^%$#@!~<;>.?/'{\"[{]}|\\" // _-+=)(*&^%$#@!~<;>.?/'{\"[{]}|\\
        .ToCharArray();
    private static readonly Random random = new Random();

    public string Id { get; private set; }

    private CID(string id) => Id = id;

    public static CID New()
    {
        int length = 12; // temp

        StringBuilder idBuilder = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            int index = random.Next(characterSet.Length);
            idBuilder.Append(characterSet[index]);
        }

        return new CID(idBuilder.ToString());
    }
    public override string ToString() => Id;
}