#! /bin/bash

DEST_FOLDER="bin/Debug/win64"
COMMIT_HASH=$(git rev-parse --short HEAD)
BRANCH_NAME=$(git rev-parse --abbrev-ref HEAD)
BRANCH_NAME_CLEAN=$(echo "$BRANCH_NAME" | sed -e "s/\//-/")

rm -rf "$DEST_FOLDER"
mkdir -p "$DEST_FOLDER"

godot-mono --headless --export-debug "Windows Desktop" "$DEST_FOLDER/FishingGame.exe"
cp "steam_appid.txt" "$DEST_FOLDER"

cd "$DEST_FOLDER" || exit 1
zip -r "FishingGame-0.0.0-win64-$BRANCH_NAME_CLEAN-SNAPSHOT-$COMMIT_HASH-debug.zip" .
