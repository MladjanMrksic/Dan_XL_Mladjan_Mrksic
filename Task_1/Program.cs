using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Task_1
{
    class Program
    {
        public delegate void Notification();
        public event Notification NotificationPublished;
        static Barrier bar = new Barrier(11);
        public static List<string> ColourList = new List<string> { "Yellow", "Red", "Blue", "Green", "Magenta", "Cyan", "DarkYellow", "DarkGreen", "White", "DarkBlue" };
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
            Console.WriteLine("\t\t\tWelcome to Printer Simulation!");
            pr.CreateColours();
            pr.NotificationPublished += () =>{Console.WriteLine("\n\t\t\tAll files are successfuly printed!");};
            for (int i = 1; i < 11; i++)
            {
                Thread.Sleep(100);
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
            bar.SignalAndWait();
            pr.OnNotificationPublished();
            Console.ReadLine();
        }
        public static void A3Printer(object Request)
        {
            lock (A3Lock)
            {                  
                PrintRequest pr = (PrintRequest)Request;
                TextcColour(pr.Colour);
                Console.WriteLine("\n" + pr.T.Name + " sent print request for document in A3 format.\nColour: " + pr.Colour + ".\nOrientation: " + pr.Orientation);
                Console.ResetColor();
                Thread.Sleep(1000);
                TextcColour(pr.Colour);
                Console.WriteLine("\n\tUser of " + pr.T.Name + " can collect their document in A3 format.");
                Console.ResetColor();
                Monitor.Pulse(A3Lock);
            }
            bar.SignalAndWait();
        }
        public static void A4Printer(object Request)
        {
            lock (A4Lock)
            {
                PrintRequest pr = (PrintRequest)Request;
                TextcColour(pr.Colour);
                Console.WriteLine("\n" + pr.T.Name + " sent print request for document in A4 format.\nColour: " + pr.Colour + ".\nOrientation: " + pr.Orientation);
                Console.ResetColor();
                Thread.Sleep(1000);
                TextcColour(pr.Colour);
                Console.WriteLine("\n\tUser of " + pr.T.Name + " can collect their document in A4 format.");
                Console.ResetColor();
                Monitor.Pulse(A4Lock);
            }
            bar.SignalAndWait();
        }
        public void CreateColours()
        {
            sw = new StreamWriter(ColourPath);
            if (!File.Exists(ColourPath))
                File.Create(ColourPath).Close();

            using (sw)
            {
                foreach (var colour in ColourList)
                    sw.WriteLine(colour);
            }
        }
        public static void TextcColour(string colour)
        {
            if (colour == "Yellow")
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (colour == "Red")
                Console.ForegroundColor = ConsoleColor.Red;
            else if (colour == "Blue")
                Console.ForegroundColor = ConsoleColor.Blue;
            else if (colour == "Green")
                Console.ForegroundColor = ConsoleColor.Green;
            else if (colour == "Magenta")
                Console.ForegroundColor = ConsoleColor.Magenta;
            else if (colour == "Cyan")
                Console.ForegroundColor = ConsoleColor.Cyan;
            else if (colour == "DarkYellow")
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            else if (colour == "DarkGreen")
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            else if (colour == "White")
                Console.ForegroundColor = ConsoleColor.White;
            else if (colour == "DarkBlue")
                Console.ForegroundColor = ConsoleColor.DarkBlue;
        }
        protected virtual void OnNotificationPublished()
        {
            NotificationPublished?.Invoke();
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
