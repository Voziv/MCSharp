using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace LevelConverter
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Title = "MCSharp - Level Converter"; // Set the console title

            LevelConverter SourceLevel = new LevelConverter();

            Console.WriteLine("Choose a file to convert...");
            string file = Console.ReadLine();
            if (File.Exists(file))
            {
                SourceLevel.FileName = file;

                if (SourceLevel.Load())
                {
                    Console.WriteLine("File sucessfully loaded!");
                    Console.WriteLine(SourceLevel.InputFormat + " level format detected!");
                    Console.Write("Saving as MCSharp Level format: ");
                    if (SourceLevel.SaveNewMCSharp())
                    {
                        Console.WriteLine("Saved OK.");
                    }
                    else
                    {
                        Console.WriteLine("Saved FAILED.");
                    }
                }
                else
                {
                    Console.WriteLine("Could not load file.");
                }
            }
            else
            {
                Console.WriteLine("File not found!");
            }
            Console.WriteLine("");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
