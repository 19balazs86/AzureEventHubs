@echo off

REM https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite

cd /d "c:\program files\microsoft visual studio\2022\community\common7\ide\extensions\microsoft\Azure Storage Emulator"

azurite.exe --location "C:\Users\Balazs\AppData\Local\.vstools\azurite" --debug "C:\Users\Balazs\AppData\Local\.vstools\azurite\debug.log" --skipApiVersionCheck