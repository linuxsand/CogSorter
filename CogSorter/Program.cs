using System;
using System.Collections.Generic;
using System.IO;
using Cognex.VisionPro;
using Cognex.VisionPro.QuickBuild;

namespace CogSorter
{
    class Program
    {
        static CogJobManager manager = null;
        static void Main()
        {
            const string HELP = @"https://github.com/linuxsand/CogSorter/
USAGE:
> load xxx.vpp
> list
> sort 1,2,0
> save yyy.vpp
";
            bool hasLoaded = false;
            Console.WriteLine(HELP);
            while (true)
            {
                Console.Write("\r\n> ");
                string line = Console.ReadLine();
                if (string.IsNullOrEmpty(line)) return;

                var parts = line.Split(new char[] {' '});
                string action = parts[0].ToLower();
                if (action == "load")
                {
                    if (parts.Length != 2)
                    {
                        Console.WriteLine("please provide .vpp path");
                        break;
                    }
                    hasLoaded = loadManager(parts[1]);
                }
                else if (action == "list" && hasLoaded)
                {
                    listJobs();
                }
                else if (action == "sort")
                {
                    if (parts.Length != 2)
                    {
                        Console.WriteLine("please provide job sequence");
                        break;
                    }
                    sort(parts[1]);
                }
                else if (action == "save")
                {
                    if (parts.Length != 2)
                    {
                        Console.WriteLine("please provide new file path");
                        break;
                    }
                    saveToFile(parts[1]);
                }
                else if (action == "exit")
                {
                    break;
                }
            }
            unload();
            Console.WriteLine("exited");
        }

        static void unload()
        {
            if (manager != null)
            {
                manager.Shutdown();
            }
        }

        static void saveToFile(string vppPath)
        {
            try
            {
                CogSerializer.SaveObjectToFile(manager, vppPath);
                Console.WriteLine("saved");
            }
            catch (Exception)
            {
                Console.WriteLine("sorry, can not save");
            }
        }

        static bool loadManager(string vppPath)
        {
            if (manager != null)
            {
                unload();
            }

            if (!File.Exists(vppPath))
            {
                Console.WriteLine("file does not exist");
                return false;
            }
            Console.WriteLine("loading...");
            try
            {
                manager = (CogJobManager)CogSerializer.LoadObjectFromFile(vppPath);
            }
            catch (Exception)
            {
                Console.WriteLine("can not load");
                return false;
            }
            Console.WriteLine("loaded");
            listJobs();
            return true;

        }

        static void listJobs()
        {
            if (manager != null)
            {
                for (int i = 0; i < manager.JobCount; i++)
                {
                    Console.WriteLine(string.Format("{0}:\t\t{1}", manager.Job(i).Name, i)); 
                }
            }
        }

        static void sort(string seqStr)
        {
         /*
          +----------+----------+------+----------+------------+
          | old jobs |  old seq |  --> | new jobs |  input seq |
          +----------+----------+------+----------+------------+
          | job_a    |  0       |  --> | job_b    |  1         |
          +----------+----------+------+----------+------------+
          | job_b    |  1       |  --> | job_c    |  2         |
          +----------+----------+------+----------+------------+
          | job_c    |  2       |  --> | job_a    |  0         |
          +----------+----------+------+----------+------------+
         */
            if (manager == null) return;
            var seq = seqStr.Split(new char[] { ',' });

            HashSet<string> sets = new HashSet<string>(seq);
            if (sets.Count != manager.JobCount)
            {
                Console.WriteLine("user input numbers have duplicated value");
                return;
            }
            int jobCount = manager.JobCount;
            foreach (string index in seq)
            {
                int _index = 0;
                if (!int.TryParse(index, out _index) || _index < 0 || _index >= jobCount)
                {
                    Console.WriteLine("user input numbers are invalid");
                    return;
                }
            }

            List<CogJob> jobs = new List<CogJob>();
            List<string> jobNames = new List<string>();
            for (int i = 0; i < jobCount; i++)
            {
                var job = manager.Job(Convert.ToInt32(seq[i])); // 1, 2, 0 --> b, c, a
                jobs.Add(job);
                jobNames.Add(job.Name);
            }
            foreach (var name in jobNames)
            {
                manager.JobRemove(name);
            }
            foreach (var item in jobs)
            {
                manager.JobAdd(item);
            }
            Console.WriteLine("sorted:");
            listJobs();
        }
    }
}
