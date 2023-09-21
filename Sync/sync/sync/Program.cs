using System;
using System.IO;
using System.Timers;
using Timer = System.Timers.Timer;

class Program
{
    private static string src = @"C:\Source";
    private static string dst = @"C:\Destination";
    private static string logFile = @"C:\Logs\Log.txt";
    private static System.Timers.Timer timer;

    static void Main(string[] args)
    {
        int timeMin = 0;
        Console.WriteLine("Introduce time interval in minutes! ");

        if (!int.TryParse(Console.ReadLine(), out timeMin) || timeMin <= 0)
        {
            Console.WriteLine("Invalid input. Please enter a valid positive integer for the sync interval.");
            return;
        }

        SetTimer(timeMin);
        Console.ReadLine();
    }

    private static void SetTimer(int time)
    {
        timer = new Timer();
        timer.Interval = time * 60 * 1000;
        timer.Elapsed += ElapsedTime;
        timer.AutoReset = true;
        timer.Start();
    }

    private static void ElapsedTime(object sender, ElapsedEventArgs e)
    {
        Console.WriteLine("Copying files...");
        SyncronizeFolder();
    }

    private static void SyncronizeFolder()
    {
        try
        {
            if (!Directory.Exists(src))
            {
                // If there is no source there is nothing to copy 
                Console.WriteLine("Source folder does not exist!");
                return;
            }

            if (!Directory.Exists(dst))
            {
                // If there is no destination, create it
                Directory.CreateDirectory(dst);
            }


            string[] files = Directory.GetFiles(src);
            string logContent = "";

            // Add
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                string srcFile = Path.Combine(src, fileName);
                string dstFile = Path.Combine(dst, fileName);

                if (!File.Exists(dstFile))
                {
                    File.Copy(srcFile, dstFile, true); // update existing files
                    Console.WriteLine($"Copied {fileName} to the destination folder.");
                    logContent += $"Copied {fileName} to the destination folder.\n";
                }
            }

            // Remove
            foreach (var dstFile in Directory.GetFiles(dst))
            {
                string fileName = Path.GetFileName(dstFile);
                string srcFile = Path.Combine(src, fileName);

                if (!File.Exists(srcFile))
                {
                    File.Delete(dstFile);
                    Console.WriteLine($"Deleted {fileName} from the destination folder.");
                    logContent += $"Deleted {fileName} from the destination folder.\n";
                }
            }

            // Add info in log file
            File.AppendAllText(logFile, logContent);
            Console.WriteLine($"Synchronization log saved to {logFile}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }


}