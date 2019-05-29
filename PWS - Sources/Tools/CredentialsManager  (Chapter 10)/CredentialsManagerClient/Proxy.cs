// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System.Runtime.Serialization;
using System.ServiceModel;
using System.Net;
using System.Web.Security;
using System.ServiceModel.Channels;


[DataContract]
public partial class UserInfo : IExtensibleDataObject
{
   ExtensionDataObject m_ExtensionData;
   string m_Email;
   bool m_IsApproved;
   bool m_IsLockedOut;
   string m_Name;
   string m_PasswordQuestion;

   public ExtensionDataObject ExtensionData
   {
      get
      {
         return m_ExtensionData;
      }
      set
      {
         m_ExtensionData = value;
      }
   }

   [DataMember]
   public string Email
   {
      get
      {
         return m_Email;
      }
      set
      {
         m_Email = value;
      }
   }

   [DataMember]
   public bool IsApproved
   {
      get
      {
         return m_IsApproved;
      }
      set
      {
         m_IsApproved = value;
      }
   }

   [DataMember]
   public bool IsLockedOut
   {
      get
      {
         return m_IsLockedOut;
      }
      set
      {
         m_IsLockedOut = value;
      }
   }

   [DataMember]
   public string Name
   {
      get
      {
         return m_Name;
      }
      set
      {
         m_Name = value;
      }
   }

   [DataMember]
   public string PasswordQuestion
   {
      get
      {
         return m_PasswordQuestion;
      }
      set
      {
         m_PasswordQuestion = value;
      }
   }
}


[ServiceContract]
public interface IApplicationManager
{
   [OperationContract]
   void DeleteAllApplications();

   [OperationContract]
   void DeleteApplication(string application);

   [OperationContract]
   string[] GetApplications();
}

public partial class ApplicationManagerProxy : ClientBase<IApplicationManager>,IApplicationManager
{
   public ApplicationManagerProxy(string address)
   {
      Endpoint.Address = new EndpointAddress(address);
   }

   public ApplicationManagerProxy(string endpointConfigurationName,string remoteAddress) : base(endpointConfigurationName,remoteAddress)
   {}

   public ApplicationManagerProxy(string endpointConfigurationName,EndpointAddress remoteAddress) : base(endpointConfigurationName,remoteAddress)
   {}

   public ApplicationManagerProxy(Binding binding,EndpointAddress remoteAddress) : base(binding,remoteAddress)
   {}

   public void DeleteAllApplications()
   {
      Channel.DeleteAllApplications();
   }

   public void DeleteApplication(string application)
   {
      Channel.DeleteApplication(application);
   }

   public string[] GetApplications()
   {
      return Channel.GetApplications();
   }
}


[ServiceContract]
public interface IMembershipManager
{
   [OperationContract]
   MembershipCreateStatus CreateUser(string application,string userName,string password,string email,string passwordQuestion,string passwordAnswer,bool isApproved);

   [OperationContract]
   void DeleteAllUsers(string application,bool deleteAllRelatedData);

   [OperationContract]
   bool DeleteUser(string application,string userName,bool deleteAllRelatedData);

   [OperationContract]
   string[] GetAllUsers(string application);

   [OperationContract]
   int GetNumberOfUsersOnline(string application);

   [OperationContract]
   UserInfo GetUserInfo(string application,string userName);

   [OperationContract]
   string GetUserNameByEmail(string application,string email);

   [OperationContract]
   void UpdateUser(string application,string userName,string email,string oldAnswer,string newQuestion,string newAnswer,bool isApproved,bool isLockedOut);

   [OperationContract]
   int UserIsOnlineTimeWindow(string application);
}

public partial class MembershipManagerProxy : ClientBase<IMembershipManager>,IMembershipManager
{
   public MembershipManagerProxy()
   {}

   public MembershipManagerProxy(string address)
   {
      Endpoint.Address = new EndpointAddress(address);
   }

   public MembershipManagerProxy(string endpointConfigurationName,string remoteAddress) : base(endpointConfigurationName,remoteAddress)
   {}

   public MembershipManagerProxy(string endpointConfigurationName,EndpointAddress remoteAddress) : base(endpointConfigurationName,remoteAddress)
   {}

   public MembershipManagerProxy(Binding binding,EndpointAddress remoteAddress) : base(binding,remoteAddress)
   {}

   public System.Web.Security.MembershipCreateStatus CreateUser(string application,string userName,string password,string email,string passwordQuestion,string passwordAnswer,bool isApproved)
   {
      return Channel.CreateUser(application,userName,password,email,passwordQuestion,passwordAnswer,isApproved);
   }

   public void DeleteAllUsers(string application,bool deleteAllRelatedData)
   {
      Channel.DeleteAllUsers(application,deleteAllRelatedData);
   }

   public bool DeleteUser(string application,string userName,bool deleteAllRelatedData)
   {
      return Channel.DeleteUser(application,userName,deleteAllRelatedData);
   }

   public string[] GetAllUsers(string application)
   {
      return Channel.GetAllUsers(application);
   }

   public int GetNumberOfUsersOnline(string application)
   {
      return Channel.GetNumberOfUsersOnline(application);
   }

   public UserInfo GetUserInfo(string application,string userName)
   {
      return Channel.GetUserInfo(application,userName);
   }

   public string GetUserNameByEmail(string application,string email)
   {
      return Channel.GetUserNameByEmail(application,email);
   }

   public void UpdateUser(string application,string userName,string email,string oldAnswer,string newQuestion,string newAnswer,bool isApproved,bool isLockedOut)
   {
      Channel.UpdateUser(application,userName,email,oldAnswer,newQuestion,newAnswer,isApproved,isLockedOut);
   }

   public int UserIsOnlineTimeWindow(string application)
   {
      return Channel.UserIsOnlineTimeWindow(application);
   }
}


[ServiceContract]
public interface IPasswordManager
{
   [OperationContract]
   void ChangePasswordWithAnswer(string application,string userName,string passwordAnswer,string newPassword);

   [OperationContract]
   bool EnablePasswordReset(string application);

   [OperationContract]
   bool EnablePasswordRetrieval(string application);

   [OperationContract]
   string GeneratePassword(string application,int length,int numberOfNonAlphanumericCharacters);

   [OperationContract]
   int GetMaxInvalidPasswordAttempts(string application);

   [OperationContract]
   int GetMinRequiredNonAlphanumericCharacters(string application);

   [OperationContract]
   int GetMinRequiredPasswordLength(string application);

   [OperationContract]
   string GetPassword(string application,string userName,string passwordAnswer);

   [OperationContract]
   int GetPasswordAttemptWindow(string application);

   [OperationContract]
   string GetPasswordStrengthRegularExpression(string application);

   [OperationContract]
   bool RequiresQuestionAndAnswer(string application);

   [OperationContract]
   string ResetPassword(string application,string userName);

   [OperationContract]
   string ResetPasswordWithQuestionAndAnswer(string application,string userName,string passwordAnswer);

   [OperationContract]
   string GetPasswordQuestion(string application,string userName);

   [OperationContract]
   void ChangePassword(string application,string userName,string newPassword);
}

public partial class PasswordManagerProxy : ClientBase<IPasswordManager>,IPasswordManager
{
   public PasswordManagerProxy()
   {}

   public PasswordManagerProxy(string address)
   {
      Endpoint.Address = new EndpointAddress(address);
   }

   public PasswordManagerProxy(string endpointConfigurationName,string remoteAddress) : base(endpointConfigurationName,remoteAddress)
   {}

   public PasswordManagerProxy(string endpointConfigurationName,EndpointAddress remoteAddress) : base(endpointConfigurationName,remoteAddress)
   {}

   public PasswordManagerProxy(Binding binding,EndpointAddress remoteAddress) : base(binding,remoteAddress)
   {}

   public void ChangePasswordWithAnswer(string application,string userName,string passwordAnswer,string newPassword)
   {
      Channel.ChangePasswordWithAnswer(application,userName,passwordAnswer,newPassword);
   }

   public bool EnablePasswordReset(string application)
   {
      return Channel.EnablePasswordReset(application);
   }

   public bool EnablePasswordRetrieval(string application)
   {
      return Channel.EnablePasswordRetrieval(application);
   }

   public string GeneratePassword(string application,int length,int numberOfNonAlphanumericCharacters)
   {
      return Channel.GeneratePassword(application,length,numberOfNonAlphanumericCharacters);
   }

   public int GetMaxInvalidPasswordAttempts(string application)
   {
      return Channel.GetMaxInvalidPasswordAttempts(application);
   }

   public int GetMinRequiredNonAlphanumericCharacters(string application)
   {
      return Channel.GetMinRequiredNonAlphanumericCharacters(application);
   }

   public int GetMinRequiredPasswordLength(string application)
   {
      return Channel.GetMinRequiredPasswordLength(application);
   }

   public string GetPassword(string application,string userName,string passwordAnswer)
   {
      return Channel.GetPassword(application,userName,passwordAnswer);
   }

   public int GetPasswordAttemptWindow(string application)
   {
      return Channel.GetPasswordAttemptWindow(application);
   }

   public string GetPasswordStrengthRegularExpression(string application)
   {
      return Channel.GetPasswordStrengthRegularExpression(application);
   }

   public bool RequiresQuestionAndAnswer(string application)
   {
      return Channel.RequiresQuestionAndAnswer(application);
   }

   public string ResetPassword(string application,string userName)
   {
      return Channel.ResetPassword(application,userName);
   }

   public string ResetPasswordWithQuestionAndAnswer(string application,string userName,string passwordAnswer)
   {
      return Channel.ResetPasswordWithQuestionAndAnswer(application,userName,passwordAnswer);
   }

   public string GetPasswordQuestion(string application,string userName)
   {
      return Channel.GetPasswordQuestion(application,userName);
   }
   public void ChangePassword(string application,string userName,string newPassword)
   {
      Channel.ChangePassword(application,userName,newPassword);
   }
}


[ServiceContract]
public interface IRoleManager
{
   [OperationContract]
   void AddUserToRole(string application,string userName,string role);

   [OperationContract]
   void AddUserToRoles(string application,string userName,string[] roles);

   [OperationContract]
   void AddUsersToRole(string application,string[] userNames,string role);

   [OperationContract]
   void AddUsersToRoles(string application,string[] userNames,string[] roles);

   [OperationContract]
   void CreateRole(string application,string role);

   [OperationContract]
   void DeleteAllRoles(string application,bool throwOnPopulatedRole);

   [OperationContract]
   bool DeleteRole(string application,string role,bool throwOnPopulatedRole);

   [OperationContract]
   string[] GetAllRoles(string application);

   [OperationContract]
   string[] GetRolesForUser(string application,string userName);

   [OperationContract]
   string[] GetUsersInRole(string application,string role);

   [OperationContract]
   bool IsRolesEnabled(string application);

   [OperationContract]
   void RemoveUserFromRole(string application,string userName,string roleName);

   [OperationContract]
   void RemoveUserFromRoles(string application,string user,string[] roles);

   [OperationContract]
   void RemoveUsersFromRole(string application,string[] users,string role);

   [OperationContract]
   void RemoveUsersFromRoles(string application,string[] users,string[] roles);

   [OperationContract]
   bool RoleExists(string application,string role);
}

public partial class RoleManagerProxy : ClientBase<IRoleManager>,IRoleManager
{
   public RoleManagerProxy()
   {}

   public RoleManagerProxy(string address)
   {
      Endpoint.Address = new EndpointAddress(address);
   }

   public RoleManagerProxy(string endpointConfigurationName,string remoteAddress) : base(endpointConfigurationName,remoteAddress)
   {}

   public RoleManagerProxy(string endpointConfigurationName,EndpointAddress remoteAddress) : base(endpointConfigurationName,remoteAddress)
   {}

   public RoleManagerProxy(Binding binding,EndpointAddress remoteAddress) : base(binding,remoteAddress)
   {}

   public void AddUserToRole(string application,string userName,string role)
   {
      Channel.AddUserToRole(application,userName,role);
   }

   public void AddUserToRoles(string application,string userName,string[] roles)
   {
      Channel.AddUserToRoles(application,userName,roles);
   }

   public void AddUsersToRole(string application,string[] userNames,string role)
   {
      Channel.AddUsersToRole(application,userNames,role);
   }

   public void AddUsersToRoles(string application,string[] userNames,string[] roles)
   {
      Channel.AddUsersToRoles(application,userNames,roles);
   }

   public void CreateRole(string application,string role)
   {
      Channel.CreateRole(application,role);
   }

   public void DeleteAllRoles(string application,bool throwOnPopulatedRole)
   {
      Channel.DeleteAllRoles(application,throwOnPopulatedRole);
   }

   public bool DeleteRole(string application,string role,bool throwOnPopulatedRole)
   {
      return Channel.DeleteRole(application,role,throwOnPopulatedRole);
   }

   public string[] GetAllRoles(string application)
   {
      return Channel.GetAllRoles(application);
   }

   public string[] GetRolesForUser(string application,string userName)
   {
      return Channel.GetRolesForUser(application,userName);
   }

   public string[] GetUsersInRole(string application,string role)
   {
      return Channel.GetUsersInRole(application,role);
   }

   public bool IsRolesEnabled(string application)
   {
      return Channel.IsRolesEnabled(application);
   }

   public void RemoveUserFromRole(string application,string userName,string roleName)
   {
      Channel.RemoveUserFromRole(application,userName,roleName);
   }

   public void RemoveUserFromRoles(string application,string user,string[] roles)
   {
      Channel.RemoveUserFromRoles(application,user,roles);
   }

   public void RemoveUsersFromRole(string application,string[] users,string role)
   {
      Channel.RemoveUsersFromRole(application,users,role);
   }

   public void RemoveUsersFromRoles(string application,string[] users,string[] roles)
   {
      Channel.RemoveUsersFromRoles(application,users,roles);
   }

   public bool RoleExists(string application,string role)
   {
      return Channel.RoleExists(application,role);
   }
}


[ServiceContract]
public interface IUserManager
{
   [OperationContract]
   bool Authenticate(string applicationName,string userName,string password);

   [OperationContract]
   string[] GetRoles(string applicationName,string userName);

   [OperationContract]
   bool IsInRole(string applicationName,string userName,string role);
}

public partial class UserManagerProxy : ClientBase<IUserManager>,IUserManager
{
   public UserManagerProxy()
   {}

   public UserManagerProxy(string address)
   {
      Endpoint.Address = new EndpointAddress(address);
   }

   public UserManagerProxy(string endpointConfigurationName,string remoteAddress) : base(endpointConfigurationName,remoteAddress)
   {}

   public UserManagerProxy(string endpointConfigurationName,EndpointAddress remoteAddress) : base(endpointConfigurationName,remoteAddress)
   {}

   public UserManagerProxy(Binding binding,EndpointAddress remoteAddress) : base(binding,remoteAddress)
   {}

   public bool Authenticate(string applicationName,string userName,string password)
   {
      return Channel.Authenticate(applicationName,userName,password);
   }

   public string[] GetRoles(string applicationName,string userName)
   {
      return Channel.GetRoles(applicationName,userName);
   }

   public bool IsInRole(string applicationName,string userName,string role)
   {
      return Channel.IsInRole(applicationName,userName,role);
   }
}