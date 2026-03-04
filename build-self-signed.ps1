# Load .env file if it exists
$envFile = ".\.env"
$envVars = @{}
if (Test-Path $envFile) {
    Write-Host "Loading environment variables from .env file..." -ForegroundColor Cyan
    Get-Content $envFile | ForEach-Object {
        if ($_ -match '^\s*([^#][^=]*)\s*=\s*(.*)$') {
            $name = $matches[1].Trim()
            $value = $matches[2].Trim()
            $envVars[$name] = $value
            [Environment]::SetEnvironmentVariable($name, $value, "Process")
        }
    }
} else {
    Write-Host "No .env file found." -ForegroundColor Yellow
}

# Configuration
$projectPath = "IBudget.GUI\IBudget.GUI.csproj"
$releasesPath = ".\releases"

# Always build both Windows and macOS (ARM64 for M series Macs)
$platforms = @("win-x64", "osx-arm64")
Write-Host "Building for platform(s): $($platforms -join ', ')" -ForegroundColor Cyan

# Get and increment version from .env
if ($envVars.ContainsKey("VERSION")) {
    $version = $envVars["VERSION"]
    Write-Host "Current version: $version" -ForegroundColor Cyan
    
    # Parse and increment version (major.minor.patch)
    if ($version -match '^(\d+)\.(\d+)\.(\d+)$') {
        $major = [int]$matches[1]
        $minor = [int]$matches[2]
        $patch = [int]$matches[3]
        
        # Increment patch version
        $patch++
        $version = "$major.$minor.$patch"
    } else {
        Write-Host "Invalid version format, resetting to 0.1.0" -ForegroundColor Yellow
        $version = "0.1.0"
    }
} else {
    Write-Host "No VERSION found in .env, starting at 0.1.0" -ForegroundColor Yellow
    $version = "0.1.0"
}

Write-Host "New version: $version" -ForegroundColor Green

# Update .env file with new version
$envContent = Get-Content $envFile
$envContent = $envContent | ForEach-Object {
    if ($_ -match '^\s*VERSION\s*=') {
        "VERSION=$version"
    } else {
        $_
    }
}
Set-Content -Path $envFile -Value $envContent

# Step 1: Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Cyan
Remove-Item -Path $releasesPath -Recurse -ErrorAction SilentlyContinue

# Ensure releases directory exists
if (-not (Test-Path $releasesPath)) {
    New-Item -ItemType Directory -Path $releasesPath | Out-Null
}

# Build for each platform
foreach ($runtime in $platforms) {
    Write-Host "`n========================================" -ForegroundColor Magenta
    Write-Host "Building for $runtime" -ForegroundColor Magenta
    Write-Host "========================================`n" -ForegroundColor Magenta
    
    $publishPath = ".\publish\$runtime"
    
    # Clean platform-specific publish folder
    Remove-Item -Path $publishPath -Recurse -ErrorAction SilentlyContinue
    
    # Step 2: Publish the application
    Write-Host "Publishing application for $runtime..." -ForegroundColor Cyan
    dotnet publish $projectPath -c Release -r $runtime --self-contained true -o $publishPath
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Build failed for $runtime!" -ForegroundColor Red
        exit 1
    }
    
    # Step 3: Package with Velopack
    if ($runtime -eq "win-x64") {
        Write-Host "Creating Velopack package for Windows..." -ForegroundColor Cyan
        vpk pack -u IBudget -v $version -p $publishPath -e IBudget.GUI.exe -o $releasesPath --channel win
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Packaging failed for Windows!" -ForegroundColor Red
            exit 1
        }
        
        # Step 4: Sign Windows installer with self-signed certificate
        Write-Host "`nSigning Windows installer with self-signed certificate..." -ForegroundColor Cyan
        
        $setupFile = Get-ChildItem -Path $releasesPath -Filter "*Setup.exe" | Select-Object -First 1
        
        if ($setupFile) {
            Write-Host "Found Windows setup file: $($setupFile.FullName)" -ForegroundColor Green
            
            # Create self-signed certificate if it doesn't exist
            $certName = "StacksAppSigning"
            $cert = Get-ChildItem -Path Cert:\CurrentUser\My | Where-Object { $_.Subject -match $certName } | Select-Object -First 1
            
            if (-not $cert) {
                Write-Host "Creating self-signed certificate..." -ForegroundColor Yellow
                $cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=$certName" -CertStoreLocation Cert:\CurrentUser\My
                
                # Trust the certificate by adding it to Trusted Root
                Write-Host "Trusting the certificate (requires admin)..." -ForegroundColor Yellow
                $store = New-Object System.Security.Cryptography.X509Certificates.X509Store("Root", "CurrentUser")
                $store.Open("ReadWrite")
                $store.Add($cert)
                $store.Close()
                Write-Host "Certificate trusted!" -ForegroundColor Green
            }
            
            # Sign the file
            $signResult = Set-AuthenticodeSignature -FilePath $setupFile.FullName -Certificate $cert -TimestampServer "http://timestamp.digicert.com"
            
            if ($signResult.Status -eq "Valid" -or $signResult.Status -eq "UnknownError") {
                Write-Host "Signing complete (self-signed)!" -ForegroundColor Green
                Write-Host "NOTE: Users will see 'Unknown Publisher' warning unless they also trust this certificate" -ForegroundColor Yellow
            } else {
                Write-Host "Signing status: $($signResult.Status)" -ForegroundColor Yellow
                Write-Host "This is normal for self-signed certificates" -ForegroundColor Yellow
            }
        } else {
            Write-Host "Windows setup file not found, skipping signing" -ForegroundColor Yellow
        }
    }
    elseif ($runtime -eq "osx-arm64") {
        Write-Host "Creating macOS package for Apple Silicon (M series)..." -ForegroundColor Cyan
        
        # Note: Creating proper .pkg installers requires macOS tools (pkgbuild/productbuild)
        # which are not available on Windows. Creating a portable zip archive instead.
        
        $zipName = "IBudget-$version-osx-arm64.zip"
        $zipPath = Join-Path $releasesPath $zipName
        
        if (Test-Path $zipPath) {
            Remove-Item $zipPath -Force
        }
        
        # Create zip archive
        Compress-Archive -Path "$publishPath\*" -DestinationPath $zipPath -Force
        
        if (Test-Path $zipPath) {
            Write-Host "Created: $zipPath" -ForegroundColor Green
            
            # Calculate hashes
            $sha1 = (Get-FileHash -Path $zipPath -Algorithm SHA1).Hash
            $sha256 = (Get-FileHash -Path $zipPath -Algorithm SHA256).Hash
            $size = (Get-Item $zipPath).Length
            
            # Create releases.osx.json
            $releasesOsxJson = @{
                Assets = @(
                    @{
                        PackageId = "IBudget"
                        Version = $version
                        Type = "Full"
                        FileName = $zipName
                        SHA1 = $sha1
                        SHA256 = $sha256
                        Size = $size
                    }
                )
            } | ConvertTo-Json -Depth 3
            
            $releasesOsxJsonPath = Join-Path $releasesPath "releases.osx.json"
            Set-Content -Path $releasesOsxJsonPath -Value $releasesOsxJson
            Write-Host "Created: $releasesOsxJsonPath" -ForegroundColor Green
        } else {
            Write-Host "Failed to create zip for macOS!" -ForegroundColor Red
            exit 1
        }
        
        Write-Host "`nNOTE: Creating .pkg installers requires macOS. This build provides a portable zip." -ForegroundColor Yellow
        Write-Host "To run: Extract the zip, then right-click IBudget.GUI and select 'Open' to bypass Gatekeeper." -ForegroundColor Yellow
    }
}

Write-Host "`nRelease packages created in: $releasesPath" -ForegroundColor Green

# Step 5: Upload to GitHub Releases
if ($env:GITHUB_TOKEN) {
    Write-Host "Uploading to GitHub Releases..." -ForegroundColor Cyan
    
    # Use Velopack for Windows uploads
    vpk upload github `
        --repoUrl https://github.com/zlo120/IBudget `
        --token $env:GITHUB_TOKEN `
        --releaseName "v$version" `
        --tag "v$version" `
        --channel win `
        --publish
    
    # Upload macOS files using GitHub API
    Write-Host "`nUploading macOS files..." -ForegroundColor Cyan
    
    # Get the release ID
    $headers = @{
        "Authorization" = "token $($env:GITHUB_TOKEN)"
        "Accept" = "application/vnd.github.v3+json"
    }
    
    try {
        $releaseResponse = Invoke-RestMethod -Uri "https://api.github.com/repos/zlo120/IBudget/releases/tags/v$version" -Headers $headers -Method Get
        $releaseId = $releaseResponse.id
        $uploadUrl = $releaseResponse.upload_url -replace '\{\?name,label\}', ''
        
        Write-Host "Found release ID: $releaseId" -ForegroundColor Green
        
        # Find and upload macOS package (could be .zip, .pkg, or portable)
        $macFiles = @()
        $macFiles += Get-ChildItem -Path $releasesPath -Filter "IBudget-*-osx*.zip" -ErrorAction SilentlyContinue
        $macFiles += Get-ChildItem -Path $releasesPath -Filter "IBudget-osx*.zip" -ErrorAction SilentlyContinue
        $macFiles += Get-ChildItem -Path $releasesPath -Filter "*.pkg" -ErrorAction SilentlyContinue
        $macFiles = $macFiles | Select-Object -Unique
        
        foreach ($macFile in $macFiles) {
            if ($macFile) {
                Write-Host "Uploading $($macFile.Name)..." -ForegroundColor Cyan
                $fileBytes = [System.IO.File]::ReadAllBytes($macFile.FullName)
                
                $contentType = switch -Wildcard ($macFile.Extension) {
                    ".zip" { "application/zip" }
                    ".pkg" { "application/x-newton-compatible-pkg" }
                    ".nupkg" { "application/octet-stream" }
                    default { "application/octet-stream" }
                }
                
                $uploadHeaders = @{
                    "Authorization" = "token $($env:GITHUB_TOKEN)"
                    "Content-Type" = $contentType
                }
                
                $uploadUri = "$uploadUrl`?name=$($macFile.Name)"
                Invoke-RestMethod -Uri $uploadUri -Headers $uploadHeaders -Method Post -Body $fileBytes | Out-Null
                Write-Host "Uploaded $($macFile.Name)" -ForegroundColor Green
            }
        }
        
        # Upload releases.osx.json
        $macJsonFile = Join-Path $releasesPath "releases.osx.json"
        if (Test-Path $macJsonFile) {
            Write-Host "Uploading releases.osx.json..." -ForegroundColor Cyan
            $fileName = "releases.osx.json"
            $fileBytes = [System.IO.File]::ReadAllBytes($macJsonFile)
            
            $uploadHeaders = @{
                "Authorization" = "token $($env:GITHUB_TOKEN)"
                "Content-Type" = "application/json"
            }
            
            $uploadUri = "$uploadUrl`?name=$fileName"
            Invoke-RestMethod -Uri $uploadUri -Headers $uploadHeaders -Method Post -Body $fileBytes | Out-Null
            Write-Host "Uploaded releases.osx.json" -ForegroundColor Green
        }
    }
    catch {
        Write-Host "Error uploading macOS files: $_" -ForegroundColor Red
        Write-Host "macOS files are in $releasesPath - upload manually." -ForegroundColor Yellow
    }
} else {
    Write-Host "Skipping GitHub upload (GITHUB_TOKEN not set)" -ForegroundColor Yellow
}

Write-Host "`nBuild complete!" -ForegroundColor Green
