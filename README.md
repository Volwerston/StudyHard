## Requirements ##
SQL Server 2018,
.NET Core SDK 3.1

## DB Connection ##
Open **appsettings.json** and **appsettings.test.json** inside, look for variable called *"StudyHardDatabase"*.
Set the value you need for main and test databases respectively.

## Migrations ##
You should have 2 databases: main and testing (look at *DB Connection section*)
All migrations should be executed on **both** of them.

Migrations are executed manually (for now).
If you update your database with the latest changes from Git:
- Find files you haven't run on your DB. Open a query tool and execute sql-scripts.
If you create a new migration:
- Don't change existing files, create a new .sql file and call him according to `newVersion_Title` format.
- Try to make the file be safe to multiple times executing, in case anybody else forgets about his DB version
