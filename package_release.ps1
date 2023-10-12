
$version = (Get-Item .\Timeboxer\bin\Release\Timeboxer.exe).VersionInfo.FileVersionRaw

Remove-Item -Recurse .\Staging

$null = New-Item .\Staging -ItemType Directory
$null = New-Item .\Staging\Timeboxer -ItemType Directory

$line = 0
(Get-Content .\README.md) | 
    Foreach-Object {
        if ($line -eq 0)
        {
            $_ + " " + $version
            $line=1
        } elseif ($line -eq 1) {
            $_ + "========"
            $line=2
        } else {
            $_ # send the current line to output

        }
    } | Set-Content .\Staging\Timeboxer\README.md

Copy-Item .\Timeboxer\bin\Release\Timeboxer.exe .\Staging\Timeboxer

Set-Location .\Staging

Compress-Archive .\Timeboxer -DestinationPath Timeboxer-$version.zip

Set-Location ..
