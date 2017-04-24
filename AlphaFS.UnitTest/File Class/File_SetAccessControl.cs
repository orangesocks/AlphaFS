/*  Copyright (C) 2008-2016 Peter Palotas, Jeffrey Jangli, Alexandr Normuradov
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
using System.Security.AccessControl;
using System.Security.Principal;

namespace AlphaFS.UnitTest
{
   partial class FileTest
   {
      // Pattern: <class>_<function>_<scenario>_<expected result>

      [TestMethod]
      public void File_SetAccessControl_LocalAndNetwork_Success()
      {
         if (!UnitTestConstants.IsAdmin())
            Assert.Inconclusive();

         File_SetAccessControl(false);
         File_SetAccessControl(true);
      }




      private void File_SetAccessControl(bool isNetwork)
      {
         UnitTestConstants.PrintUnitTestHeader(isNetwork);

         var tempPath = System.IO.Path.GetTempPath();
         if (isNetwork)
            tempPath = Alphaleonis.Win32.Filesystem.Path.LocalToUnc(tempPath);


         using (var rootDir = new TemporaryDirectory(tempPath, "File.SetAccessControl"))
         {
            var file = rootDir.RandomFileFullPath + ".txt";


            using (System.IO.File.Create(file)) {}

            var sysIO = System.IO.File.GetAccessControl(file);
            var sysIOaccessRules = sysIO.GetAccessRules(true, true, typeof(NTAccount));

            var alphaFS = Alphaleonis.Win32.Filesystem.File.GetAccessControl(file);
            var alphaFSaccessRules = alphaFS.GetAccessRules(true, true, typeof(NTAccount));


            Console.WriteLine("\nInput File Path: [{0}]", file);
            Console.WriteLine("\n\tSystem.IO rules found: [{0}]\n\tAlphaFS rules found  : [{1}]", sysIOaccessRules.Count, alphaFSaccessRules.Count);
            Assert.AreEqual(sysIOaccessRules.Count, alphaFSaccessRules.Count);


            // Sanity check.
            UnitTestConstants.TestAccessRules(sysIO, alphaFS);


            // Remove inherited properties.
            // Passing true for first parameter protects the new permission from inheritance,
            // and second parameter removes the existing inherited permissions 
            Console.WriteLine("\n\tRemove inherited properties and persist it.");
            alphaFS.SetAccessRuleProtection(true, false);
            Alphaleonis.Win32.Filesystem.File.SetAccessControl(file, alphaFS, AccessControlSections.Access);


            // Re-read, using instance methods.
            var sysIOfi = new System.IO.FileInfo(file);
            var alphaFSfi = new Alphaleonis.Win32.Filesystem.FileInfo(file);

            sysIO = sysIOfi.GetAccessControl(AccessControlSections.Access);
            alphaFS = alphaFSfi.GetAccessControl(AccessControlSections.Access);

            // Sanity check.
            UnitTestConstants.TestAccessRules(sysIO, alphaFS);


            // Restore inherited properties.
            Console.WriteLine("\n\tRestore inherited properties and persist it.");
            alphaFS.SetAccessRuleProtection(false, true);
            Alphaleonis.Win32.Filesystem.File.SetAccessControl(file, alphaFS, AccessControlSections.Access);


            // Re-read.
            sysIO = System.IO.File.GetAccessControl(file, AccessControlSections.Access);
            alphaFS = Alphaleonis.Win32.Filesystem.File.GetAccessControl(file, AccessControlSections.Access);

            // Sanity check.
            UnitTestConstants.TestAccessRules(sysIO, alphaFS);
         }
      }
   }
}
