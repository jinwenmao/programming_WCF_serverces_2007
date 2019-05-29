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
   public class AppDomainHost<T> : IDisposable
   {
      ServiceHostActivator m_ServiceHostActivator;

      public AppDomainHost() : this("AppDomain Host For " + typeof(T) + " " + Guid.NewGuid())
      {}

      public AppDomainHost(string appDomainName)
      {
         Debug.Assert(AppDomain.CurrentDomain.FriendlyName != appDomainName); 
         AppDomain newDomain = AppDomain.CreateDomain(appDomainName);
         string assemblyName = Assembly.GetAssembly(typeof(ServiceHostActivator)).FullName;
         m_ServiceHostActivator = newDomain.CreateInstanceAndUnwrap(assemblyName,typeof(ServiceHostActivator).ToString()) as ServiceHostActivator;
         m_ServiceHostActivator.SetType(typeof(T));
      }
      public AppDomainHost(AppDomain appDomain)
      {
         string assemblyName = Assembly.GetAssembly(typeof(ServiceHostActivator)).FullName;
         m_ServiceHostActivator = appDomain.CreateInstanceAndUnwrap(assemblyName,typeof(ServiceHostActivator).ToString()) as ServiceHostActivator;
         m_ServiceHostActivator.SetType(typeof(T));
      }
      public void Open()
      {
         m_ServiceHostActivator.Open();
      }
      public void Close()
      {
         m_ServiceHostActivator.Close();
      }
      public void Abort()
      {
         m_ServiceHostActivator.Abort();
      }
      void IDisposable.Dispose()
      {
         Close();
      }
   }
}
