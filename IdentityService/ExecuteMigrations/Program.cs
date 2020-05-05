using System;
using DbUp;

namespace ExecuteMigrations
{
    class Program
    {
        static void Main(string[] args)
        {
            // IMPORTANT: before running migration with new scripts,
            // make sure you set 'Copy to Output Directory' as 'Copy always' for new .sql files

            const string connectionString = "SpecifyBeforeRunning";

            var upgradeEngine = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsFromFileSystem("migrations")
                .LogToConsole()
                .Build();

            var result = upgradeEngine.PerformUpgrade();
            if (!result.Successful)
            {
                Console.WriteLine(result.Error);
            }

            Console.ReadLine();
        }
    }
}
