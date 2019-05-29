// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.ServiceModel;
using System.Runtime.Serialization;

//For transient subscribers
[ServiceContract]
public interface ISubscriptionService
{
   [OperationContract]
   void Subscribe(string eventOperation);

   [OperationContract]
   void Unsubscribe(string eventOperation);
}
//For persistent subscribers
[DataContract]
public struct PersistentSubscription
{
   string m_Address;

   string m_EventsContract;

   string m_EventOperation;

   [DataMember]
   public string Address
   {
      get
      {
         return m_Address;
      }
      set
      {
         m_Address = value;
      }
   }
   [DataMember]
   public string EventsContract
   {
      get
      {
         return m_EventsContract;
      }
      set
      {
         m_EventsContract = value;
      }
   }
   [DataMember]
   public string EventOperation
   {
      get
      {
         return m_EventOperation;
      }
      set
      {
         m_EventOperation = value;
      }
   }
}

[ServiceContract]
public interface IPersistentSubscriptionService
{
   [OperationContract]
   [TransactionFlow(TransactionFlowOption.Allowed)]
   void Subscribe(string address,string eventsContract,string eventOperation);

   [OperationContract]
   [TransactionFlow(TransactionFlowOption.Allowed)]
   void Unsubscribe(string address,string eventsContract,string eventOperation);

   [OperationContract]
   [TransactionFlow(TransactionFlowOption.Allowed)]
   PersistentSubscription[] GetAllSubscribers();

   [OperationContract]
   [TransactionFlow(TransactionFlowOption.Allowed)]
   PersistentSubscription[] GetSubscribersToContract(string eventsContract);

   [OperationContract]
   [TransactionFlow(TransactionFlowOption.Allowed)]
   string[] GetSubscribersToContractEventType(string eventsContract,string eventOperation);

   [OperationContract]
   [TransactionFlow(TransactionFlowOption.Allowed)]
   PersistentSubscription[] GetAllSubscribersFromAddress(string address);
}