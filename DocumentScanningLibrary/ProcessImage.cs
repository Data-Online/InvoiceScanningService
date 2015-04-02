using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accusoft.SmartZoneOCRSdk;
using System.Text.RegularExpressions;
using DocumentScanningLibrary.App_Code;   // **GPA --> move to "models"?
using DocumentScanningLibrary.DataTools;
using System.IO;

namespace DocumentScanningLibrary
{
    public class ProcessImage
    {
        private ScanningDataClassesDataContext db = new ScanningDataClassesDataContext();

        public ScanPageStatus ReadFile(string filename)
        {
            // Look for file in common location
            // move to in process
            // scan
            // move to processed (archive)
            
            // Need to have multiple pages available
            ScanPageStatus scanPageStatus = new ScanPageStatus();

            try
            {
                // Database connection via DocumentScanningLibrary, this is intermediate table for data loads
                ScannedRecord scannedRecord = new ScannedRecord();
               // { CompanyName = "test", CustomerNumber = 1, InvoiceNumber = 1 };
                ////var test = db.ScannedRecords.Count();
                ////db.ScannedRecords.InsertOnSubmit(scannedRecord);
                ////db.SubmitChanges();

                if (!String.IsNullOrEmpty(filename))
                { 
                    // ScanPDF useses the Tesseract public domain OCR tool set acting in PDF via Syncfusion toolset

                    switch (Path.GetExtension(filename).ToLower())
                    {
                        case ".pdf":
                            ScanPDF scanPDF = new ScanPDF();
                            scanPageStatus = scanPDF.Process(filename, ref scannedRecord);//ScanPDF.Process(filename);
                            break;
                        case ".jpg":
                        case ".tiff":
                            // Scan Page uses the Accusoft.SmartZoneOCRSdk to process .jpg source files.
                            // Needs to return "scanPageStatus, if used in the future
                            var result = ScanImage.Main(filename);
                            //if (result.ToString().Length > 0) {}
                            break;
                        default:
                            break;
                    }

                    if (scanPageStatus.rc == 0)
                    { 
                        // Persist record in the database, note in alert log if there are failed scans


                        if (scannedRecord.FailedMatches == 0)
                        {
                            scanPageStatus.rc = 0;
                            scanPageStatus.statusMessage = String.Format("File {0} scanned and data persisted to database", filename);
                        }
                        else if (scannedRecord.FailedMatches <= GetConstants.ScanErrorLimit())
                        {
                            scanPageStatus.rc = 1;
                            scanPageStatus.statusMessage = String.Format("Recorded {0} scan failures for source file {1}", scannedRecord.FailedMatches.ToString(), filename);
                        }
                        else
                        {
                            scanPageStatus.rc = -1;
                            scanPageStatus.statusMessage = String.Format("Scan failures of {0} too high. No data saved for source file {1}", scannedRecord.FailedMatches.ToString(), filename);
                        }
                        if (scanPageStatus.rc >= 0)
                        {
                            // Persist data to database
                            //var test = scannedRecord.ScannedFileName.
                            scannedRecord.ScannedFileName = Path.GetFileName(filename);
                            scannedRecord.ScanDate = DateTime.Now;  // Will need to ensure this is correct time zone *** GPA
                            db.ScannedRecords.InsertOnSubmit(scannedRecord);
                            db.SubmitChanges();
                        }
                    }
                    //ss.Dispose();
                }
                else
                {
                    scanPageStatus.rc = 1;
                    scanPageStatus.statusMessage = String.Format("Source file {0} null or empty", filename);
                }
            }
            catch (Exception ex)
            {
                scanPageStatus.rc = -1;
                scanPageStatus.statusMessage = String.Format("Exception {0} when running OCR on source file {1}", ex, filename);
            }

            return scanPageStatus;
        }

        private ScannedRecord ProcessContactInvoice(string scannedText)
        {
            ScannedRecord scannedRecord = new ScannedRecord();
            // Logic for Contact invoice type.

            // 1. "Invoice number: xxxxx"
 
            scannedRecord.InvoiceNumber = Convert.ToString(Regex.Match(scannedText, "Invoice Number: [0-9]*")).Split(':')[1];
            scannedRecord.AccountNumber = Convert.ToString(Regex.Match(scannedText, "Account Number: [0-9]*")).Split(':')[1];
            scannedRecord.ICPnumber = Convert.ToString(Regex.Match(scannedText, "ICP Number: ([0-9])([A-Z])(\\-)([A-Z])([0-9])")).Split(':')[1];
        //
            return scannedRecord;
        }

        private ScannedRecord ProcessMeridianInvoice(string scannedText)
        {
            ScannedRecord scannedRecord = new ScannedRecord();
            // Logic for Meridian invoice type.

            // 1. "Invoice number: xxxxx"
            scannedRecord.InvoiceNumber = Convert.ToString(Regex.Match(scannedText, "Invoice Number: [0-9]*")).Split(':')[1];
            scannedRecord.AccountNumber = Convert.ToString(Regex.Match(scannedText, "Account Number: [0-9]*")).Split(':')[1];
            scannedRecord.ICPnumber = Convert.ToString(Regex.Match(scannedText, "ICP Number: [0-9]*")).Split(':')[1];
            //
            return scannedRecord;
        }
    }
}
