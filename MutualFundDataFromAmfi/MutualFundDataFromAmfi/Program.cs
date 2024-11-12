using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MutualFundDataFromAmfi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePathFromAmfi = @"C:\Users\shafi\Downloads\AMFI.txt";
            var mfList = new List<MutualFund>();

            var amfiDatas = File.ReadAllLines(filePathFromAmfi).ToList();
            foreach (var amfiData in amfiDatas)
            {
                var entries = amfiData.Split(';');
                MutualFund mf = new MutualFund();
                if (entries.Length == 1)
                {
                    mf.MutualFundName = entries[0];
                    mfList.Add(mf);
                    continue;
                }
                mf.SchemeCode = entries[0];
                mf.SchemeName = entries[3];
                mf.Rate = entries[4];
                mf.Date = entries[5];
                mfList.Add(mf);
            }

            var mutualFundTitle = mfList.Where(x => x.SchemeCode == null && x.MutualFundName.Contains("Mutual Fund")).ToList();
            var excludeDuplicateTitles = mutualFundTitle.Select(i => new { i.MutualFundName })
            .Distinct().Select(x => new MutualFund { MutualFundName = x.MutualFundName }).OrderBy(x => x.MutualFundName).ToList();

            var mutualFunds = new List<MutualFund>();
            foreach (var mfTitle in excludeDuplicateTitles)
            {
                var tilteSplit = mfTitle.MutualFundName.Split(' ').FirstOrDefault();
                tilteSplit = tilteSplit + " ";
                var funds = mfList.Where(x => x.SchemeName != null && x.SchemeName.Contains(tilteSplit)).ToList();
                foreach (var fund in funds)
                {
                    fund.MutualFundName = mfTitle.MutualFundName;
                    fund.IsVisible = true;
                    mutualFunds.Add(fund);
                }
            }
            string json = JsonConvert.SerializeObject(mutualFunds, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            //write string to file
            System.IO.File.WriteAllText(@"C:\Users\shafi\Downloads\path.json", json);
            Console.ReadLine();
        }
    }
}
public class MutualFund
{
    public string MutualFundName { get; set; }
    public string SchemeCode { get; set; }
    public string SchemeName { get; set; }
    public string Rate { get; set; }
    public string Date { get; set; }
    public bool IsVisible { get; set; }
}
