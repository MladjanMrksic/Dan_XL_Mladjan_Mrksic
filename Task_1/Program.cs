using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task_1
{
    class Program
    {
        public static Random rnd = new Random();
        static string[] Formats = new string[] { "A3", "A4" };
        public static readonly string ColourPath = @".../.../ColourPallet.txt";
        static StreamWriter sw = new StreamWriter(ColourPath);
        static StreamReader sr = new StreamReader(ColourPath);
        static object A3Lock = new object();
        static object A4Lock = new object();
        static void Main(string[] args)
        {
        }
        public void A3Printer(object Request)
        {
            while (Monitor.TryEnter(A3Lock) == false)
            {
                Thread.Sleep(100);
            }
            lock (A3Lock)
            {                  
                PrintRequest pr = (PrintRequest)Request;
                Console.WriteLine(pr.T.Name + " sent print request for document in A3 format.\nColour: " + pr.Colour + ".\nOrientation: " + pr.Orientation);
                Thread.Sleep(1000);
                Console.WriteLine("User of " + pr.T.Name + " can collect their document in A3 format.");
            }
        }
        public void A4Printer(object Request)
        {
            while (Monitor.TryEnter(A3Lock) == false)
            {
                Thread.Sleep(100);
            }
            lock (A3Lock)
            {
                PrintRequest pr = (PrintRequest)Request;
                Console.WriteLine(pr.T.Name + " sent print request for document in A4 format.\nColour: " + pr.Colour + ".\nOrientation: " + pr.Orientation);
                Thread.Sleep(1000);
                Console.WriteLine("User of " + pr.T.Name + " can collect their document in A4 format.");
            }
        }
    }
    class PrintRequest
    {
        string[] lines = File.ReadAllLines(@".../.../ColourPallet.txt");
        Random r = new Random();

        internal Thread T;
        internal string Colour;
        internal string Orientation;

        public PrintRequest(Thread t)
        {   
            T = t;
            Colour = lines[r.Next(0, lines.Length - 1)];
        }
    }
}
