using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Task_1
{
    class Program
    {
        //Added event and a delegate to handle operations when all files finish printing
        public delegate void Notification();
        public event Notification NotificationPublished;
        //Created a barrier to wait untill all files are printed to trigger an event
        static Barrier bar = new Barrier(11);
        //List of all available colours which are to be written in text file
        public static List<string> ColourList = new List<string> { "Yellow", "Red", "Blue", "Green", "Magenta", "Cyan", "DarkYellow", "DarkGreen", "White", "DarkBlue" };
        public static Random rnd = new Random();
        //List of available paper orientations
        public static List<string> Orientation = new List<string> { "Landscape", "Portrait" };
        //List of available paper formats
        static string[] Formats = new string[] { "A3", "A4" };
        public static readonly string ColourPath = @".../.../ColourPallet.txt";
        static StreamWriter sw;
        //Locks are used to lock printers so only 1 thread can print at a time
        static object A3Lock = new object();
        static object A4Lock = new object();
        static Program pr = new Program();
        static void Main(string[] args)
        {
            Console.WriteLine("\t\t\tWelcome to Printer Simulation!");
            //Running CreateColours method to write colours to file
            pr.CreateColours();
            //Subscribing a method to an event (anonymous method in this case)
            pr.NotificationPublished += () =>{Console.WriteLine("\n\t\t\tAll files are successfuly printed!");};
            for (int i = 1; i < 11; i++)
            {
                //Thread.Sleep(100) ensures each individual print request is issued at regular intervals
                Thread.Sleep(100);
                //Randomly deciding what format the next print request will be (A3 or A4)
                string format = Formats[rnd.Next(0, 2)];
                if (format == "A3")
                {
                    //Creating new PrintRequest and a new Thread with the appropriate method
                    PrintRequest pr = new PrintRequest(new Thread(A3Printer));
                    //Orientation assigned randomly
                    pr.Orientation = Orientation[rnd.Next(0, 2)];
                    //Name of the thread is the name of the computer
                    pr.T.Name = string.Format("Computer " + i);
                    pr.T.Start(pr);                    
                }
                else if (format == "A4")
                {
                    //Creating new PrintRequest and a new Thread with the appropriate method
                    PrintRequest pr = new PrintRequest(new Thread(A4Printer));
                    //Orientation assigned randomly
                    pr.Orientation = Orientation[rnd.Next(0, 2)];
                    //Name of the thread is the name of the computer
                    pr.T.Name = string.Format("Computer " + i);
                    pr.T.Start(pr);
                }
            }
            //Signaling and waiting until all threads are done
            bar.SignalAndWait();
            //Event is triggered when all threads are done
            pr.OnNotificationPublished();
            Console.ReadLine();
        }
        /// <summary>
        /// This method simulates the work of a printer printing in A3 format
        /// </summary>
        /// <param name="Request">An instance of PrintRequest class which is being serviced</param>
        public static void A3Printer(object Request)
        {
            //Only one print request can be serviced at a time
            lock (A3Lock)
            {                  
                PrintRequest pr = (PrintRequest)Request;
                //Colour Coding
                TextcColour(pr.Colour);
                Console.WriteLine("\n" + pr.T.Name + " sent print request for document in A3 format.\nColour: " + pr.Colour + ".\nOrientation: " + pr.Orientation);
                Console.ResetColor();
                //Simulating printing
                Thread.Sleep(1000);
                //Colour Coding
                TextcColour(pr.Colour);
                Console.WriteLine("\n\tUser of " + pr.T.Name + " can collect their document in A3 format.");
                Console.ResetColor();
                //Notifying the next threads that this one is about to release the lock
                Monitor.Pulse(A3Lock);
            }
            //Waits untill all threads are done to finish
            bar.SignalAndWait();
        }
        /// <summary>
        /// This method simulates the work of a printer printing in A3 format
        /// </summary>
        /// <param name="Request">An instance of PrintRequest class which is being serviced</param>
        public static void A4Printer(object Request)
        {
            lock (A4Lock)
            {
                PrintRequest pr = (PrintRequest)Request;
                //Colour Coding
                TextcColour(pr.Colour);
                Console.WriteLine("\n" + pr.T.Name + " sent print request for document in A4 format.\nColour: " + pr.Colour + ".\nOrientation: " + pr.Orientation);
                Console.ResetColor();
                //Simulating printing
                Thread.Sleep(1000);
                //Colour Coding
                TextcColour(pr.Colour);
                Console.WriteLine("\n\tUser of " + pr.T.Name + " can collect their document in A4 format.");
                Console.ResetColor();
                //Notifying the next threads that this one is about to release the lock
                Monitor.Pulse(A4Lock);
            }
            //Waits untill all threads are done to finish
            bar.SignalAndWait();
        }
        /// <summary>
        /// This method reads colours from the list and writes them to text file
        /// </summary>
        public void CreateColours()
        {
            //Creating new instance of StreamWriter
            sw = new StreamWriter(ColourPath);
            //Creating the file if it doesn't exist
            if (!File.Exists(ColourPath))
                File.Create(ColourPath).Close();
            //Using StreamWriter to write colours into text file, one by one
            using (sw)
            {
                foreach (var colour in ColourList)
                    sw.WriteLine(colour);
            }
        }
        /// <summary>
        /// This method changes the colour of console text depending on the value of Colour property in PrintRequest class
        /// </summary>
        /// <param name="colour">The value of Colour property</param>
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
        /// <summary>
        /// This method triggers an event if there are subscribers to that event
        /// </summary>
        protected virtual void OnNotificationPublished()
        {
            NotificationPublished?.Invoke();
        }
    }
    /// <summary>
    /// Class PrintRequest holds information about individual print requests
    /// </summary>
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
