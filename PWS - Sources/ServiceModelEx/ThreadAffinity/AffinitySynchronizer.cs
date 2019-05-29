// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Security.Permissions;

namespace ServiceModelEx
{
   [Serializable]
   internal class WorkItem
   {
      object m_State;
      SendOrPostCallback m_Method;
      ManualResetEvent m_AsyncWaitHandle;

      public WaitHandle AsyncWaitHandle
      {
         get
         {
            return m_AsyncWaitHandle;
         }
      }

      internal WorkItem(SendOrPostCallback method,object state)
      {
         m_Method = method;
         m_State = state;
         m_AsyncWaitHandle = new ManualResetEvent(false);
      }

      //This method is called on the worker thread to execute the method
      internal void CallBack()
      {
         m_Method(m_State);
         m_AsyncWaitHandle.Set();
      }
   }
   internal class WorkerThread
   {
      SynchronizationContext m_Context;
      public Thread m_ThreadObj;
      bool m_EndLoop;
      AutoResetEvent m_ItemAdded;
      Queue<WorkItem> m_WorkItemQueue;

      public int ManagedThreadId
      {
         get
         {
            return m_ThreadObj.ManagedThreadId;
         }
      }

      internal void QueueWorkItem(WorkItem workItem)
      {
         lock (m_WorkItemQueue)
         {
            m_WorkItemQueue.Enqueue(workItem);
            m_ItemAdded.Set();
         }
      }
      internal WorkerThread(string name,SynchronizationContext context)
      {
         m_Context = context;

         m_EndLoop = false;
         m_ThreadObj = null;
         m_ItemAdded = new AutoResetEvent(false);
         m_WorkItemQueue = new Queue<WorkItem>();

         m_ThreadObj = new Thread(Run);
         m_ThreadObj.IsBackground = true;
         m_ThreadObj.Name = name;
         m_ThreadObj.Start();
      }
      bool EndLoop
      {
         set
         {
            lock (this)
            {
               m_EndLoop = value;
            }
         }
         get
         {
            lock (this)
            {
               return m_EndLoop;
            }
         }
      }
      void Start()
      {
         Debug.Assert(m_ThreadObj != null);
         Debug.Assert(m_ThreadObj.IsAlive == false);
         m_ThreadObj.Start();
      }
      bool QueueEmpty
      {
         get
         {
            lock (m_WorkItemQueue)
            {
               if(m_WorkItemQueue.Count > 0)
               {
                  return false;
               }
               return true;
            }
         }
      }
      WorkItem GetNext()
      {
         if(QueueEmpty)
         {
            return null;
         }
         lock (m_WorkItemQueue)
         {
            return m_WorkItemQueue.Dequeue();
         }
      }
      void Run()
      {
         Debug.Assert(SynchronizationContext.Current == null);
         SynchronizationContext.SetSynchronizationContext(m_Context);

         while (EndLoop == false)
         {
            while (QueueEmpty == false)
            {
               if(EndLoop == true)
               {
                  return;
               }
               WorkItem workItem = GetNext();
               workItem.CallBack();
            }
            m_ItemAdded.WaitOne();
         }
      }
      public void Kill()
      {
         //Kill is called on client thread - must use cached thread object
         Debug.Assert(m_ThreadObj != null);
         if(m_ThreadObj.IsAlive == false)
         {
            return;
         }
         EndLoop = true;
         m_ItemAdded.Set();

         //Wait for thread to die
         m_ThreadObj.Join();

         if(m_ItemAdded != null)
         {
            m_ItemAdded.Close();
         }
      }
   }

   [SecurityPermission(SecurityAction.Demand,ControlThread = true)]
   public class AffinitySynchronizer : SynchronizationContext,IDisposable
   {
      WorkerThread m_WorkerThread;

      public AffinitySynchronizer() : this("AffinitySynchronizer Worker Thread")
      {}

      public AffinitySynchronizer(string threadName)
      {
         m_WorkerThread = new WorkerThread(threadName,this);
      }
      public void Dispose()
      {
         m_WorkerThread.Kill();
      }
      public override SynchronizationContext CreateCopy()
      {
         return this;
      }
      public override void Post(SendOrPostCallback method,object state)
      {
         WorkItem workItem = new WorkItem(method,state);
         m_WorkerThread.QueueWorkItem(workItem);
      }
      public override void Send(SendOrPostCallback method,object state)
      {
         //If already on the correct context, must invoke now to avoid deadlock
         if(SynchronizationContext.Current == this)
         {
            method(state);
            return;
         }
         WorkItem workItem = new WorkItem(method,state);
         m_WorkerThread.QueueWorkItem(workItem);
         workItem.AsyncWaitHandle.WaitOne();
      }
   }
}