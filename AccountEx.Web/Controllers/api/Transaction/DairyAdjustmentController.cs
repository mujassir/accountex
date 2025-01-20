using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System;
using System.Web.Http;
using AccountEx.CodeFirst.Models.Transactions;
using Elmah.ContentSyndication;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AccountEx.Web.Controllers.api.Transaction
{
    public class DairyAdjustmentController : BaseApiController
    {
        public ApiResponse Get(int id)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var key = queryString["key"].ToLower();
                var repo = new GenericRepository<DairyAdjustment>();
                var maxVoucherNumber = repo.GetAll().OrderByDescending(p => p.VoucherNumber).FirstOrDefault()?.VoucherNumber ?? 0;
                var record = new List<DairyAdjustment>();
                switch (key)
                {
                    case "first":
                        record = repo.GetAll(p => p.VoucherNumber == 1).ToList();
                        break;
                    case "last":
                        record = repo.GetAll(p => p.VoucherNumber == maxVoucherNumber).ToList();
                        break;
                    case "next":
                        record = repo.GetAll(p => p.VoucherNumber == id + 1).OrderBy(p => p.VoucherNumber).ToList();
                        break;
                    case "previous":
                        record = repo.GetAll(p => p.VoucherNumber == id - 1).OrderByDescending(p => p.VoucherNumber).ToList();
                        break;
                    case "same":
                        record = repo.GetAll(p => p.VoucherNumber == id);
                        break;
                    case "nextvouchernumber":
                        record = repo.GetAll(p => p.VoucherNumber == maxVoucherNumber).OrderByDescending(p => p.VoucherNumber).ToList();
                        foreach (var item in record)
                        {
                            item.Id = 0;
                            item.VoucherNumber = 0;
                        }
                        break;

                }
                var currentVNum = record.FirstOrDefault()?.VoucherNumber ?? 0;
                var Next = currentVNum == 0 ? 0 : maxVoucherNumber > currentVNum ? currentVNum + 1 : 0;
                var Previous = currentVNum > 0 ? currentVNum - 1 : maxVoucherNumber;
                var VoucherNumber = currentVNum == 0 ? maxVoucherNumber + 1 : currentVNum;
                if(key == "nextvouchernumber")
                {
                    foreach (var item in record)
                    {
                        item.Id = 0;
                        item.Date = DateTime.Now;
                        item.VoucherNumber = VoucherNumber;
                    }
                }
                response = new ApiResponse
                {
                    Success = true,
                    Data = new
                    {
                        Order = record.Count > 0 ? record : null,
                        Next,
                        Previous,
                        VoucherNumber,

                    }
                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public ApiResponse Post([FromBody] DairyAdjustmentRequest input)
        {
            ApiResponse response;
            try
            {
                if (input.Items.Count == 0)
                    throw new Exception("Invalid Data");
                var err = "";
                if (err == "")
                {
                    AdjustmentManager.SaveDairyAdjustment(input.Items);
                    response = new ApiResponse { Success = true };
                }
                else
                {
                    response = new ApiResponse()
                    {
                        Success = false,
                        Error = err
                    };

                }


            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;

        }
        private ApiResponse GetDataTable()
        {
            ApiResponse response;
            try
            {
                var result = "";
                var queryString = Request.RequestUri.ParseQueryString();
                var vouchertype = (VoucherType)Convert.ToByte((queryString["type"]));
                var voucher = Numerics.GetInt((queryString["voucher"]));
                var data = TransactionManager.GetVocuherDetail(voucher, vouchertype, queryString["key"]);
                result = JsonConvert.SerializeObject(data, GetJsonSetting());
                response = new ApiResponse
                {
                    Success = true,
                    Data = result

                };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }
            return response;
        }

        public ApiResponse Delete(int id)
        {
            ApiResponse response;
            try
            {
                var queryString = Request.RequestUri.ParseQueryString();
                var voucherNo = Numerics.GetInt(queryString["voucherNo"]);
                AdjustmentManager.DeleteDairyAdjustment(voucherNo);
                response = new ApiResponse { Success = true };
            }
            catch (Exception ex)
            {
                response = new ApiResponse { Success = false, Error = ErrorManager.Log(ex) }; ;
            }

            return response;
        }

    }
    public class DairyAdjustmentRequest
    {
        public List<DairyAdjustment> Items { get; set; }
    }
}
