@ECHO OFF
SET ConfigurationName=%1
SET SolutionDir=%~2
SET TargetDir=%~3
SET ProjectDir=%~4

SET CurrentDir=%~dp0

CALL "%CurrentDir%\UpdateVersion.bat" %ConfigurationName% %SolutionDir% %TargetDir% %ProjectDir%
CALL "%CurrentDir%\AlwaysClean.bat" %ConfigurationName% %SolutionDir% %TargetDir% %ProjectDir%