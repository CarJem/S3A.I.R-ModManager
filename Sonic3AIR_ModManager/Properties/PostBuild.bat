ECHO off
SET ConfigurationName=%1
SET SolutionDir=%~2
SET TargetPath=%3


("%SolutionDir%Installer\GenerationsLib.VersionExtracter.exe" %TargetPath%) > temp.txt
SET /p FileVersion=<temp.txt

IF %ConfigurationName% == "Publish" (
CALL "%SolutionDir%Installer\MakeInstaller.bat" "%FileVersion%"
)