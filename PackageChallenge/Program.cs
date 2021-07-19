using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PackageChallenge.Model;

namespace PackageChallenge
{
    class Program
    {
        public static string inputTextFile = "C:\\Users\\Love Sharma\\OneDrive\\Desktop\\PackageChallengeProblem.txt";
        public static string packageOutputFile = "C:\\Users\\Love Sharma\\OneDrive\\Desktop\\PackageChallengeOutput.txt";

        public static int comNumber = 0;
        public static List<string> outputList = new List<string>();
        public static List<PackageChallengeModel> outputChallenge = new List<PackageChallengeModel>();
        public static string outputResult = string.Empty;
        public static List<PackageModel> packageModelList = new List<PackageModel>();

        /// <summary>
        /// Main method has been created , intially called this method first.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                ReadFile();
            }
            catch (Exception ex)
            {
                WriteLog(string.Concat("Message : ", ex.Message, " InnerException :", ex.InnerException));
            }
        }
        /// <summary>
        /// This method is used for Calculating Package Items.
        /// </summary>
        public static void CalculatePackageChallange()
        {
            for (int i = 1; i <= packageModelList.Count; i++)
            {
                comNumber = i;
                var indexList = packageModelList.OrderBy(s => s.Index).Select(s => s.Index.ToString()).ToList();
                FilterPackageItem(1, indexList, string.Empty);
            }
        }
        /// <summary>
        /// This method is used for filtering package item after happening all the calculations for specific package.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="list"></param>
        /// <param name="output"></param>
        public static void FilterPackageItem(int order, List<string> list, string output)
        {
            string dataOutput = string.Empty;
            if (order == comNumber)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    outputList.Add(output + list[i]);

                    dataOutput = output + list[i];
                    var arOutput = dataOutput.Split(',').Select(s => Convert.ToDouble(s)).ToList().OrderBy(s => s).ToList();
                    string compositIndex = string.Empty;
                    foreach (var item in arOutput)
                    {
                        compositIndex = compositIndex + item.ToString();
                    }
                    outputChallenge.Add(new PackageChallengeModel
                    {
                        Index = dataOutput,
                        Amount = packageModelList.Where(s => arOutput.Any(a => a.ToString() == s.Index.ToString())).ToList().Sum(p => p.Amount),
                        Weight = packageModelList.Where(s => arOutput.Any(a => a.ToString() == s.Index.ToString())).ToList().Sum(p => p.Weight),
                        CompositIndex = compositIndex
                    });
                }
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var dataList = list.Where(s => s.ToString() != list[i].ToString()).ToList();
                    string resultPut = output + list[i] + ",";
                    //recursion method is used for creating sets in order to creating number of combinations
                    FilterPackageItem(order + 1, dataList, resultPut);
                }
            }
        }
        /// <summary>
        /// This method is used for reading text file and creating list containing all the items in it.
        /// </summary>
        /// <returns></returns>
        public static string ReadFile()
        {
            string response = string.Empty;

            if (File.Exists(inputTextFile))
            {
                string[] lines = File.ReadAllLines(inputTextFile);
                foreach (string line in lines)
                {
                    string data = line.Replace(" ", string.Empty);
                    List<PackageModel> packageList = new List<PackageModel>();

                    int pkAmountEnd = data.IndexOf(':');
                    double pkWeight = Convert.ToDouble(data.Substring(0, pkAmountEnd));
                    while (!string.IsNullOrEmpty(data))
                    {
                        int index1 = data.IndexOf('(');
                        int index2 = data.IndexOf(')');
                        int length = index2 - index1;
                        var item = data.Substring(index1 + 1, length - 1);
                        data = data.Substring(index2 + 1, ((data.Length) - (index2 + 1)));
                        var arItem = item.Split(',');
                        var amount = arItem[2].Replace("€", string.Empty);
                        packageList.Add(new PackageModel { Index = Convert.ToInt32(arItem[0]), Weight = Convert.ToDouble(arItem[1]), Amount = Convert.ToDouble(amount) });
                    }
                    outputChallenge = new List<PackageChallengeModel>();

                    packageList = packageList.Where(a => a.Weight <= pkWeight).ToList();
                    packageModelList = packageList;

                    CalculatePackageChallange();

                    if (outputChallenge != null && outputChallenge.Count > 0)
                    {
                        string outputResult = "";
                        outputChallenge = outputChallenge.GroupBy(x => x.CompositIndex).Select(s => s.FirstOrDefault()).ToList();

                        outputChallenge = outputChallenge.Where(s => s.Weight <= pkWeight).ToList();
                        var outputData = outputChallenge.Where(o => o.Amount == outputChallenge.Max(s => s.Amount)).ToList();
                        if (outputData != null && outputData.Count > 1)
                        {
                            outputResult = outputData.Where(s => s.Weight == outputData.Min(p => p.Weight)).FirstOrDefault().Index;
                        }
                        else
                        {
                            if (outputData != null && outputData.Count() > 0)
                                outputResult = outputData.FirstOrDefault().Index;
                            else
                                outputResult = "-";
                        }
                        response = response + outputResult + Environment.NewLine;
                    }
                    else
                    {
                        response = response + "-" + Environment.NewLine;
                    }
                }
                WriteLog(response);
            }
            return string.Empty;
        }

        /// <summary>
        /// This method is used for writing output and error file to the text file.
        /// </summary>
        /// <param name="output"></param>
        public static void WriteLog(string output)
        {
            using (StreamWriter writer = new StreamWriter(packageOutputFile))
            {
                writer.WriteLine(output);
            }
        }
    }
}
