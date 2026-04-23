using Shader;
using Shader.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RyuTest
{
    internal class ShaderTest : ITest
    {
        public int CalculateDifference(byte[] original, byte[] actual)
        {
            int length = Math.Min(original.Length, actual.Length);
            int difference = Math.Max(original.Length, actual.Length) - length;

            for (int i = 0; i < length; i++)
            {
                if (original[i] != actual[i])
                {
                    difference++;
                }
            }
            return difference;
        }

        public void Run()
        {
            foreach (var file in Directory.EnumerateFiles("cima_d3d11"))
            {
                var original = File.ReadAllBytes(file);
                var sh = AutoYakuzaShader.Read(file);

                if (sh is YakuzaShader) {
                    YakuzaShader gsfx = (YakuzaShader)sh;
                    foreach (var block in gsfx.Blocks)
                    {
                        if (block is Block<GSVS>)
                        {
                            var gsvs = (Block<GSVS>)block;
                            Console.WriteLine($"{Path.GetFileName(file).PadRight(50)} : {gsvs.Data.InputSemantics}");
                        }
                    }
                }
            }
        }

        public void RunMismatch()
        {
            (string, int) biggestMismatch = ("NULL", 0);

            foreach (var file in Directory.EnumerateFiles("cima_d3d11"))
            {
                var original = File.ReadAllBytes(file);
                var sh = AutoYakuzaShader.Read(file);
                var regenerated = sh.Write();

                var diff = CalculateDifference(original, regenerated);

                if (biggestMismatch.Item2 < diff)
                {
                    biggestMismatch.Item1 = file;
                    biggestMismatch.Item2 = diff;
                }

                Console.WriteLine($"{Path.GetFileName(file).PadRight(50)} : {diff,4} bytes. (out of {original.Length,7} bytes)");
            }

            Console.WriteLine($"\nBiggest Offender is: {biggestMismatch.Item1} with {biggestMismatch.Item2} bytes mismatching.");
        }
    }
}
