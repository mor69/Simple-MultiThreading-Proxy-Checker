using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple_Proxy_Checker
{
    class Program
    {
        static int Count = 0;
        static List<string> Proxies = new List<string>();
        static List<string> Alive = new List<string>();
        static List<string> Dead = new List<string>();
        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "MultiThreading Proxy Checker";
            selectfile:
            Console.Clear();
            PrintLogo();
            WriteLine("Select proxy file");
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Proxy File|*.txt";
            dialog.Title = "Please Select Proxy File";
            dialog.ShowDialog();
            string filename = dialog.FileName;
            if (string.IsNullOrEmpty(filename))
            {
                Console.SetCursorPosition(0,4);
                WriteLine("Error, (1) File isn't selected, (2) File is empty. Press a key to try again!");
                Console.ReadKey();
                goto selectfile;
            }
            foreach (string line in File.ReadAllLines(filename))
            {
                try
                {
                    var found = line.Split(':');
                    var proxy = found[0].ToString();
                    var port = found[1].ToString();
                    Proxies.Add(proxy + ":" + port);
                }
                catch (Exception) { }
            }
            WriteLine("Task is running, Press a key to close and save all.");
            Task.Run(()=> new Thread(new ThreadStart(Check)).Start());
            Console.ReadKey();
            WriteLine("Saving, Please wait.");
            SaveAll();
            Environment.Exit(0);
        }
        static void Check()
        {
            while (Count < Proxies.Count)
            {
                Count++;
                try
                {
                    var proxy = Proxies[Count];
                    var ip = proxy.Split(':')[0];
                    var port = proxy.Split(':')[1];
                    var ping = new Ping();
                    var replyfromserver = ping.Send(ip);
                    if (replyfromserver.Status == IPStatus.Success)
                    {
                        Alive.Add(proxy);
                        Console.ForegroundColor = ConsoleColor.Green;
                        WriteLine("Alive - " + proxy);
                    }
                    else
                    {
                        Dead.Add(proxy);
                        Console.ForegroundColor = ConsoleColor.Red;
                        WriteLine("Dead - " + proxy);
                    }
                    Console.Title = string.Format("MultiThreading Proxy Checker | Alive ({0}) Dead ({1})", Alive.Count(), Dead.Count()); ;
                }
                catch { }
            }
        }
        static void SaveAll()
        {
            if (!Directory.Exists("Results")) { Directory.CreateDirectory("Results"); }
            using (TextWriter tx = new StreamWriter("Results/Alive.txt")) {
                foreach (string line in Alive) { tx.WriteLine(line); }
                tx.Close();
            }
            using (TextWriter tx = new StreamWriter("Results/Dead.txt"))
            {
                foreach (string line in Dead) { tx.WriteLine(line); }
                tx.Close();
            }
        }
        static void PrintLogo()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(@" - [MultiThreading Proxy Checker | Supported All Proxies Types]
 - [Credit]
 Discord Account - Mor#1046
 Github - https://github.com/mor69
 Discord Server - https://discord.gg/nsmAxzh");
        }
        static void WriteLine(string line)
        {
            Console.WriteLine(" - " + line);
        }
    }
}
