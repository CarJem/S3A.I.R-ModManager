@echo off
SET ConfigurationName=%1
SET SolutionDir=%~2
SET TargetDir=%~3
SET ProjectDir=%~4

taskkill /F /IM VBCSCompiler.exe

rmdir "%TargetDir%bin/roslyn " /s /q
rmdir "%TargetDir%Lib " /s /q

SET COPYCMD=/Y

:: Move the Binaries
move  "%TargetDir%*.dll" "%TargetDir%/Lib"
move  "%TargetDir%*.pdb" "%TargetDir%/Lib"
move  "%TargetDir%*.pdb" "%TargetDir%/Lib"

robocopy  "%TargetDir%roslyn" "%TargetDir%bin/roslyn" /S /Move /is
robocopy  "%TargetDir%cs-CZ" "%TargetDir%Lib/cs-CZ" /S /Move /is
robocopy  "%TargetDir%cs-CZ" "%TargetDir%Lib/cs-CZ" /S /Move /is
robocopy  "%TargetDir%de" "%TargetDir%Lib/de" /S /Move /is
robocopy  "%TargetDir%es" "%TargetDir%Lib/es" /S /Move /is
robocopy  "%TargetDir%fr" "%TargetDir%Lib/fr" /S /Move /is
robocopy  "%TargetDir%hu" "%TargetDir%Lib/hu" /S /Move /is
robocopy  "%TargetDir%nl" "%TargetDir%Lib/nl" /S /Move /is
robocopy  "%TargetDir%it" "%TargetDir%Lib/it" /S /Move /is
robocopy  "%TargetDir%pt-BR" "%TargetDir%Lib/pt-BR" /S /Move /is
robocopy  "%TargetDir%ro" "%TargetDir%Lib/ro" /S /Move /is
robocopy  "%TargetDir%ru" "%TargetDir%Lib/ru" /S /Move /is
robocopy  "%TargetDir%sv" "%TargetDir%Lib/sv" /S /Move /is
robocopy  "%TargetDir%x64" "%TargetDir%Lib/x64" /S /Move /is
robocopy  "%TargetDir%x86" "%TargetDir%Lib/x86" /S /Move /is
robocopy  "%TargetDir%zh-Hans" "%TargetDir%Lib/zh-Hans" /S /Move /is

move  "%TargetDir%libSkiaSharp.dylib" "%TargetDir%/Lib"
move  "%TargetDir%RSDKv5.dll.config" "%TargetDir%/Lib"
