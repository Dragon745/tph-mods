# QualificationUtils Mod for Two Point Hospital

A comprehensive mod for Two Point Hospital that allows players to easily manage staff qualifications, traits, and create specialized staff members with a single click.

## Features

### 🎯 **Staff Management**

- **Rank Management**: Set staff rank from 1-5 with a single click
- **Qualification Control**: Add or remove individual qualifications
- **Trait Management**: Add or remove character traits individually

### 🚀 **One-Click Transformations**

- **Perfect Assistant**: Transform any staff member into a perfect customer service assistant
- **Perfect GP**: Transform any staff member into a perfect general practitioner

### 🔧 **Advanced Features**

- Real-time trait availability updates (when game is not paused)
- Automatic salary adjustment when changing ranks
- Proper modifier management for qualifications and traits
- Comprehensive error handling and logging

## Installation

### Prerequisites

- Two Point Hospital (Steam version)
- Unity Mod Manager (UMM) installed and configured

### Steps

1. Download the latest release from this repository
2. Extract the `QualificationUtils` folder to your UMM mods directory
3. Enable the mod through Unity Mod Manager
4. Start or load a game

## How to Use

### Basic Usage

1. **Select Staff**: Use the Inspector Menu (right-click on staff) to select a staff member
2. **Open Mod Menu**: Press the mod hotkey (usually `Ctrl+F10`) to open the QualificationUtils interface
3. **Manage Staff**: Use the various buttons and controls to modify your selected staff member

### Staff Transformations

#### Make Perfect Assistant

- Sets staff to rank 5
- Removes all existing qualifications
- Adds: Customer Service, Customer Service II, Customer Service III, Emotional Int, Motivation
- Removes all existing traits
- Adds positive traits: Charming, Entertainer, Fast Learner, Funny, Healer, Hygienic, Inspiring, Motivated, Positive, Teacher, Tireless

#### Make Perfect GP

- Sets staff to rank 5
- Removes all existing qualifications
- Adds: General Practice, General Practice II, General Practice III, General Practice IV, General Practice V
- Removes all existing traits
- Adds the same positive traits as the Assistant transformation

### Manual Management

#### Rank Management

- Click on rank numbers (1-5) to set staff rank
- Current rank is displayed above the buttons
- Salary automatically adjusts to match the new rank

#### Qualification Management

- **Remove**: Click on qualification names to remove them (required qualifications cannot be removed)
- **Add**: Scroll through available qualifications and click to add them
- Only qualifications valid for the selected staff type will be shown

#### Trait Management

- **Remove**: Click on active trait names to remove them
- **Add**: Scroll through available traits and click to add them
- Traits update in real-time when the game is not paused
- Only traits valid for the staff member will be shown

## Technical Details

### Dependencies

- **Harmony12**: For runtime code patching
- **UnityModManagerNet**: For mod framework and GUI
- **TH20**: Two Point Hospital game API

### Architecture

- **Main.cs**: Core mod logic and GUI implementation
- **InspectorMenu_Inspect_Patch.cs**: Harmony patch for staff selection
- **Info.json**: Mod metadata and configuration

### Compatibility

- **Game Version**: Two Point Hospital (latest)
- **Framework**: .NET Framework 4.8
- **Platform**: Windows

## Troubleshooting

### Common Issues

#### Mod Not Loading

- Ensure Unity Mod Manager is properly installed
- Check that all DLL dependencies are present
- Verify the mod folder structure is correct

#### Staff Selection Issues

- Make sure you're using the Inspector Menu (right-click on staff)
- The mod only works with staff members, not patients
- Try selecting a different staff member

#### Qualifications/Traits Not Adding

- Check if the staff member has available qualification slots
- Verify the staff type is compatible with the qualification/trait
- Ensure the game is not paused for trait updates

### Error Reporting

If you encounter issues:

1. Check the Unity Mod Manager console for error messages
2. Look for specific error codes in the mod's log output
3. Report issues with the error message and steps to reproduce

## Development

### Building from Source

1. Clone this repository
2. Open `QualificationUtils.sln` in Visual Studio
3. **Configure your TPH installation path** (see Configuration section below)
4. Build the solution
5. The mod will automatically build to your TPH mods directory

### Configuration

#### Setting Your TPH Installation Path

**IMPORTANT**: Before building, you must update the TPH installation path in the project file to match your system.

**File**: `QualificationUtils/QualificationUtils.csproj`  
**Lines to edit**: 22-23

```xml
<TPHRootFolder>D:\SteamLibrary\steamapps\common\TPH</TPHRootFolder>
<ModsFolder>$(TPHRootFolder)\Mods\QualificationUtils</ModsFolder>
```

**Change the path on line 22** to match your Two Point Hospital installation:

**Common Steam paths:**

- **Steam (C: drive)**: `C:\Program Files (x86)\Steam\steamapps\common\TPH`
- **Steam (D: drive)**: `D:\SteamLibrary\steamapps\common\TPH`
- **Steam (E: drive)**: `E:\SteamLibrary\steamapps\common\TPH`
- **Steam (F: drive)**: `F:\SteamLibrary\steamapps\common\TPH`

**Epic Games Store paths:**

- **Epic (C: drive)**: `C:\Program Files\Epic Games\TPH`
- **Epic (D: drive)**: `D:\Epic Games\TPH`

**Example for your system:**
If your TPH is installed at `C:\Games\TwoPointHospital`, change line 22 to:

```xml
<TPHRootFolder>C:\Games\TwoPointHospital</TPHRootFolder>
```

**Line 23 will automatically update** since it references the `TPHRootFolder` variable.

#### What Gets Built Where

After configuration, the build process will:

- **Source files**: Compiled from `QualificationUtils/` folder
- **Output location**: `[YourTPHPath]\Mods\QualificationUtils\`
- **Files included**: `QualificationUtils.dll`, `Info.json`, and all dependencies

#### DLL References and Dependencies

**IMPORTANT**: The project references several DLLs from your TPH installation. These paths are automatically updated when you change the `TPHRootFolder`, but you should verify they're correct.

**File**: `QualificationUtils/QualificationUtils.csproj`  
**Lines**: 45-75 (Reference section)

**All DLL references use the `$(TPHRootFolder)` variable, so they automatically update when you change line 22:**

```xml
<!-- Harmony12 - Runtime patching library -->
<Reference Include="Harmony12">
  <HintPath>$(TPHRootFolder)\TPH_Data\Managed\UnityModManager\0Harmony12.dll</HintPath>
  <Private>False</Private>
</Reference>

<!-- Game assembly files -->
<Reference Include="Assembly-CSharp">
  <HintPath>$(TPHRootFolder)\TPH_Data\Managed\Assembly-CSharp.dll</HintPath>
  <Private>False</Private>
</Reference>

<Reference Include="Assembly-CSharp-firstpass">
  <HintPath>$(TPHRootFolder)\TPH_Data\Managed\Assembly-CSharp-firstpass.dll</HintPath>
  <Private>False</Private>
</Reference>

<!-- Unity engine modules -->
<Reference Include="UnityEngine">
  <HintPath>$(TPHRootFolder)\TPH_Data\Managed\UnityEngine.dll</HintPath>
  <Private>False</Private>
</Reference>

<Reference Include="UnityEngine.CoreModule">
  <HintPath>$(TPHRootFolder)\TPH_Data\Managed\UnityEngine.CoreModule.dll</HintPath>
  <Private>False</Private>
</Reference>

<Reference Include="UnityEngine.IMGUIModule">
  <HintPath>$(TPHRootFolder)\TPH_Data\Managed\UnityEngine.IMGUIModule.dll</HintPath>
  <Private>False</Private>
</Reference>

<Reference Include="UnityEngine.TextRenderingModule">
  <HintPath>$(TPHRootFolder)\TPH_Data\Managed\UnityEngine.TextRenderingModule.dll</HintPath>
  <Private>False</Private>
</Reference>

<!-- Unity Mod Manager -->
<Reference Include="UnityModManager">
  <HintPath>$(TPHRootFolder)\TPH_Data\Managed\UnityModManager\UnityModManager.dll</HintPath>
</Reference>
```

**What this means:**

- **Only change line 22** (`TPHRootFolder`) - all DLL paths update automatically
- **All DLLs are located in**: `[YourTPHPath]\TPH_Data\Managed\`
- **Harmony and UMM are in**: `[YourTPHPath]\TPH_Data\Managed\UnityModManager\`
- **Game assemblies are in**: `[YourTPHPath]\TPH_Data\Managed\`

**If you have a non-standard TPH installation structure**, you may need to manually adjust individual DLL paths on lines 45-75.

#### Verification Steps

1. **Check your TPH installation path** by looking in Steam/Epic for the game location
2. **Update line 22** in `QualificationUtils.csproj` with the correct path
3. **Build the solution** - you should see output going to your TPH mods folder
4. **Verify the build** by checking `[YourTPHPath]\Mods\QualificationUtils\` contains the mod files

### Project Structure

```
QualificationUtils/
├── QualificationUtils/
│   ├── Main.cs                    # Core mod logic
│   ├── Patches/                   # Harmony patches
│   │   └── InspectorMenu_Inspect_Patch.cs
│   ├── Properties/
│   │   └── AssemblyInfo.cs
│   ├── Info.json                  # Mod metadata
│   └── QualificationUtils.csproj  # Project file
└── QualificationUtils.sln         # Solution file
```

## Contributing

Contributions are welcome! Please feel free to submit issues, feature requests, or pull requests.

### Development Guidelines

- Follow the existing code style and structure
- Add proper error handling for new features
- Test thoroughly before submitting changes
- Update documentation for new features

## License

This mod is provided as-is for educational and entertainment purposes. Use at your own risk.

## Credits

- **Original Game**: Two Point Hospital by Two Point Studios
- **Mod Framework**: Unity Mod Manager
- **Patching Library**: Harmony

## Version History

### v1.0.0

- Initial release with basic qualification and trait management
- Perfect Assistant transformation
- Perfect GP transformation
- Comprehensive staff management tools

---

**Note**: This mod modifies game files at runtime. While it's designed to be safe, always backup your save files before using mods.
