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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace AlphaFS.UnitTest
{
   public partial class ByHandleFileInfoTest
   {
      // Pattern: <class>_<function>_<scenario>_<expected result>


      [TestMethod]
      public void AlphaFS_File_GetFileInfoByHandle_LocalAndNetwork_Success()
      {
         File_GetFileInfoByHandle(false);
         File_GetFileInfoByHandle(true);
      }




      private void File_GetFileInfoByHandle(bool isNetwork)
      {
         UnitTestConstants.PrintUnitTestHeader(isNetwork);

         var tempPath = System.IO.Path.GetTempPath();
         if (isNetwork)
            tempPath = Alphaleonis.Win32.Filesystem.Path.LocalToUnc(tempPath);


         using (var rootDir = new TemporaryDirectory(tempPath, MethodBase.GetCurrentMethod().Name))
         {
            var file = rootDir.RandomFileFullPath;
            Console.WriteLine("\nInput File Path: [{0}]]", file);


            var fileInfo = new System.IO.FileInfo(file);

            using (var stream = fileInfo.OpenWrite())
            {
               var size = new Random().Next(1000, 10000);
               stream.Write(new byte[size], 0, size);

               var bhfi = Alphaleonis.Win32.Filesystem.File.GetFileInfoByHandle(stream.SafeFileHandle);
               Assert.IsTrue(UnitTestConstants.Dump(bhfi, -18));


               Assert.AreEqual(fileInfo.CreationTimeUtc, bhfi.CreationTimeUtc);
               Assert.AreEqual(fileInfo.LastAccessTimeUtc, bhfi.LastAccessTimeUtc);
               Assert.AreEqual(fileInfo.LastWriteTimeUtc, bhfi.LastWriteTimeUtc);

               Assert.AreEqual(fileInfo.Length, bhfi.FileSize);
            }
         }

         Console.WriteLine();
      }
   }
}
