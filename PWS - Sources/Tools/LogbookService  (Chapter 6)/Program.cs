// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ServiceModel;

namespace LogbookService
{
   static class Program
   {
      static void Main()
      {
         ServiceHost host = new ServiceHost(typeof(LogbookManager),new Uri("http://localhost:8005/"));

         host.Open();

         Application.EnableVisualStyles();
         Application.Run(new LogbookHostForm());

         host.Close();
      }
   }
}