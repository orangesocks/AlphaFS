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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AlphaFS.UnitTest
{
   public partial class PathTest
   {
      // Pattern: <class>_<function>_<scenario>_<expected result>


      [TestMethod]
      public void AlphaFS_Path_IsUncPath()
      {
         UnitTestConstants.PrintUnitTestHeader();

         var pathCnt = 0;
         var errorCnt = 0;
         var uncPathCnt = 0;

         foreach (var path in UnitTestConstants.InputPaths)
         {
            var actual = false;

            Console.WriteLine("#{0:000}\tInput Path: [{1}]", ++pathCnt, path);

            // AlphaFS
            try
            {
               var expected = path.StartsWith(Alphaleonis.Win32.Filesystem.Path.UncPrefix, StringComparison.OrdinalIgnoreCase);

               actual = Alphaleonis.Win32.Filesystem.Path.IsUncPath(path);

               if (!(!path.StartsWith(Alphaleonis.Win32.Filesystem.Path.GlobalRootPrefix, StringComparison.OrdinalIgnoreCase) ||
                     !path.StartsWith(Alphaleonis.Win32.Filesystem.Path.VolumePrefix, StringComparison.OrdinalIgnoreCase)))

                  Assert.AreEqual(expected, actual);

               if (actual)
                  uncPathCnt++;
            }
            catch (Exception ex)
            {
               errorCnt++;

               Console.WriteLine("\tCaught [AlphaFS] {0}: [{1}]", ex.GetType().FullName, ex.Message.Replace(Environment.NewLine, "  "));
            }

            Console.WriteLine("\t    AlphaFS   : [{0}]", actual);

            Console.WriteLine();
         }


         // Hand counted 32 True's.
         Assert.AreEqual(32, uncPathCnt, "Number of UNC paths do not match.");


         Assert.AreEqual(0, errorCnt, "No errors were expected.");
      }
   }
}
