// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.ServiceModel;
using System.Windows.Forms;
using System.Diagnostics;

namespace ServiceModelEx
{
   public static class DebugHelper
   {
      public const bool IncludeExceptionDetailInFaults = 
      #if DEBUG
            true;
      #else
            false;
      #endif
   }
}