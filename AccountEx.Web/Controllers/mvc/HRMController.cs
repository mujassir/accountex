using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccountEx.BussinessLogic;
using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using AccountEx.Repositories;
using Newtonsoft.Json;

namespace AccountEx.Web.Controllers.mvc
{
    [AccountEx.Web.Code.Compress]
    public class HRMController : BaseController
    {
        //
        // GET: /HRM/

        public ActionResult SalaryConfig()
        {
            ViewBag.Department = new GenericRepository<Department>().GetNames();
            ViewBag.Designation = new GenericRepository<Designation>().GetNames();

            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Employees", Value = SettingManager.EmployeeHeadId });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();

        }
        public ActionResult ApproveSalary()
        {
            var setting = new List<SettingExtra>();
            ViewBag.Department = new GenericRepository<Department>().GetNames();
            ViewBag.Designation = new GenericRepository<Designation>().GetNames();
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<Employee>((byte)AccountDetailFormType.Employees) });
            var x = new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<Employee>((byte)AccountDetailFormType.Employees) };
            //setting.Add(new SettingExtra() { Key = "Employees", Value = SettingManager.EmployeeHeadId });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();

        }
        public ActionResult SalaryBankReconciliation()
        {
            var setting = new List<SettingExtra>();
            ViewBag.Department = new GenericRepository<Department>().GetNames();
            ViewBag.Designation = new GenericRepository<Designation>().GetNames();
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<Employee>((byte)AccountDetailFormType.Employees) });
            var x = new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<Employee>((byte)AccountDetailFormType.Employees) };
            //setting.Add(new SettingExtra() { Key = "Employees", Value = SettingManager.EmployeeHeadId });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();

        }
        public ActionResult GenerateSalary()
        {
            ViewBag.Department = new GenericRepository<Department>().GetNames();
            ViewBag.Designation = new GenericRepository<Designation>().GetNames();

            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Employees", Value = SettingManager.EmployeeHeadId });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll().Where(p => p.Code != null) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult EmployeeOTHours()
        {
            ViewBag.OverTimeTypes = new GenericRepository<OverTimeType>().GetNames();

            var setting = new List<SettingExtra>();
            setting.Add(new SettingExtra() { Key = "Employees", Value = SettingManager.EmployeeHeadId });
         //   setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll().Where(p => p.Code != null) });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<Employee>((byte)AccountDetailFormType.Employees) });
            var y =new SettingExtra() { Key = "Employees", Value = SettingManager.EmployeeHeadId };
            var x = new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<Employee>((byte)AccountDetailFormType.Employees) };
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult Employeeleaves()
        {
            ViewBag.LeaveTypes = new GenericRepository<LeaveType>().GetNames();
            var setting = new List<SettingExtra>();
           
            setting.Add(new SettingExtra() { Key = "Employees", Value = SettingManager.EmployeeHeadId });
            //setting.Add(new SettingExtra() { Key = "LeaveTypes", Value = new GenericRepository<AccountEx.CodeFirst.Models.LeaveType>().GetAll() });
            setting.Add(new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<Employee>((byte)AccountDetailFormType.Employees) });
            var y = new SettingExtra() { Key = "Employees", Value = SettingManager.EmployeeHeadId };
            var x = new SettingExtra() { Key = "AccountDetails", Value = new AccountDetailRepository().GetAll<Employee>((byte)AccountDetailFormType.Employees) };
            //setting.Add(new SettingExtra() { Key = "EmployeeIncomeConfigs", Value = new EmployeeIncomeConfigRepository().GetAll().Where(p => p.AccountId != 0) });
            //setting.Add(new SettingExtra() { Key = "EmployeeLeaves", Value = new EmployeeLeaveRepository().GetAll().Where(p => p.AccountId != 0) });
            ViewBag.FormSetting = JsonConvert.SerializeObject(setting);
            return View();
        }
        public ActionResult Fiscals() 
        {
            return View();
        }
    }
}
