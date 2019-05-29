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
using System.ServiceModel.Security;
using System.ServiceModel.Channels;

namespace ServiceModelEx
{
   public abstract class SecureClientBase<T> : ClientBase<T> where T : class
   {
      protected SecureClientBase()
      {}
      //These constructors use the default endpoint
      /// <summary>
      /// 
      /// </summary>
      //<param name="mode">Allowed values are ServiceSecurity.None and ServiceSecurity.Anonymous</param>
      protected SecureClientBase(ServiceSecurity mode)
      {
         switch(mode)
         {
            case ServiceSecurity.None:
            {
               SecurityHelper.UnsecuredProxy(this);
               break;
            }
            case ServiceSecurity.Anonymous:
            {
               SecurityHelper.AnonymousProxy(this);
               break;
            }
            default:
            {
               throw new InvalidOperationException(mode + " is unsupported with this constructor");
            }
         }
      }
      protected SecureClientBase(string userName,string password) 
      {
         SecurityHelper.SecureProxy(this,userName,password);
      }
      protected SecureClientBase(string domain,string userName,string password,TokenImpersonationLevel impersonationLevel)
      {
         SecurityHelper.SecureProxy(this,domain,userName,password,impersonationLevel);
      }
      protected SecureClientBase(string domain,string userName,string password) : this(domain,userName,password,TokenImpersonationLevel.Identification)
      {}
      protected SecureClientBase(string clientCertificateName) 
      {
         SecurityHelper.SecureProxy(this,clientCertificateName);
      }
      protected SecureClientBase(StoreLocation storeLocation,StoreName storeName,X509FindType findType,string clientCertificateName) 
      {
         SecurityHelper.SecureProxy(this,storeLocation,storeName,findType,clientCertificateName);
      }


      //These constructors use configured endpoint
      /// <summary>
      /// 
      /// </summary>
      //<param name="mode">Allowed values are ServiceSecurity.None and ServiceSecurity.Anonymous</param>
      protected SecureClientBase(ServiceSecurity mode,string endpointName) : base(endpointName)
      {
         switch(mode)
         {
            case ServiceSecurity.None:
            {
               SecurityHelper.UnsecuredProxy(this);
               break;
            }
            case ServiceSecurity.Anonymous:
            {
               SecurityHelper.AnonymousProxy(this);
               break;
            }
            default:
            {
               throw new InvalidOperationException(mode + " is unsupported with this constructor");
            }
         }
      }
      protected SecureClientBase(UserNamePasswordClientCredential credentials,string endpointName) : base(endpointName)
      {
         SecurityHelper.SecureProxy(this,credentials.UserName,credentials.Password);
      }
      protected SecureClientBase(string domain,string userName,string password,TokenImpersonationLevel impersonationLevel,string endpointName) : base(endpointName)
      {
         SecurityHelper.SecureProxy(this,domain,userName,password,impersonationLevel);
      }
      protected SecureClientBase(string domain,string userName,string password,string endpointName) : this(domain,userName,password,TokenImpersonationLevel.Identification,endpointName)
      {}
      protected SecureClientBase(string clientCertificateName,bool overrideConfig,string endpointName) : base(endpointName)
      {
         if(overrideConfig)
         {
            SecurityHelper.SecureProxy(this,clientCertificateName);
         }
      }
      protected SecureClientBase(StoreLocation storeLocation,StoreName storeName,X509FindType findType,string clientCertificateName,string endpointName) : base(endpointName)
      {
         SecurityHelper.SecureProxy(this,storeLocation,storeName,findType,clientCertificateName);
      }

   
      //These constructors use programatic address and binding

      /// <summary>
      /// 
      /// </summary>
      //<param name="mode">Allowed values are ServiceSecurity.None and ServiceSecurity.Anonymous</param>
      protected SecureClientBase(ServiceSecurity mode,Binding binding,EndpointAddress remoteAddress) : base(binding,remoteAddress)
      {
         switch(mode)
         {
            case ServiceSecurity.None:
               {
                  SecurityHelper.UnsecuredProxy(this);
                  break;
               }
            case ServiceSecurity.Anonymous:
               {
                  SecurityHelper.AnonymousProxy(this);
                  break;
               }
            default:
               {
                  throw new InvalidOperationException(mode + " is unsupported with this constructor");
               }
         }
      }
      protected SecureClientBase(UserNamePasswordClientCredential credentials,Binding binding,EndpointAddress remoteAddress) : base(binding,remoteAddress)
      {
         SecurityHelper.SecureProxy(this,credentials.UserName,credentials.Password);
      }
      protected SecureClientBase(string domain,string userName,string password,TokenImpersonationLevel impersonationLevel,Binding binding,EndpointAddress remoteAddress) : base(binding,remoteAddress)
      {
         SecurityHelper.SecureProxy(this,domain,userName,password,impersonationLevel);
      }
      protected SecureClientBase(string domain,string userName,string password,Binding binding,EndpointAddress remoteAddress) : this(domain,userName,password,TokenImpersonationLevel.Identification,binding,remoteAddress)
      {}
      protected SecureClientBase(string clientCertificateName,bool overrideConfig,Binding binding,EndpointAddress remoteAddress) : base(binding,remoteAddress)
      {
         if(overrideConfig)
         {
            SecurityHelper.SecureProxy(this,clientCertificateName);
         }
      }
      protected SecureClientBase(StoreLocation storeLocation,StoreName storeName,X509FindType findType,string clientCertificateName,Binding binding,EndpointAddress remoteAddress) : base(binding,remoteAddress)
      {
         SecurityHelper.SecureProxy(this,storeLocation,storeName,findType,clientCertificateName);
      }
   }
}