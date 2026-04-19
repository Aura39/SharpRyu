using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shader
{
    public abstract class BlockData
    {
        public abstract void Read(BinaryReader br);
        public abstract void Write(BinaryWriter bw);
        public abstract string GetMagic();
    }
}
