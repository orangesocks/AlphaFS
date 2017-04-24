﻿/*  Copyright (C) 2008-2016 Peter Palotas, Jeffrey Jangli, Alexandr Normuradov
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
using System.Text;

namespace AlphaFS.UnitTest
{
   partial class FileTest
   {
      // Pattern: <class>_<function>_<scenario>_<expected result>

      [TestMethod]
      public void File_OpenWrite_LocalAndNetwork_Success()
      {
         File_OpenWrite(false);
         File_OpenWrite(true);
      }




      private void File_OpenWrite(bool isNetwork)
      {
         UnitTestConstants.PrintUnitTestHeader(isNetwork);

         var tempPath = System.IO.Path.GetTempPath();
         if (isNetwork)
            tempPath = Alphaleonis.Win32.Filesystem.Path.LocalToUnc(tempPath);


         using (var rootDir = new TemporaryDirectory(tempPath, "File.OpenWrite"))
         {
            var file1 = rootDir.RandomFileFullPath + ".txt";
            var file2 = rootDir.RandomFileFullPath + ".txt";
            Console.WriteLine("\nInput File1 Path: [{0}]", file1);
            Console.WriteLine("\nInput File2 Path: [{0}]", file2);


            using (var stream = System.IO.File.OpenWrite(file1))
            {
               var info = new UTF8Encoding(true).GetBytes(UnitTestConstants.TextHelloWorld);
               stream.Write(info, 0, info.Length);
            }

            using (var stream = Alphaleonis.Win32.Filesystem.File.OpenWrite(file2))
            {
               var info = new UTF8Encoding(true).GetBytes(UnitTestConstants.TextHelloWorld);
               stream.Write(info, 0, info.Length);
            }


            Assert.AreEqual(System.IO.File.ReadAllText(file1), System.IO.File.ReadAllText(file2), "The content of the two files is not equal, but is expected to.");
         }

         Console.WriteLine();
      }
   }
}
