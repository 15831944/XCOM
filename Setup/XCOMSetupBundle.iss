#dim Version[4]
#expr ParseVersion("..\XCOM\bin\Release\XCOM.dll", Version[0], Version[1], Version[2], Version[3])
#define AppVersion Str(Version[0]) + "." + Str(Version[1]) + "." + Str(Version[2]) + "." + Str(Version[3])
#define ShortAppVersion Str(Version[0]) + "." + Str(Version[1])

[Setup]
PrivilegesRequired=none
AppName="{cm:AppName}"
AppVersion={#ShortAppVersion}
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
DisableProgramGroupPage=yes
OutputBaseFilename=XCOMPluginSetup v{#ShortAppVersion}
OutputDir=Bin
DisableDirPage=yes
DefaultDirName={userappdata}\Autodesk\ApplicationPlugins\XCOM.bundle
UsePreviousAppDir=no
DisableReadyPage=yes
ShowLanguageDialog=no
WizardImageFile=LargelImage.bmp
WizardSmallImageFile=SmallImage.bmp
UsePreviousLanguage=no
SetupIconFile=Setup.ico

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "tr"; MessagesFile: "Turkish.isl"

[Files]
; Application manifest
Source: "..\Package\PackageContents.xml"; DestDir: "{app}"; Flags: ignoreversion; AfterInstall: PreparePackageXML('{#ShortAppVersion}')
; Menu resources
Source: "..\Menu\*.cuix"; DestDir: "{app}\Resources"
Source: "..\MenuIcons\Release\MenuIcons.dll"; DestDir: "{app}\Resources"; DestName: "XCOM_Menu.dll"
Source: "..\MenuIcons_Light\Release\MenuIcons_Light.dll"; DestDir: "{app}\Resources"; DestName: "XCOM_Menu_light.dll"
Source: "..\Package\icon.bmp"; DestDir: "{app}\Resources"
; Libraries
Source: "..\XCOM\bin\Release\XCOM.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion
Source: "..\AcadUtility\bin\Release\AcadUtility.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion
Source: "..\XCOM\Libraries\LicenseCheck.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion
Source: "..\XCOM\Libraries\SourceGrid.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion
Source: "..\XCOM\Libraries\Triangle.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion

[Tasks]
Name: "CHK_KBSHORTCUTS"; Description: "{cm:KeyboardShortcuts}"; Flags: unchecked

[CustomMessages]
AppName=XCOM AutoCAD Plugin (64 bit)
tr.AppName=XCOM AutoCAD Eklentisi (64 bit)
KeyboardShortcuts=Install keyboard shortcuts
tr.KeyboardShortcuts=Klavye kýsayollarýný etkinleþtir

[Code]
procedure PreparePackageXML(Version: String);
var
  XMLDoc, RootNode, ComponentsNode, MenuNode: Variant;
  FileName: String;
begin
  FileName := ExpandConstant(CurrentFileName);
  XMLDoc := CreateOleObject('MSXML2.DOMDocument');
  XMLDoc.async := False;
  XMLDoc.resolveExternals := False;
  XMLDoc.load(FileName);
  
  RootNode := XMLDoc.documentElement;
  RootNode.setAttribute('AppVersion', Version);

  // Keyboard shortcuts 
  if WizardForm.TasksList.Checked[0] then 
  begin
    ComponentsNode := XMLDoc.selectSingleNode('//Components');
    MenuNode := XMLDoc.createElement('ComponentEntry');
    MenuNode.setAttribute('AppName', 'XCOM');
    MenuNode.setAttribute('ModuleName', './Resources/XCOM_KeyboardShortcuts.cuix');
    MenuNode.setAttribute('LoadOnAutoCADStartup', 'True');
    ComponentsNode.appendChild(MenuNode);
  end;
     
  XMLDoc.Save(FileName);
end;

