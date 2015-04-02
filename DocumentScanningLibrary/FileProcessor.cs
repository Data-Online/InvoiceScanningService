using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using DocumentScanningLibrary.DataTools;

// Reads source directory for scanned documents and returns the next valid file. Files with the wrong extension are moved to
// the ./errors directory and event is logged.
//
// Assumes three directories:
//      in : New files imported for processing
//      processed : Files that have scanned in correctly
//      errors : Files that have an error of some form

// -10 : Invalid source directory
// -1 : Invalid file format
// 0 : File okay, ready to scan
// 1 : No files to process

// Will pass back name of file, being either .jpg or .pdf

namespace DocumentScanningLibrary
{
    public class FileProcessor
    {

        public FileProcessor()
        {
        }

        //private string inDir = GetConstants.Directory("indir");
        //private string processedDir = GetConstants.Directory("processed");
        //private string errorDir = GetConstants.Directory("error");
        //private string home = GetConstants.Home();

        //public const string inDir = "in";
        //public const string processedDir = "processed";
        //public const string errorDir = "error";

        public FileStatus CheckNextFile()
        {
            FileStatus fileStatus = new FileStatus();

            //FileProcessor tools = new FileProcessor();
            //string targetPath = Path.Combine(home, inDir);
            string targetPath = Path.Combine(GetConstants.Home(), GetConstants.Directory("indir"));

            if (Directory.Exists(targetPath))
            {
                fileStatus.fileName = GetNextFile(targetPath);
                if (fileStatus.fileName.Length == 0)
                {
                    fileStatus.rc = 1;
                    fileStatus.statusMessage = String.Format("No more files in source directory {0}", targetPath);
                }
                else
                {
                    fileStatus.rc = CheckFile(fileStatus.fileName);
                    if (fileStatus.rc < 0)
                    {
                        if (MoveFile(Path.GetFileName(fileStatus.fileName), GetConstants.Directory("indir"), GetConstants.Directory("error")))
                            fileStatus.statusMessage = String.Format("Soure file {0} invalid. Moved to error directory", Path.GetFileName(fileStatus.fileName));
                        else
                            fileStatus.statusMessage = String.Format("Soure file {0} invalid; error moving to error directory", Path.GetFileName(fileStatus.fileName));
                    }
                }
            }
            else
            {
                fileStatus.statusMessage = String.Format("Invalid source directory {0}", targetPath);
                fileStatus.rc = -10;
            }
            return fileStatus;
        }

        // Process all files in the directory passed in, recurse on any directories  
        // that are found, and process the files they contain. 
        private string GetNextFile(string targetDirectory)
        {
            FileStatus fileStatus = new FileStatus();
            // Process the list of files found in the directory. 
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            //foreach (string fileName in fileEntries)
            if (fileEntries.Length > 0)
                return fileEntries[0];
            return "";
        }

        // Insert logic for processing found files here. 
        private int CheckFile(string fileName)
        {
            var _fileExtension = Path.GetExtension(fileName).ToLower();

            switch(_fileExtension)
            {
                case ".jpg":
                case ".pdf":
                    return 0;
                default:
                    return -1;
            }
        }


        // GPA ** These need to be improved!
        public bool MoveFile(string fileName, string sourceDir, string targetDir)
        {
            // Need to check that the source file exists, and that target directory is valid.
            string sourceFile = Path.Combine(GetConstants.Home(), sourceDir, fileName);
            string targetFile = Path.Combine(GetConstants.Home(), targetDir, fileName);
            try
            {
                File.Move(sourceFile, targetFile);
            }
            catch
            { return false;  }
            return true;
//            GetProcessedFromPath(fileName);
        }

        public bool MoveFile(string fileName, string target)
        {
            string sourceFile = Path.Combine(GetConstants.Home(), GetConstants.Directory("indir"), fileName);
            string targetFile = Path.Combine(GetConstants.Home(), GetConstants.Directory(target), fileName);
            try
            {
                File.Move(sourceFile, targetFile);
            }
            catch
            { return false; }
            return true;
        }

    }
}
