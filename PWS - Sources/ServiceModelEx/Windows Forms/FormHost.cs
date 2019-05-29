// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Windows.Forms;
using System.Threading;
using System.ServiceModel;


namespace ServiceModelEx
{
   [Serializable]
   [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
   public abstract class FormHost<F> : Form where F : Form
   {
      ServiceHost<F> m_Host;

      protected ServiceHost<F> Host
      {
         get
         {
            return m_Host;
         }
         set
         {
            m_Host = value;
         }
      }
      public FormHost(params string[] baseAddresses)
      {
         m_Host = new ServiceHost<F>(this as F,baseAddresses);
         Load += delegate
                 {
                    if(Host.State == CommunicationState.Created)
                    {
                       Host.Open();
                    }
                 };         
         FormClosed += delegate
                       {
                          if(Host.State == CommunicationState.Opened)
                          {
                             Host.Close();
                          }
                       };
      }
   }
}