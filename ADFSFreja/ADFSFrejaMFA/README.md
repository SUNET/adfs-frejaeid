## Instructions for installing and uninstalling the Freja MFA Adapter 

:exclamation: This will enable the new paginated theme on you adfs farm!
### Certificates
Obtain and install client certificate and allow adfs service user rights on the private key
Install root and intermediate certificates for freja api.
### Config
Url's to the different endpoints can't contain a slash at the end, breaks json
Certificate password not needed, as the certificates should be installed on the servers.
Choose lookup method for civicnumber "UserLookupMethod": "SQL" or "UserLookupMethod": "LDAP"
For sql, multiple connections are available

### Copy files to ADFS
Copy files to ADFS server (c:\admin\install\<ADFSFrejaMFA>) from ADFSFrejaMFA
- freja_eid2.jpg												\images\freja_eid2.jpg
- ADFSFrejaMFAMerged.dll		\bin\<configuration>\ADFSFrejaMFAMerged.dll
- FrejaSettings.json		
- Newtonsoft.json.dll		
- Install-ADFSFrejaMFA.ps1		\scripts\
- Uninstall-ADFSFrejaMFA.ps1	\scripts\
### Install
```
Install-ADFSFrejaMFA.ps1 -InstallDirectory "path to folder" -ConfigFile "path to settingsfile <FrejaSettings.json>"
```
### Uninstall
```
Uninstall-ADFSFrejaMFA -InstallDirectory "path to folder"
```
