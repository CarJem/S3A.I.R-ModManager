SET scriptLocation="D:\Users\CarJem\source\sonic3air_repos\Sonic3AIR_ModManager\Installer\Sonic 3 A.I.R. Mod Manager.nsi"
SET makeNISIWLocation="C:\Program Files (x86)\NSIS\makensisw.exe"
SET Version=%1

%makeNISIWLocation% /V4 /DVersion=%Version% %scriptLocation%