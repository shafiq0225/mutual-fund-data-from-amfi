using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
                mf.Date = entries[5] == "Date" ? entries[5] : DateTime.Parse(entries[5]).Date.ToString("d");
                mfList.Add(mf);
            }

            var mutualFundTitle = mfList.Where(x => x.SchemeCode == null && x.MutualFundName.Contains("Mutual Fund")).ToList();
            var excludeDuplicateTitles = mutualFundTitle.
                Select(i => new { i.MutualFundName }).
                Distinct().Select(x => new MutualFund { MutualFundName = x.MutualFundName }).
                OrderBy(x => x.MutualFundName).ToList();

            var fundList = new List<Fund>();     
            var ex = excludeDuplicateTitles.Select(x => x.MutualFundName).ToList();
            var id = 1;
            foreach (var item in ex)
            {
                var f = new Fund();
                f.MutualFundName = item;
                f.IsMutualFundVisible = true;
                f.MutualFundCode = item.Split(' ').FirstOrDefault() + "_" + id++;
                fundList.Add(f);
            }
            string mu = JsonConvert.SerializeObject(fundList, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(mu);
            //write string to file
            System.IO.File.WriteAllText(@"C:\Users\shafi\Downloads\mf.json", mu);
            var mutualFunds = new List<MutualFund>();
            var index = 1;
            foreach (var mfTitle in excludeDuplicateTitles)
            {            
                var tilteSplit = mfTitle.MutualFundName.Split(' ').FirstOrDefault();
                tilteSplit = tilteSplit + " ";
                var funds = mfList.Where(x => x.SchemeName != null && x.SchemeName.Contains(tilteSplit)).ToList();
                foreach (var fund in funds)
                {
                    if (fund.Fund == null)
                    {
                        fund.Fund = new Fund();
                    }
                    fund.Fund.MutualFundName = mfTitle.MutualFundName;
                    fund.Fund.MutualFundCode = tilteSplit + "_" + index;
                    fund.Fund.IsMutualFundVisible = true;
                    fund.IsSchemeVisible = true;
                    mutualFunds.Add(fund);
                }
                index++;
            }
            string json = JsonConvert.SerializeObject(mutualFunds, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            //write string to file
            System.IO.File.WriteAllText(@"C:\Users\shafi\Downloads\nav.json", json);
            Console.ReadLine();
        }
    }
}

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class MutualFund
{
    [JsonIgnore]
    public string MutualFundName { get; set; }
    public Fund Fund { get; set; }
    public string SchemeCode { get; set; }
    public string SchemeName { get; set; }
    public string Rate { get; set; }
    public string Date { get; set; }
    public bool IsSchemeVisible { get; set; }
}

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class Fund
{
    public string MutualFundCode { get; set; }
    public string MutualFundName { get; set; }
    public bool IsMutualFundVisible { get; set; }
}
