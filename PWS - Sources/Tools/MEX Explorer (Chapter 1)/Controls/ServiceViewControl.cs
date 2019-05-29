using System;
using System.Windows.Forms;
using System.Threading;

namespace ServiceModelEx
{
   partial class ServiceViewControl : NodeViewControl
   {
      string m_Address;
      public ServiceViewControl()
      {
         InitializeComponent();
      }
      public void Refresh(string address)
      {
         if(m_Address != address)
         {
            m_Address = address;
            m_WebBrowser.Navigate(m_Address);
         }
      }
      public string ExtractServiceName()
      {
         while(m_WebBrowser.IsBusy)
         {
            Application.DoEvents();
            Thread.Sleep(0);
         }
         string title = m_WebBrowser.DocumentTitle;
         if(title == "Invalid syntax error" || title == "No page to display" || title == "Cannot find server")
         {
            title = "Unknown";
         }
         if(String.IsNullOrEmpty(title))
         {
            return title;
         }
         return title.Split(new string[]{" Service"},StringSplitOptions.RemoveEmptyEntries)[0];
      }
   }
}
