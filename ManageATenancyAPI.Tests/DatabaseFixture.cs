using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Text;
using System.Threading;
using Xunit;

namespace ManageATenancyAPI.Tests
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            Execute();
        }

        public void Dispose()
        {
        }

    
        //Build database

        public void Execute()
        {

            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                PowerShellInstance.AddScript(".\\Createdatabase.ps1");
                IAsyncResult result = PowerShellInstance.BeginInvoke();
                while (result.IsCompleted == false)
                {
                    Console.WriteLine("Waiting for pipeline to finish...");
                    Thread.Sleep(1000);
                }
                Console.WriteLine("Finished!");
                Console.ReadKey();
            }
            //using (var ps = PowerShell.Create())
            //{
            //    var results = ps.AddScript("Createdatabase.ps1").Invoke();
            //    foreach (var result in results)
            //    {
            //        Debug.Write(result.ToString());
            //    }
            //}
        }
    }

    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

}
