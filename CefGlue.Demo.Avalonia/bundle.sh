create_app_structure() {
    APPNAME=$1
    APPDIR="$APPNAME.app/Contents"
    APPICONS="/System/Library/CoreServices/CoreTypes.bundle/Contents/Resources/GenericApplicationIcon.icns"

    if [ ! -d "$APPDIR" ]; then
        echo "creating app structure $APPDIR"

        mkdir -vp "$APPDIR"/{MacOS,Resources,Frameworks}
        cp -v "$APPICONS" "$APPDIR/Resources/$APPNAME.icns"
    fi
}

emit_plist() {
    PLIST_APPNAME=$1
    PLIST_PATH="$2/Info.plist" 
    if [ ! -f "$PLIST_PATH" ]; then
        echo "emiting $PLIST_PATH"
        cat <<EOF > "$PLIST_PATH"
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleExecutable</key>
    <string>$PLIST_APPNAME</string>
    <key>CFBundleGetInfoString</key>
    <string>$PLIST_APPNAME</string>
    <key>CFBundleIconFile</key>
    <string>$PLIST_APPNAME</string>
    <key>CFBundleName</key>
    <string>$PLIST_APPNAME</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleSignature</key>
    <string>4242</string>
</dict>
</plist>
EOF
    fi
}

NETTARGET="netcoreapp3.1"
echo "building cefglue avalonia demo"
dotnet publish CefGlue.Demo.Avalonia.csproj -r osx-x64 -f "$NETTARGET" --self-contained

echo "building cefglue browser process"
dotnet publish ../CefGlue.BrowserProcess/CefGlue.BrowserProcess.csproj -r osx-x64 -f "$NETTARGET" --self-contained

TARGET="tmp"
if [ ! -d "$TARGET" ]; then
    mkdir "$TARGET"
fi

cd "$TARGET"

CEFZIP="cef.tar.bz2"
CEFBINARIES="cef_binaries"
if [ ! -f "$CEFZIP" ]; then
    echo "downloading cef binaries"
    curl -o "$CEFZIP" "https://cef-builds.spotifycdn.com/cef_binary_85.3.12%2Bg3e94ebf%2Bchromium-85.0.4183.121_macosx64_minimal.tar.bz2"
fi

if [ ! -d "$CEFBINARIES" ]; then
    echo "unzipping cef binaries"
    mkdir "$CEFBINARIES"
    tar -jxvf "$CEFZIP" -C "./$CEFBINARIES"
fi

CEFFRAMEWORK_DIR="$(find $CEFBINARIES -name "Release")"

APPNAME="Xilium.CefGlue.Demo.Avalonia"
APPDIR="$APPNAME.app/Contents"

create_app_structure "$APPNAME"
emit_plist "$APPNAME" "$APPDIR"

cp -R "$CEFFRAMEWORK_DIR/Chromium Embedded Framework.framework" "$APPDIR/Frameworks/"
cp "$APPDIR/Frameworks/Chromium Embedded Framework.framework/Chromium Embedded Framework" "$APPDIR/MacOS/libcef"

cp -R "../bin/Debug/$NETTARGET/osx-x64/publish/" "$APPDIR/MacOS"
chmod +x "$APPDIR/MacOS/$APPNAME"

cd "$APPDIR/Frameworks"

APPNAME="Xilium.CefGlue.Demo.Avalonia Helper"
APPDIR="$APPNAME.app/Contents"
create_app_structure "$APPNAME"
emit_plist "$APPNAME" "$APPDIR"
cp -R "Chromium Embedded Framework.framework" "$APPDIR/Frameworks/"

pwd
cp -R "../../../../../CefGlue.BrowserProcess/bin/Debug/$NETTARGET/osx-x64/publish/" "$APPDIR/MacOS"
cp "$APPDIR/Frameworks/Chromium Embedded Framework.framework/Chromium Embedded Framework" "$APPDIR/MacOS/libcef"
mv "$APPDIR/MacOS/Xilium.CefGlue.BrowserProcess" "$APPDIR/MacOS/$APPNAME"
chmod +x "$APPDIR/MacOS/$APPNAME"




