# ExchangeRate Service

To run on local, you should first run the docker-compose file.
Default it uses the docker containers in appsettings file. All uri and host informations are given with that assemption.

Mainly, it connects to external exchange service and give customers currencies they want.

It has only one method for beginning. 

To add migration for sql server in Data project
dotnet ef --startup-project ..\ExchangeRate.Api\ migrations add XXX-{MigrationName}

To update sql database 
dotnet ef database update