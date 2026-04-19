using Shader;
using Shader.Blocks;
using System.Text;

namespace RyuTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var fxo = AutoYakuzaShader.Read("gsfx.fxo");    // Returns YakuzaShader
            var vso = AutoYakuzaShader.Read("gsvs.vso");    // Returns Block<VertexData>
            var pso = AutoYakuzaShader.Read("gsps.pso");    // Returns Block<PixelData>
            var cso = AutoYakuzaShader.Read("gscs.cso");    // Returns Block<ComputeData>

            fxo.Write("gsfx_new.fxo");
            vso.Write("gsvs_new.vso");
            pso.Write("gsps_new.pso");
            cso.Write("gscs_new.cso");
        }
    }
}
