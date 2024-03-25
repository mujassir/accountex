using AccountEx.CodeFirst.Models;
using System.Linq;
using System;

namespace AccountEx.Repositories
{
    public class TransporterRepository : GenericRepository<Transporter>
    {
        public Transporter GetByCode(string code)
        {
            return Collection.FirstOrDefault(p => p.Code.ToLower()==code.ToLower());
        }
        public Transporter GetByCode(string code, int id)
        {
            return Collection.FirstOrDefault(p => p.Code.ToLower() == code.ToLower() && p.Id != id);
        }
        public Transporter GetByName(string name, int id)
        {
            return Collection.FirstOrDefault(p => p.Name.ToLower() == name.ToLower() && p.Id != id);
        }
        
        public string GetNextAccountCode()
        {
            var lastCode=  Collection.Any() ? Collection.OrderByDescending(p => p.Id).FirstOrDefault().Code:"";
            if (string.IsNullOrWhiteSpace(lastCode))
                return "1001";
            if (IsNumber(lastCode))
                return (Convert.ToInt32(lastCode) + 1) + "";
            return IncreaseCode(lastCode);
        }

         private static string IncreaseCode(string input)
         {
             var resultString = System.Text.RegularExpressions.Regex.Match(input, @"\d+").Value;
             if (string.IsNullOrWhiteSpace(resultString)) return input + "2";
             var newNumber = (Convert.ToInt32(resultString) + 1) + "";
             var newCode = input.Replace(resultString, newNumber);
             if (newCode == input) newCode = newCode + "2";
             return newCode;
         }

         private static bool IsNumber(string input)
         {
             return input.All(char.IsDigit);
         }
    }
}
