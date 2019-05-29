// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.ServiceModel;

namespace ServiceModelEx
{
   public class InstanceContext<T> 
   {
      InstanceContext m_InstanceContext;
      public InstanceContext(T implementation)
      {
         m_InstanceContext = new InstanceContext(implementation);
      }
      public InstanceContext Context
      {
         get
         {
            return m_InstanceContext;
         }
      }
      public void ReleaseServiceInstance()
      {
         m_InstanceContext.ReleaseServiceInstance();
      }
      //public object GetServiceInstance();
      public T ServiceInstance
      {
         get
         {
            return (T)m_InstanceContext.GetServiceInstance();
         }
      }
   }
}
