// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;

namespace ServiceModelEx
{
   public static class ThreadAffinityHelper 
   {
      static Dictionary<Type,AffinitySynchronizer> m_Contexts = new Dictionary<Type,AffinitySynchronizer>();

      [MethodImpl(MethodImplOptions.Synchronized)]
      internal static void ApplyDispatchBehavior(Type type,string threadName,DispatchRuntime dispatch)
      {
         Debug.Assert(dispatch.SynchronizationContext == null);

         if(m_Contexts.ContainsKey(type) == false)
         {
            m_Contexts[type] = new AffinitySynchronizer(threadName);
         }
         dispatch.SynchronizationContext = m_Contexts[type];
      }
      [MethodImpl(MethodImplOptions.Synchronized)]
      public static void CloseThread(Type type)
      {
         if(m_Contexts.ContainsKey(type))
         {
            m_Contexts[type].Dispose();
            m_Contexts.Remove(type);
         }     
      }
   }
}