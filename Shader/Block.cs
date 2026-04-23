using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Shader
{
    public interface IBlock
    {
        public string GetMagic();
        public void Read(BinaryReader br);
        public void Write(BinaryWriter bw);
        public void Read(string Path);
        public void Write(string Path);
        public void Read(byte[] bytes);
        public byte[] Write();
    }
    
    public class Block<T> : IBlock where T : BlockData, new()
    {
        ushort VersionMajor;
        ushort VersionMinor;

        public T Data;

        public byte[] CompiledBinary;

        public void Read(BinaryReader br)
        {
            br.ReadChars(4); // Magic is read but not saved because you can derive it from the generic type

            br.ReadUInt32();

            VersionMinor = br.ReadUInt16();
            VersionMajor = br.ReadUInt16();

            var bytecodeSize = br.ReadInt32();

            br.ReadBytes(48); // Just a skip

            Data = new T();

            Data.Read(br);

            CompiledBinary = br.ReadBytes(bytecodeSize);
        }

        public void Write(BinaryWriter bw)
        {
            var blockBeginningOffset = bw.BaseStream.Position;
            bw.Write(Data.GetMagic().ToCharArray());

            bw.Write(32);

            bw.Write(VersionMinor);
            bw.Write(VersionMajor);

            bw.Write(CompiledBinary.Length);

            bw.Write(0); bw.Write(0);
            
            var blockSizeOffset = bw.BaseStream.Position;
            bw.Write(0);

            bw.Write(CompiledBinary.Length);

            bw.Write(new byte[16]);

            bw.Write(System.IO.Hashing.Crc32.Hash(CompiledBinary)); // Bytecode CRC-32

            bw.Write(new byte[12]);

            Data.Write(bw);

            var blockEndOffset = bw.BaseStream.Position;

            bw.Seek((int)blockSizeOffset, SeekOrigin.Begin);
            bw.Write((int)(blockEndOffset - blockBeginningOffset));
            bw.Seek((int)blockEndOffset, SeekOrigin.Begin);

            bw.Write(CompiledBinary);
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

        public void Read(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            var br = new BinaryReader(stream);
            Read(br);
        }

        public byte[] Write()
        {
            var stream = new MemoryStream();
            var bw = new BinaryWriter(stream);
            Write(bw);
            return stream.ToArray();
        }

        public string GetMagic()
        {
            return Data.GetMagic();
        }
    }
}
