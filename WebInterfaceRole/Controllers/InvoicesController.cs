using DocumentScanningLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebInterfaceRole.Models;
using MvcLoggingDemo.Services.Logging.Log4Net;  // **GPA need to refactor this.
using System.IO;
using DocumentScanningLibrary.DataTools;
using DocumentScanningLibrary.App_Code;

namespace WebInterfaceRole.Controllers
{
    public class InvoicesController : Controller
    {
        private ScannedDataContext db = new ScannedDataContext();
        private ScanningDataClassesDataContext scannedTargetdb = new ScanningDataClassesDataContext(); // Linq used to connect to database... move to Entity Framework in current model, so no dependancy with "DocumentScanningLibrary"
        private Log4NetLogger logger = new Log4NetLogger();


        //readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Invoices
        public ActionResult Index()
        {
            // Log to database

            FileStatus fileStatus = new FileStatus();
            ScanPageStatus scanPageStatus = new ScanPageStatus();
            FileProcessor fileProcessor = new FileProcessor();
            //fileStatus.rc = 0;

            while (fileStatus.rc <= 0)
            { 
                fileStatus = fileProcessor.CheckNextFile();
                // If the file is incorrect it will be moved to error directory automatically GPA *** move all functions to the same place!

                if (CheckFileStatus(fileStatus))
                {
                    // Scan the file reading in any data available
                    if (ReadFileData(fileStatus.fileName))
                    {
                        // If this is positive, then should have
                        //  - identified the company
                        //  - scanned in and returned the page with 'key' data (as a string)
                        // Now need to interprit this data and write to the database for further checking / uploading
                        fileProcessor.MoveFile(Path.GetFileName(fileStatus.fileName), "processed");
                        CheckAndReadScannedData();
                    }
                    else
                    {
                        fileProcessor.MoveFile(Path.GetFileName(fileStatus.fileName), "error");
                    }
                }
                else
                {
                    // Error - log message
                }
            }
            //FileProcessor.MoveFile()
            var invoices = db.Invoices.Include(i => i.Customer).Include(i => i.Supplier);
            return View(invoices.ToList());
        }

        #region "Read data and update database with scanned text"

        private void CheckAndReadScannedData()
        {
            //ScannedRecord scannedRecord = new ScannedRecord() { CompanyName = "test", CustomerNumber = 1, InvoiceNumber = "1" };
            //////var test = db.ScannedRecords.Count();
            //////db.ScannedRecords.InsertOnSubmit(scannedRecord);
            //////db.SubmitChanges();

            //scannedTargetdb.ScannedRecords.InsertOnSubmit(scannedRecord);
            //scannedTargetdb.SubmitChanges();

        }

        #endregion
        #region "File read and status check utilities"
        private bool ReadFileData(string filename)
        {
            //
            // If PDF and scanning method is image, then PDF needs to be split into separate images for each page
            // These individual pages then need to be scanned in using the Acusoft toolset (Syncfusion does not support images, only PDF image source files)

            //// Splid PDF into pages
            ////if (Path.GetExtension(fileStatus.fileName).ToLower() == ".pdf")
            ////{
            ////    // Process PDF file ready for scanning - split pages and export multiple .jpg.
            ////    // This is currently dummy code, not yet implimented. See samples from Syncfusion on how to complete, if required
            ////    var zz = ProcessPDF.Main(fileStatus.fileName);
            ////}
            ScanPageStatus scanPageStatus = new ScanPageStatus();
            ProcessImage processImage = new ProcessImage();

            scanPageStatus = processImage.ReadFile(filename);

            if (scanPageStatus.rc < 0)
            {
                logger.Error(scanPageStatus.statusMessage);
                return false;
            }
            if (scanPageStatus.rc == 0)
                logger.Info(scanPageStatus.statusMessage);
            else
                logger.Warn(scanPageStatus.statusMessage);

            return true;
        }

        private bool CheckFileStatus(FileStatus fileStatus)
        {
            if (fileStatus.rc < 0)
            {
                logger.Error(fileStatus.statusMessage);
            }
            else if (fileStatus.rc > 0)
            {
                logger.Info(fileStatus.statusMessage);
            }
            else  // File okay for scanning
            {
                return true;
            }
            return false;
        }
        #endregion

        // GET: Invoices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // GET: Invoices/Create
        public ActionResult Create()
        {
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name");
            ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name");
            return View();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,InvoiceNumber,Date,CustomerId,SupplierId")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                db.Invoices.Add(invoice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name", invoice.CustomerId);
            ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name", invoice.SupplierId);
            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name", invoice.CustomerId);
            ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name", invoice.SupplierId);
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,InvoiceNumber,Date,CustomerId,SupplierId")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invoice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "Id", "Name", invoice.CustomerId);
            ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name", invoice.SupplierId);
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invoice invoice = db.Invoices.Find(id);
            db.Invoices.Remove(invoice);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
