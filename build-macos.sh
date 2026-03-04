#!/bin/bash

# Build script for IBudget - creates Windows .exe and macOS .pkg installers
# Run this on macOS to generate proper .pkg files

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
NC='\033[0m' # No Color

# Load .env file if it exists
ENV_FILE="./.env"
declare -A ENV_VARS

if [ -f "$ENV_FILE" ]; then
    echo -e "${CYAN}Loading environment variables from .env file...${NC}"
    while IFS='=' read -r key value; do
        # Skip comments and empty lines
        [[ $key =~ ^#.*$ ]] && continue
        [[ -z $key ]] && continue
        # Trim whitespace
        key=$(echo "$key" | xargs)
        value=$(echo "$value" | xargs)
        if [ -n "$key" ]; then
            ENV_VARS[$key]=$value
            export "$key=$value"
        fi
    done < "$ENV_FILE"
else
    echo -e "${YELLOW}No .env file found.${NC}"
fi

# Configuration
PROJECT_PATH="IBudget.GUI/IBudget.GUI.csproj"
RELEASES_PATH="./releases"
APP_NAME="IBudget"
APP_IDENTIFIER="com.ibudget.app"

# Always build both Windows and macOS
PLATFORMS=("win-x64" "osx-arm64")
echo -e "${CYAN}Building for platform(s): ${PLATFORMS[*]}${NC}"

# Get and increment version from .env
if [ -n "${ENV_VARS[VERSION]}" ]; then
    VERSION="${ENV_VARS[VERSION]}"
    echo -e "${CYAN}Current version: $VERSION${NC}"
    
    # Parse and increment version (major.minor.patch)
    if [[ $VERSION =~ ^([0-9]+)\.([0-9]+)\.([0-9]+)$ ]]; then
        MAJOR="${BASH_REMATCH[1]}"
        MINOR="${BASH_REMATCH[2]}"
        PATCH="${BASH_REMATCH[3]}"
        
        # Increment patch version
        PATCH=$((PATCH + 1))
        VERSION="$MAJOR.$MINOR.$PATCH"
    else
        echo -e "${YELLOW}Invalid version format, resetting to 0.1.0${NC}"
        VERSION="0.1.0"
    fi
else
    echo -e "${YELLOW}No VERSION found in .env, starting at 0.1.0${NC}"
    VERSION="0.1.0"
fi

echo -e "${GREEN}New version: $VERSION${NC}"

# Update .env file with new version
if [ -f "$ENV_FILE" ]; then
    if grep -q "^VERSION=" "$ENV_FILE"; then
        sed -i.bak "s/^VERSION=.*/VERSION=$VERSION/" "$ENV_FILE" && rm -f "$ENV_FILE.bak"
    else
        echo "VERSION=$VERSION" >> "$ENV_FILE"
    fi
fi

# Step 1: Clean previous builds
echo -e "${CYAN}Cleaning previous builds...${NC}"
rm -rf "$RELEASES_PATH"
mkdir -p "$RELEASES_PATH"

# Build for each platform
for RUNTIME in "${PLATFORMS[@]}"; do
    echo -e "\n${MAGENTA}========================================${NC}"
    echo -e "${MAGENTA}Building for $RUNTIME${NC}"
    echo -e "${MAGENTA}========================================\n${NC}"
    
    PUBLISH_PATH="./publish/$RUNTIME"
    
    # Clean platform-specific publish folder
    rm -rf "$PUBLISH_PATH"
    
    # Step 2: Publish the application
    echo -e "${CYAN}Publishing application for $RUNTIME...${NC}"
    dotnet publish "$PROJECT_PATH" -c Release -r "$RUNTIME" --self-contained true -o "$PUBLISH_PATH"
    
    if [ $? -ne 0 ]; then
        echo -e "${RED}Build failed for $RUNTIME!${NC}"
        exit 1
    fi
    
    # Step 3: Package
    if [ "$RUNTIME" == "win-x64" ]; then
        echo -e "${CYAN}Creating Velopack package for Windows...${NC}"
        vpk pack -u "$APP_NAME" -v "$VERSION" -p "$PUBLISH_PATH" -e IBudget.GUI.exe -o "$RELEASES_PATH" --channel win
        
        if [ $? -ne 0 ]; then
            echo -e "${RED}Packaging failed for Windows!${NC}"
            exit 1
        fi
        
        echo -e "${YELLOW}NOTE: Windows installer cannot be signed on macOS.${NC}"
        echo -e "${YELLOW}Users will see 'Unknown Publisher' warning.${NC}"
        
    elif [ "$RUNTIME" == "osx-arm64" ]; then
        echo -e "${CYAN}Creating macOS .pkg installer for Apple Silicon (M series)...${NC}"
        
        # Create app bundle structure
        APP_BUNDLE="$PUBLISH_PATH/$APP_NAME.app"
        mkdir -p "$APP_BUNDLE/Contents/MacOS"
        mkdir -p "$APP_BUNDLE/Contents/Resources"
        
        # Copy executable and dependencies
        cp -R "$PUBLISH_PATH"/* "$APP_BUNDLE/Contents/MacOS/" 2>/dev/null || true
        rm -rf "$APP_BUNDLE/Contents/MacOS/$APP_NAME.app"  # Remove nested app bundle if created
        
        # Create Info.plist
        cat > "$APP_BUNDLE/Contents/Info.plist" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleExecutable</key>
    <string>IBudget.GUI</string>
    <key>CFBundleIdentifier</key>
    <string>$APP_IDENTIFIER</string>
    <key>CFBundleName</key>
    <string>$APP_NAME</string>
    <key>CFBundleDisplayName</key>
    <string>$APP_NAME</string>
    <key>CFBundleVersion</key>
    <string>$VERSION</string>
    <key>CFBundleShortVersionString</key>
    <string>$VERSION</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleSignature</key>
    <string>????</string>
    <key>LSMinimumSystemVersion</key>
    <string>11.0</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>LSApplicationCategoryType</key>
    <string>public.app-category.finance</string>
</dict>
</plist>
EOF
        
        # Make executable
        chmod +x "$APP_BUNDLE/Contents/MacOS/IBudget.GUI"
        
        # Create .pkg installer using pkgbuild
        PKG_NAME="$APP_NAME-$VERSION-osx-arm64.pkg"
        PKG_PATH="$RELEASES_PATH/$PKG_NAME"
        
        echo -e "${CYAN}Building .pkg installer...${NC}"
        pkgbuild --root "$APP_BUNDLE" \
                 --identifier "$APP_IDENTIFIER" \
                 --version "$VERSION" \
                 --install-location "/Applications/$APP_NAME.app" \
                 "$PKG_PATH"
        
        if [ -f "$PKG_PATH" ]; then
            echo -e "${GREEN}Created: $PKG_PATH${NC}"
            
            # Calculate hashes
            SHA1=$(shasum -a 1 "$PKG_PATH" | awk '{print toupper($1)}')
            SHA256=$(shasum -a 256 "$PKG_PATH" | awk '{print toupper($1)}')
            SIZE=$(stat -f%z "$PKG_PATH" 2>/dev/null || stat -c%s "$PKG_PATH" 2>/dev/null)
            
            # Create releases.osx.json
            cat > "$RELEASES_PATH/releases.osx.json" << EOF
{
    "Assets": [
        {
            "PackageId": "$APP_NAME",
            "Version": "$VERSION",
            "Type": "Full",
            "FileName": "$PKG_NAME",
            "SHA1": "$SHA1",
            "SHA256": "$SHA256",
            "Size": $SIZE
        }
    ]
}
EOF
            echo -e "${GREEN}Created: $RELEASES_PATH/releases.osx.json${NC}"
        else
            echo -e "${RED}Failed to create .pkg for macOS!${NC}"
            exit 1
        fi
        
        # Also create a portable zip for users who prefer it
        ZIP_NAME="$APP_NAME-$VERSION-osx-arm64-portable.zip"
        ZIP_PATH="$RELEASES_PATH/$ZIP_NAME"
        echo -e "${CYAN}Creating portable zip...${NC}"
        cd "$PUBLISH_PATH" && zip -r "../../$ZIP_PATH" "$APP_NAME.app" && cd - > /dev/null
        
        if [ -f "$ZIP_PATH" ]; then
            echo -e "${GREEN}Created portable: $ZIP_PATH${NC}"
        fi
        
        echo -e "\n${YELLOW}NOTE: macOS .pkg is unsigned. Users may need to:${NC}"
        echo -e "${YELLOW}1. Right-click the .pkg and select 'Open'${NC}"
        echo -e "${YELLOW}2. Or go to System Preferences > Security & Privacy to allow it${NC}"
    fi
done

echo -e "\n${GREEN}Release packages created in: $RELEASES_PATH${NC}"

# List created files
echo -e "\n${CYAN}Created files:${NC}"
ls -la "$RELEASES_PATH"

# Step 5: Upload to GitHub Releases
# Use GITHUB_TOKEN from .env file
GITHUB_TOKEN="${ENV_VARS[GITHUB_TOKEN]}"

if [ -n "$GITHUB_TOKEN" ]; then
    echo -e "${CYAN}Uploading to GitHub Releases...${NC}"
    
    # Use Velopack for Windows uploads
    vpk upload github \
        --repoUrl https://github.com/zlo120/IBudget \
        --token "$GITHUB_TOKEN" \
        --releaseName "v$VERSION" \
        --tag "v$VERSION" \
        --channel win \
        --publish
    
    # Upload macOS files using GitHub API
    echo -e "\n${CYAN}Uploading macOS files...${NC}"
    
    # Get the release ID
    RELEASE_RESPONSE=$(curl -s -H "Authorization: token $GITHUB_TOKEN" \
        -H "Accept: application/vnd.github.v3+json" \
        "https://api.github.com/repos/zlo120/IBudget/releases/tags/v$VERSION")
    
    RELEASE_ID=$(echo "$RELEASE_RESPONSE" | grep -o '"id": [0-9]*' | head -1 | grep -o '[0-9]*')
    UPLOAD_URL=$(echo "$RELEASE_RESPONSE" | grep -o '"upload_url": "[^"]*"' | head -1 | sed 's/"upload_url": "//;s/{.*//')
    
    if [ -n "$RELEASE_ID" ]; then
        echo -e "${GREEN}Found release ID: $RELEASE_ID${NC}"
        
        # Upload .pkg file
        PKG_FILE=$(find "$RELEASES_PATH" -name "*.pkg" | head -1)
        if [ -f "$PKG_FILE" ]; then
            PKG_FILENAME=$(basename "$PKG_FILE")
            echo -e "${CYAN}Uploading $PKG_FILENAME...${NC}"
            curl -s -X POST \
                -H "Authorization: token $GITHUB_TOKEN" \
                -H "Content-Type: application/octet-stream" \
                --data-binary @"$PKG_FILE" \
                "${UPLOAD_URL}?name=$PKG_FILENAME" > /dev/null
            echo -e "${GREEN}Uploaded $PKG_FILENAME${NC}"
        fi
        
        # Upload portable zip if exists
        ZIP_FILE=$(find "$RELEASES_PATH" -name "*portable.zip" | head -1)
        if [ -f "$ZIP_FILE" ]; then
            ZIP_FILENAME=$(basename "$ZIP_FILE")
            echo -e "${CYAN}Uploading $ZIP_FILENAME...${NC}"
            curl -s -X POST \
                -H "Authorization: token $GITHUB_TOKEN" \
                -H "Content-Type: application/zip" \
                --data-binary @"$ZIP_FILE" \
                "${UPLOAD_URL}?name=$ZIP_FILENAME" > /dev/null
            echo -e "${GREEN}Uploaded $ZIP_FILENAME${NC}"
        fi
        
        # Upload releases.osx.json
        if [ -f "$RELEASES_PATH/releases.osx.json" ]; then
            echo -e "${CYAN}Uploading releases.osx.json...${NC}"
            curl -s -X POST \
                -H "Authorization: token $GITHUB_TOKEN" \
                -H "Content-Type: application/json" \
                --data-binary @"$RELEASES_PATH/releases.osx.json" \
                "${UPLOAD_URL}?name=releases.osx.json" > /dev/null
            echo -e "${GREEN}Uploaded releases.osx.json${NC}"
        fi
    else
        echo -e "${RED}Could not find release. macOS files are in $RELEASES_PATH - upload manually.${NC}"
    fi
else
    echo -e "${YELLOW}Skipping GitHub upload (GITHUB_TOKEN not set in .env)${NC}"
    echo -e "${YELLOW}To upload, add GITHUB_TOKEN=your_token to your .env file${NC}"
fi

echo -e "\n${GREEN}Build complete!${NC}"
