// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Collections.ObjectModel;

namespace ServiceModelEx
{
   [AttributeUsage(AttributeTargets.Class)]
   public class ThreadAffinityBehaviorAttribute : Attribute,IContractBehavior,IServiceBehavior
   {
      string m_ThreadName;
      Type m_ServiceType;

      public string ThreadName
      {
         get 
         { 
            return m_ThreadName; 
         }
         set 
         { 
            m_ThreadName = value; 
         }
      }
      public ThreadAffinityBehaviorAttribute(Type serviceType) : this(serviceType,null)
      {}
      public ThreadAffinityBehaviorAttribute(Type serviceType,string threadName)
      {
         m_ThreadName = threadName;
         m_ServiceType = serviceType;
      }
      void IContractBehavior.AddBindingParameters(ContractDescription description,ServiceEndpoint endpoint,BindingParameterCollection parameters)
      {}

      void IContractBehavior.ApplyClientBehavior(ContractDescription description,ServiceEndpoint endpoint,ClientRuntime proxy)
      {}

      void IContractBehavior.ApplyDispatchBehavior(ContractDescription description,ServiceEndpoint endpoint,DispatchRuntime dispatchRuntime)
      {
         m_ThreadName = m_ThreadName ?? "Executing endpoints of " + m_ServiceType;
         ThreadAffinityHelper.ApplyDispatchBehavior(m_ServiceType,m_ThreadName,dispatchRuntime);
      }
      void IContractBehavior.Validate(ContractDescription description,ServiceEndpoint endpoint)
      {}

      void IServiceBehavior.AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
      {}

      void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
      {}

      void IServiceBehavior.Validate(ServiceDescription description,ServiceHostBase serviceHostBase)
      {
         serviceHostBase.Closed += delegate
                                   {
                                      ThreadAffinityHelper.CloseThread(m_ServiceType);
                                   };
      }
   }
}