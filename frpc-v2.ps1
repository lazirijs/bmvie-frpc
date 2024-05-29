# FRPC installer tool title
Write-Host ""
Write-Host "<-- FRPC installer tool -->"
Write-Host ""

# Set client name
do {
    $clientName = Read-Host "Enter the client name only "
    $isInvalidClientName = -not $clientName -or $clientName -match "\s"
    if ($isInvalidClientName) {
        Write-Host "Enter a valid client name (no spaces allowed)"
    }
} while ($isInvalidClientName)

# Define paths and URLs
$folderPath = "C:\frpc"
$frpcServiceName = "frpc"
$frpcFolderUrl = "https://bit.ly/bmvie-frpc"
$zipFilePathOutput = "C:\frpc.zip"
$destinationFolder = "C:\"
$clientHostName = "http://$clientName.bmvie.net:8080"

# Create folder and add to antivirus exclusion list
mkdir $folderPath
Write-Host ""
Write-Host "1 - Folder Created : $folderPath"

Add-MpPreference -ExclusionPath $folderPath
Write-Host ""
Write-Host "2 - Folder add to antivirus exclusion list : $folderPath"

# Download and extract ZIP file
Write-Host ""
Write-Host "3 - Start downloading zip file to : $zipFilePathOutput"
Write-Host ""
Invoke-WebRequest -Uri $frpcFolderUrl -OutFile $zipFilePathOutput
Write-Host "4 - downloading complited"
Write-Host ""

Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::ExtractToDirectory($zipFilePathOutput, $destinationFolder)
Write-Host "5 - Extracting zip file complited to : $destinationFolder"
Write-Host ""

# Replace "ClientNameHere" with $clientName in all frpc.ini files
Get-ChildItem -Path $folderPath -Filter "frpc.ini" -File | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    if ($content -match "ClientNameHere") {
        $newContent = $content -replace "ClientNameHere", $clientName
        Set-Content -Path $_.FullName -Value $newContent
        Write-Host "6 - frpc.ini edit completed: Text replaced in $($_.FullName)"
    } else {
        Write-Host "6 - frpc.ini edit skipped: 'ClientNameHere' not found in $($_.FullName)"
    }
}

Write-Host ""

# Install and start the service
Write-Host "7 - installing service $folderPath\frpc_service.exe"
Write-Host ""
Start-Process -FilePath "$folderPath\frpc_service.exe" -ArgumentList "install" -NoNewWindow -Wait
Start-Service -Name $frpcServiceName
Write-Host ""
Write-Host "8 - the url is ready to use $clientHostName"
Write-Host ""
#Start-Process -FilePath $clientHostName