REM Used by the compiler to sign a component or application. It's ok to remove if no signing needs to occur.
"C:\Program Files (x86)\Windows Kits\8.0\bin\x64\signtool" sign /sha1 274AA21D7f13EA647D4AB118E0E5FF37DB555248 /du "http://opentools.homeip.net" /d %1 %2
