/*  Copyright (C) 2008-2018 Peter Palotas, Jeffrey Jangli, Alexandr Normuradov
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
   public partial class CopyTest
   {
      // Pattern: <class>_<function>_<scenario>_<expected result>


      [TestMethod]
      public void AlphaFS_File_Copy_ThrowAlreadyExistsException_DestinationFileAlreadyExists_LocalAndNetwork_Success()
      {
         File_Copy_ThrowAlreadyExistsException_DestinationFileAlreadyExists(false);
         File_Copy_ThrowAlreadyExistsException_DestinationFileAlreadyExists(true);
      }


      private void File_Copy_ThrowAlreadyExistsException_DestinationFileAlreadyExists(bool isNetwork)
      {
         UnitTestConstants.PrintUnitTestHeader(isNetwork);

         using (var tempRoot = new TemporaryDirectory(isNetwork ? Alphaleonis.Win32.Filesystem.Path.LocalToUnc(UnitTestConstants.TempPath) : UnitTestConstants.TempPath, MethodBase.GetCurrentMethod().Name))
         {
            var srcFile = UnitTestConstants.CreateFile(tempRoot.Directory.FullName);
            var dstFile = srcFile + "-Existing File";

            Console.WriteLine("\nSrc File Path: [{0}]", srcFile);
            Console.WriteLine("Dst File Path: [{0}]", dstFile);

            System.IO.File.Copy(srcFile.FullName, dstFile);


            var gotException = false;

            try
            {
               Alphaleonis.Win32.Filesystem.File.Copy(srcFile.FullName, dstFile);
            }
            catch (Exception ex)
            {
               var exType = ex.GetType();

               gotException = exType == typeof(Alphaleonis.Win32.Filesystem.AlreadyExistsException);

               Console.WriteLine("\n\tCaught {0} Exception: [{1}] {2}", gotException ? "EXPECTED" : "UNEXPECTED", exType.Name, ex.Message);
            }


            Assert.IsTrue(gotException, "The exception is not caught, but is expected to.");
         }
         
         Console.WriteLine();
      }
   }
}
