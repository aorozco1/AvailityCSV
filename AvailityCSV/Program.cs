using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;



namespace AvailityCSV
{
    class Program
    {
        static void Main(string[] args)
        {
           var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = true

            };
            using var streamReader = File.OpenText("AvailityCSV.csv");
            using var csvReader = new CsvReader(streamReader, csvConfig);

            var users = csvReader.GetRecords<User>();
             Dictionary<string, List<User>> dict = new Dictionary<string, List<User>>();
            //creating a dictionary of an insurance company mapped to each of the users
            foreach (var user in users)
            {     
                Console.WriteLine(user.UserID + " " + user.FirstLast + " " + user.Version + " " +user.InsuranceCompany);
                if (!dict.ContainsKey(user.InsuranceCompany)) {
                    List<User> nList = new List<User>();
                    nList.Add(user);
                    dict.Add(user.InsuranceCompany, nList);
                    Console.WriteLine(user.InsuranceCompany);
                } else {
                    List<User> oldList = dict[user.InsuranceCompany];
                    dict.Remove(user.InsuranceCompany);
                    oldList.Add(user);
                    dict.Add(user.InsuranceCompany,oldList);
                }
                    
            }
      
            //should abstract this out to a helper function
            foreach (var InsuranceCompany in dict) {
                Console.WriteLine(InsuranceCompany.Value.First().UserID);
                List<User> currCompany = new List<User>();
                currCompany.AddRange(InsuranceCompany.Value);
  
                var list = currCompany.OrderBy(i=>i.FirstLast).ToList();
                var distinctList = list.GroupBy(x => x.UserID)
                         .Select(g => g.First())
                         .ToList();
                using (var writer = new StreamWriter(InsuranceCompany.Key + ".csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(distinctList);
                }

            }
        }

            
        }
     }
