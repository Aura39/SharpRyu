using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shader.Blocks
{
    public class GSGS : BlockData
    {
        public GSGS()
        {
            
        }

        public override string GetMagic()
        {
            return "GSGS";
        }

        public override void Read(BinaryReader br)
        {
            br.ReadBytes(64);
        }

        public override void Write(BinaryWriter bw)
        {
            bw.Write(new byte[64]);
        }
    }
}
