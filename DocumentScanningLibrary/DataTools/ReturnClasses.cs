using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentScanningLibrary.DataTools
{

    public class FileStatus : ReturnStatus
    {
        public string fileName { get; set; }
    }

    public class ScanPageStatus : ReturnStatus
    {
        public string pageText { get; set; }
        public string companyName { get; set; }
        public string scannedFileName { get; set; }
    }

    public class ReturnStatus
    {
        public ReturnStatus()
        {
            rc = 0;
            statusMessage = "";
        }

        public int rc { get; set; }
        public string statusMessage { get; set; }
    }
}
