using Syncfusion.OCRProcessor;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DocumentScanningLibrary.DataTools;
using DocumentScanningLibrary.App_Code;

namespace DocumentScanningLibrary
{
    class ScanPDF
    {
        //public static int Main(string fileName)
        //{
        //    // Read fileName source file and extract all pages
        //    //string zz = Path.GetDirectoryName(GetType().Assembly.Location);
        //    string[] s = new string[4] { "", "", "", "" };
        //    CreateTxtFromPDF(fileName, ref s);
        //    return 0;
        //}

        //public ScanPDF() { }

        ScanPageStatus scanPageStatus = new ScanPageStatus();

        public ScanPageStatus Process(string fileName, ref ScannedRecord scannedRecord)
        {
           //ScanPageStatus scanPageStatus = new ScanPageStatus();
            bool _debug = System.Diagnostics.Debugger.IsAttached;
            _debug = false;

            // Scan the PDF, create readable PDF output file
            if (_debug)
            {
                scanPageStatus.rc = 0;
            }
            else
            {
                CreateTxtFromPDF(fileName);
            }
            if (scanPageStatus.rc == 0)
            {
                string companyData;
                // If file has scanned correctly, then read in processed PDF, identifying company name and key data (only return the key data)
                if (_debug)
                {
                    companyData = "ACCOUNT DETAILS Page 2 of 3 Account Number: 2707108410 Electricity Charges for 25D Bouverie Street Invoice Number: 659510 From 1 OCt0ber2011 to 31 October 2011 (31 Days) ’ 111 _ d @l ICP Number: 0O01452560UN-B21 Line Charges I Fixed Line Charge (GX99) 31 Day($l) $20.49 per Day $635.34 Variable Line Charge (GX99) 66,916.44 kWh 0.720c per kWh $481.80 Anytime Maximum Demand (GX99) $6.65 per kVA $2,211.45 Assessed Capacity (GX99) 31 Day(s) 1,000 kVA * 1.71c/day22 $530.10 Total Line Charges $3,858.69 Energy kWh kWh Losses cents per kWh Weekdays (0000 - 0400) 1,943.98 54.43 7.794 $155.76 Weekdays (0400 - 0800) 6,309.36 176.66 8.591 $557.21 Weekdays (0800 - 1200) 13,086.08 366.41 11.428 $1,537.35 Weekdays (1200 — 1600) 11,622.74 325.44 10.803 $1,290.76 Weekdays (1600 ~ 2000) 10,750.22 301.01 11.838 $1,308.24 Weekdays (2000 - 2400) 2,501.30 70.04 10.171 $261.53 Weekends (0000 ~ 0400) 997.00 27.92 6.494 $66.56 Weekends (0400 — 0800) 1,943.94 54.43 7.159 $143.06 Weekends (0800 — 1200) 6,588.34 184.47 9.523 $644.98 Weekends (1200 - 1600) 5,920.26 165.77 9.002 $547.86 Weekends (1600 - 2000) 4,233.14 118.53 9.864 $429.25 Weekends (2000 ~ 2400) 1,020.08 28.56 8.475 $88.87 Energy Totals 66,916.44 1,873.66 $7,031.43 Other Charges Administration Charge 31 Day(s) $1.32 per Day $40.92 Electricity Commission Levies 68,790.00 kWh 0.172c per kWh $118.32 $159.24 (:57 15% 1,657.40 Total Electricity Charges $12,706.76 in (D o o o o E ";
                    scanPageStatus.statusMessage = String.Format("Debug mode, no scan started");
                    scanPageStatus.rc = 0;
                    scanPageStatus.companyName = "contact";
                }
                else
                {
                    companyData = ReadInText();
                }

                // Once read, interprit the data and return easily digestable format for persiting to database in calling project
                if (scanPageStatus.rc == 0)
                {
                    // Can now interprit the data
                    ExtractCompanyData extractCompanyData = new ExtractCompanyData(scanPageStatus.companyName, companyData);
                    //ScannedRecord scannedRecord = new ScannedRecord();

                    // Scanned Page Status is not used from this point. Rely on status of returned record to decide if there are issues to report
                    scannedRecord = extractCompanyData.GetKeyData();
                }
            }
            return scanPageStatus; // temp -- need to take into account rc != 0 above
        }
        
        private void CreateTxtFromPDF(string filename)
        {
            //ScanPageStatus scanPageStatus = new ScanPageStatus();

            string tesseractPath = Path.Combine(AssemblyDirectory(), GetConstants.TesseractBinaries());
            string tesseractData = Path.Combine(AssemblyDirectory(), GetConstants.TesseractData());

            try {
                using (OCRProcessor processor = new OCRProcessor(tesseractPath))
                {
                    //Stream pdfStream2 = filename; // FileUpload1.PostedFile.InputStream;

                    // Read in PDF image file, and convert to searchable TXT pdf file
                    PdfLoadedDocument IDoc = new PdfLoadedDocument(filename);
                    processor.Settings.Language = Languages.English;
                    processor.Settings.Performance = Performance.Slow;
                    // var zz = processor.Settings.Performance;
                    //string tessdata = tesseractPath + @"\\Tessdata\\";
                    processor.PerformOCR(IDoc, tesseractData);
                    string outFileName = Path.GetFileName(filename) + "_OCR" + Path.GetExtension(filename);
                    string homePath = Path.GetDirectoryName(Path.GetDirectoryName(filename));
                    string savePath = Path.Combine(homePath, GetConstants.Directory("out"), outFileName);

                    // If file exists - delete it first.
                    if (File.Exists(savePath))
                    {
                        File.SetAttributes(savePath, FileAttributes.Normal);
                        File.Delete(savePath);
                    }

                    IDoc.Save(savePath);
                    IDoc.Close(true);
                    IDoc.Dispose();

                    scanPageStatus.scannedFileName = savePath;
                    scanPageStatus.rc = 0;
                    scanPageStatus.statusMessage = String.Format("File {0} scanned and saved to {1}", filename, scanPageStatus.scannedFileName);
                }
            }
            catch ( Exception ex )
            {
                scanPageStatus.scannedFileName = "";
                scanPageStatus.statusMessage = String.Format("Error {0} when running OCR on source file {1}", ex, filename);
                scanPageStatus.rc = -1;
            }

           // return scanPageStatus;
        }

//        private static ScanPageStatus ReadInText(ScanPageStatus scanPageStatus)
        private string ReadInText()
        {
            // Read in pages searchable PDF file created above
            //string ss = "";

            string[] s = new string[6] { "", "", "", "", "", "" };

            try { 
                PdfLoadedDocument ldoc = new PdfLoadedDocument(scanPageStatus.scannedFileName);
                PdfLoadedPageCollection loadedPages = ldoc.Pages;
                // Extract text from PDF document pages
                // char[] zz = {'A','\n'};

                // Load each page into string[]
                //string scanData = "";
                int _page = 0;
                foreach (PdfLoadedPage lpage in loadedPages)
                {
                    //scanData = lpage.ExtractText();
                    //if (scanData == "") { }
                    s[_page] += Regex.Replace(lpage.ExtractText(), @"\s+", " ");  //.Split(zz); 
                    //ss = Regex.Replace(s[_page], @"\s+", " ");
                    _page++;
                }
            }
            catch (Exception ex)
            {
                scanPageStatus.statusMessage = String.Format("Error {0} at 'ReadInText' when loading scanned document {1}", ex, scanPageStatus.scannedFileName);
                scanPageStatus.rc = -1;
                return "";
            }
            // Identify which company invoice is from and return the appropriate page of data for that company, based on defined key terms
            return IdentifyCompanyAndPage(ref s);
            //return scanPageStatus;
        }

        private string IdentifyCompanyAndPage(ref string[] s)
        {
            string[] companyNames = GetConstants.CompanyNames();
            string companyData = "";
            int status = -1;

            foreach (string company in companyNames)
            {
                KeyTerms keyTerms = new KeyTerms(company);
                status = FindInPage(keyTerms.ForCompanyName(), ref s);
                if (status >= 0)
                {
                    scanPageStatus.companyName = keyTerms.CurrentCompanyName();
                    int _dataPage = FindInPage(keyTerms.PageIdentification(), ref s);
                    //if (scanPageStatus.companyName == "contact") { _dataPage = 0; } // Test
                    if (_dataPage >= 0)
                    {
                        scanPageStatus.pageText = s[_dataPage]; // <<--- dont need to retain this string in the scanPageStatus object, just return it...
                        companyData = s[_dataPage];
                        if (companyData.Length > 0)
                        {
                            scanPageStatus.rc = 0;
                            scanPageStatus.statusMessage = String.Format("File {0} scanned and key page identified", scanPageStatus.scannedFileName);
                        }
                        else
                        {
                            scanPageStatus.rc = 1;
                            scanPageStatus.statusMessage = String.Format("File {0} scanned but key data page empty", scanPageStatus.scannedFileName);
                        }
                    }
                    else
                    {
                        // Unable to find the key page for this company..
                        scanPageStatus.rc = -1;
                        scanPageStatus.statusMessage = String.Format("Unable to locate key page for {0} bill (source file {1})", scanPageStatus.companyName, scanPageStatus.scannedFileName);
                        scanPageStatus.pageText = "";
                    }
                    break; // Found company, so no need to continue
                }
            }
            if (status < 0)
            { 
                // What if company not found in document .. not a valid file?
                scanPageStatus.rc = -1;
                scanPageStatus.statusMessage = String.Format("Unable to identify company for source file {1} (source file {2})", scanPageStatus.companyName, scanPageStatus.scannedFileName);
            }
            return companyData;
        }

        private static int FindInPage(string term, ref string[] s)
        {
            int _pageCount = 0;
            foreach (string pageText in s)
            {
                if (Regex.IsMatch(pageText, term, RegexOptions.IgnoreCase))
                {
                    // Match for current company. Now find key page and return it
                    return _pageCount;
                }
                _pageCount++;
            }
            return -1;
        }

        public static string AssemblyDirectory()
        {
            string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

    }
}
