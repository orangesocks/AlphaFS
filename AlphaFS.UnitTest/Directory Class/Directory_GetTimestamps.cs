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

using System;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlphaFS.UnitTest
{
   partial class DirectoryTest
   {
      // Pattern: <class>_<function>_<scenario>_<expected result>

      [TestMethod]
      public void Directory_GetXxxTimeXxx_LocalAndNetwork_Success()
      {
         Directory_GetXxxTimeXxx(false);
         Directory_GetXxxTimeXxx(true);
      }


      [TestMethod]
      public void Directory_GetXxxTimeXxx_NonExistingDirectory_Success()
      {
         const string Path = @"z:\nonExistingPath\nonExistingSubFolder";
         Assert.IsFalse(System.IO.Directory.Exists(Path));

         var newDateTime = new DateTime(1601, 1, 1);
         var newDateTimeLocaltime = new DateTime(1601, 1, 1).ToLocalTime();


         Assert.AreEqual(newDateTimeLocaltime, System.IO.Directory.GetCreationTime(Path));
         Assert.AreEqual(newDateTimeLocaltime, Alphaleonis.Win32.Filesystem.Directory.GetCreationTime(Path));

         Assert.AreEqual(newDateTime, System.IO.Directory.GetCreationTimeUtc(Path));
         Assert.AreEqual(newDateTime, Alphaleonis.Win32.Filesystem.Directory.GetCreationTimeUtc(Path));


         Assert.AreEqual(newDateTimeLocaltime, System.IO.Directory.GetLastAccessTime(Path));
         Assert.AreEqual(newDateTimeLocaltime, Alphaleonis.Win32.Filesystem.Directory.GetLastAccessTime(Path));

         Assert.AreEqual(newDateTime, System.IO.Directory.GetLastAccessTimeUtc(Path));
         Assert.AreEqual(newDateTime, Alphaleonis.Win32.Filesystem.Directory.GetLastAccessTimeUtc(Path));


         Assert.AreEqual(newDateTimeLocaltime, System.IO.Directory.GetLastWriteTime(Path));
         Assert.AreEqual(newDateTimeLocaltime, Alphaleonis.Win32.Filesystem.Directory.GetLastWriteTime(Path));

         Assert.AreEqual(newDateTime, System.IO.Directory.GetLastWriteTimeUtc(Path));
         Assert.AreEqual(newDateTime, Alphaleonis.Win32.Filesystem.Directory.GetLastWriteTimeUtc(Path));
      }




      private void Directory_GetXxxTimeXxx(bool isNetwork)
      {
         UnitTestConstants.PrintUnitTestHeader(isNetwork);

         var tempPath = System.IO.Path.GetTempPath();
         if (isNetwork)
            tempPath = Alphaleonis.Win32.Filesystem.Path.LocalToUnc(tempPath);


         using (var rootDir = new TemporaryDirectory(tempPath, MethodBase.GetCurrentMethod().Name))
         {
            var folder = isNetwork ? Alphaleonis.Win32.Filesystem.Path.LocalToUnc(UnitTestConstants.SysRoot32) : UnitTestConstants.SysRoot32;

            Console.WriteLine("\nInput Directory Path: [{0}]", folder);


            Assert.AreEqual(System.IO.Directory.GetCreationTime(folder), Alphaleonis.Win32.Filesystem.Directory.GetCreationTime(folder));
            Assert.AreEqual(System.IO.Directory.GetCreationTimeUtc(folder), Alphaleonis.Win32.Filesystem.Directory.GetCreationTimeUtc(folder));
            Assert.AreEqual(System.IO.Directory.GetLastAccessTime(folder), Alphaleonis.Win32.Filesystem.Directory.GetLastAccessTime(folder));
            Assert.AreEqual(System.IO.Directory.GetLastAccessTimeUtc(folder), Alphaleonis.Win32.Filesystem.Directory.GetLastAccessTimeUtc(folder));
            Assert.AreEqual(System.IO.Directory.GetLastWriteTime(folder), Alphaleonis.Win32.Filesystem.Directory.GetLastWriteTime(folder));
            Assert.AreEqual(System.IO.Directory.GetLastWriteTimeUtc(folder), Alphaleonis.Win32.Filesystem.Directory.GetLastWriteTimeUtc(folder));


            // We cannot compare ChangeTime against .NET because it does not exist.
            // Creating a directory and renaming it triggers ChangeTime, so test for that.

            folder = rootDir.RandomDirectoryFullPath;
            if (isNetwork) folder = Alphaleonis.Win32.Filesystem.Path.LocalToUnc(folder);
            Console.WriteLine("Input Directory Path: [{0}]", folder);

            var dirInfo = new System.IO.DirectoryInfo(folder);
            dirInfo.Create();
            var fileName = dirInfo.Name;


            var changeTimeActual = Alphaleonis.Win32.Filesystem.Directory.GetChangeTime(folder);
            var changeTimeUtcActual = Alphaleonis.Win32.Filesystem.Directory.GetChangeTimeUtc(folder);


            // Sleep for three seconds.
            var delay = 3;

            dirInfo.MoveTo(dirInfo.FullName.Replace(fileName, fileName + "-Renamed"));
            Thread.Sleep(delay * 1000);
            dirInfo.MoveTo(dirInfo.FullName.Replace(fileName + "-Renamed", fileName));


            var newChangeTime = changeTimeActual.AddSeconds(delay);
            Assert.AreEqual(changeTimeActual.AddSeconds(delay), newChangeTime);

            newChangeTime = changeTimeUtcActual.AddSeconds(delay);
            Assert.AreEqual(changeTimeUtcActual.AddSeconds(delay), newChangeTime);
         }
      }
   }
}
