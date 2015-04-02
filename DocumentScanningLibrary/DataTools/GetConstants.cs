using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// GPA *** must be a better way to impliment this?
namespace DocumentScanningLibrary.DataTools
{
    partial class GetConstants
    {
        public static string Directory(string dir)
        {
            switch (dir.ToLower())
            {
                case "indir":
                    return "in";
                case "processed":
                    return "processed";
                case "error":
                    return "error";
                case "out":
                    return "out";       // PDF files that have been scanned by the OCR software
                default:
                    return "";
            }      
        }

        public static string TesseractBinaries()
        {
            return "TesseractBinaries";
        }

        public static string Home()
        {
            return @"P:\\Projects\\Cloud_Development\\GitHub\\InvoiceScanningService\\_sourceImages";
        }

        internal static string TesseractData()
        {
            return "TesseractBinaries\\Tessdata\\";
        }

        public static string[] CompanyNames()
        {
            return new string[] { "contact", "meridian" };
        }

        public static int ScanErrorLimit()
        {
            // Any more than this number of read data errors when extracting invoice data then reject the import
            return 5;
        }
    }
}
