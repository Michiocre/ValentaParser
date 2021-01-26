using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Valenta
{
    class Program
    {
        static void Main(string[] args)
        {
            string newPath = "Jobs.csv";

            if (File.Exists(newPath))
            {
                Console.WriteLine("Die Datei \"Jobs.csv\" existiert bereits. Programm Aufruf wird abgebrochen.");
                Console.ReadLine();
            }
            else
            {
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml");

                List<string> output = new List<string>();

                output.Add("JobStateTime;OrderName;PartName;Count;ProductionTimeEffective");

                for (int i = 0; i < files.Length; i++)
                {
                    XElement jobExportData = XElement.Load(files[i]);

                    IEnumerable<XElement> sheets = jobExportData.Descendants("Sheet");

                    foreach (XElement sheet in sheets)
                    {
                        IEnumerable<XElement> orders = sheet.Descendants("Order");

                        string jobStateTime = (string)sheet.Element("JobStateTime");

                        foreach (XElement order in orders)
                        {
                            IEnumerable<XElement> parts = order.Descendants("Part");

                            string orderName = (string)order.Element("Name");

                            foreach (XElement part in parts)
                            {
                                string partName = (string)part.Element("Name");
                                string count = (string)part.Element("Count");
                                string productionTimeEffective = (string)part.Element("ProductionTimeEffective");

                                if (productionTimeEffective == null)
                                {
                                    productionTimeEffective = (string)part.Element("ProductionTime");
                                }

                                output.Add(jobStateTime + ";" + orderName + ";" + partName + ";" + count + ";" + productionTimeEffective);
                            }
                        }
                    }
                }

                Console.WriteLine("Anzahl an verwendeten xml Dateien: " + files.Length);
                Console.WriteLine("Anzahl an erfassten Datenpunkten: " + (output.Count - 1));

                if (output.Count > 1)
                {
                    using (StreamWriter sw = File.CreateText(newPath))
                    {
                        foreach (string line in output)
                        {
                            sw.WriteLine(line);
                        }
                    }
                    Console.WriteLine("Zusammengefasst in Jobs.csv");
                } else
                {
                    Console.WriteLine("Da keine Datenpunkte erfasst wurden wurde keine Ausgabedatei erstellt.");
                }
                
                Console.WriteLine();

                if (files.Length > 0)
                {
                    Console.WriteLine("Sollen die Ursprungsdateien gelöscht werden? (j/n):");
                    string delete = Console.ReadLine();

                    if (delete.ToLower() == "j" || delete.ToLower() == "ja")
                    {
                        foreach (string file in files)
                        {
                            File.Delete(file);
                        }
                    }
                } else
                {
                    Console.ReadLine();
                }              
            }
        }
    }
}