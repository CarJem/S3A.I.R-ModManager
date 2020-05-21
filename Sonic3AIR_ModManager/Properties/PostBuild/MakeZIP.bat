@ECHO OFF
set Input=%~1
set Output=%~2

powershell.exe Compress-Archive -Path '%Input%' -DestinationPath '%Output%' -Force
powershell.exe Start explorer.exe '%Output%'