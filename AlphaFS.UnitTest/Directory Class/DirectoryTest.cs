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

using Alphaleonis.Win32.Filesystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using Directory = Alphaleonis.Win32.Filesystem.Directory;
using File = Alphaleonis.Win32.Filesystem.File;
using Path = Alphaleonis.Win32.Filesystem.Path;

namespace AlphaFS.UnitTest
{
   /// <summary>This is a test class for Directory and is intended to contain all Directory class Unit Tests.</summary>
   [TestClass]
   public partial class DirectoryTest
   {
      private void DumpGetProperties(bool isLocal)
      {
         Console.WriteLine("\n=== TEST {0} ===", isLocal ? UnitTestConstants.Local : UnitTestConstants.Network);
         var path = isLocal ? UnitTestConstants.SysRoot32 : Path.LocalToUnc(UnitTestConstants.SysRoot32);

         Console.WriteLine("\n\tAggregated properties of file system objects from Directory: [{0}]\n", path);

         UnitTestConstants.StopWatcher(true);
         var props = Directory.GetProperties(path, DirectoryEnumerationOptions.FilesAndFolders | DirectoryEnumerationOptions.Recursive | DirectoryEnumerationOptions.ContinueOnException);
         var report = UnitTestConstants.Reporter();

         var total = props["Total"];
         var file = props["File"];
         var size = props["Size"];
         var cnt = 0;

         foreach (var key in props.Keys)
            Console.WriteLine("\t\t#{0:000}\t{1, -17} = [{2}]", ++cnt, key, props[key]);

         Console.WriteLine("\n\t{0}", report);

         if (cnt == 0)
            Assert.Inconclusive("Nothing is enumerated, but it is expected.");

         Assert.IsTrue(total > 0, "0 Objects.");
         Assert.IsTrue(file > 0, "0 Files.");
         Assert.IsTrue(size > 0, "0 Size.");
         Console.WriteLine();
      }


      private bool HasInheritedPermissions(string path)
      {
         var acl = Directory.GetAccessControl(path);
         return acl.GetAccessRules(false, true, typeof(SecurityIdentifier)).Count > 0;
      }


      #region .NET

      [TestMethod]
      public void Directory_GetDirectoryRoot()
      {
         Console.WriteLine("Directory.GetDirectoryRoot()");

         var pathCnt = 0;
         var errorCnt = 0;

         #region ArgumentException

         var gotException = false;
         try
         {
            Directory.GetDirectoryRoot(@"\\\\.txt");
         }
         catch (Exception ex)
         {
            var exType = ex.GetType();

            gotException = exType == typeof(ArgumentException);

            Console.WriteLine("\n\tCaught {0} Exception: [{1}] {2}", gotException ? "EXPECTED" : "UNEXPECTED", exType.Name, ex.Message);
         }

         Assert.IsTrue(gotException, "The exception is not caught, but is expected to.");
         Console.WriteLine();

         #endregion // ArgumentException

         UnitTestConstants.StopWatcher(true);
         foreach (var path in UnitTestConstants.InputPaths)
         {
            string expected = null;
            string actual = null;
            var skipAssert = false;

            Console.WriteLine("\n#{0:000}\tInput Path: [{1}]", ++pathCnt, path);

            // System.IO
            try
            {
               expected = System.IO.Directory.GetDirectoryRoot(path);
            }
            catch (Exception ex)
            {
               skipAssert = ex is ArgumentException;

               Console.WriteLine("\tCaught [System.IO] {0}: [{1}]", ex.GetType().FullName, ex.Message.Replace(Environment.NewLine, "  "));
            }
            Console.WriteLine("\t   System.IO : [{0}]", expected ?? "null");


            // AlphaFS
            try
            {
               actual = Directory.GetDirectoryRoot(path);

               if (!skipAssert)
                  Assert.AreEqual(expected, actual);
            }
            catch (Exception ex)
            {
               errorCnt++;

               Console.WriteLine("\tCaught [AlphaFS] {0}: [{1}]", ex.GetType().FullName, ex.Message.Replace(Environment.NewLine, "  "));
            }
            Console.WriteLine("\t   AlphaFS   : [{0}]", actual ?? "null");
         }
         Console.WriteLine("\n{0}", UnitTestConstants.Reporter(true));

         Assert.AreEqual(0, errorCnt, "Encountered paths where AlphaFS != System.IO");
      }
      

      [TestMethod]
      public void Directory_GetFileSystemEntries_LongPathWithPrefix_ShouldReturnCorrectEntries()
      {
         using (var tempDir = new TemporaryDirectory(MethodBase.GetCurrentMethod().Name))
         {
            var longDir = Path.Combine(tempDir.Directory.FullName, new string('x', 128), new string('x', 128), new string('x', 128), new string('x', 128));
            Directory.CreateDirectory(longDir);
            Directory.CreateDirectory(Path.Combine(longDir, "A"));
            Directory.CreateDirectory(Path.Combine(longDir, "B"));
            File.WriteAllText(Path.Combine(longDir, "C"), "C");

            var entries = Directory.GetFileSystemEntries("\\\\?\\" + longDir).ToArray();

            Assert.AreEqual(3, entries.Length);
         }
      }

      [TestMethod]
      public void Directory_GetFileSystemEntries_LongPathWithoutPrefix_ShouldReturnCorrectEntries()
      {
         using (var tempDir = new TemporaryDirectory(MethodBase.GetCurrentMethod().Name))
         {
            var longDir = Path.Combine(tempDir.Directory.FullName, new string('x', 128), new string('x', 128), new string('x', 128), new string('x', 128));
            Directory.CreateDirectory(longDir);
            Directory.CreateDirectory(Path.Combine(longDir, "A"));
            Directory.CreateDirectory(Path.Combine(longDir, "B"));
            File.WriteAllText(Path.Combine(longDir, "C"), "C");

            var entries = Directory.GetFileSystemEntries(longDir).ToArray();

            Assert.AreEqual(3, entries.Length);
         }
      }

      

      [TestMethod]
      public void Directory_GetParent()
      {
         Console.WriteLine("Directory.GetParent()");

         var pathCnt = 0;
         var errorCnt = 0;

         UnitTestConstants.StopWatcher(true);
         foreach (var path in UnitTestConstants.InputPaths)
         {
            string expected = null;
            string actual = null;
            var skipAssert = false;

            Console.WriteLine("\n#{0:000}\tInput Path: [{1}]", ++pathCnt, path);

            // System.IO
            try
            {
               var result = System.IO.Directory.GetParent(path);
               expected = result == null ? null : result.FullName;
            }
            catch (Exception ex)
            {
               skipAssert = ex is ArgumentException;

               Console.WriteLine("\tCaught [System.IO] {0}: [{1}]", ex.GetType().FullName, ex.Message.Replace(Environment.NewLine, "  "));
            }
            Console.WriteLine("\t   System.IO : [{0}]", expected ?? "null");


            // AlphaFS
            try
            {
               var result = Directory.GetParent(path);
               actual = result == null ? null : result.FullName;

               if (!skipAssert)
                  Assert.AreEqual(expected, actual);
            }
            catch (Exception ex)
            {
               errorCnt++;

               Console.WriteLine("\tCaught [AlphaFS] {0}: [{1}]", ex.GetType().FullName, ex.Message.Replace(Environment.NewLine, "  "));
            }
            Console.WriteLine("\t   AlphaFS   : [{0}]", actual ?? "null");
         }
         Console.WriteLine("\n{0}", UnitTestConstants.Reporter(true));

         Assert.AreEqual(0, errorCnt, "Encountered paths where AlphaFS != System.IO");
      }

      #endregion // .NET


      [TestMethod]
      public void AlphaFS_Directory_GetProperties()
      {
         Console.WriteLine("Directory.GetProperties()");

         DumpGetProperties(true);
         DumpGetProperties(false);
      }


      [TestMethod]
      public void AlphaFS_Directory_HasInheritedPermissions()
      {
         Console.WriteLine("Directory.HasInheritedPermissions()\n");

         var searchPattern = Path.WildcardStarMatchAll;
         var searchOption = SearchOption.TopDirectoryOnly;

         var cnt = 0;
         UnitTestConstants.StopWatcher(true);
         foreach (var dir in Directory.EnumerateDirectories(UnitTestConstants.SysRoot, searchPattern, searchOption))
         {
            try
            {
               var hasIp = Directory.HasInheritedPermissions(dir);

               if (hasIp)
                  Console.WriteLine("\t#{0:000}\t[{1}]\t\tDirectory has inherited permissions: [{2}]", ++cnt, hasIp, dir);

               Assert.AreEqual(hasIp, HasInheritedPermissions(dir));
            }
            catch (Exception ex)
            {
               Console.Write("\t#{0:000}\tCaught {1} for directory: [{2}]\t[{3}]\n", cnt, ex.GetType().FullName, dir, ex.Message.Replace(Environment.NewLine, "  "));
            }
         }
         Console.Write("\n{0}", UnitTestConstants.Reporter());
      }
   }
}
