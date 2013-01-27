SET MSBUILD=C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe
SET BIN_DIR=.\Bin

%MSBUILD% .\server\NotificationService.sln /property:Configuration=Release /p:OutPutPath=%~dp0%BIN_DIR%
%MSBUILD% .\clients\windows\PushNotification\PushNotification.sln /property:Configuration=Release /p:OutPutPath=%~dp0%BIN_DIR%
