using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Shader.Blocks
{
    public class GSVS : BlockData
    {
        public string InputSemantics;

        public GSVS()
        {

        }
        public override string GetMagic()
        {
            return "GSVS";
        }

        public override void Read(BinaryReader br)
        {
            br.ReadBytes(192);
            InputSemantics = new string(br.ReadChars(128)).TrimEnd('\0');
        }

        public override void Write(BinaryWriter bw)
        {
            bw.Write(new byte[192]);

            char[] semantic = new char[128];
            Array.Fill(semantic, '\0');
            InputSemantics.ToCharArray().CopyTo(semantic, 0);
            bw.Write(semantic);
        }
    }
}
