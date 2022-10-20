using System.Security.Cryptography;
using System.Text;
using Spectre.Console;

namespace T2_SHA256;

public class Program
{
    private const int BlockSize = 1024;

    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            AnsiConsole.MarkupLine("[red]Usage:[/] T2_SHA256.exe [green]<file>[/]");
            return;
        }

        var h0 = CalculateHash(args[0]);
        AnsiConsole.MarkupLine($"h0:\t\t[green]{Convert.ToHexString(h0)}[/]");
    }

    private static byte[] CalculateHash(string fileName)
    {
        using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        using var br = new BinaryReader(fs, Encoding.ASCII);
        using var sha256 = SHA256.Create();

        var lastBlockSize = fs.Length % BlockSize;
        var blockCount = (int)((fs.Length - lastBlockSize) / BlockSize);

        br.BaseStream.Seek(fs.Length - lastBlockSize, SeekOrigin.Begin);
        var block = br.ReadBytes(BlockSize);

        var hash = sha256.ComputeHash(block);

        AnsiConsole.Progress().Start(ctx =>
        {
            var task = ctx.AddTask("Calculating hash...", maxValue: blockCount);
            for (var i = blockCount - 1; i >= 0; i--)
            {
                br.BaseStream.Seek((long)i * BlockSize, SeekOrigin.Begin);
                block = br.ReadBytes(BlockSize);
                var newHash = new byte[block.Length + hash.Length];
                Buffer.BlockCopy(block, 0, newHash, 0, block.Length);
                Buffer.BlockCopy(hash, 0, newHash, block.Length, hash.Length);
                hash = sha256.ComputeHash(newHash);

                task.Increment(1);
            }
        });

        return hash;
    }
}