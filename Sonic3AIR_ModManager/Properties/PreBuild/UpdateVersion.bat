echo off
SET ConfigurationName=%1
SET SolutionDir=%~2
SET TargetDir=%~3
SET ProjectDir=%~4
set ignoreFile="False"
set ignoreInternal="False"


:: Update Version
if %ConfigurationName% == "Publish" (	
	call "%ProjectDir%Properties\Versioning\GenerationsLib.Versioning.exe" "%ProjectDir%Properties\AssemblyInfo.cs" "Sonic3AIR_ModManager" %ignoreFile% %ignoreInternal%
)