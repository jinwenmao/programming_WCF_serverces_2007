// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Messaging;
using System.ServiceModel;
using System.Diagnostics;
using System.Threading;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Xml;

namespace ServiceModelEx
{
   public class ResponseScope<T> : IDisposable where T : class
   {
      OperationContextScope m_Scope;

      public readonly T Response;

      public ResponseScope() : this(new NetMsmqBinding())
      {}
      public ResponseScope(string bindingConfiguration) : this(new NetMsmqBinding(bindingConfiguration))
      {}

      public ResponseScope(NetMsmqBinding binding)
      {
         ResponseContext responseContext = ResponseContext.Current;
         Debug.Assert(responseContext != null);

         EndpointAddress address = new EndpointAddress(responseContext.ResponseAddress);

         ChannelFactory<T> factory = new ChannelFactory<T>(binding,address);
         QueuedServiceHelper.VerifyQueue(factory.Endpoint);

         Response = factory.CreateChannel();

         //Switching context now
         m_Scope = new OperationContextScope(Response as IContextChannel);
         ResponseContext.Current = responseContext;
      }
      public void Dispose()
      {
         IDisposable disposable = Response as IDisposable;
         disposable.Dispose();
         m_Scope.Dispose();
      }
   }
}





