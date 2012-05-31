@ECHO OFF

set USERNAME=
set /P USERNAME= Type Username: %=%

set PASSWORD=
set /P PASSWORD= Type Password: %=%

C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe Example.build /t:Deploy /p:Configuration=Release /p:BuildViews=true /p:UserName=%USERNAME% /p:Password=%PASSWORD% /p:WebServerName=someServer /p:MigrationConnection=Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.16.175.8)(PORT=1527)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ACCUMOP)));User Id=mcs_connected;Password=mcs_connected;Persist Security Info=True;

pause
