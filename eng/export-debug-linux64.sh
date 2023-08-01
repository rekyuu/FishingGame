#! /bin/bash

DEST_FOLDER="bin/Debug/linux64"
COMMIT_HASH=$(git rev-parse --short HEAD)
BRANCH_NAME=$(git rev-parse --abbrev-ref HEAD)
BRANCH_NAME_CLEAN=$(echo "$BRANCH_NAME" | sed -e "s/\//-/")

rm -rf "$DEST_FOLDER"
mkdir -p "$DEST_FOLDER"

godot-mono --headless --export-debug "Linux/X11" "$DEST_FOLDER/FishingGame.x86_64"
cp "steam_appid.txt" "$DEST_FOLDER"

cd "$DEST_FOLDER" || exit 1
zip -r "FishingGame-0.0.0-linux64-$BRANCH_NAME_CLEAN-SNAPSHOT-$COMMIT_HASH-debug.zip" .
