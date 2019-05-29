// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.ServiceModel;
using System.Reflection;
using System.Diagnostics;

namespace ServiceModelEx
{
   public abstract class ResponseClientBase<T> : ClientBase<T> where T : class
   {
      public readonly string ResponseAddress;

      public ResponseClientBase(string responseAddress)
      {
         ResponseAddress = responseAddress;
         QueuedServiceHelper.VerifyQueue(Endpoint);
         Debug.Assert(Endpoint.Binding is NetMsmqBinding);
      }
      public ResponseClientBase(string responseAddress,string endpointName) : base(endpointName)
      {
         ResponseAddress = responseAddress;
         QueuedServiceHelper.VerifyQueue(Endpoint);
         Debug.Assert(Endpoint.Binding is NetMsmqBinding);
      }
      public ResponseClientBase(string responseAddress,string endpointName,string remoteAddress) : base(endpointName,remoteAddress)
      {
         ResponseAddress = responseAddress;
         QueuedServiceHelper.VerifyQueue(Endpoint);
         Debug.Assert(Endpoint.Binding is NetMsmqBinding);
      }
      public ResponseClientBase(string responseAddress,string endpointName,EndpointAddress remoteAddress) : base(endpointName,remoteAddress)
      {
         ResponseAddress = responseAddress;
         QueuedServiceHelper.VerifyQueue(Endpoint);
         Debug.Assert(Endpoint.Binding is NetMsmqBinding);
      }
      public ResponseClientBase(string responseAddress,NetMsmqBinding binding,EndpointAddress remoteAddress) : base(binding,remoteAddress)
      {
         ResponseAddress = responseAddress;
         QueuedServiceHelper.VerifyQueue(Endpoint);
      }
      protected string Enqueue(string operation,params object[] args)
      {
         using(OperationContextScope contextScope = new OperationContextScope(InnerChannel))
         {
            string methodId = GenerateMethodId();
            ResponseContext responseContext = new ResponseContext(ResponseAddress,methodId);
            ResponseContext.Current = responseContext;

            Type contract = typeof(T);
            MethodInfo methodInfo = contract.GetMethod(operation);//Does not support contract hierarchy or overloading 
            methodInfo.Invoke(Channel,args);

            return responseContext.MethodId;
         }
      }
      protected virtual string GenerateMethodId()
      {
         return Guid.NewGuid().ToString();
      }
   }
}
