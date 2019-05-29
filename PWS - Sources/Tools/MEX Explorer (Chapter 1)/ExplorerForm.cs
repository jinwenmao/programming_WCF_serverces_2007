// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Windows.Forms;
using ServiceModelEx.Properties;
using System.ServiceModel.Description;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Threading;
using WinFormsEx;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.ServiceModel.Channels;

namespace ServiceModelEx
{
   partial class ExplorerForm : Form
   {
      const int MessageMultiplier = 5;

      const int AddressIndex = 0;
      const int BindingIndex = 1;
      const int ContractIndex = 2;
      const int EndpointIndex = 3;
      const int OperationIndex = 4;
      const int ServiceIndex = 5;
      const int ServiceError = 6;
      const int AddressUnspecified = 7;
      
      TreeNode m_Root;
      NodeViewControl m_CurrentViewControl;

      public ExplorerForm()
      {
         InitializeComponent();
         m_MexTree.ImageList = new ImageList();
         m_MexTree.ImageList.Images.Add(Resources.Address);
         m_MexTree.ImageList.Images.Add(Resources.Binding);
         m_MexTree.ImageList.Images.Add(Resources.Contract);
         m_MexTree.ImageList.Images.Add(Resources.PieEndpoint);
         m_MexTree.ImageList.Images.Add(Resources.Operation);
         m_MexTree.ImageList.Images.Add(Resources.Service);
         m_MexTree.ImageList.Images.Add(Resources.ServiceError);
         m_MexTree.ImageList.Images.Add(Resources.AddressUnspecified);

         m_CurrentViewControl = m_BlankViewControl;

         m_Root = new ServiceNode(this,"Unspecified Base Address",AddressUnspecified,AddressUnspecified);
         m_MexTree.Nodes.Add(m_Root);
      }

      void OnExplore(object sender,EventArgs e)
      {
         m_ExploreButton.Enabled = false;

         string mexAddress = m_MexAddressTextBox.Text;
         if(String.IsNullOrEmpty(mexAddress))
         {
            Debug.Assert(false,"Empty address");
         }
         m_MexTree.Nodes.Clear();
         DisplayBlankControl();

         SplashScreen splash = new SplashScreen(Resources.Progress);         
         try
         {
            Uri address = new Uri(mexAddress);
            ServiceEndpointCollection endpoints = null;

            if(address.Scheme == "http")
            {
               HttpTransportBindingElement httpBindingElement = new HttpTransportBindingElement();
               httpBindingElement.MaxReceivedMessageSize *= MessageMultiplier;

               //Try the HTTP MEX Endpoint
               try
               {
                  endpoints = GetEndpoints(httpBindingElement);
               }
               catch
               {}
               //Try over HTTP-GET
               if(endpoints == null)
               {
                  string httpGetAddress = mexAddress;
                  if(mexAddress.EndsWith("?wsdl") == false)
                  {
                     httpGetAddress += "?wsdl";
                  }
                  CustomBinding binding = new CustomBinding(httpBindingElement);
                  MetadataExchangeClient MEXClient = new MetadataExchangeClient(binding);
                  MetadataSet metadata = MEXClient.GetMetadata(new Uri(httpGetAddress),MetadataExchangeClientMode.HttpGet);
                  MetadataImporter importer = new WsdlImporter(metadata);
                  endpoints = importer.ImportAllEndpoints();
               }
            }
            if(address.Scheme == "https")
            {
               HttpsTransportBindingElement httpsBindingElement = new HttpsTransportBindingElement();
               httpsBindingElement.MaxReceivedMessageSize *= MessageMultiplier;

               //Try the HTTPS MEX Endpoint
               try
               {
                  endpoints = GetEndpoints(httpsBindingElement);
               }
               catch
               {
               }
               //Try over HTTP-GET
               if(endpoints == null)
               {
                  string httpsGetAddress = mexAddress;
                  if(mexAddress.EndsWith("?wsdl") == false)
                  {
                     httpsGetAddress += "?wsdl";
                  }
                  CustomBinding binding = new CustomBinding(httpsBindingElement);
                  MetadataExchangeClient MEXClient = new MetadataExchangeClient(binding);
                  MetadataSet metadata = MEXClient.GetMetadata(new Uri(httpsGetAddress),MetadataExchangeClientMode.HttpGet);
                  MetadataImporter importer = new WsdlImporter(metadata);
                  endpoints = importer.ImportAllEndpoints();
               }
            }
            if(address.Scheme == "net.tcp")
            {
               TcpTransportBindingElement tcpBindingElement = new TcpTransportBindingElement();
               tcpBindingElement.MaxReceivedMessageSize *= MessageMultiplier;
               endpoints = GetEndpoints(tcpBindingElement);
            }
            if(address.Scheme == "net.pipe")
            {
               NamedPipeTransportBindingElement ipcBindingElement = new NamedPipeTransportBindingElement();
               ipcBindingElement.MaxReceivedMessageSize *= MessageMultiplier;
               endpoints = GetEndpoints(ipcBindingElement);
            }
            ProcessMetaData(endpoints);
         }
         catch
         {
            m_MexTree.Nodes.Clear();

            m_Root = new ServiceNode(this,"Invalid Base Address",ServiceError,ServiceError);
            m_MexTree.Nodes.Add(m_Root);
            return;
         }
         finally
         {
            splash.Close();
            m_ExploreButton.Enabled = true;
         }
      }
      ServiceEndpointCollection GetEndpoints(BindingElement bindingElement)
      {
         CustomBinding binding = new CustomBinding(bindingElement);

         MetadataExchangeClient MEXClient = new MetadataExchangeClient(binding);
         MetadataSet metadata = MEXClient.GetMetadata(new EndpointAddress(m_MexAddressTextBox.Text));
         MetadataImporter importer = new WsdlImporter(metadata);
         return importer.ImportAllEndpoints();
      }
      void ProcessMetaData(ServiceEndpointCollection endpoints)
      {
         if(endpoints.Count == 0)
         {
            m_Root = new ServiceNode(this,"Service has no endpoints",ServiceIndex,ServiceIndex);
            m_MexTree.Nodes.Add(m_Root);
            return;
         }
         else
         {
            m_Root = new ServiceNode(this,"Exploring...",ServiceIndex,ServiceIndex);
            m_MexTree.Nodes.Add(m_Root);
         }
         int index = 1;
         foreach(ServiceEndpoint endpoint in endpoints)
         {
            AddEndPoint(endpoint,"Endpoint"+index);
            index++;
         }
         DisplayServiceControl();
      }
      void AddContract(TreeNode endpointNode,ContractDescription contract)
      {
         TreeNode contractNode = new ContractNode(this,contract,contract.Name,ContractIndex,ContractIndex);
         endpointNode.Nodes.Add(contractNode);

         foreach(OperationDescription operation in contract.Operations)
         {
            AddOperation(contractNode,operation);
         }
      }
      void AddCallbackContract(TreeNode endpointNode,ContractDescription contract)
      {
      }
      internal static string ExtractTypeName(Type type)
      {
         string typeName = type.ToString();
         string typeNamespace = type.Namespace;

         if(typeNamespace == null)
         {
            return typeName;
         }

         return typeName.Substring(typeNamespace.Length+1,typeName.Length-typeNamespace.Length-1);
      }
      void AddBinding(TreeNode endpointNode,System.ServiceModel.Channels.Binding binding)
      {
         string bindingName = ExtractTypeName(binding.GetType());

         TreeNode bindingNode = new BindingNode(this,binding,bindingName,BindingIndex,BindingIndex);
         endpointNode.Nodes.Add(bindingNode);
      }
      void AddAddress(TreeNode endpointNode,EndpointAddress address)
      {
         TreeNode addressNode = new AddressNode(this,address,address.Uri.AbsoluteUri,AddressIndex,AddressIndex);
         endpointNode.Nodes.Add(addressNode);
      }
      void AddOperation(TreeNode contractNode,OperationDescription operation)
      {
         TreeNode operationNode = new OperationNode(this,operation,operation.Name,OperationIndex,OperationIndex);
         contractNode.Nodes.Add(operationNode);
      }
      void AddEndPoint(ServiceEndpoint endpoint,string name)
      {
         TreeNode endpointNode = new EndpointNode(this,endpoint,name,EndpointIndex,EndpointIndex);

         AddAddress(endpointNode,endpoint.Address);
         AddBinding(endpointNode,endpoint.Binding);
         AddContract(endpointNode,endpoint.Contract);

         m_Root.Nodes.Add(endpointNode);
      }

      void OnItemSelected(object sender,TreeViewEventArgs treeEventArgs)
      {
         MexNode node = treeEventArgs.Node as MexNode;
         node.DisplayControl();
      }
      void DiplayControl(NodeViewControl control)
      {
         m_CurrentViewControl.Visible = false;
         control.Visible = true;
         m_CurrentViewControl = control;
      }
      internal void DisplayEndpointConrol(ServiceEndpoint endpoint)
      {
         m_EndpointViewControl.Refresh(endpoint);
         DiplayControl(m_EndpointViewControl);
      }
      internal void DisplayBlankControl()
      {
         DiplayControl(m_BlankViewControl);
      }
      internal void DisplayServiceControl()
      {
         if(m_Root.Text == "Unspecified Base Address")
         {
            DisplayBlankControl();
            return;
         }
         m_ServiceViewControl.Refresh(m_MexAddressTextBox.Text);
         DiplayControl(m_ServiceViewControl);
         string serviceName = "";
         while(serviceName == "")
         {
            Application.DoEvents();
            serviceName = m_ServiceViewControl.ExtractServiceName();
         }
         m_Root.Text = serviceName;
         if(m_Root.Text == "Unknown")
         {
            DisplayBlankControl();
         }
      }

      internal void DisplayBindingConrol(System.ServiceModel.Channels.Binding binding)
      {
         m_BindingViewControl.Refresh(binding);
         DiplayControl(m_BindingViewControl);
      }

      internal void DisplayOperationConrol(OperationDescription operation)
      {
         m_OperationViewControl.Refresh(operation);
         DiplayControl(m_OperationViewControl);
      }

      internal void DisplayContractConrol(ContractDescription contract)
      {
         m_ContractViewControl.Refresh(contract);
         DiplayControl(m_ContractViewControl);
      }

      internal void DisplayAddressConrol(EndpointAddress address)
      {
         m_AddressViewControl.Refresh(address);
         DiplayControl(m_AddressViewControl);
      }

      void OnGenerateProxy(object sender,EventArgs e)
      {
         string currentDirectoty = Directory.GetCurrentDirectory();
         string arguments =  m_MexAddressTextBox.Text + @"  /Out:Proxy.cs /noconfig";
         try
         {
            Process.Start(@"C:\Program Files\Microsoft SDKs\Windows\v6.0\Bin\SvcUtil.exe",arguments);
            BringToFront();
         }
         catch
         {
            MessageBox.Show("Cannot Find SvcUtil.exe","IDesign Metadata Explorer",MessageBoxButtons.OK,MessageBoxIcon.Error);
         }
      }

      void OnAbout(object sender,EventArgs e)
      {
         AboutBox about = new AboutBox();
         about.ShowDialog();
      }
   }
}