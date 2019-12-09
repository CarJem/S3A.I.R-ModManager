echo off
set ConfigurationName=%1
set SolutionDir=%~2

if %ConfigurationName% == "Publish" (
call "%SolutionDir%Installer\MakeInstaller.bat"
)