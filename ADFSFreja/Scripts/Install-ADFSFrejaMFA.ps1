    param(
        [parameter(Mandatory=$true,Position = 0)]
        [string] $InstallDirectory,
        [parameter(Mandatory=$true, Position=1)]
        [string] $ConfigFile
    )

    [System.Reflection.Assembly]::Load("System.EnterpriseServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a") 
    $publish = New-Object System.EnterpriseServices.Internal.Publish

    #Set-Location $InstallDirectory
    $name = "ADFSFrejaMFA"
    $logName ="Freja eID"
    #region Prerequisites
    ## EventLog
    if(![System.Diagnostics.EventLog]::SourceExists("$logName")){
     New-EventLog -LogName "Application" -Source $logName
	}
    ## Dependencies
    $newtonsoftDep = Join-Path $InstallDirectory "Newtonsoft.Json.dll"
    if(Test-Path $newtonsoftDep)
    {
        $dep = ([system.reflection.assembly]::loadfile("$newtonsoftDep")).FullName
        $publish.GacInstall($newtonsoftDep)
	}
    
    #endregion Prerequisites


    
    $dll = Join-Path $InstallDirectory 'ADFSFrejaMFAMerged.dll'
    if(Test-Path $dll)
    {
        Write-Host "Dll exists"
        $authProviders = Get-AdfsAuthenticationProvider

        if (!(Get-AdfsGlobalAuthenticationPolicy).AllowAdditionalAuthenticationAsPrimary) 
        {
            Set-AdfsGlobalAuthenticationPolicy -AllowAdditionalAuthenticationAsPrimary $true -Force | Out-Null
        }
        $publish.GacInstall($dll)
        $fn = ([System.Reflection.Assembly]::LoadFile($dll)).FullName
        $typeName = "ADFSFrejaMFA.FrejaRefedsAdapter, " + $fn.ToString() + ", processorArchitecture=MSIL"
        
        if ([string]::IsNullOrEmpty((Get-AdfsAuthenticationProvider -Name $name))) {
            Register-AdfsAuthenticationProvider -TypeName $typeName -Name $name -ConfigurationFilePath $ConfigFile
        }
        else
        {
            Write-Host "Already installed on at least one farm node"     
		}
        
        Write-Host "$name successfully installed"

        Write-Host "Restarting service"
        restart-service adfssrv
        Write-Host "Done!"

        $authPolicy = Get-AdfsGlobalAuthenticationPolicy
        if($authPolicy.AdditionalAuthenticationProvider -notcontains $name){
            $authPolicy.AdditionalAuthenticationProvider.Add($name)
            Set-AdfsGlobalAuthenticationPolicy -AdditionalAuthenticationProvider $authPolicy.AdditionalAuthenticationProvider | Out-Null
            Write-Host "$name enabled!"
        }

        #//Logo
        $imgFreja ="freja_eid2.png"
        $imgFrejaPath = Join-Path $InstallDirectory $imgFreja
        $activetheme =Get-AdfsWebConfig
        $theme = Get-AdfsWebTheme -Name $activetheme.ActiveThemeName
        if(!$theme.IsBuiltinTheme){
            if(!$theme.AdditionalFileResources.Keys.Contains("/adfs/portal/images/$imgFreja")){            
                if( Test-Path $imgBankIDPath){
                    Set-AdfsWebTheme -TargetName  $theme.name -AdditionalFileResource @{Uri="/adfs/portal/images/$imgFreja";path="$imgFrejaPath"} 
                }
            }
        }
    }
    else
    {
        Write-Host "Dll doesn´t exist"
        #exit $LASTEXITCODE
    }
    
    

    

