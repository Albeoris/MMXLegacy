# MMXLegacy
Might and Magic X Rewritten Engine

# Documentation
Welcome to the our [Wiki](https://github.com/Albeoris/MMXLegacy/wiki)!

# Install
1. Update the game to the **latest** version.
2. Update MMXLegacy to the **latest** version.
3. Copy new libraries to "\Might and Magic X Legacy_Data\Managed"

# Build
1. Use Visual Studio 2017 or JetBrains Rider.
2. Copy "*.dll" from "\Might and Magic X Legacy_Data\Managed" to the "\References" folder in the solution directory.
3. Restore NuGet packages manualy or enable automaticaly package downloads.

# Restrictions (for developers)
1. **Please** don't change any data that can be sent to the game server! We don't want any trouble.
2. Don't change a serializable data that can be deserialized by the Unity Engine. The game will crash or corrupt.

# Debugging (for developers)
1. Run Legacy.Debugger.exe
2. Attach to the game process (Debug -> Attach Unity Debugger in the Visual Studio 2017 Tools for Unity)
