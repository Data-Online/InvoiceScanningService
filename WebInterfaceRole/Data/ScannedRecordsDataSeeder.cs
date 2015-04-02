using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebInterfaceRole.Models;

namespace WebInterfaceRole.Data
{
    //class ScannedRecordsDataSeeder : DropCreateDatabaseIfModelChanges<ScannedDataContext>
    class ScannedRecordsDataSeeder : IDatabaseInitializer<ScannedDataContext>
    {
        public void InitializeDatabase(ScannedDataContext context)
        {
            // Only add data if no records in the database
             if (context.Customers.Count() == 0)
             { 
                Customer  customer = new Customer()
                {
                   Name = "Customer 1", Invoices = new List<Invoice>()
                   {
                       new Invoice()
                       {
                           InvoiceNumber = "1234",
                           Date = DateTime.Now,
                           Supplier = new Supplier()
                           {
                               Name = "Supplier 1"
                           }
                       }
                   }
                };

                context.Customers.Add(customer);
                context.SaveChanges();
             }
            //base.Seed(context);
        }
    }
}
