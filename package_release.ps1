# Prepare a release package for upload to github
#
# DOES NOT BUILD OR TEST.
#
# Process is: 
#
# * In visual studio, use the project properties to set the version number
# * Make sure the README talks about the latest features
# * clone the repo to a temp workdir
# * in the temp clone, rebuild project in Release Build (to check all needed files are committed)
# * Test that you can drag the .exe someplace else and start it from there
# * run this script. It reads the version resource from the exe and uses it to update the readme and to name the zipfile. It removes and replaces the Staging/ folder.
# * Github things
#

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
