// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.ServiceModel;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace ServiceModelEx
{
   [AttributeUsage(AttributeTargets.Class)]
   public class CallbackBaseAddressBehaviorAttribute : Attribute,IEndpointBehavior
   {
      int m_CallbackPort = 80;

      public int CallbackPort
      {
         get 
         { 
            return m_CallbackPort; 
         }
         set 
         { 
            m_CallbackPort = value; 
         }
      }

      void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint,BindingParameterCollection bindingParameters)
      {
         if(CallbackPort == 80)
         {
            return;
         }
         lock(typeof(WsDualProxyHelper))
         {
            if(CallbackPort == 0)
            {
               CallbackPort = WsDualProxyHelper.FindPort();
            }
            WSDualHttpBinding binding = endpoint.Binding as WSDualHttpBinding;
            if(binding != null)
            {
               binding.ClientBaseAddress = new Uri("http://localhost:" + CallbackPort +"/");
            }
         }
      }

      void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint,ClientRuntime clientRuntime)
      {
         //clientRuntime.ChannelInitializers;
         clientRuntime.CallbackDispatchRuntime.ChannelDispatcher.Opening +=   delegate(object sender,EventArgs args)
                                                                              {
                                                                                 Trace.WriteLine("Opening");
                                                                              };
         clientRuntime.CallbackDispatchRuntime.ChannelDispatcher.Opened +=   delegate(object sender,EventArgs args)
                                                                        {
                                                                           Trace.WriteLine("Opened");
                                                                        };
         clientRuntime.CallbackDispatchRuntime.ChannelDispatcher.Closed +=   delegate(object sender,EventArgs args)
                                                               {
                                                                  Trace.WriteLine("Closed");
                                                               };


      }



      void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint,EndpointDispatcher endpointDispatcher)
      {}

      void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
      {}
   }
}

