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

        public void Execute()
        {
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                PowerShellInstance.AddScript("powershell .\\Scripts\\Createdatabase.ps1");
                var result = PowerShellInstance.Invoke();
              
                //break point here to see errors
            }
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
