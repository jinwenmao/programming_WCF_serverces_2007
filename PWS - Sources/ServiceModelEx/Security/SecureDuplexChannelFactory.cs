// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Collections.ObjectModel;
using System.Security.Principal;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace ServiceModelEx
{
   public class SecureDuplexChannelFactory<T,C> : DuplexChannelFactory<T,C> where T : class
   {
      public SecureDuplexChannelFactory(C callback) : base(callback)
      {}
      public SecureDuplexChannelFactory(InstanceContext<C> context,Binding binding) : base(context,binding)
      {}
      public SecureDuplexChannelFactory(InstanceContext<C> context,ServiceEndpoint endpoint) : base(context,endpoint)
      {}

      public SecureDuplexChannelFactory(InstanceContext<C> context,string endpointName) : base(context,endpointName)
      {}
      public SecureDuplexChannelFactory(C callback,Binding binding) : base(callback,binding)
      {}
      public SecureDuplexChannelFactory(C callback,ServiceEndpoint endpoint): base(callback,endpoint)
      {}
      public SecureDuplexChannelFactory(C callback,string endpointName): base(callback,endpointName)
      {}
      public SecureDuplexChannelFactory(InstanceContext<C> context,Binding binding,EndpointAddress endpointAddress): base(context,binding,endpointAddress)
      {}
      public SecureDuplexChannelFactory(InstanceContext<C> context,string endpointName,EndpointAddress endpointAddress): base(context,endpointName,endpointAddress)
      {}
      public SecureDuplexChannelFactory(C callback,Binding binding,EndpointAddress endpointAddress): base(callback,binding,endpointAddress)
      {}
      public SecureDuplexChannelFactory(C callback,string endpointName,EndpointAddress endpointAddress): base(callback,endpointName,endpointAddress)
      {}

      public void SetSecurityMode(ServiceSecurity mode)
      {
         switch(mode)
         {
            case ServiceSecurity.None:
            {
               if(State == CommunicationState.Opened)
               {
                  throw new InvalidOperationException("Proxy channel is already opened");
               }
               Collection<ServiceEndpoint> endpoints = new Collection<ServiceEndpoint>();
               endpoints.Add(Endpoint);

               SecurityBehavior.ConfigureNone(endpoints);

               break;
            }
            case ServiceSecurity.Anonymous:
            {
               if(State == CommunicationState.Opened)
               {
                  throw new InvalidOperationException("Proxy channel is already opened");
               }
               Collection<ServiceEndpoint> endpoints = new Collection<ServiceEndpoint>();
               endpoints.Add(Endpoint);

               SecurityBehavior.ConfigureAnonymous(endpoints);
               break;
            }
            default:
            {
               throw new InvalidOperationException(mode + " is unsupported with this constructor");
            }
         }
      }
      public void SetCredentials(string userName,string password) 
      {
         if(State == CommunicationState.Opened)
         {
            throw new InvalidOperationException("Proxy channel is already opened");
         }
         Collection<ServiceEndpoint> endpoints = new Collection<ServiceEndpoint>();
         endpoints.Add(Endpoint);

         SecurityBehavior.ConfigureInternet(endpoints,true);//True even for Windows

         Credentials.UserName.UserName = userName;
         Credentials.UserName.Password = password;
      }
      public void SetCredentials(string domain,string userName,string password,TokenImpersonationLevel impersonationLevel)
      {
         if(State == CommunicationState.Opened)
         {
            throw new InvalidOperationException("Proxy channel is already opened");
         }
         Collection<ServiceEndpoint> endpoints = new Collection<ServiceEndpoint>();
         endpoints.Add(Endpoint);

         SecurityBehavior.ConfigureIntranet(endpoints);

         NetworkCredential credentials = new NetworkCredential();
         credentials.Domain   = domain;
         credentials.UserName = userName;
         credentials.Password = password;

         Credentials.Windows.ClientCredential = credentials;
         Credentials.Windows.AllowedImpersonationLevel = impersonationLevel;
      }
      public void SetCredentials(string domain,string userName,string password) 
      {
         SetCredentials(domain,userName,password,TokenImpersonationLevel.Identification);
      }
      public void SetCredentials(string clientCertificateName) 
      {
         SetCredentials(StoreLocation.LocalMachine,StoreName.My,X509FindType.FindBySubjectName,clientCertificateName);
      }
      public void SetCredentials(StoreLocation storeLocation,StoreName storeName,X509FindType findType,string clientCertificateName) 
      {
         if(State == CommunicationState.Opened)
         {
            throw new InvalidOperationException("Proxy channel is already opened");
         }
         Credentials.ClientCertificate.SetCertificate(storeLocation,storeName,findType,clientCertificateName);
         
         Collection<ServiceEndpoint> endpoints = new Collection<ServiceEndpoint>();
         endpoints.Add(Endpoint);

         SecurityBehavior.ConfigureBusinessToBusiness(endpoints);
      }   
   }
}
