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
        public static List<string> ColourList = new List<string> { "Yellow", "Red", "Blue", "Green", "Orange", "Pink", "Purple", "Brown", "White", "LightBlue" };
        public static Random rnd = new Random();
        public static List<string> Orientation = new List<string> { "Landscape", "Portrait" };
        static string[] Formats = new string[] { "A3", "A4" };
        public static readonly string ColourPath = @".../.../ColourPallet.txt";
        static StreamWriter sw;
        static object A3Lock = new object();
        static object A4Lock = new object();
        static Program pr = new Program();
        static void Main(string[] args)
        {

            pr.CreateColours();

            Thread.Sleep(1000);
            for (int i = 1; i < 11; i++)
            {
                string format = Formats[rnd.Next(0, 2)];
                if (format == "A3")
                {
                    PrintRequest pr = new PrintRequest(new Thread(A3Printer));
                    pr.Orientation = Orientation[rnd.Next(0, 2)];
                    pr.T.Name = string.Format("Computer " + i);
                    pr.T.Start(pr);
                }
                else if (format == "A4")
                {
                    PrintRequest pr = new PrintRequest(new Thread(A4Printer));
                    pr.Orientation = Orientation[rnd.Next(0, 2)];
                    pr.T.Name = string.Format("Computer " + i);
                    pr.T.Start(pr);
                }
            }
            Console.ReadLine();
        }
        public static void A3Printer(object Request)
        {
            lock (A3Lock)
            {                  
                PrintRequest pr = (PrintRequest)Request;
                Console.WriteLine(pr.T.Name + " sent print request for document in A3 format.\nColour: " + pr.Colour + ".\nOrientation: " + pr.Orientation);
                Thread.Sleep(1000);
                Console.WriteLine("User of " + pr.T.Name + " can collect their document in A3 format.");
                Monitor.Pulse(A3Lock);
            }
        }
        public static void A4Printer(object Request)
        {
            lock (A4Lock)
            {
                PrintRequest pr = (PrintRequest)Request;
                Console.WriteLine(pr.T.Name + " sent print request for document in A4 format.\nColour: " + pr.Colour + ".\nOrientation: " + pr.Orientation);
                Thread.Sleep(1000);
                Console.WriteLine("User of " + pr.T.Name + " can collect their document in A4 format.");
                Monitor.Pulse(A4Lock);
            }
        }
        public void CreateColours()
        {
            sw = new StreamWriter(ColourPath);
            if (!File.Exists(ColourPath))
                File.Create(ColourPath).Close();

            using (sw)
            {
                foreach (var colour in ColourList)
                {
                    sw.WriteLine(colour);
                }
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
