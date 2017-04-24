﻿/*  Copyright (C) 2008-2016 Peter Palotas, Jeffrey Jangli, Alexandr Normuradov
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy 
 *  of this software and associated documentation directorys (the "Software"), to deal 
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
using System.IO;

namespace AlphaFS.UnitTest
{
   /// <summary>This is a test class for FileInfo and is intended to contain all FileInfo UnitTests.</summary>
   public partial class DirectoryInfoTest
   {
      // Pattern: <class>_<function>_<scenario>_<expected result>

      [TestMethod]
      public void DirectoryInfo_Attributes_LocalAndNetwork_Success()
      {
         DirectoryInfo_Attributes(false);
         DirectoryInfo_Attributes(true);
      }




      private void DirectoryInfo_Attributes(bool isNetwork)
      {
         UnitTestConstants.PrintUnitTestHeader(isNetwork);

         var tempPath = System.IO.Path.GetTempPath();
         if (isNetwork)
            tempPath = Alphaleonis.Win32.Filesystem.Path.LocalToUnc(tempPath);


         using (var rootDir = new TemporaryDirectory(tempPath, "DirectoryInfo.Attributes"))
         {
            var folder = rootDir.RandomFileFullPath;
            var dirInfo = new Alphaleonis.Win32.Filesystem.DirectoryInfo(folder + "-AlphaFS");

            Console.WriteLine("\nAlphaFS Input Directory Path: [{0}]", dirInfo.FullName);

            dirInfo.Create();


            dirInfo.Attributes |= FileAttributes.ReadOnly;
            Assert.IsTrue((dirInfo.Attributes & FileAttributes.ReadOnly) != 0, "The directory is not ReadOnly, but is expected to be.");

            dirInfo.Attributes &= ~FileAttributes.ReadOnly;
            Assert.IsTrue((dirInfo.Attributes & FileAttributes.ReadOnly) == 0, "The directory is ReadOnly, but is expected not to be.");


            dirInfo.Attributes |= FileAttributes.Hidden;
            Assert.IsTrue((dirInfo.Attributes & FileAttributes.Hidden) != 0, "The directory is not Hidden, but is expected to be.");

            dirInfo.Attributes &= ~FileAttributes.Hidden;
            Assert.IsTrue((dirInfo.Attributes & FileAttributes.Hidden) == 0, "The directory is Hidden, but is expected not to be.");


            dirInfo.Attributes |= FileAttributes.System;
            Assert.IsTrue((dirInfo.Attributes & FileAttributes.System) != 0, "The directory is not System, but is expected to be.");

            dirInfo.Attributes &= ~FileAttributes.System;
            Assert.IsTrue((dirInfo.Attributes & FileAttributes.System) == 0, "The directory is System, but is expected not to be.");
         }

         Console.WriteLine();
      }
   }
}
