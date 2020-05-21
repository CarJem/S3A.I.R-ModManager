SET ProjectDir=%~1
SET Version=%2

SET ScriptLocation="%ProjectDir%\Properties\PostBuild\Installer\Script.nsi"
SET NISIWLocation="C:\Program Files (x86)\NSIS\makensisw.exe"

%NISIWLocation% /V4 /DVersion=%Version% %ScriptLocation%