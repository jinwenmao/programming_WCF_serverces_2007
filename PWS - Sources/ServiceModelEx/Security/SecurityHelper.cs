// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.Net;
using System.Security.Principal;
using System.ServiceModel.Description;
using System.ServiceModel.Security;

namespace ServiceModelEx
{
   public static class SecurityHelper
   {
      public static void ImpersonateAll(ServiceHostBase host)
      {
         host.Authorization.ImpersonateCallerForAllOperations = true;
         ServiceDescription description = host.Description;
         ImpersonateAll(description);
      }
      public static void ImpersonateAll(ServiceDescription description)
      {
         foreach(ServiceEndpoint endpoint in description.Endpoints)
         {
            foreach(OperationDescription operation in endpoint.Contract.Operations)
            {
               foreach(IOperationBehavior behavior in operation.Behaviors)
               {
                  if(behavior is OperationBehaviorAttribute)
                  {
                     OperationBehaviorAttribute attribute = behavior as OperationBehaviorAttribute;
                     if(attribute.Impersonation == ImpersonationOption.NotAllowed)
                     {
                        Trace.WriteLine("Overriding impersonation setting of " + endpoint.Contract.Name + "." + operation.Name);
                     }
                     attribute.Impersonation = ImpersonationOption.Required; 
                     break;
                  }
               }
            }
         }
      }

      /// <summary>
      /// Can only call before using the proxy for the first time
      /// </summary>
      public static void UnsecuredProxy<T>(ClientBase<T> proxy) where T : class
      {
         if(proxy.State == CommunicationState.Opened)
         {
            throw new InvalidOperationException("Proxy channel is already opened");
         }
         Collection<ServiceEndpoint> endpoints = new Collection<ServiceEndpoint>();
         endpoints.Add(proxy.Endpoint);

         SecurityBehavior.ConfigureNone(endpoints);
      }
      /// <summary>
      /// Can only call before using the proxy for the first time
      /// </summary>
      public static void AnonymousProxy<T>(ClientBase<T> proxy) where T : class
      {
         if(proxy.State == CommunicationState.Opened)
         {
            throw new InvalidOperationException("Proxy channel is already opened");
         }
         Collection<ServiceEndpoint> endpoints = new Collection<ServiceEndpoint>();
         endpoints.Add(proxy.Endpoint);
         proxy.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerTrust;

         SecurityBehavior.ConfigureAnonymous(endpoints);
      }

      /// <summary>
      /// Can only call before using the proxy for the first time
      /// </summary>
      public static void SecureProxy<T>(ClientBase<T> proxy,string userName,string password) where T : class
      {
         if(proxy.State == CommunicationState.Opened)
         {
            throw new InvalidOperationException("Proxy channel is already opened");
         }
         Collection<ServiceEndpoint> endpoints = new Collection<ServiceEndpoint>();
         endpoints.Add(proxy.Endpoint);

         SecurityBehavior.ConfigureInternet(endpoints,true);//True even for Windows

         proxy.ClientCredentials.UserName.UserName = userName;
         proxy.ClientCredentials.UserName.Password = password;
         proxy.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerTrust;
      }
            /// <summary>
      /// Can only call before using the proxy for the first time
      /// </summary>
      public static void SecureProxy<T>(ClientBase<T> proxy,string domain,string userName,string password) where T : class
      {
         SecureProxy<T>(proxy,domain,userName,password,TokenImpersonationLevel.Identification);
      }
      /// <summary>
      /// Can only call before using the proxy for the first time
      /// </summary>
      public static void SecureProxy<T>(ClientBase<T> proxy,string domain,string userName,string password,TokenImpersonationLevel impersonationLevel) where T : class
      {
         if(proxy.State == CommunicationState.Opened)
         {
            throw new InvalidOperationException("Proxy channel is already opened");
         }
         Collection<ServiceEndpoint> endpoints = new Collection<ServiceEndpoint>();
         endpoints.Add(proxy.Endpoint);

         SecurityBehavior.ConfigureIntranet(endpoints);

         NetworkCredential credentials = new NetworkCredential();
         credentials.Domain   = domain;
         credentials.UserName = userName;
         credentials.Password = password;

         proxy.ClientCredentials.Windows.ClientCredential = credentials;
         proxy.ClientCredentials.Windows.AllowedImpersonationLevel = impersonationLevel;
      }
      /// <summary>
      /// Can only call before using the proxy for the first time
      /// </summary>
      /// <param name="clientCertificateName">Certificate is looked up by name from LocalMachine/My store</param>
      public static void SecureProxy<T>(ClientBase<T> proxy,string clientCertificateName) where T : class 
      {
         proxy.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerTrust;
         SecureProxy<T>(proxy,StoreLocation.LocalMachine,StoreName.My,X509FindType.FindBySubjectName,clientCertificateName);
      }

      /// <summary>
      /// Can only call before using the proxy for the first time
      /// </summary>
      public static void SecureProxy<T>(ClientBase<T> proxy,StoreLocation storeLocation,StoreName storeName,X509FindType findType,string clientCertificateName) where T : class
      {
         if(proxy.State == CommunicationState.Opened)
         {
            throw new InvalidOperationException("Proxy channel is already opened");
         }
         SetCertificate(proxy,storeLocation,storeName,findType,clientCertificateName);
         Collection<ServiceEndpoint> endpoints = new Collection<ServiceEndpoint>();
         endpoints.Add(proxy.Endpoint);

         proxy.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerTrust;

         SecurityBehavior.ConfigureBusinessToBusiness(endpoints);
      }
      /// <summary>
      /// Can only call before using the proxy for the first time
      /// </summary>
      /// <param name="clientCertificateName">Certificate is looked up by name from LocalMachine/My store</param>
      public static void SetCertificate<T>(ClientBase<T> proxy,string clientCertificateName) where T : class
      {
         SetCertificate<T>(proxy,StoreLocation.LocalMachine,StoreName.My,X509FindType.FindBySubjectName,clientCertificateName);
      }

      /// <summary>
      /// Can only call before using the proxy for the first time
      /// </summary>
      public static void SetCertificate<T>(ClientBase<T> proxy,StoreLocation storeLocation,StoreName storeName,X509FindType findType,string clientCertificateName) where T : class
      {
         if(proxy.State == CommunicationState.Opened)
         {
            throw new InvalidOperationException("Proxy channel is already opened");
         }
         if(String.IsNullOrEmpty(clientCertificateName) == false)
         {
            proxy.ClientCredentials.ClientCertificate.SetCertificate(storeLocation,storeName,findType,clientCertificateName);
         }
         proxy.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.PeerTrust;
      }
   }
}