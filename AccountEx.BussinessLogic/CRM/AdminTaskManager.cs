using AccountEx.CodeFirst.Models.CRM;
using AccountEx.Common;
using AccountEx.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.BussinessLogic.CRM
{

    public static class AdminTaskManager
    {



        public static void TransferProjectPMC()
        {
            var err = ValidatePMCProjectTransfer();
            if (string.IsNullOrWhiteSpace(err))
            {
                using (var scope = TransactionScopeBuilder.Create())
                {
                    var repo = new AdminTaskRepository();
                    var query = string.Format("EXEC [DBO].[CRM_CopyPMCProjectsToNextFiscal] @COMPANYID = {0},@FISCALID={1},@UserId={2}", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, SiteContext.Current.User.Id);
                    repo.ExecuteQuery(query);
                    //query = string.Format("EXEC [DBO].[CRM_CopyProjectsToNextFiscal] @COMPANYID = {0},@FISCALID={1},@UserId={2}", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, SiteContext.Current.User.Id);
                    //repo.ExecuteQuery(query);
                    FiscalSettingManager.SaveFiscalSettings(FiscalSettingManager.ProjectPmcTransferredKey, "true");
                    FiscalSettingManager.RefreshSetting();
                    scope.Complete();
                }
            }
            else
            {
                throw new OwnException(err);
            }

        }
        public static void LockProject()
        {
            var err = "";
            if (string.IsNullOrWhiteSpace(err))
            {
                using (var scope = TransactionScopeBuilder.Create())
                {
                    var repo = new AdminTaskRepository();
                    FiscalSettingManager.SaveFiscalSettings(FiscalSettingManager.ProjectLockedKey, FiscalSettingManager.IsProjectLocked ? "false" : "true");
                    FiscalSettingManager.RefreshSetting();
                    scope.Complete();
                }
            }
            else
            {
                throw new OwnException(err);
            }

        }
        public static void LockPMC()
        {
            var err = "";
            if (string.IsNullOrWhiteSpace(err))
            {
                using (var scope = TransactionScopeBuilder.Create())
                {
                    var repo = new AdminTaskRepository();
                    FiscalSettingManager.SaveFiscalSettings(FiscalSettingManager.PmcLockedKey, FiscalSettingManager.IsPmcLocked ? "false" : "true");
                    FiscalSettingManager.RefreshSetting();
                    scope.Complete();
                }
            }
            else
            {
                throw new OwnException(err);
            }

        }

        public static void CreateMissingPMC()
        {
            var err = "";
            if (string.IsNullOrWhiteSpace(err))
            {
                using (var scope = TransactionScopeBuilder.Create())
                {
                    var repo = new AdminTaskRepository();
                    var query = string.Format("EXEC [DBO].[CRM_CreateMissingPMCs] @COMPANYID = {0},@FISCALID={1},@UserId={2}", SiteContext.Current.User.CompanyId, SiteContext.Current.Fiscal.Id, SiteContext.Current.User.Id);
                    repo.ExecuteQuery(query);
                    scope.Complete();
                }
            }
            else
            {
                throw new OwnException(err);
            }

        }

        private static string ValidatePMCProjectTransfer()
        {
            var err = ",";
            try
            {
                var fiscalRepo = new FiscalRepository();

                if (SiteContext.Current.Fiscal == null)
                {
                    err += "No fiscal year found.,";

                }
                if (SiteContext.Current.Fiscal.IsClosed)
                {
                    err += "Fiscal year is closed No action can be done.,";
                }

                var fiscal = fiscalRepo.GetNextFiscal(SiteContext.Current.Fiscal.ToDate, SiteContext.Current.User.CompanyId, false);
                if (fiscal == null)
                {
                    err += "Please create next year before transfer.,";
                }

            }
            catch (Exception ex)
            {

                err = ErrorManager.Log(ex);
            }
            err = err.Trim(',');
            return err;


        }




    }
}
