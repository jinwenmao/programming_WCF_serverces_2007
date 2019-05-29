// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Diagnostics;
using System.Reflection;

namespace ServiceModelEx
{
   class ServiceHostActivator : MarshalByRefObject
   {
      ServiceHost m_Host;

      public void SetType(Type serviceType)
      {
         Uri baseAddress = new Uri("net.pipe://localhost/");
         m_Host = new ServiceHost(serviceType,baseAddress);
      }
      public void Open()
      {
         m_Host.Open();
      }
      public void Close()
      {
         m_Host.Close();
      }
      public void Abort()
      {
         m_Host.Abort();
      }
   }
}
