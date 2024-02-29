using System;
using System.IO;
using System.Threading;
using System.Management;
using System.Linq;
public class Menu
{
    public static void Main(string[] args)
    {
        bool exit = false;
        String filePath = "test.txt";
        while (!exit)
        {
            if (File.Exists(filePath) && new FileInfo(filePath).Length == 0)
            {
                Console.WriteLine("Please select an option:");
                Console.WriteLine("2. To add a new app to ban");
                Console.WriteLine("3. To exit");
                int thisResponse = GetResponse();
                if (thisResponse == 2)
                {
                    WriteToFile(filePath);
                }
                else if (thisResponse == 5)
                {
                    string[] lines = File.ReadAllLines(filePath);
                    lines = Array.FindAll(lines, line => !string.IsNullOrEmpty(line.Trim()));
                    File.WriteAllLines(filePath, lines);
                }
                else
                {
                    exit = true;
                }
            }
            else
            {
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1. To view list of banned apps");
                Console.WriteLine("2. To add a new app to ban");
                Console.WriteLine("3. To exit");
                Console.WriteLine("4. To test Killer");
                Console.WriteLine("5. Rid of the emptiness within");
                int thisResponse = GetResponse();
                if (thisResponse == 1)
                {
                    ReadFromFile(filePath);
                }
                else if (thisResponse == 2)
                {
                    WriteToFile(filePath);
                }
                else if (thisResponse == 4)
                {
                    Killer(filePath);
                }
                else if (thisResponse == 5)
                {
                    string[] lines = File.ReadAllLines(filePath);
                    lines = Array.FindAll(lines, line => !string.IsNullOrEmpty(line.Trim()));
                    File.WriteAllLines(filePath, lines);
                }
                else
                {
                    exit = true;
                }
            }
        }
    }
    public static void ReadFromFile(string filePath)
    {
        /* StreamReader*/
        StreamReader reader = new StreamReader(filePath);
        while (!reader.EndOfStream)
        {
            string s = reader.ReadLine();
            string[] a = s.Split(',');
            Console.WriteLine($"\n{a[0]} is playable for {a[1]} hours a day\n");
        }
        reader.Close();
    }

    public static int GetResponse()
    {
        bool validResponse = false;
        int response = 0;
        do
        {
            try
            {
                response = Convert.ToInt32(Console.ReadLine());
                validResponse = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Please input a valid integer...");
            }
        }
        while (!validResponse);
        return response;
    }

    public static void WriteToFile(string filePath)
    {
        /*  StreamWriter*/
        StreamWriter writer = new StreamWriter(filePath, true);
        string name = string.Empty;
        do
        {
            Console.WriteLine("Enter name (with a ,[how many hrs you want a day to be playable] or Enter blank to return to menu: ");
            name = Console.ReadLine();
            if (name != "")
            {
                writer.WriteLine(name);
            }
        }
        while (name != "");
        writer.Close();
    }


    public static void Killer(string filePath)
    {
        StreamReader reader = new StreamReader(filePath);

        string[] bannedApps = reader.ReadToEnd().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        Console.WriteLine(bannedApps[0]);
        while (true)
        {
            foreach (string bannedApp in bannedApps)
            {
                string[] app = bannedApp.Split(',');
                string appName = app[0];
                Console.WriteLine(appName);
                int playtime = int.Parse(app[1]);

                ManagementObjectCollection processes = GetProcesses(appName);

                if (processes.Cast<ManagementObject>().Any())
                {
                    Console.WriteLine("{0} is running. Waiting {1} seconds before terminating...", appName, playtime);
                    Thread.Sleep(playtime * 1000); /* playtime * one second*/
                    TerminateProcesses(processes);
                }
            }
            Thread.Sleep(5000); // Wait for 5 seconds before checking again
        }
    }

    private static ManagementObjectCollection GetProcesses(string processName)
    {
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Process WHERE Name='" + processName + "'");
        return searcher.Get();
    }

    private static void TerminateProcesses(ManagementObjectCollection processes)
    {
        foreach (ManagementObject process in processes)
        {
            try
            {
                process.InvokeMethod("Terminate", null);
                Console.WriteLine("Terminated process {0} ({1})", process["Name"], process["ProcessId"]);
            }
            catch (ManagementException e)
            {
                Console.WriteLine("An error occurred while terminating process {0}: {1}", process["Name"], e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Access denied while terminating process {0}: {1}", process["Name"], e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while terminating process {0}: {1}", process["Name"], e.Message);
            }
        }

    }
}