// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Windows.Forms;
using System.Threading;

namespace ServiceModelEx
{
   static class Program
   {
      [STAThread]
      static void Main()
      {
         Thread.CurrentThread.Name = "Main UI Thread";

         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(new ExplorerForm());
      }
   }
}