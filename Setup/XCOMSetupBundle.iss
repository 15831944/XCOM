[Setup]
ShowLanguageDialog=no
AppName=XCOM x64
AppVersion=1.5
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
DisableProgramGroupPage=yes
OutputBaseFilename=XCOMPluginSetup
OutputDir=Bin
DisableDirPage=yes
DefaultDirName="{userappdata}\Autodesk\ApplicationPlugins\XCOM.bundle"
UsePreviousAppDir=no

[Files]
Source: "Package\PackageContents.xml"; DestDir: "{userappdata}\Autodesk\ApplicationPlugins\XCOM.bundle";
Source: "Package\Resources\*"; DestDir: "{userappdata}\Autodesk\ApplicationPlugins\XCOM.bundle\Contents\Resources";
Source: "..\XCOMCore\bin\Release\XCOMCore.dll"; DestDir: "{userappdata}\Autodesk\ApplicationPlugins\XCOM.bundle\Contents\2012"; Flags: ignoreversion
Source: "..\XCOMCore\bin\Release\XCOMCore.dll"; DestDir: "{userappdata}\Autodesk\ApplicationPlugins\XCOM.bundle\Contents\2014"; Flags: ignoreversion
Source: "..\XCOM2012\bin\Release\XCOM2012.dll"; DestDir: "{userappdata}\Autodesk\ApplicationPlugins\XCOM.bundle\Contents\2012"; Flags: ignoreversion
Source: "..\XCOM2014\bin\Release\XCOM2014.dll"; DestDir: "{userappdata}\Autodesk\ApplicationPlugins\XCOM.bundle\Contents\2014"; Flags: ignoreversion

