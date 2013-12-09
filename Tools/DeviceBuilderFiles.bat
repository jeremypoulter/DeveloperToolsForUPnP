cd %1
del %1\DeviceBuilder\DeviceBuilderFiles.exe
"C:\Program Files\WinRAR\winrar.exe" a -sfx -r -m5 -s %1\DeviceBuilder\DeviceBuilderFiles.exe -x@%1\DeviceBuilderFiles.exclude.txt