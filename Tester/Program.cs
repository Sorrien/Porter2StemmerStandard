using Porter2StemmerStandard;
using System;
using System.Diagnostics;
using System.Linq;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var stemmer = new EnglishPorter2Stemmer();

            var sw = new Stopwatch();
            sw.Start();

            var lines = System.IO.File.ReadAllLines(@"C:\tmp\terms\ENCOUNTERPROBLEMS-che.csv");

            var splitChars = "/ ,.\\:;\"[]()-_'+&*".ToArray();

            var words = lines
                .Where(line => line.Length > 0)
                .Select(line => line.Split('¤')[2].Trim('"'))
                .SelectMany(field => field.Split(splitChars))
                .ToList();

            Console.WriteLine($"tests loaded in {sw.ElapsedMilliseconds} ms");

            sw.Restart();

            var ct = 0;

            foreach(var word in words)
            {
                stemmer.Stem(word);

                ct++;

                if(ct % 250_000 == 0)
                {
                    Console.WriteLine($"{ct:N0}/{words.Count:N0}");
                }
            }

            sw.Stop();

            Console.WriteLine($"Done. {words.Count} in {sw.ElapsedMilliseconds} ms");
            Console.ReadLine();

        }
    }
}
