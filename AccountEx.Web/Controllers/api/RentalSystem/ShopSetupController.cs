using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Web.Http;
using System.Net.Http;
using System.Text;
using AccountEx.Common;
using System.Web;
using System.Web.Mvc;
using Entities.CodeFirst;
using AccountEx.Repositories;
using AccountEx.Web.Controllers.api.Shared;
using AccountEx.CodeFirst.Models.COA;
using AccountEx.BussinessLogic;


namespace AccountEx.Web.Controllers.api.Manage
{
    public class ShopSetupController : GenericApiController<Shop>
    {

        public ApiResponse Get(string key)
        {

            ApiResponse response;
            try
            {
                response = new ApiResponse
                {
                    Success = true,
                    Data = ShopRepository.GetNextAccountCode(),
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public override ApiResponse Post([FromBody]Shop input)
        {
            ApiResponse response;
            try
            {
                var err = ServerVaildateSave(input);
                if (err == "")
                {
                    new ShopRepository().Save(input);
                    response = new ApiResponse() { Success = true };
                }
                else
                {

                    response = new ApiResponse() { Success = false, Error = err };
                }
            }
            catch (Exception ex)
            {
                ;
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) };
            }
            return response;
        }


        protected override JQueryResponse GetDataTable()
        {
            var queryString = Request.RequestUri.ParseQueryString();
            var coloumns = new[] { "ShopCode", "ShopNo", "Block", "TotalArea", "" };
            var echo = Convert.ToInt32(queryString["sEcho"]);
            var displayLength = Convert.ToInt32(queryString["iDisplayLength"]);
            var colIndex = Convert.ToInt32(queryString["iSortCol_0"]);
            var displayStart = Convert.ToInt32(queryString["iDisplayStart"]);
            var search = (queryString["sSearch"] + "").Trim();
            var decimalSearch = Numerics.GetDecimal((queryString["sSearch"] + "").Trim());
            var type = (queryString["type"] + "").Trim();
            //var dal = new ShopRepository();
            // var records = dal.AsQueryable();
            var records = Repository.AsQueryable();
            var totalRecords = records.Count();

            var filteredList = records;
            if (search != "")
                filteredList = records.Where(p =>
                     p.ShopCode.Contains(search) ||
                     p.ShopNo.Contains(search) ||
                     p.Block.Contains(search) ||
                     (decimalSearch > 0 && p.TotalArea == decimalSearch)
                   );

            var totalDisplayRecords = filteredList.Count();
            var orderedList = filteredList.OrderByDescending(p => p.Id);
            if (colIndex < coloumns.Length && coloumns[colIndex] + "" != "")
            {
                var sortDir = queryString["sSortDir_0"];
                orderedList = sortDir == "asc" ? filteredList.OrderBy(coloumns[colIndex]) :
                    filteredList.OrderByDescending(coloumns[colIndex]);
            }
            var sb = new StringBuilder();
            sb.Clear();

            var sr = 0;
            var rs = new JQueryResponse();
            foreach (var item in orderedList.Skip(displayStart).Take(displayLength))
            {
                var data = new List<string>();
                //data.Add("<td><input type='checkbox' class='checkboxes' value='1' /></td>");
                data.Add(item.ShopCode);
                data.Add(item.ShopNo);

                var block = new BlockRepository().GetById(item.BlockId);
                data.Add(block != null ? block.Name : "");
                data.Add(Convert.ToString(item.TotalArea));
                var editIcon = "<i class='fa fa-edit' onclick=\"ShopSetup.Edit(" + item.Id + ")\" title='Edit' ></i>";
                var deleteIcon = "<i class='fa fa-trash-o' onclick=\"ShopSetup.Delete(" + item.Id + ")\" title='Delete' ></i>";
                var icons = "<span class='action'>";
                icons += editIcon;
                icons += deleteIcon;
                icons += "</span>";
                if (type != "report") data.Add(icons);
                rs.aaData.Add(data);
            }
            rs.sEcho = echo;
            rs.iTotalRecords = totalRecords;
            rs.iTotalDisplayRecords = totalDisplayRecords;
            return rs;
        }

        private string ServerVaildateSave(Shop input)
        {
            var err = "";
            if (new ShopRepository().IsShopCodeExist(input.Id, input.ShopCode))
            {
                err += "Shop Code already exists.,";
            }
            if (new ShopRepository().IsShopExistInBlock(input.Id, input.ShopCode, input.BlockId))
            {
                var block = new BlockRepository().GetById(input.BlockId);
                err += "Shop " + input.ShopCode + " already exists in " + block.Name + " Block.,";
            }

            return err;
        }

    }
}
