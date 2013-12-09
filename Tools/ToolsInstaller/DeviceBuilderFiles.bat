rem cd C:\Dev\SourceCode\DeveloperToolsForUPnP\Tools\DeviceBuilder
rem del C:\Dev\SourceCode\DeveloperToolsForUPnP\Tools\DeviceBuilder\DeviceBuilderFiles.exe
rem "C:\Program Files\WinRAR\winrar.exe" a -sfx -r -m5 -s C:\Dev\SourceCode\DeveloperToolsForUPnP\Tools\DeviceBuilder\DeviceBuilderFiles.exe -x@C:\Dev\SourceCode\DeveloperToolsForUPnP\Tools\DeviceBuilder\DeviceBuilderFiles.exc

cd ..\..\..\DeviceBuilder
del DeviceBuilderFiles.exe
"C:\Program Files\WinRAR\winrar.exe" a -sfx -r -m5 -s DeviceBuilderFiles.exe @DeviceBuilderFiles.inc
