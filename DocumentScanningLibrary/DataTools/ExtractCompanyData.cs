using DocumentScanningLibrary.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentScanningLibrary.DataTools
{
    class ExtractCompanyData
    {

        private string _companyName = "";
        private string _companyData = "";

        public ExtractCompanyData(string CompanyName, string CompanyData)
        {
            _companyName = CompanyName;
            _companyData = CompanyData;
        }

        public ScannedRecord GetKeyData()
        {
            // I'm sure there is a smarter way to do this... ****GPA
            ScannedRecord scannedRecord = new ScannedRecord();
            switch (_companyName)
            {
                case "contact":
                    ContactEnergy companyTools = new ContactEnergy(_companyData);
                    scannedRecord = companyTools.ProcessContactInvoice();                    
                    break;
                case "meridian":
                    break;
            }

            return scannedRecord;
        }
    }
}
