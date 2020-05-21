echo off
set ConfigurationName=%1
set SolutionDir=%~2
set ignoreFile="False"
set ignoreInternal="False"


:: Update Version
if %ConfigurationName% == "Publish" (	
	call "%SolutionDir%Properties\Versioning\GenerationsLib.Versioning.exe" "%SolutionDir%Properties\AssemblyInfo.cs" "Sonic3AIR_ModManager" %ignoreFile% %ignoreInternal%
)