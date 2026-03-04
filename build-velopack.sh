#!/bin/bash

# Build script for IBudget - creates Windows Setup.exe and macOS .pkg installers
# Run this on macOS to generate proper .pkg files and cross-compile Windows .exe installers
# Uses Velopack (vpk) for Windows installer creation and pkgbuild for macOS .pkg

set -e

# Validate that we're running on macOS (required for pkgbuild and codesign tools)
if [ "$(uname -s)" != "Darwin" ]; then
    echo "Error: This script must be run on macOS." >&2
    exit 1
fi

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
NC='\033[0m' # No Color

# Load .env file if it exists
ENV_FILE="./.env"

if [ -f "$ENV_FILE" ]; then
    echo -e "${CYAN}Loading environment variables from .env file...${NC}"
    while IFS='=' read -r key value || [ -n "$key" ]; do
        # Skip comments and empty lines
        [[ $key =~ ^#.*$ ]] && continue
        [[ -z $key ]] && continue
        # Trim whitespace
        key=$(echo "$key" | xargs)
        value=$(echo "$value" | xargs)
        if [ -n "$key" ]; then
            export "$key=$value"
        fi
    done < "$ENV_FILE"
else
    echo -e "${YELLOW}No .env file found.${NC}"
fi

# Configuration
PROJECT_PATH="IBudget.GUI/IBudget.GUI.csproj"
RELEASES_PATH="./releases"
APP_NAME="Stacks"
APP_IDENTIFIER="com.stacks.app"
GITHUB_REPO="zlo120/IBudget"

# Always build both Windows and macOS
PLATFORMS=("win-x64" "osx-arm64")
echo -e "${CYAN}Building for platform(s): ${PLATFORMS[*]}${NC}"

# Get and increment version from .env
if [ -n "$VERSION" ]; then
    CURRENT_VERSION="$VERSION"
    echo -e "${CYAN}Current version: $CURRENT_VERSION${NC}"
    
    # Parse and increment version (major.minor.patch)
    if [[ $CURRENT_VERSION =~ ^([0-9]+)\.([0-9]+)\.([0-9]+)$ ]]; then
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
mkdir -p "$RELEASES_PATH/win"
mkdir -p "$RELEASES_PATH/osx"

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
    dotnet publish "$PROJECT_PATH" -c Release -r "$RUNTIME" --self-contained true -o "$PUBLISH_PATH" /p:Version="$VERSION" /p:InformationalVersion="$VERSION"
    
    if [ $? -ne 0 ]; then
        echo -e "${RED}Build failed for $RUNTIME!${NC}"
        exit 1
    fi
    
    # Step 3: Package
    if [ "$RUNTIME" == "win-x64" ]; then
        echo -e "${CYAN}Creating Windows installer (.exe) via Velopack cross-compile...${NC}"
        
        # Use vpk [win] directive to cross-compile Windows installer on macOS
        # Brackets are quoted to prevent shell glob expansion
        WIN_RELEASES="$RELEASES_PATH/win"
        
        vpk "[win]" pack \
            -u "$APP_NAME" \
            -v "$VERSION" \
            -p "$PUBLISH_PATH" \
            -e IBudget.GUI.exe \
            -o "$WIN_RELEASES"
        
        if [ $? -ne 0 ]; then
            echo -e "${RED}Velopack packaging failed for Windows!${NC}"
            exit 1
        fi
        
        echo -e "${CYAN}Windows release files:${NC}"
        ls -la "$WIN_RELEASES"
        
        echo -e "${YELLOW}NOTE: Windows installer is unsigned. Users may see 'Unknown Publisher' warning.${NC}"
        
    elif [ "$RUNTIME" == "osx-arm64" ]; then
        echo -e "${CYAN}Creating macOS installer via Velopack...${NC}"
        
        # Create a custom Info.plist for the app bundle
        PLIST_PATH="$PUBLISH_PATH/Info.plist"
        cat > "$PLIST_PATH" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleExecutable</key>
    <string>IBudget.GUI</string>
    <key>CFBundleIconFile</key>
    <string>stacks-logo</string>
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
        
        # Use vpk pack to create Velopack-managed macOS package
        # This creates .pkg, portable .zip, and releases.osx.json with version metadata
        OSX_RELEASES="$RELEASES_PATH/osx"
        
        vpk pack \
            -u "$APP_NAME" \
            -v "$VERSION" \
            -p "$PUBLISH_PATH" \
            -e IBudget.GUI \
            -o "$OSX_RELEASES" \
            --packTitle "$APP_NAME" \
            --icon "IBudget.GUI/Assets/stacks-logo.icns" \
            --plist "$PLIST_PATH"
        
        if [ $? -ne 0 ]; then
            echo -e "${RED}Velopack packaging failed for macOS!${NC}"
            exit 1
        fi
        
        echo -e "${CYAN}macOS release files:${NC}"
        ls -la "$OSX_RELEASES"
        
        echo -e "\n${YELLOW}NOTE: macOS installer is unsigned. Users may need to:${NC}"
        echo -e "${YELLOW}1. Right-click the .pkg and select 'Open'${NC}"
        echo -e "${YELLOW}2. Or go to System Preferences > Security & Privacy to allow it${NC}"
    fi
done

echo -e "\n${GREEN}Release packages created in: $RELEASES_PATH${NC}"

# List created files
echo -e "\n${CYAN}Windows release files:${NC}"
ls -la "$RELEASES_PATH/win" 2>/dev/null || echo "(none)"
echo -e "\n${CYAN}macOS release files:${NC}"
ls -la "$RELEASES_PATH/osx" 2>/dev/null || echo "(none)"

# Step 5: Upload to GitHub Releases using GitHub API
# GITHUB_TOKEN is already exported from .env

if [ -n "$GITHUB_TOKEN" ]; then
    echo -e "\n${CYAN}Creating GitHub Release v$VERSION...${NC}"
    
    # Check if release already exists and delete it if so
    EXISTING_RESPONSE=$(curl -s -H "Authorization: token $GITHUB_TOKEN" \
        -H "Accept: application/vnd.github.v3+json" \
        "https://api.github.com/repos/$GITHUB_REPO/releases/tags/v$VERSION")
    
    EXISTING_ID=$(echo "$EXISTING_RESPONSE" | grep -o '"id": [0-9]*' | head -1 | grep -o '[0-9]*' || true)
    
    if [ -n "$EXISTING_ID" ]; then
        echo -e "${YELLOW}Release v$VERSION already exists (ID: $EXISTING_ID). Deleting...${NC}"
        curl -s -X DELETE \
            -H "Authorization: token $GITHUB_TOKEN" \
            -H "Accept: application/vnd.github.v3+json" \
            "https://api.github.com/repos/$GITHUB_REPO/releases/$EXISTING_ID"
        
        # Also delete the tag so it can be recreated
        curl -s -X DELETE \
            -H "Authorization: token $GITHUB_TOKEN" \
            -H "Accept: application/vnd.github.v3+json" \
            "https://api.github.com/repos/$GITHUB_REPO/git/refs/tags/v$VERSION" > /dev/null 2>&1 || true
        
        echo -e "${GREEN}Deleted existing release.${NC}"
        sleep 2
    fi
    
    # Create the release via GitHub API
    CREATE_RESPONSE=$(curl -s -X POST \
        -H "Authorization: token $GITHUB_TOKEN" \
        -H "Accept: application/vnd.github.v3+json" \
        "https://api.github.com/repos/$GITHUB_REPO/releases" \
        -d "{
            \"tag_name\": \"v$VERSION\",
            \"name\": \"v$VERSION\",
            \"body\": \"Release v$VERSION\",
            \"draft\": false,
            \"prerelease\": false
        }")
    
    RELEASE_ID=$(echo "$CREATE_RESPONSE" | grep -o '"id": [0-9]*' | head -1 | grep -o '[0-9]*' || true)
    UPLOAD_URL=$(echo "$CREATE_RESPONSE" | grep -o '"upload_url": "[^"]*"' | head -1 | sed 's/"upload_url": "//;s/{.*//' || true)
    
    if [ -z "$RELEASE_ID" ]; then
        echo -e "${YELLOW}Release may already exist, trying to fetch it...${NC}"
        FETCH_RESPONSE=$(curl -s -H "Authorization: token $GITHUB_TOKEN" \
            -H "Accept: application/vnd.github.v3+json" \
            "https://api.github.com/repos/$GITHUB_REPO/releases/tags/v$VERSION")
        
        RELEASE_ID=$(echo "$FETCH_RESPONSE" | grep -o '"id": [0-9]*' | head -1 | grep -o '[0-9]*' || true)
        UPLOAD_URL=$(echo "$FETCH_RESPONSE" | grep -o '"upload_url": "[^"]*"' | head -1 | sed 's/"upload_url": "//;s/{.*//' || true)
    fi
    
    if [ -n "$RELEASE_ID" ] && [ -n "$UPLOAD_URL" ]; then
        echo -e "${GREEN}Release ID: $RELEASE_ID${NC}"
        
        # Upload all release assets
        upload_asset() {
            local FILE_PATH="$1"
            local CONTENT_TYPE="$2"
            local FILENAME=$(basename "$FILE_PATH")
            
            echo -e "${CYAN}Uploading $FILENAME...${NC}"
            UPLOAD_RESPONSE=$(curl -s -X POST \
                -H "Authorization: token $GITHUB_TOKEN" \
                -H "Content-Type: $CONTENT_TYPE" \
                --data-binary @"$FILE_PATH" \
                "${UPLOAD_URL}?name=$FILENAME")
            
            if echo "$UPLOAD_RESPONSE" | grep -q '"state": "uploaded"'; then
                echo -e "${GREEN}Uploaded $FILENAME${NC}"
            else
                echo -e "${YELLOW}Upload response for $FILENAME: $(echo "$UPLOAD_RESPONSE" | grep -o '"message": "[^"]*"' || echo "check manually")${NC}"
            fi
        }
        
        # Upload all release assets from both platform directories
        for PLATFORM_DIR in "$RELEASES_PATH/win" "$RELEASES_PATH/osx"; do
            if [ -d "$PLATFORM_DIR" ]; then
                for FILE in "$PLATFORM_DIR"/*; do
                    [ -f "$FILE" ] || continue
                    FILENAME=$(basename "$FILE")
                    case "$FILENAME" in
                        *.json) CONTENT_TYPE="application/json" ;;
                        *.zip)  CONTENT_TYPE="application/zip" ;;
                        *)      CONTENT_TYPE="application/octet-stream" ;;
                    esac
                    upload_asset "$FILE" "$CONTENT_TYPE"
                done
            fi
        done
        
        echo -e "\n${GREEN}All assets uploaded to GitHub Release v$VERSION${NC}"
        echo -e "${CYAN}View at: https://github.com/$GITHUB_REPO/releases/tag/v$VERSION${NC}"
    else
        echo -e "${RED}Could not create or find release. Files are in $RELEASES_PATH - upload manually.${NC}"
        echo -e "${RED}API response: $CREATE_RESPONSE${NC}"
    fi
else
    echo -e "${YELLOW}Skipping GitHub upload (GITHUB_TOKEN not set in .env)${NC}"
    echo -e "${YELLOW}To upload, add GITHUB_TOKEN=your_token to your .env file${NC}"
fi

echo -e "\n${GREEN}Build complete!${NC}"
