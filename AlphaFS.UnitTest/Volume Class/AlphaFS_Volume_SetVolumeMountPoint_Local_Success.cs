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
using System.Linq;
using System.Reflection;

namespace AlphaFS.UnitTest
{
   /// <summary>This is a test class for Volume and is intended to contain all Volume class Unit Tests.</summary>
   public partial class VolumeTest
   {
      // Pattern: <class>_<function>_<scenario>_<expected result>


      [TestMethod]
      public void AlphaFS_Volume_SetVolumeMountPoint_And_DeleteVolumeMountPoint_Local_Success()
      {
         if (!UnitTestConstants.IsAdmin())
            Assert.Inconclusive();


         UnitTestConstants.PrintUnitTestHeader(false);

         var tempPath = System.IO.Path.GetTempPath();

         using (var rootDir = new TemporaryDirectory(tempPath, MethodBase.GetCurrentMethod().Name))
         {
            var dirInfo = System.IO.Directory.CreateDirectory(rootDir.RandomDirectoryFullPath);
            Console.WriteLine("\nInput Directory Path: [{0}]", dirInfo.FullName);


            var guid = Alphaleonis.Win32.Filesystem.Volume.GetUniqueVolumeNameForPath(UnitTestConstants.SysDrive);




            Alphaleonis.Win32.Filesystem.Volume.SetVolumeMountPoint(dirInfo.FullName, guid);

            Assert.IsTrue(Alphaleonis.Win32.Filesystem.Volume.EnumerateVolumeMountPoints(guid).Any(mountPoint => dirInfo.FullName.EndsWith(mountPoint.TrimEnd('\\'))));




            var lvi = Alphaleonis.Win32.Filesystem.Directory.GetLinkTargetInfo(dirInfo.FullName);
            UnitTestConstants.Dump(lvi, -14);

            Assert.IsTrue(lvi.SubstituteName.StartsWith(Alphaleonis.Win32.Filesystem.Path.NonInterpretedPathPrefix));
            Assert.IsTrue(lvi.SubstituteName.EndsWith(guid.Replace(Alphaleonis.Win32.Filesystem.Path.LongPathPrefix, string.Empty)));




            Alphaleonis.Win32.Filesystem.Volume.DeleteVolumeMountPoint(dirInfo.FullName);

            Assert.IsFalse(Alphaleonis.Win32.Filesystem.Volume.EnumerateVolumeMountPoints(guid).Any(mountPoint => dirInfo.FullName.EndsWith(mountPoint.TrimEnd('\\'))));
         }
      }
   }
}
