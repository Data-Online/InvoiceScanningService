using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentScanningLibrary.DataTools
{
    class KeyTerms
    {
        private string CompanyName = "";

        public KeyTerms(string companyName)
        {
            CompanyName = companyName;
        }

        // Return key term used to identify invoice is from this company
        public string ForCompanyName()
        {
            switch (CompanyName)
            {
                case "meridian":
                    return "meridian";
                case "contact":
                    return "contact";
                default:
                    return "";
            }
        }

        // Return key text that will identify page required
        public string PageIdentification()
        {
            switch (CompanyName)
            {
                case "meridian":
                    return "NOT DEFINED";
                case "contact":
                    return "page 2 of 3";
                default:
                    return "";
            }
        }

        public string CurrentCompanyName()
        {
            return CompanyName;
        }
    }
}
