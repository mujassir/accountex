using AccountEx.CodeFirst.Models.COA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories
{
    public class ShopRepository : GenericRepository<Shop>
    {
        public ShopRepository() : base() { }
        public ShopRepository(BaseRepository repo) 
        {
            Db = repo.GetContext();
        }
        public static string GetNextAccountCode()
        {
            var lastCode = new ShopRepository().GetLastAccountCode();
            if (string.IsNullOrWhiteSpace(lastCode))
                return "1001";
            if (IsNumber(lastCode))
                return (Convert.ToInt32(lastCode) + 1) + "";
            return IncreaseCode(lastCode);
        }

        public string GetLastAccountCode()
        {
            return Collection.Any() ? Collection.OrderByDescending(p => p.Id).FirstOrDefault().ShopCode : "";
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

        public bool IsShopCodeExist(int id, string ShopCode)
        {
            return Collection.Any(p => p.ShopCode == ShopCode && p.Id != id);    
        }

        public bool IsShopExistInBlock(int id, string ShopCode, int BlockId)
        {
            return Collection.Any(p => p.ShopCode == ShopCode && p.BlockId == BlockId && p.Id != id);
        }

    }
}
