// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.ServiceModel;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;

namespace ServiceModelEx
{
   public static class WsDualProxyHelper
   {
      public static void SetClientBaseAddress<T>(DuplexClientBase<T> proxy,int port) where T : class
      {
         if(proxy.State == CommunicationState.Opened)
         {
            throw new InvalidOperationException("Proxy is already opened");
         }
         WSDualHttpBinding binding = proxy.Endpoint.Binding as WSDualHttpBinding;
         Debug.Assert(binding != null);

         binding.ClientBaseAddress = new Uri("http://localhost:" + port +"/");
      }
      public static void SetClientBaseAddress<T>(DuplexClientBase<T> proxy) where T : class
      {
         lock(typeof(WsDualProxyHelper))
         {
            int portNumber = FindPort();
            SetClientBaseAddress(proxy,portNumber);
            proxy.Open();
         }
      }
      internal static int FindPort()
      {
         IPEndPoint endPoint = new IPEndPoint(IPAddress.Any,0);
         using(Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp))
         {
            socket.Bind(endPoint);
            IPEndPoint local = (IPEndPoint)socket.LocalEndPoint;
            return local.Port;
         }
      }
   }
}

