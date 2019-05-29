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
   public class SecureChannelFactory<T> : ChannelFactory<T> 
   {
      public SecureChannelFactory()
      {}
      public SecureChannelFactory(Binding binding) : base(binding)
      {}
      public SecureChannelFactory(ServiceEndpoint endpoint) : base(endpoint)
      {}
      public SecureChannelFactory(string endpointName) : base(endpointName)
      {}
      public SecureChannelFactory(Binding binding,EndpointAddress remoteAddress) : base(binding,remoteAddress)
      {}
      public SecureChannelFactory(Binding binding,string remoteAddress) : base(binding,remoteAddress)
      {}
      public SecureChannelFactory(string endpointName,EndpointAddress remoteAddress) : base(endpointName,remoteAddress)
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
