// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Messaging;
using System.ServiceModel;
using System.Diagnostics;
using System.Threading;
using System.Runtime.Serialization;

namespace ServiceModelEx
{
   [DataContract]
   public class ResponseContext
   {
      [DataMember]
      public readonly string ResponseAddress;

      [DataMember]
      public readonly string FaultAddress;

      [DataMember]
      public readonly string MethodId;

      public ResponseContext(string responseAddress,string methodId) : this(responseAddress,methodId,null)
      {}
      public ResponseContext(ResponseContext responseContext) : this(responseContext.ResponseAddress,responseContext.MethodId,responseContext.FaultAddress)
      {}
      public ResponseContext(string responseAddress) : this(responseAddress,Guid.NewGuid().ToString())
      {}
      public ResponseContext(string responseAddress,string methodId,string faultAddress)
      {
         ResponseAddress = responseAddress;
         MethodId = methodId;
         FaultAddress = faultAddress;
      }
      public static ResponseContext Current
      {
         get
         {
            OperationContext context = OperationContext.Current;
            if(context == null)
            {
               return null;
            }
            try
            {
               return context.IncomingMessageHeaders.GetHeader<ResponseContext>("ResponseContext","ServiceModelEx");
            }
            catch(Exception exception)
            {
               Debug.Assert(exception is MessageHeaderException && exception.Message == "There is not a header with name ResponseContext and namespace ServiceModelEx in the message.");
               return null;
            }
         }
         set
         {
            OperationContext context = OperationContext.Current;
            Debug.Assert(context != null);
            //Having multiple ResponseContext headers is an error
            bool headerExists = false;
            try
            {
               context.OutgoingMessageHeaders.GetHeader<ResponseContext>("ResponseContext","ServiceModelEx");
               headerExists = true;
            }
            catch(MessageHeaderException exception)
            {
               Debug.Assert(exception.Message == "There is not a header with name ResponseContext and namespace ServiceModelEx in the message.");
            }
            if(headerExists)
            {
               throw new InvalidOperationException("A header with name ResponseContext and namespace ServiceModelEx already exists in the message.");
            }
            MessageHeader<ResponseContext> responseHeader = new MessageHeader<ResponseContext>(value);
            context.OutgoingMessageHeaders.Add(responseHeader.GetUntypedHeader("ResponseContext","ServiceModelEx"));
         }
      }
   }
}





