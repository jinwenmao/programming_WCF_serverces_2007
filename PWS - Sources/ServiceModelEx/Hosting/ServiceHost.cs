// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Messaging;
using System.ServiceModel;
using System.Diagnostics;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ServiceModelEx
{
   public class ServiceHost<T> : ServiceHost
   {
      class ErrorHandlerBehavior : IServiceBehavior,IErrorHandler
      {
         IErrorHandler m_ErrorHandler;
         public ErrorHandlerBehavior(IErrorHandler errorHandler)
         {
            m_ErrorHandler = errorHandler;
         }
         void IServiceBehavior.Validate(ServiceDescription description,ServiceHostBase host)
         {}
         void IServiceBehavior.AddBindingParameters(ServiceDescription description,ServiceHostBase host,Collection<ServiceEndpoint> endpoints,BindingParameterCollection parameters)
         {}
         void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription description,ServiceHostBase host)
         {
            foreach(ChannelDispatcher dispatcher in host.ChannelDispatchers)
            {
               dispatcher.ErrorHandlers.Add(this);
            }
         }
         bool IErrorHandler.HandleError(Exception error)
         {
            return m_ErrorHandler.HandleError(error);
         }
         void IErrorHandler.ProvideFault(Exception error,MessageVersion version,ref System.ServiceModel.Channels.Message fault)
         {
            m_ErrorHandler.ProvideFault(error,version,ref fault);
         }
      }

      AffinitySynchronizer m_AffinitySynchronizer;
      List<IServiceBehavior> m_ErrorHandlers = new List<IServiceBehavior>();

      /// <summary>
      /// Can only call before openning the host
      /// </summary>
      public void AddErrorHandler(IErrorHandler errorHandler)
      {
         if(State == CommunicationState.Opened)
         {
            throw new InvalidOperationException("Host is already opened");
         }
         Debug.Assert(errorHandler != null);
         IServiceBehavior errorHandlerBehavior = new ErrorHandlerBehavior(errorHandler);

         m_ErrorHandlers.Add(errorHandlerBehavior);
      }
      /// <summary>
      /// Can only call before openning the host
      /// </summary>
      public void AddErrorHandler()
      {
         if(State == CommunicationState.Opened)
         {
            throw new InvalidOperationException("Host is already opened");
         }
         IServiceBehavior errorHandlerBehavior = new ErrorHandlerBehaviorAttribute();
         m_ErrorHandlers.Add(errorHandlerBehavior);
      } 
      /// <summary>
      /// Can only call before openning the host. Does not override config values if present 
      /// </summary>
      public void SetThrottle(ServiceThrottlingBehavior serviceThrottle)
      {
         SetThrottle(serviceThrottle,false);
      }
      
      /// <summary>
      /// Can only call before openning the host
      /// </summary>
      public void ImpersonateAll()
      {
         if(State == CommunicationState.Opened)
         {
            throw new InvalidOperationException("Host is already opened");
         }
         SecurityHelper.ImpersonateAll(this);
      }


      /// <summary>
      /// Can only call before openning the host
      /// </summary>
      public bool EnableMetadataExchange
      {
         set
         {
            if(State == CommunicationState.Opened)
            {
               throw new InvalidOperationException("Host is already opened");
            }

            ServiceMetadataBehavior metadataBehavior;
            metadataBehavior = Description.Behaviors.Find<ServiceMetadataBehavior>();
            if(metadataBehavior == null)
            {
               metadataBehavior = new ServiceMetadataBehavior();
               metadataBehavior.HttpGetEnabled = value;
               Description.Behaviors.Add(metadataBehavior);
            }
            if(value == true)
            {
               if(HasMexEndpoint == false)
               {
                  AddAllMexEndPoints();
               }
            }         
         }
         get
         {
            ServiceMetadataBehavior metadataBehavior;
            metadataBehavior = Description.Behaviors.Find<ServiceMetadataBehavior>();
            if(metadataBehavior == null)
            {
               return false;
            }
            return metadataBehavior.HttpGetEnabled;
         }
      }
      public void AddAllMexEndPoints()
      {
         Debug.Assert(HasMexEndpoint == false);

         foreach(Uri baseAddress in BaseAddresses)
         {
            BindingElement bindingElement = null;
            switch(baseAddress.Scheme)
            {
               case "net.tcp":
               {
                  bindingElement = new TcpTransportBindingElement();
                  break;
               }
               case "net.pipe":
               {
                  bindingElement = new NamedPipeTransportBindingElement();
                  break;
               }
               case "http":
               {
                  bindingElement = new HttpTransportBindingElement();
                  break;
               }
               case "https":
               {
                  bindingElement = new HttpsTransportBindingElement();
                  break;
               }
            }
            if(bindingElement != null)
            {
               Binding binding = new CustomBinding(bindingElement);
               AddServiceEndpoint(typeof(IMetadataExchange),binding,"MEX");
            }         
         }
      }
      public bool HasMexEndpoint
      {
         get
         {
            Predicate<ServiceEndpoint> mexEndPoint = delegate(ServiceEndpoint endpoint)
                                                     {
                                                        return endpoint.Contract.ContractType == typeof(IMetadataExchange);
                                                     };
            return Collection.Exists(Description.Endpoints,mexEndPoint);
         }
      }
      /// <summary>
      /// Can only call before openning the host
      /// </summary>
      public void SetThrottle(int maxCalls,int maxSessions,int maxInstances)
      {
         ServiceThrottlingBehavior throttle = new ServiceThrottlingBehavior();
         throttle.MaxConcurrentCalls = maxCalls;
         throttle.MaxConcurrentSessions = maxSessions;
         throttle.MaxConcurrentInstances = maxInstances;
         SetThrottle(throttle);
      }
      /// <summary>
      /// Can only call before openning the host
      /// </summary>
      /// <param name="serviceThrottle"></param>
      /// <param name="overrideConfig"></param>
      public void SetThrottle(ServiceThrottlingBehavior serviceThrottle,bool overrideConfig)
      {
         if(State == CommunicationState.Opened)
         {
            throw new InvalidOperationException("Host is already opened");
         }
         ServiceThrottlingBehavior throttle = Description.Behaviors.Find<ServiceThrottlingBehavior>();
         if(throttle == null)
         {
            Description.Behaviors.Add(serviceThrottle);
            return; 
         }
         if(overrideConfig == false)
         {
            return;
         }
         Description.Behaviors.Remove(throttle);
         Description.Behaviors.Add(serviceThrottle);
      }
      /// <summary>
      /// Can only call before openning the host
      /// </summary>
      /// <param name="mode">If set to ServiceSecurity.Anonymous,ServiceSecurity.BusinessToBusiness or ServiceSecurity.Internet then the service certificate must be listed in config file</param>
      public void SetSecurityBehavior(ServiceSecurity mode,bool useAspNetProviders,string applicationName,bool impersonateAll)
      {
         SetSecurityBehavior(mode,StoreLocation.LocalMachine,StoreName.My,X509FindType.FindBySubjectName,null,useAspNetProviders,applicationName,impersonateAll);
      }
      /// <summary>
      /// Can only call before openning the host
      /// </summary>
      /// <param name="mode">Certificate is looked up by name from LocalMachine/My store</param>
      public void SetSecurityBehavior(ServiceSecurity mode,string serviceCertificateName,bool useAspNetProviders,string applicationName,bool impersonateAll) 
      {
         SetSecurityBehavior(mode,StoreLocation.LocalMachine,StoreName.My,X509FindType.FindBySubjectName,serviceCertificateName,useAspNetProviders,applicationName,impersonateAll);
      }

      /// <summary>
      /// Can only call before openning the host
      /// </summary>
      public void SetSecurityBehavior(ServiceSecurity mode,StoreLocation storeLocation,StoreName storeName,X509FindType findType,string serviceCertificateName,bool useAspNetProviders,string applicationName,bool impersonateAll)
      {
         if(State == CommunicationState.Opened)
         {
            throw new InvalidOperationException("Host is already opened");
         }
         SecurityBehavior securityBehavior = new SecurityBehavior(mode,storeLocation,storeName,findType,serviceCertificateName);

         securityBehavior.UseAspNetProviders = useAspNetProviders;
         securityBehavior.ApplicationName = applicationName;
         securityBehavior.ImpersonateAll = impersonateAll;

         Description.Behaviors.Add(securityBehavior);
      }
      protected override void OnOpening()
      {
         foreach(IServiceBehavior behavior in m_ErrorHandlers)
         {
            Description.Behaviors.Add(behavior);
         }

         foreach(ServiceEndpoint endpoint in Description.Endpoints)
         {
            QueuedServiceHelper.VerifyQueue(endpoint); 
         } 
         base.OnOpening();
      }
      protected override void OnClosing()
      {
         using(m_AffinitySynchronizer)
         {}
         PurgeQueues();
         base.OnClosing();
      }
      [Conditional("DEBUG")]
      void PurgeQueues()
      {
         foreach(ServiceEndpoint endpoint in Description.Endpoints)
         {
            QueuedServiceHelper.PurgeQueue(endpoint);
         }
      }

      /// <summary>
      ///  Can only call before openning the host
      /// </summary>
      public void SetThreadAffinity(string threadName)
      {
         if(State == CommunicationState.Opened)
         {
            throw new InvalidOperationException("Host is already opened");
         }

         Debug.Assert(m_AffinitySynchronizer == null);//Can call only once

         m_AffinitySynchronizer = new AffinitySynchronizer(threadName);
         SynchronizationContext.SetSynchronizationContext(m_AffinitySynchronizer);
      }
      /// <summary>
      ///  Can only call before openning the host
      /// </summary>
      public void SetThreadAffinity()
      {
         SetThreadAffinity("Executing all endpoints of " + typeof(T));
      }           
      public ServiceThrottlingBehavior ThrottleBehavior
      {
         get
         {
            return Description.Behaviors.Find<ServiceThrottlingBehavior>();
         }
      }
      /// <summary>
      /// Can only call after openning the host
      /// </summary>
      public ServiceThrottle Throttle
      {
         get
         {
            if(State != CommunicationState.Opened)
            {
               throw new InvalidOperationException("Host is not opened");
            }

            ChannelDispatcher dispatcher = OperationContext.Current.Host.ChannelDispatchers[0] as ChannelDispatcher;
            return dispatcher.ServiceThrottle;
         }
      } 
      /// <summary>
      /// Can only call before openning the host
      /// </summary>
      public bool IncludeExceptionDetailInFaults
      {
         set
         {
            if(State == CommunicationState.Opened)
            {
               throw new InvalidOperationException("Host is already opened");
            }
            ServiceBehaviorAttribute debuggingBehavior = Description.Behaviors.Find<ServiceBehaviorAttribute>();
            debuggingBehavior.IncludeExceptionDetailInFaults = value;
         }
         get
         {
            ServiceBehaviorAttribute debuggingBehavior = Description.Behaviors.Find<ServiceBehaviorAttribute>();
            return debuggingBehavior.IncludeExceptionDetailInFaults;
         }
      }

       /// <summary>
      /// Can only call before openning the host
      /// </summary>
      public bool SecurityAuditEnabled
      {
         get
         {
            ServiceSecurityAuditBehavior securityAudit = Description.Behaviors.Find<ServiceSecurityAuditBehavior>();
            if(securityAudit != null)
            {
               return securityAudit.MessageAuthenticationAuditLevel == AuditLevel.SuccessOrFailure &&
                      securityAudit.ServiceAuthorizationAuditLevel == AuditLevel.SuccessOrFailure;
            }
            else
            {
               return false;
            }
         }
         set
         {
            if(State == CommunicationState.Opened)
            {
               throw new InvalidOperationException("Host is already opened");
            }
            ServiceSecurityAuditBehavior securityAudit = Description.Behaviors.Find<ServiceSecurityAuditBehavior>();
            if(securityAudit == null && value == true)
            {
               securityAudit = new ServiceSecurityAuditBehavior();
               securityAudit.MessageAuthenticationAuditLevel = AuditLevel.SuccessOrFailure;
               securityAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;
               Description.Behaviors.Add(securityAudit);
            }
         }
      }
      public ServiceHost() : base(typeof(T))
      {}
      public ServiceHost(params string[] baseAddresses) : base(typeof(T),Convert(baseAddresses))
      {}
      public ServiceHost(params Uri[] baseAddresses) : base(typeof(T),baseAddresses)
      {}
      public ServiceHost(T singleton,params string[] baseAddresses) : base(singleton,Convert(baseAddresses))
      {}
      public ServiceHost(T singleton) : base(singleton)
      {}
      public ServiceHost(T singleton,params Uri[] baseAddresses) : base(singleton,baseAddresses)
      {}
      public virtual T Singleton
      {
         get
         {
            if(SingletonInstance == null)
            {
               return default(T);
            }
            Debug.Assert(SingletonInstance is T);
            return (T)SingletonInstance;
         }
      }
      static Uri[] Convert(string[] baseAddresses)
      {
         Converter<string,Uri> convert =  delegate(string address)
                                          {
                                             return new Uri(address); 
                                          };
         return Array.ConvertAll(baseAddresses,convert); 
      }
   }
}





