using System.Security.Cryptography;
using System.Text;

namespace T2_SHA256;

public class Program
{
    private const int ChunkSize = 1024;

    public static void Main(string[] args)
    {
        const string fileName = "C:/Dev/T2-SHA256/Dados/FuncoesResumo - SHA1.mp4";

        CalculateHash(fileName);
    }

    private static byte[] CalculateHash(string fileName)
    {
        using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        using var br = new BinaryReader(fs, Encoding.ASCII);
        using var sha256 = SHA256.Create();

        long lastChunkSize = fs.Length % ChunkSize;
        var chunkCount = (int)((fs.Length - lastChunkSize) / ChunkSize);

        br.BaseStream.Position = fs.Length - lastChunkSize;
        byte[] chunk = br.ReadBytes(ChunkSize);

        byte[] hash = sha256.ComputeHash(chunk);
        Console.WriteLine(Convert.ToHexString(hash));

        for (int i = chunkCount - 1; i >= 0; i++)
        {
            br.BaseStream.Position = i * ChunkSize;
            chunk = br.ReadBytes(ChunkSize);
            var block = new byte[chunk.Length + hash.Length];
            Buffer.BlockCopy(chunk, 0, block, 0, chunk.Length);
            Buffer.BlockCopy(hash, 0, block, chunk.Length, hash.Length);
            hash = sha256.ComputeHash(block);
        }

        return null;
    }
}