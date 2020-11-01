using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Colorful;
using Console = Colorful.Console;

namespace LinesOfCode
{
    class Program
    {
        static void Main(string[] args)
        {
            bool useConfig = false;
            List<string> files = new List<string>();
            Console.WriteAscii("LinesOfCode", Color.MediumSlateBlue);
            Log("[INPUT] Enter your path to be scanned: ");
            string path = Console.ReadLine().ToString();
            string configPath = path + @"\linesConfig.json";
            if (File.Exists(configPath))
            {
                Log("[QUESTION] There is a configuration file present for LinesOfCode in this directory. Do you want to use it? (y/n): ");
                string choice = Console.ReadLine();
                useConfig = choice == "y" ? true : false;
            }

            List<string> extList = new List<string>();
            List<string> ignoreList = new List<string>();

            if (!useConfig)
            {
                Log("[INPUT] Enter your extensions to be scanned for (ex: .cs, .php): ");
                extList.AddRange(((Console.ReadLine().ToString()).Replace(" ", "")).Split(','));
                Log("[INPUT] Enter your ignore list (ex: designer, Version): ");
                string ignoreEntries = Convert.ToString(Console.ReadLine());
                if (!string.IsNullOrEmpty(ignoreEntries))
                {
                    ignoreList.AddRange(((ignoreEntries.ToString()).Replace(" ", "")).Split(','));
                }
                Log("[QUESTION] Do you want your current configuration to be written to the scan path as a config file? (y/n): ");
                bool writeConfig = Console.ReadLine() == "y" ? true : false;
                if (writeConfig)
                {
                    Common.ReaderConfig config = new Common.ReaderConfig(extList, ignoreList);
                    //
                    string configJson = JsonConvert.SerializeObject(config);
                    using (StreamWriter sw = new StreamWriter(configPath))
                    {
                        sw.Write(configJson);
                    }
                }
            }

            else if (useConfig)
            {
                using (StreamReader sr = new StreamReader(configPath))
                {
                    Log("[INFO] Loading config file...", true);
                    string configJson = sr.ReadToEnd();
                    Common.ReaderConfig config = JsonConvert.DeserializeObject<Common.ReaderConfig>(configJson);
                    extList.AddRange(config.extList);
                    ignoreList.AddRange(config.ignoreList);
                    Log("[INFO] Config file loaded!\n[INFO] Starting Operation...", true);
                }
            }

            foreach (string filePath in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
            {
                string fileName = filePath.Split('/').Last();
                foreach(string ext in extList)
                {
                    if (fileName.Contains(ext)) {
                        bool ignorable = false;
                        if (ignoreList.Count != 0)
                        {
                            foreach (string ignore in ignoreList)
                            {
                                Log($"[INFO] Scanning {fileName} for ignore word [{ignore}] - ");
                                if (!fileName.Contains(ignore))
                                {
                                    Log("Check Failed", true);
                                }
                                else
                                {
                                    Log("Check Fired", true);
                                    ignorable = true;
                                    break;
                                }
                            }
                        }

                        if (!ignorable)
                        {
                            Log($"[INFO] Adding {fileName} to scanlist...", true);
                            files.Add(filePath);
                        }
                    }
                }
            }
            int fileNum = 0;
            foreach (string file in files)
            {
                fileNum++;
            }
            foreach (string file in files)
            {
                Log("[MATCH] " + file, true);
            }

            Int64 lines = 0;

            foreach (string file in files)
            {
                Log($"[INFO] Counting lines in {file}", true);
                using (StreamReader sr = new StreamReader(file))
                {
                    string currentLine = string.Empty;
                    Int64 currentLineCount = 0;
                    while ((currentLine = sr.ReadLine()) != null)
                    {
                        if (currentLine != string.Empty)
                        {
                            currentLineCount++;
                            lines++;
                        }
                    }
                }
            }

            Log($"[INFO] Found {fileNum} files which match your filters.", true);
            Log($"[INFO] The results contain a total of {lines} line(s).", true);
            Log($"[INFO] Thank you for using LinesOfCode - Made by Tibix.");
            Console.ReadLine();
        }

        public static void Log(string logMsg, bool line = false)
        {
            StyleSheet styleSheet = new StyleSheet(Color.White);
            styleSheet.AddStyle("INPUT[a-z]*", Color.MediumSlateBlue);
            styleSheet.AddStyle("WARN[a-z]*", Color.Yellow);
            styleSheet.AddStyle("ERROR[a-z]*", Color.DarkRed);
            styleSheet.AddStyle("INFO[a-z]*", Color.Aqua);
            styleSheet.AddStyle("MATCH[a-z]*", Color.Aquamarine);
            styleSheet.AddStyle("QUESTION[a-z]*", Color.Coral);
            styleSheet.AddStyle("Check Failed[a-z]*", Color.MediumSlateBlue);
            styleSheet.AddStyle("Check Fired[a-z]*", Color.Coral);
            if (line)
            {
                Console.WriteLineStyled(logMsg, styleSheet);
            }
            else
            {
                Console.WriteStyled(logMsg, styleSheet);
            }
        }
    }
}
