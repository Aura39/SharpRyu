using Shader.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shader
{
    public class AutoYakuzaShader
    {
        public static IBlock? Read(byte[] Data)
        {
            var stream = new MemoryStream(Data);
            using (var reader = new BinaryReader(stream, Encoding.ASCII, false))
            {
                return Read(reader);
            }
        }
        public static IBlock? Read(string Path)
        {
            using (var stream = File.Open(Path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.ASCII, false))
                {
                    return Read(reader);
                }
            }
        }
        public static IBlock? Read(BinaryReader br)
        {
            br.BaseStream.Seek(0, SeekOrigin.Begin);
            var magic = new string(br.ReadChars(4));

            br.BaseStream.Seek(0, SeekOrigin.Begin);
            switch (magic)
            {
                case "GSFX":
                    var gsfx = new YakuzaShader();
                    gsfx.Read(br);
                    return gsfx;
                case "GSVS":
                    var gsvs = new Block<GSVS>();
                    gsvs.Read(br);
                    return gsvs;
                case "GSPS":
                    var gsps = new Block<GSPS>();
                    gsps.Read(br);
                    return gsps;
                case "GSCS":
                    var gscs = new Block<GSCS>();
                    gscs.Read(br);
                    return gscs;
                case "GSGS":
                    var gsgs = new Block<GSGS>();
                    gsgs.Read(br);
                    return gsgs;
            }
            return null;
        }
    }
}
