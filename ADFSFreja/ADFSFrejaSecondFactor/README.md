## Instructions for installing and uninstalling the Freja eID second factor Adapter

### Certificates
Obtain and install client certificate and allow adfs service user rights on the private key
Install root and intermediate certificates for freja api.
### EventLog
Should be created during install, otherwise run:
New-EventLog -LogName "Application" -Source "Freja eID"
### Config
Url's to the different endpoints can't contain a slash at the end, breaks json
Certificate password not needed, as the certificates should be installed on the servers.
Choose lookup method for civicnumber "UserLookupMethod": "SQL" or "UserLookupMethod": "LDAP"
For sql, multiple connections are available
### Copy files to ADFS
Copy files to ADFS server (c:\admin\install\<ADFSFrejaSecondFactor>) from ADFSFrejaSecondFactor
freja_eid2.jpgg						\images\freja_eid2.jpg
ADFSFrejaSecondFactorMerged.dll		\bin\<configuration>\ADFSFrejaSecondFactorMerged.dll
FrejaSettings.json		
Newtonsoft.json.dll		
Install-ADFSFrejaSecondFactor.ps1	\scripts\
Uninstall-ADFSFrejaSecondFactor.ps1	\scripts\
### Install
- Install with following command:
```PowerShell
Install-ADFSFrejaSecondFactor.ps1 -InstallDirectory "path to folder" -ConfigFile "path to settingsfile <FrejaSettings.json>"
```
### Uninstall
- Uninstall with following command:
```PowerShell
Uninstall-ADFSFrejaSecondFactor -InstallDirectory "path to folder"
```