/*  Copyright (C) 2008-2017 Peter Palotas, Jeffrey Jangli, Alexandr Normuradov
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy 
 *  of this software and associated documentation files (the "Software"), to deal 
 *  in the Software without restriction, including without limitation the rights 
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 *  copies of the Software, and to permit persons to whom the Software is 
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 *  THE SOFTWARE. 
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AlphaFS.UnitTest
{
   /// <summary>This is a test class for FileSystemEntryInfo and is intended to contain all FileSystemEntryInfo Unit Tests.</summary>
   public partial class FileSystemEntryInfoTest
   {
      // Pattern: <class>_<function>_<scenario>_<expected result>


      [TestMethod]
      public void AlphaFS_File_GetFileSystemEntryInfo_LocalAndNetwork_Success()
      {
         File_GetFileSystemEntryInfo(false);
         File_GetFileSystemEntryInfo(true);
      }



      [TestMethod]
      public void AlphaFS_File_GetFileSystemEntryInfo_CatchFileNotFoundException_DirectoryExistsWithSameNameAsFile_LocalAndNetwork_Success()
      {
         File_GetFileSystemEntryInfo_CatchFileNotFoundException_DirectoryExistsWithSameNameAsFile(false);
         File_GetFileSystemEntryInfo_CatchFileNotFoundException_DirectoryExistsWithSameNameAsFile(true);
      }




      private void File_GetFileSystemEntryInfo(bool isNetwork)
      {
         if (!System.IO.File.Exists(UnitTestConstants.NotepadExe))
            Assert.Inconclusive("Test ignored because {0} was not found.", UnitTestConstants.NotepadExe);


         UnitTestConstants.PrintUnitTestHeader(isNetwork);

         var tempPath = UnitTestConstants.NotepadExe;
         if (isNetwork)
            tempPath = Alphaleonis.Win32.Filesystem.Path.LocalToUnc(tempPath);


         Console.WriteLine("\nInput File Path: [{0}]", tempPath);

         var fsei = Alphaleonis.Win32.Filesystem.File.GetFileSystemEntryInfo(tempPath);
         UnitTestConstants.Dump(fsei, -19);
         
         Assert.IsTrue(fsei.GetType().IsEquivalentTo(typeof(Alphaleonis.Win32.Filesystem.FileSystemEntryInfo)));
         Assert.IsTrue(fsei.Attributes != System.IO.FileAttributes.Directory, "The directory attribute is found, but is not expected.");
         Assert.AreEqual(tempPath, fsei.FullPath, "The paths are not equal, but are expected to be.");
         
         Console.WriteLine();
      }


      private void
         File_GetFileSystemEntryInfo_CatchFileNotFoundException_DirectoryExistsWithSameNameAsFile(bool isNetwork)
      {
         var path = UnitTestConstants.SysRoot;

         if (!System.IO.Directory.Exists(path))
            Assert.Inconclusive("Test ignored because {0} was not found.", path);


         UnitTestConstants.PrintUnitTestHeader(isNetwork);

         var tempPath = path;
         if (isNetwork)
            tempPath = Alphaleonis.Win32.Filesystem.Path.LocalToUnc(tempPath);


         Console.WriteLine("\nInput File Path: [{0}]", tempPath);


         var gotException = false;
         try
         {
            Alphaleonis.Win32.Filesystem.File.GetFileSystemEntryInfo(tempPath);
         }
         catch (Exception ex)
         {
            var exName = ex.GetType().Name;
            gotException = exName.Equals("FileNotFoundException", StringComparison.OrdinalIgnoreCase);
            Console.WriteLine("\n\tCaught {0} Exception: [{1}] {2}", gotException ? "EXPECTED" : "UNEXPECTED", exName, ex.Message);
         }
         Assert.IsTrue(gotException, "The exception is not caught, but is expected to.");


         Console.WriteLine();
      }
   }
}
