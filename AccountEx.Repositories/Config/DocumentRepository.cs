using AccountEx.CodeFirst.Models;
using AccountEx.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Repositories.Config
{
   public  class DocumentRepository:GenericRepository<UploadDocument>
    {
       public static void SaveDocuments(List<UploadDocument> documents)
       {
           using (var scope = TransactionScopeBuilder.Create())
           {

               var repo = new DocumentRepository();
               repo.Add(documents, false, false);
               repo.SaveChanges();
               repo.SaveChanges();
               scope.Complete();
           }

       }
    }
}
