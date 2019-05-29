// © 2007 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.ServiceModel;
using System.Diagnostics;
using ServiceModelEx;
using System.Transactions;
using LogbookService;
using LogbookService.WCFLogbookDataSetTableAdapters;
using System.Collections.Generic;

[ServiceContract(Name = "ILogbookManager")]
interface ILogbookManagerService
{
   [OperationContract(IsOneWay = true)]
   void LogEntry(LogbookEntryService entry);

   [OperationContract(IsOneWay = true)]
   void Clear();

   [OperationContract]
   LogbookEntryService[] GetEntries();
}
[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)] 
class LogbookManager : ILogbookManagerService
{
   [OperationBehavior(TransactionScopeRequired = true)]
   public void LogEntry(LogbookEntryService entry)
   {
      EntriesTableAdapter adapter = new EntriesTableAdapter();
      adapter.Insert(entry.MachineName,entry.HostName,entry.AssemblyName,entry.FileName,entry.LineNumber,entry.TypeName,entry.MemberAccessed,entry.Date,entry.Time,entry.ExceptionName,entry.ExceptionMessage,entry.ProvidedFault,entry.ProvidedMessage,entry.Event);
   }
   [OperationBehavior(TransactionScopeRequired = true)]
   public void Clear()
   {
      EntriesTableAdapter adapter = new EntriesTableAdapter();
      adapter.ClearAll();
   }
   [OperationBehavior(TransactionScopeRequired = true)]
   public LogbookEntryService[] GetEntries()
   {
      EntriesTableAdapter adapter = new EntriesTableAdapter();

      WCFLogbookDataSet.EntriesDataTable table = new WCFLogbookDataSet.EntriesDataTable();
      adapter.Fill(table);

      Converter<WCFLogbookDataSet.EntriesRow,LogbookEntryService> convert = delegate(WCFLogbookDataSet.EntriesRow row)
                                                                            {
                                                                              return new LogbookEntryService(row.MachineName,row.HostName,row.EntryDate,row.EntryTime,row.AssemblyName,row.FileName,row.LineNumber,row.TypeName,row.MemberAccessed,row.ExceptionName,row.ExceptionMessage,row.ProvidedFault,row.ProvidedMessage,row.Event);
                                                                            };
      return DataTableHelper.ToArray<WCFLogbookDataSet.EntriesRow,LogbookEntryService>(table,convert);
   }
}

