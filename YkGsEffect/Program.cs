using Shader;
using Shader.Blocks;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace YkGsEffect
{
    internal class Program
    {
        static public byte[] GetBytecode(IBlock block)
        {
            switch (block)
            {
                case Block<GSVS> gsvs:
                    {
                        return gsvs.CompiledBinary;
                    }
                case Block<GSPS> gsps:
                    {
                        return gsps.CompiledBinary;
                    }
                case Block<GSCS> gscs:
                    {
                        return gscs.CompiledBinary;
                    }
                case Block<GSGS> gsgs:
                    {
                        return gsgs.CompiledBinary;
                    }
                default:
                    return Array.Empty<byte>();
            }
        }

        static public object GetDump(IBlock block)
        {
            switch (block)
            {
                case Block<GSVS> gsvs:
                    {
                        return new
                        {
                            Type = gsvs.Data.GetMagic(),
                            Semantic = gsvs.Data.InputSemantics
                        };
                    }
                case Block<GSPS> gsps:
                    {
                        return new { Type = gsps.Data.GetMagic() };
                    }
                case Block<GSCS> gscs:
                    {
                        return new { Type = gscs.Data.GetMagic() };
                    }
                case Block<GSGS> gsgs:
                    {
                        return new { Type = gsgs.Data.GetMagic() };
                    }
                case null:
                    return new { Type = "NULL" };
                default:
                    return new { };
            }
        }

        static public void Unpack(string FilePath)
        {
            var sh = AutoYakuzaShader.Read(FilePath);
            var dirName = Path.Join(Path.GetDirectoryName(FilePath), Path.GetFileNameWithoutExtension(FilePath));
            Directory.CreateDirectory(dirName);
            switch (sh)
            {
                case YakuzaShader gsfx:
                    {
                        var json = new
                        {
                            InternalName = gsfx.Name,
                            Version = new ushort[2] { gsfx.VersionMajor, gsfx.VersionMinor },
                            Blocks = new object[gsfx.Blocks.Length],
                        };

                        int i = 0;
                        foreach(var block in gsfx.Blocks)
                        {
                            json.Blocks[i] = GetDump(block);

                            if (block is not null)
                                File.WriteAllBytes(dirName + $"/block{i}_{block.GetMagic()}.dxbc", GetBytecode(block));
                            i++;
                        }

                        File.WriteAllText(dirName + $"/shader.json", JsonSerializer.Serialize(json, new JsonSerializerOptions() { WriteIndented = true }));
                        break;
                    }
                case Block<GSVS> gsvs:
                    {
                        File.WriteAllBytes(dirName + "/gsvs.dxbc", GetBytecode(gsvs));
                        File.WriteAllText(dirName + $"/shader.json", JsonSerializer.Serialize(GetDump(gsvs), new JsonSerializerOptions() { WriteIndented = true }));
                        break;
                    }
                case Block<GSPS> gsps:
                    {
                        File.WriteAllBytes(dirName + "/gsps.dxbc", GetBytecode(gsps));
                        File.WriteAllText(dirName + $"/shader.json", JsonSerializer.Serialize(GetDump(gsps), new JsonSerializerOptions() { WriteIndented = true }));
                        break;
                    }
                case Block<GSCS> gscs:
                    {
                        File.WriteAllBytes(dirName + "/gscs.dxbc", GetBytecode(gscs));
                        File.WriteAllText(dirName + $"/shader.json", JsonSerializer.Serialize(GetDump(gscs), new JsonSerializerOptions() { WriteIndented = true }));
                        break;
                    }
                case Block<GSGS> gsgs:
                    {
                        File.WriteAllBytes(dirName + "/gsgs.dxbc", GetBytecode(gsgs));
                        File.WriteAllText(dirName + $"/shader.json", JsonSerializer.Serialize(GetDump(gsgs), new JsonSerializerOptions() { WriteIndented = true }));
                        break;
                    }
            }

        }

        static public void Repack(string FolderPath)
        {

        }

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage:\n");
                Console.WriteLine("    Specify .fxo/.vso/.pso/.cso (Drag & Drop) - Unpack into folder");
                Console.WriteLine("    Specify unpacked folder (Drag & Drop) - Pack into .fxo/.vso/.pso/.cso");
            }
            else
            {
                foreach (var arg in args)
                {
                    if (File.Exists(arg))
                        Unpack(arg);
                    if (Directory.Exists(arg))
                        Repack(arg);
                }
            }
        }
    }
}
