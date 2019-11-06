namespace OrganisationRegistry.File.Splitter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No file specified!");
                return;
            }

            var fileName = args[0];

            if (!File.Exists(fileName))
            {
                Console.WriteLine("File does not exist!");
                return;
            }

            if (!new[] { ".TXT", ".CSV" }.Contains(Path.GetExtension(fileName).ToUpperInvariant()))
            {
                Console.WriteLine("File extension is not supported!");
                return;
            }

            var lines = File.ReadAllLines(fileName);
            var header = lines.First();

            var linesWithoutHeader = lines.Skip(1);
            var chunks = linesWithoutHeader.ToList().SplitList(500);
            var index = 0;

            var path = Path.Combine(Path.GetDirectoryName(fileName), "output");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Array.ForEach(Directory.GetFiles(path), File.Delete);

            foreach (var chunk in chunks)
            {
                chunk.Insert(0, header);

                var fullPath = Path.Combine(path, $"{Path.GetFileNameWithoutExtension(fileName)}_{++index}.txt");

                File.WriteAllLines(fullPath, chunk);
            }
        }
    }

    public static class ListExtensions
    {
        public static IEnumerable<List<T>> SplitList<T>(this List<T> lines, int size = 30)
        {
            for (var i = 0; i < lines.Count; i += size)
                yield return lines.GetRange(i, Math.Min(size, lines.Count - i));
        }
    }
}
