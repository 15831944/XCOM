#dim Version[4]
#expr ParseVersion("..\XCOMCore\bin\Release\XCOM.dll", Version[0], Version[1], Version[2], Version[3])
#define AppVersion Str(Version[0]) + "." + Str(Version[1]) + "." + Str(Version[2]) + "." + Str(Version[3])
#define ShortAppVersion Str(Version[0]) + "." + Str(Version[1])

[Setup]
ShowLanguageDialog=no
AppName=XCOM AutoCAD Plugin x64
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

[Files]
Source: "Package\PackageContents.xml"; DestDir: "{app}"; AfterInstall: AddVersionToPackageXML('{#ShortAppVersion}')
Source: "Package\Resources\*"; DestDir: "{app}\Resources"
Source: "..\XCOM2014\bin\Release\XCOM.dll"; DestDir: "{app}\Contents"; Flags: ignoreversion

[Code]
procedure AddVersionToPackageXML(Version: String);
var
  XMLDoc, RootNode: Variant;
  FileName: String;
begin
  FileName := ExpandConstant(CurrentFileName);
  XMLDoc := CreateOleObject('MSXML2.DOMDocument');
  XMLDoc.async := False;
  XMLDoc.resolveExternals := False;
  XMLDoc.load(FileName);
  
  RootNode := XMLDoc.documentElement;
  RootNode.setAttribute('AppVersion', Version);
 
  XMLDoc.Save(FileName);
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID = wpWelcome then
    WizardForm.NextButton.Caption := SetupMessage(msgButtonInstall);
end;
