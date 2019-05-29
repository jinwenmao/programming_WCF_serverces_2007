// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.ServiceModel;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;

namespace ServiceModelEx
{
   [AttributeUsage(AttributeTargets.Class)]
   public class ErrorHandlerBehaviorAttribute : Attribute,IErrorHandler,IServiceBehavior
   {
      Type m_ServiceType;

      protected Type ServiceType
      {
         get 
         { 
            return m_ServiceType; 
         }
         set 
         { 
            m_ServiceType = value; 
         }
      }
      
      void IServiceBehavior.Validate(ServiceDescription description,ServiceHostBase host) 
      {}
      void IServiceBehavior.AddBindingParameters(ServiceDescription description,ServiceHostBase host,Collection<ServiceEndpoint> endpoints,BindingParameterCollection parameters)
      {}
      void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription description,ServiceHostBase host)
      {
         ServiceType = description.ServiceType;
         foreach(ChannelDispatcher dispatcher in host.ChannelDispatchers)
         {
            dispatcher.ErrorHandlers.Add(this);
         }
      }
      
      bool IErrorHandler.HandleError(Exception error)
      {
         ErrorHandlerHelper.LogError(error);
         return false;
      }
      void IErrorHandler.ProvideFault(Exception error,MessageVersion version,ref Message fault)
      {
         ErrorHandlerHelper.PromoteException(ServiceType,error,version,ref fault);
      }
   }
} 





