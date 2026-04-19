using Shader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RyuTest
{
    internal class ShaderTest : ITest
    {
        public void Run()
        {
            var fxo = AutoYakuzaShader.Read("gsfx.fxo");    // Returns YakuzaShader
            //var vso = AutoYakuzaShader.Read("gsvs.vso");    // Returns Block<VertexData>
            //var pso = AutoYakuzaShader.Read("gsps.pso");    // Returns Block<PixelData>
            //var cso = AutoYakuzaShader.Read("gscs.cso");    // Returns Block<ComputeData>

            fxo.Write("gsfx_new.fxo");
            //vso.Write("gsvs_new.vso");
            //pso.Write("gsps_new.pso");
            //cso.Write("gscs_new.cso");

            var og = File.ReadAllBytes("gsfx.fxo");
            var regen = File.ReadAllBytes("gsfx_new.fxo");

            int length = Math.Min(og.Length, regen.Length);
            int difference = Math.Max(og.Length, regen.Length) - length;

            List<int> differences = new List<int>();

            for (int i = 0; i < length; i++)
            {
                if (og[i] != regen[i])
                {
                    difference++;
                    differences.Add(i);
                }
            }

            Console.WriteLine($"GSFX Difference: {difference} bytes.");
            
            foreach (var diff in differences)
            {
                Console.Write($"0x{diff:X4} ");
            }
        }
    }
}
