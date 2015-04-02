using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentScanningLibrary.DataTools
{
    class FieldValuesForCompany
    {
        private string _companyName;
        public FieldValuesForCompany(string companyName)
        {
            _companyName = companyName;
        }

        public int[] FieldValuesForGroup(string fieldGroup)
        {
            int[] returnValues = new int[] {};
            switch (_companyName)
            {
                case "contact":
                    switch (fieldGroup)
                    {
                        case "LC":
                           return returnValues = new int[] { 1, 2, 3 };
                        default:
                           return returnValues; 
                    }
            }

            return returnValues;
        }

        public int[] LC()
        {
            int[] returnValues = new int[] { 1, 2 };
            return returnValues;
        }
    }
}
