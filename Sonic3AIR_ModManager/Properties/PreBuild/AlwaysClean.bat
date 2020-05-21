@echo off
SET ConfigurationName=%1
SET SolutionDir=%~2
SET TargetDir=%~3
SET ProjectDir=%~4

:: RMDIR /Q/S "%TargetDir%"