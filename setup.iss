[Setup]
AppId={{DA3DF66B-71E3-4089-9C3E-8167664B5676}
AppName=Service Print
AppVersion=1.0.0
AppPublisher=Mlenong
DefaultDirName={pf}\Service Print
DefaultGroupName=Service Print
OutputDir=.
OutputBaseFilename=ServicePrintInstaller
SetupIconFile=printer.ico
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
UninstallDisplayIcon={app}\printer.ico
ChangesEnvironment=yes
ChangesAssociations=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "startup"; Description: "Automatically start Service Print on Windows startup"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
Source: "ServicePrintTray.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "service-backend.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "SumatraPDF.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "printer.ico"; DestDir: "{app}"; Flags: ignoreversion
; Create 'files' directory
Source: "files\*"; DestDir: "{app}\files"; Flags: ignoreversion recursesubdirs createallsubdirs skipifsourcedoesntexist

[Icons]
Name: "{group}\Service Print"; Filename: "{app}\ServicePrintTray.exe"; IconFilename: "{app}\printer.ico"
Name: "{commondesktop}\Service Print"; Filename: "{app}\ServicePrintTray.exe"; Tasks: desktopicon; IconFilename: "{app}\printer.ico"

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "ServicePrint"; ValueData: "{app}\ServicePrintTray.exe"; Flags: uninsdeletevalue; Tasks: startup

[Run]
Filename: "{app}\ServicePrintTray.exe"; Description: "{cm:LaunchProgram,Service Print}"; Flags: nowait postinstall skipifsilent

[UninstallRun]
Filename: "taskkill"; Parameters: "/IM ServicePrintTray.exe /F"; Flags: runhidden
Filename: "taskkill"; Parameters: "/IM service-backend.exe /F"; Flags: runhidden

