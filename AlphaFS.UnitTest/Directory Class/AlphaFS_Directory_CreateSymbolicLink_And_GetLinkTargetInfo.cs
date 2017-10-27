﻿/*  Copyright (C) 2008-2017 Peter Palotas, Jeffrey Jangli, Alexandr Normuradov
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

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlphaFS.UnitTest
{
   partial class DirectoryTest
   {
      // Pattern: <class>_<function>_<scenario>_<expected result>

      [TestMethod]
      public void AlphaFS_Directory_CreateSymbolicLink_And_GetLinkTargetInfo_LocalAndNetwork_Success()
      {
         if (!UnitTestConstants.IsAdmin())
            Assert.Inconclusive();

         Directory_CreateSymbolicLink_And_GetLinkTargetInfo(false);
         Directory_CreateSymbolicLink_And_GetLinkTargetInfo(true);
      }


      [TestMethod]
      public void AlphaFS_Directory_CreateSymbolicLink_CatchIOException_FileExistsWithSameNameAsDirectory_LocalAndNetwork_Success()
      {
         if (!UnitTestConstants.IsAdmin())
            Assert.Inconclusive();

         Directory_CreateSymbolicLink_CatchIOException_FileExistsWithSameNameAsDirectory(false);
         Directory_CreateSymbolicLink_CatchIOException_FileExistsWithSameNameAsDirectory(true);
      }




      private void Directory_CreateSymbolicLink_And_GetLinkTargetInfo(bool isNetwork)
      {
         UnitTestConstants.PrintUnitTestHeader(isNetwork);
         
         var tempPath = System.IO.Path.GetTempPath();
         if (isNetwork)
            tempPath = Alphaleonis.Win32.Filesystem.Path.LocalToUnc(tempPath);


         using (var rootDir = new TemporaryDirectory(tempPath, MethodBase.GetCurrentMethod().Name))
         {
            var folderLink = System.IO.Path.Combine(rootDir.Directory.FullName, "FolderLink-ToDestinationFolder");

            var dirInfo = new System.IO.DirectoryInfo(System.IO.Path.Combine(rootDir.Directory.FullName, "DestinationFolder"));
            dirInfo.Create();

            Console.WriteLine("\nInput Directory Path: [{0}]", dirInfo.FullName);
            Console.WriteLine("Input Directory Link: [{0}]", folderLink);


            Alphaleonis.Win32.Filesystem.Directory.CreateSymbolicLink(folderLink, dirInfo.FullName);


            var lvi = Alphaleonis.Win32.Filesystem.Directory.GetLinkTargetInfo(folderLink);
            UnitTestConstants.Dump(lvi, -14);
            Assert.IsTrue(lvi.PrintName.Equals(dirInfo.FullName, StringComparison.OrdinalIgnoreCase));




            var dirInfoSysIO = new System.IO.DirectoryInfo(folderLink);
            UnitTestConstants.Dump(dirInfoSysIO, -17);

            Assert.IsTrue((dirInfoSysIO.Attributes & System.IO.FileAttributes.ReparsePoint) != 0);




            var alphaFSDirInfo = new Alphaleonis.Win32.Filesystem.DirectoryInfo(folderLink);
            UnitTestConstants.Dump(alphaFSDirInfo.EntryInfo, -17);

            Assert.AreEqual(System.IO.Directory.Exists(alphaFSDirInfo.FullName), alphaFSDirInfo.Exists);
            Assert.IsTrue(alphaFSDirInfo.EntryInfo.IsDirectory);
            Assert.IsFalse(alphaFSDirInfo.EntryInfo.IsMountPoint);
            Assert.IsTrue(alphaFSDirInfo.EntryInfo.IsReparsePoint);
            Assert.IsTrue(alphaFSDirInfo.EntryInfo.IsSymbolicLink);
            Assert.AreEqual(alphaFSDirInfo.EntryInfo.ReparsePointTag, Alphaleonis.Win32.Filesystem.ReparsePointTag.SymLink);
         }

         Console.WriteLine();
      }


      private void Directory_CreateSymbolicLink_CatchIOException_FileExistsWithSameNameAsDirectory(bool isNetwork)
      {
         UnitTestConstants.PrintUnitTestHeader(isNetwork);

         var tempPath = System.IO.Path.GetTempPath();
         if (isNetwork)
            tempPath = Alphaleonis.Win32.Filesystem.Path.LocalToUnc(tempPath);


         using (var rootDir = new TemporaryDirectory(tempPath, MethodBase.GetCurrentMethod().Name))
         {
            var folderLink = System.IO.Path.Combine(rootDir.Directory.FullName, "FolderLink-ToOriginalFolder");

            var fileInfo = new System.IO.FileInfo(System.IO.Path.Combine(rootDir.Directory.FullName, "OriginalFile.txt"));
            using (fileInfo.Create()) {}

            Console.WriteLine("\nInput File Path     : [{0}]", fileInfo.FullName);
            Console.WriteLine("Input Directory Link: [{0}]", folderLink);


            var gotException = false;

            try
            {
               Alphaleonis.Win32.Filesystem.Directory.CreateSymbolicLink(folderLink, fileInfo.FullName);

            }
            catch (Exception ex)
            {
               var exName = ex.GetType().Name;
               gotException = exName.Equals("IOException", StringComparison.OrdinalIgnoreCase);
               Console.WriteLine("\n\tCaught {0} Exception: [{1}] {2}", gotException ? "EXPECTED" : "UNEXPECTED", exName, ex.Message);
            }


            Assert.IsTrue(gotException, "The exception is not caught, but is expected to.");
         }

         Console.WriteLine();
      }
   }
}
