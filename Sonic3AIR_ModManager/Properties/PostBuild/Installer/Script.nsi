############################################################################################
# Inital Definitions
############################################################################################

!define TEMP1 $R0 ;Temp variable
!define APP_NAME "Sonic 3 A.I.R. Mod Manager"
!define GB_HANDLER "GameBanana API Installer"
!define EXTRA_ICON "AIR_Original_Icon_HD.ico"
!define LAUNCH_GAME_SHORTCUT "Sonic 3 A.I.R. Auto Boot"
!define COMP_NAME "CarJem Generations"
!define WEB_SITE "https://twitter.com/carter5467_99"

####!define VERSION "${_VERSION}"#### Obsolete ####

!define COPYRIGHT "CarJem Generations © 2019"
!define DESCRIPTION "Mod Manager for Sonic 3 A.I.R. - Angel Island Revisited"
!define INSTALLER_NAME "Setup_${VERSION}.exe"
!define INSTALLER_DIR "."
!define MAIN_APP_EXE "Sonic 3 A.I.R Mod Manager.exe"
!define INSTALL_TYPE "SetShellVarContext current"
!define REG_ROOT "HKCU"
!define REG_APP_PATH "Software\Microsoft\Windows\CurrentVersion\App Paths\${MAIN_APP_EXE}"
!define UNINSTALL_PATH "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}"

############################################################################################
# Product Details
############################################################################################

VIProductVersion "${VERSION}"
VIAddVersionKey "ProductName"  "${APP_NAME}"
VIAddVersionKey "CompanyName"  "${COMP_NAME}"
VIAddVersionKey "LegalCopyright"  "${COPYRIGHT}"
VIAddVersionKey "FileDescription"  "${DESCRIPTION}"
VIAddVersionKey "FileVersion"  "${VERSION}"

############################################################################################
# Installer Details
############################################################################################

SetCompressor ZLIB
Name "${APP_NAME}"
Caption "${APP_NAME}"
OutFile "${INSTALLER_NAME}"
BrandingText "${APP_NAME}"
XPStyle off
InstallDirRegKey "${REG_ROOT}" "${REG_APP_PATH}" ""
InstallDir "$PROGRAMFILES\Sonic 3 A.I.R. Mod Manager"
!define UNINSTALLER_OPTIONS_NAME "$INSTDIR\UninstallOptions.ini"

######################################################################

############################################################################################
# Stuff for Uninstall UI
############################################################################################

;Things that need to be extracted on startup (keep these lines before any File command!)
;Only useful for BZIP2 compression
;Use ReserveFile for your own InstallOptions INI files too!

ReserveFile /plugin InstallOptionsEx.dll
ReserveFile UninstallOptions.ini

!include "MUI.nsh"

############################################################################################
# Custom Images
############################################################################################

!define MUI_WELCOMEFINISHPAGE_BITMAP "win.bmp"
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "win.bmp"

############################################################################################
# More Definitions and Marcos
############################################################################################

!define MUI_ABORTWARNING
!define MUI_UNABORTWARNING

!insertmacro MUI_PAGE_WELCOME

!ifdef LICENSE_TXT
!insertmacro MUI_PAGE_LICENSE "${LICENSE_TXT}"
!endif

!insertmacro MUI_PAGE_DIRECTORY

!ifdef REG_START_MENU
!define MUI_STARTMENUPAGE_NODISABLE
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "Sonic 3 A.I.R. Mod Manager"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "${REG_ROOT}"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "${UNINSTALL_PATH}"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "${REG_START_MENU}"
!insertmacro MUI_PAGE_STARTMENU Application $SM_Folder'
!endif

!insertmacro MUI_PAGE_INSTFILES

!define MUI_FINISHPAGE_RUN "$INSTDIR\${MAIN_APP_EXE}"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_WELCOME

!insertmacro MUI_UNPAGE_CONFIRM

UninstPage custom un.GetUserUninstallPrefs un.GetUserOptions " : Uninstall Options"
UninstPage instfiles

!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_UNPAGE_FINISH

!insertmacro MUI_LANGUAGE "English"	

######################################################################
# Main Install Method
######################################################################

Section -MainProgram
${INSTALL_TYPE}
SetOverwrite ifnewer
SetOutPath "$INSTDIR"
File /r D:\Users\CarJem\source\sonic3air_repos\Sonic3AIR_ModManager\Sonic3AIR_ModManager\bin\Release\*.*
File D:\Users\CarJem\source\sonic3air_repos\Sonic3AIR_ModManager\Installer\UninstallOptions.ini
File D:\Users\CarJem\source\sonic3air_repos\Sonic3AIR_ModManager\Installer\AIR_Original_Icon_HD.ico
SectionEnd

######################################################################
# Add Uninstaller
######################################################################
Section -Icons_Reg
SetOutPath "$INSTDIR"
WriteUninstaller "$INSTDIR\uninstall.exe"

######################################################################
# Add Start Menu Shortcuts
######################################################################
!ifdef REG_START_MENU
!insertmacro MUI_STARTMENU_WRITE_BEGIN Application
CreateDirectory "$SMPROGRAMS\$SM_Folder"
CreateShortCut "$SMPROGRAMS\$SM_Folder\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
CreateShortCut "$SMPROGRAMS\$SM_Folder\${LAUNCH_GAME_SHORTCUT}.lnk" "$INSTDIR\${MAIN_APP_EXE}" "-auto_boot=true" "$SMPROGRAMS\$SM_Folder\$EXTRA_ICON"
CreateShortCut "$SMPROGRAMS\$SM_Folder\${GB_HANDLER}.lnk" "$INSTDIR\${GB_HANDLER}.exe"
CreateShortCut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
CreateShortCut "$SMPROGRAMS\$SM_Folder\Uninstall ${APP_NAME}.lnk" "$INSTDIR\uninstall.exe"

######################################################################
# Add Web Site Links
######################################################################
!ifdef WEB_SITE
WriteIniStr "$INSTDIR\${APP_NAME} website.url" "InternetShortcut" "URL" "${WEB_SITE}"
CreateShortCut "$SMPROGRAMS\$SM_Folder\${APP_NAME} Website.lnk" "$INSTDIR\${APP_NAME} website.url"
!endif
!insertmacro MUI_STARTMENU_WRITE_END
!endif

######################################################################
# Add Start Menu Shortcuts (App Location)
######################################################################
!ifndef REG_START_MENU
CreateDirectory "$SMPROGRAMS\Sonic 3 A.I.R. Mod Manager"
CreateShortCut "$SMPROGRAMS\Sonic 3 A.I.R. Mod Manager\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
CreateShortCut "$SMPROGRAMS\Sonic 3 A.I.R. Mod Manager\${LAUNCH_GAME_SHORTCUT}.lnk" "$INSTDIR\${MAIN_APP_EXE}" "-auto_boot=true" "$INSTDIR\${EXTRA_ICON}"
CreateShortCut "$SMPROGRAMS\Sonic 3 A.I.R. Mod Manager\${GB_HANDLER}.lnk" "$INSTDIR\${GB_HANDLER}"
CreateShortCut "$DESKTOP\${APP_NAME}.lnk" "$INSTDIR\${MAIN_APP_EXE}"
CreateShortCut "$SMPROGRAMS\Sonic 3 A.I.R. Mod Manager\Uninstall ${APP_NAME}.lnk" "$INSTDIR\uninstall.exe"

######################################################################
# Add Web Site Links (App Location)
######################################################################
!ifdef WEB_SITE
WriteIniStr "$INSTDIR\${APP_NAME} website.url" "InternetShortcut" "URL" "${WEB_SITE}"
CreateShortCut "$SMPROGRAMS\Sonic 3 A.I.R. Mod Manager\${APP_NAME} Website.lnk" "$INSTDIR\${APP_NAME} website.url"
!endif
!endif

######################################################################
# Add Registry Entries
######################################################################
WriteRegStr ${REG_ROOT} "${REG_APP_PATH}" "" "$INSTDIR\${MAIN_APP_EXE}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayName" "${APP_NAME}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "UninstallString" "$INSTDIR\uninstall.exe"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayIcon" "$INSTDIR\${MAIN_APP_EXE}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "DisplayVersion" "${VERSION}"
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "Publisher" "${COMP_NAME}"

######################################################################
# Add Web Site Registry Entries
######################################################################
!ifdef WEB_SITE
WriteRegStr ${REG_ROOT} "${UNINSTALL_PATH}"  "URLInfoAbout" "${WEB_SITE}"
!endif
SectionEnd







######################################################################
# Main Uninstall Method
######################################################################
Section Uninstall
${INSTALL_TYPE}
RmDir /r "$INSTDIR" #Remove Main Install


######################################################################
# Remove Start Menu Shortcuts
######################################################################
!ifdef REG_START_MENU
!insertmacro MUI_STARTMENU_GETFOLDER "Application" $SM_Folder
Delete "$SMPROGRAMS\$SM_Folder\${APP_NAME}.lnk"
Delete "$SMPROGRAMS\$SM_Folder\Uninstall ${APP_NAME}.lnk"
!ifdef WEB_SITE
Delete "$SMPROGRAMS\$SM_Folder\${APP_NAME} Website.lnk"
!endif
Delete "$DESKTOP\${APP_NAME}.lnk"

RmDir "$SMPROGRAMS\$SM_Folder"
!endif

######################################################################
# Remove Start Menu Shortcuts
######################################################################
!ifndef REG_START_MENU
Delete "$SMPROGRAMS\Sonic 3 A.I.R. Mod Manager\${APP_NAME}.lnk"
Delete "$SMPROGRAMS\Sonic 3 A.I.R. Mod Manager\${GB_HANDLER}.lnk"
Delete "$SMPROGRAMS\Sonic 3 A.I.R. Mod Manager\${LAUNCH_GAME_SHORTCUT}.lnk"
Delete "$SMPROGRAMS\Sonic 3 A.I.R. Mod Manager\Uninstall ${APP_NAME}.lnk"
!ifdef WEB_SITE
Delete "$SMPROGRAMS\Sonic 3 A.I.R. Mod Manager\${APP_NAME} Website.lnk"
!endif
Delete "$DESKTOP\${APP_NAME}.lnk"

RmDir "$SMPROGRAMS\Sonic 3 A.I.R. Mod Manager"
!endif

######################################################################
# Remove Registry Keys
######################################################################
DeleteRegKey ${REG_ROOT} "${REG_APP_PATH}"
DeleteRegKey ${REG_ROOT} "${UNINSTALL_PATH}"
SectionEnd

######################################################################
# Additional Uninstall Options
######################################################################
Section "Components"

  ;Get Install Options dialog user input

  ReadINIStr ${TEMP1} "${UNINSTALLER_OPTIONS_NAME}" "Field 2" "State"
  DetailPrint "${TEMP1}"
  ReadINIStr ${TEMP1} "${UNINSTALLER_OPTIONS_NAME}" "Field 3" "State"
  DetailPrint "${TEMP1}"
  
SectionEnd

Function un.GetUserUninstallPrefs
  Push ${TEMP1}

    InstallOptionsEx::dialog "${UNINSTALLER_OPTIONS_NAME}"
    Pop ${TEMP1}
  
  Pop ${TEMP1}

FunctionEnd

Function un.GetUserOptions

  Goto CheckMM

  CheckAIR:
  ReadINIStr ${TEMP1} "${UNINSTALLER_OPTIONS_NAME}" "Field 3" "State"
  StrCmp ${TEMP1} 1 AIRSuccess AIRFail
  
  CheckMM:
  ReadINIStr ${TEMP1} "${UNINSTALLER_OPTIONS_NAME}" "Field 2" "State"
  StrCmp ${TEMP1} 1 MMSuccess MMFail
  
  MMFail:
  Goto CheckAIR
  
  MMSuccess:
  RmDir /r "$APPDATA\Sonic3AIR_MM" # Remove App Data for Mod Manager
  Goto CheckAIR
  
  AIRFail:
  Goto done
  
  AIRSuccess:
  RmDir /r "$APPDATA\Sonic3AIR" # Remove App Data for AIR
  Goto done
  
  done:
  

  
FunctionEnd

######################################################################



