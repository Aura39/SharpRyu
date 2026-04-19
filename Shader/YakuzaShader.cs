using Shader.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Shader
{
    public class YakuzaShader : IBlock
    {
        string Name;
        ushort VersionMajor;
        ushort VersionMinor;

        IBlock?[] Blocks;

        public YakuzaShader()
        {
            Blocks = new IBlock[4];
        }

        public void Read(BinaryReader br)
        {
            br.ReadChars(4);    // GSFX

            br.ReadUInt32();

            VersionMinor = br.ReadUInt16();
            VersionMajor = br.ReadUInt16();

            uint fxoSize = br.ReadUInt32(); // FXO Size
            br.ReadUInt16(); // Shader Name CRC16
            Name = new string(br.ReadChars(30)).TrimEnd('\0');

            for (int i = 0; i < 4; i++)
            {
                uint offset = br.ReadUInt32();
                uint size = br.ReadUInt32();

                if (offset >= fxoSize && size == 0)
                {
                    Blocks[i] = null;
                    continue;
                }

                var rewind = br.BaseStream.Position;

                br.BaseStream.Seek(offset, SeekOrigin.Begin);
                var blockHeader = new string(br.ReadChars(4)); // Quick read of the block header to determine what block type is needed

                br.BaseStream.Seek(offset, SeekOrigin.Begin);
                switch (blockHeader)
                {

                    // Vertex Shader
                    case "GSVS":
                        {
                            var block = new Block<VertexData>();
                            block.Read(br);
                            Blocks[i] = block;
                            break;
                        }

                    // Pixel Shader
                    case "GSPS":
                        {
                            var block = new Block<PixelData>();
                            block.Read(br);
                            Blocks[i] = block;
                            break;
                        }

                    // Compute Shader (Is never encountered in GSFX, only in a separate file, still keeping for support)
                    case "GSCS":
                        {
                            var block = new Block<ComputeData>();
                            block.Read(br);
                            Blocks[i] = block;
                            break;
                        }

                }

                br.BaseStream.Seek(rewind, SeekOrigin.Begin);
            }
        }

        // Checksum used is a simple sum of every byte
        public ushort CalculateChecksum16(byte[] AsciiBytes)
        {
            ushort sum = 0;
            foreach (byte b in AsciiBytes)
            {
                sum += b;
            }
            return sum;
        }

        int CalcPadding(int BlockSize, int Length)
        {
            return (BlockSize - (Length % BlockSize)) % BlockSize;
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write("GSFX".ToCharArray());
            bw.Write(32);
            bw.Write(VersionMinor);
            bw.Write(VersionMajor);

            var fxoSizeOffset = bw.BaseStream.Position;
            bw.Write(0); // Placeholder for FXO Size
            bw.Write(CalculateChecksum16(Encoding.ASCII.GetBytes(Name))); // Placeholder for accumulated name checksum

            char[] name = new char[30];
            Array.Fill(name, '\0');
            Name.ToCharArray().CopyTo(name, 0);
            bw.Write(name);

            var slotArrayOffset = bw.BaseStream.Position;

            for (int i = 0; i < Blocks.Length; i++)
            {
                bw.Write(0); // Offset Placeholder
                bw.Write(0); // Size Placeholder
            }

            var offsetData = new (int, int)[4];

            for (int i = 0; i < Blocks.Length; i++)
            {
                if (Blocks[i] is not null)
                {
                    offsetData[i].Item1 = (int)bw.BaseStream.Position; // Offset
                    Blocks[i].Write(bw);
                    var size = bw.BaseStream.Position - offsetData[i].Item1;
                    offsetData[i].Item2 = (int)size; // Size
                    var align = CalcPadding(16, (int)bw.BaseStream.Position);
                    bw.Write(new byte[align]);
                }
            }

            var fileSize = bw.BaseStream.Length;

            // Second iteration of the loop to insert empty values
            for (int i = 0; i < offsetData.Length; i++)
            {
                if (Blocks[i] is null)
                {
                    offsetData[i].Item1 = (int)fileSize; // Offset
                    offsetData[i].Item2 = 0; // Size
                }
            }

            bw.Seek((int)slotArrayOffset, SeekOrigin.Begin);

            // Third iteration to actually replace the values
            for (int i = 0; i < offsetData.Length; i++)
            {
                bw.Write(offsetData[i].Item1);
                bw.Write(offsetData[i].Item2);
            }

            bw.Seek((int)fxoSizeOffset, SeekOrigin.Begin);
            bw.Write((uint)fileSize);
        }

        public void Read(string Path)
        {
            using (var stream = File.Open(Path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.ASCII, false))
                {
                    Read(reader);
                }
            }
        }

        public void Write(string Path)
        {
            using (var stream = File.Open(Path, FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.ASCII, false))
                {
                    Write(writer);
                }
            }
        }
    }
}
