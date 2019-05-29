// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net


using System.Windows.Forms;
using System.ServiceModel.Description;

namespace ServiceModelEx
{
   class ServiceNode : MexNode 
   {
      public ServiceNode(ExplorerForm form,string text,int imageIndex,int selectedImageIndex) : base(form,text,imageIndex,selectedImageIndex)
      {}
      public override void DisplayControl()
      {
         m_Form.DisplayServiceControl();
      }
   }
}
