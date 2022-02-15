
    param(
       [parameter(Mandatory=$true, Position=0)]
        [string] $InstallDirectory
    )
    
    Set-Location $InstallDirectory
    $name = "ADFSFrejaMFA"

    [System.Reflection.Assembly]::Load("System.EnterpriseServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
    $publish = New-Object System.EnterpriseServices.Internal.Publish

    $authPolicy = Get-AdfsGlobalAuthenticationPolicy
    if($authPolicy.AdditionalAuthenticationProvider -contains $name){
        $authPolicy.AdditionalAuthenticationProvider.Remove($name)
        Set-AdfsGlobalAuthenticationPolicy -AdditionalAuthenticationProvider $authPolicy.AdditionalAuthenticationProvider | Out-Null
        Unregister-AdfsAuthenticationProvider -Name $name -Confirm:$false
    }
    
    restart-service adfssrv

    $dll = Join-Path $InstallDirectory 'ADFSFrejaMFAMerged.dll'

    $publish.GacRemove($dll)
