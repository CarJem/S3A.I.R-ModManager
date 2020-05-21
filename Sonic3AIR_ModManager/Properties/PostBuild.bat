ECHO off
SET ConfigurationName=%1
SET SolutionDir=%~2
SET TargetPath=%3
SET TargetDir=%~4

rmdir "%TargetDir%Lib\ " /s /q

:: Move all assemblies and related files to lib folder
ROBOCOPY "%TargetDir% " "%TargetDir%Lib\ " /XF *.exe *.config *.manifest /XD Lib Settings Steam_Banners /E /IS /MOVE
if %errorlevel% leq 4 exit 0 else exit %errorlevel%

:: Set Installer Version
("%SolutionDir%Installer\GenerationsLib.VersionExtracter.exe" %TargetPath%) > temp.txt
SET /p FileVersion=<temp.txt

:: Generate Installer
IF %ConfigurationName% == "Publish" (
	CALL "%SolutionDir%Installer\MakeInstaller.bat" "%FileVersion%"
)