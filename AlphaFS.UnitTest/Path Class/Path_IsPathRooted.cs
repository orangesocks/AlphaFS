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

namespace AlphaFS.UnitTest
{
   /// <summary>This is a test class for Path and is intended to contain all Path Unit Tests.</summary>
   public partial class PathTest
   {
      // Pattern: <class>_<function>_<scenario>_<expected result>

      [TestMethod]
      public void Path_IsPathRooted_LocalAndNetwork_Success()
      {
         UnitTestConstants.StopWatcher(true);
         foreach (var path in UnitTestConstants.InputPaths)
         {
            var action = Alphaleonis.Win32.Filesystem.Path.IsPathRooted(path);
            Console.WriteLine("\tIsPathRooted: [{0}]\t\tInput Path: [{1}]", action, path);
            Assert.AreEqual(System.IO.Path.IsPathRooted(path), action);
         }
         Console.WriteLine("\n{0}", UnitTestConstants.Reporter(true));
      }


      [TestMethod]
      public void Path_IsPathRooted_NullOrEmpty_Success()
      {
         Assert.AreEqual(System.IO.Path.IsPathRooted(null), Alphaleonis.Win32.Filesystem.Path.IsPathRooted(null));
         Assert.AreEqual(System.IO.Path.IsPathRooted(string.Empty), Alphaleonis.Win32.Filesystem.Path.IsPathRooted(string.Empty));
      }
   }
}
