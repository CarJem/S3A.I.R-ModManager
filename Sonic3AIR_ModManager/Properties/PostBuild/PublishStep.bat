@ECHO OFF
SET ConfigurationName=%1
SET SolutionDir=%~2
SET TargetDir=%~3
SET ProjectDir=%~4
SET TargetPath=%5

SET CurrentDir=%~dp0
SET AssistantPath="D:\Users\CarJem\source\personal_repos\GenerationsLib\GenerationsLib.UpdateAssistant\bin\x86\Release\GenerationsLib.UpdateAssistant.exe"

:: Set Installer Version
("%SolutionDir%Installer\GenerationsLib.VersionExtracter.exe" %TargetPath%) > temp.txt
SET /p FileVersion=<temp.txt
DEL /f temp.txt


CALL "%CurrentDir%\MoveBinaries.bat" %ConfigurationName% %SolutionDir% %TargetDir% %ProjectDir%

:: Generate Installer and Prepare to Publish
if %ConfigurationName% == "Publish" (
   call "%CurrentDir%\MakeInstaller.bat" "%ProjectDir%" "%FileVersion%"
   call "%CurrentDir%\MakeZIP.bat" "%TargetDir%" "%CurrentDir%Build_%FileVersion%.zip"
   :: call "%AssistantPath%" "Sonic3AIR_ModManager"
   call "%AssistantPath%"
)

:: Generate a ZIP for a Experimental Testing Build
if %ConfigurationName% == "Experiment" (
	call "%CurrentDir%\MakeZIP.bat" "%TargetDir%" "%CurrentDir%Build.zip"
)