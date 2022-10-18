using System.Security.Cryptography;
using System.Text;

namespace T2_SHA256;

public class Program
{
    private static readonly int ChunkSize = 1024;

    public static void Main(string[] args)
    {
        var fileName = "C:/Dev/T2-SHA256/Dados/FuncoesResumo - SHA1.mp4";
        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            var lastChunkSize = fs.Length % ChunkSize;
            using (BinaryReader br = new BinaryReader(fs, Encoding.ASCII))
            {
                br.BaseStream.Position = fs.Length - lastChunkSize;
                byte[] chunk = br.ReadBytes(ChunkSize);

                using (var sha256 = SHA256.Create())
                {
                    var hash = sha256.ComputeHash(chunk);
                    Console.WriteLine(BitConverter.ToString(hash).Replace("-", "").ToLower());
                }
            }
        }
    }
}