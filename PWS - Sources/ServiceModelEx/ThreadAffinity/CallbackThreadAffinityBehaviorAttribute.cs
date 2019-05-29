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
using System.Windows.Forms;

namespace ServiceModelEx
{
   [AttributeUsage(AttributeTargets.Class)]
   public class CallbackThreadAffinityBehaviorAttribute : Attribute,IEndpointBehavior
   {
      string m_ThreadName;
      Type m_ClientType;

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
      public CallbackThreadAffinityBehaviorAttribute(Type clientType) : this(clientType,null)
      {}
      public CallbackThreadAffinityBehaviorAttribute(Type clientType,string threadName)
      {
         m_ThreadName = threadName;
         m_ClientType = clientType;

         AppDomain.CurrentDomain.ProcessExit += delegate
                                                {
                                                   ThreadAffinityHelper.CloseThread(m_ClientType);
                                                };
      }
      void IEndpointBehavior.AddBindingParameters(ServiceEndpoint serviceEndpoint,BindingParameterCollection bindingParameters)
      {}

      void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint serviceEndpoint,ClientRuntime clientRuntime)
      {
         m_ThreadName = m_ThreadName ?? "Executing callbacks of " + m_ClientType;
         ThreadAffinityHelper.ApplyDispatchBehavior(m_ClientType,m_ThreadName,clientRuntime.CallbackDispatchRuntime);
      }

      void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint,EndpointDispatcher endpointDispatcher)
      {}

      void IEndpointBehavior.Validate(ServiceEndpoint serviceEndpoint)
      {}
   }
}