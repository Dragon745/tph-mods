# QualificationUtils Mod - How It Works

## Overview Flowchart

```mermaid
flowchart TD
    A[Game Starts] --> B[Unity Mod Manager Loads]
    B --> C[QualificationUtils Mod Initializes]
    C --> D[Harmony Patches Applied]
    D --> E[Mod Ready - Waiting for User Input]

    E --> F[User Right-Clicks Staff Member]
    F --> G[Inspector Menu Opens]
    G --> H[Harmony Patch Captures Staff Object]
    H --> I[Staff Object Stored in SelectedStaff]

    I --> J[User Opens Mod Menu Ctrl+F10]
    J --> K[OnGUI Method Executes]
    K --> L{Staff Selected?}

    L -->|No| M[Show 'No staff selected' Message]
    L -->|Yes| N[Display Staff Name and Controls]

    N --> O[Show Rank Management Section]
    N --> P[Show Qualification Management]
    N --> Q[Show Trait Management]
    N --> R[Show Transformation Buttons]

    O --> S[Rank Buttons 1-5]
    S --> T[User Clicks Rank]
    T --> U[SetRank() Called]
    U --> V[Salary Updated]

    P --> W[Remove Qualifications Section]
    P --> X[Add Qualifications Section]

    W --> Y[Show Current Qualifications as Buttons]
    Y --> Z[User Clicks Qualification]
    Z --> AA[Remove Qualification & Modifiers]

    X --> BB[Show Available Qualifications in ScrollView]
    BB --> CC[User Clicks Qualification]
    CC --> DD[Add Qualification & Modifiers]

    Q --> EE[Remove Traits Section]
    Q --> FF[Add Traits Section]

    EE --> GG[Show Current Traits as Buttons]
    GG --> HH[User Clicks Trait]
    HH --> II[Remove Trait & Modifiers]

    FF --> JJ[Show Available Traits in ScrollView]
    JJ --> KK[User Clicks Trait]
    KK --> LL[Add Trait & Modifiers]

    R --> MM[Perfect Assistant Button]
    R --> NN[Perfect GP Button]

    MM --> OO[MakePerfectAssistant() Called]
    NN --> PP[MakePerfectGP() Called]

    OO --> QQ[Transformation Process]
    PP --> QQ

    QQ --> RR[Set Rank to 5]
    RR --> SS[Remove All Qualifications]
    SS --> TT[Add Specific Qualifications]
    TT --> UU[Remove All Traits]
    UU --> VV[Add Specific Positive Traits]
    VV --> WW[Log Success Message]

    M --> E
    V --> E
    AA --> E
    DD --> E
    II --> E
    LL --> E
    WW --> E
```

## Detailed Process Flow

### 1. Initialization Phase

```
Game Launch → UMM Load → Mod Load → Harmony Patch → Ready
```

### 2. Staff Selection Phase

```
Right-Click Staff → Inspector Menu → Harmony Intercept → Store Staff Reference
```

### 3. Mod Interface Phase

```
Ctrl+F10 → OnGUI() → Check Staff → Display Controls → Wait for User Input
```

### 4. User Interaction Phase

#### Rank Management

```
Click Rank Button → SetRank() → Update Salary → Refresh Display
```

#### Qualification Management

```
Remove: Click Qual → Remove from Staff → Remove Modifiers → Refresh
Add: Scroll List → Click Qual → Add to Staff → Add Modifiers → Refresh
```

#### Trait Management

```
Remove: Click Trait → Remove from Staff → Remove Modifiers → Refresh
Add: Scroll List → Click Trait → Add to Staff → Add Modifiers → Refresh
```

### 5. Transformation Phase

#### Perfect Assistant Transformation

```
1. Set Rank to 5 (emp rank 5)
2. Remove All Existing Qualifications
3. Add: Customer Service, Customer Service II, Customer Service III, Emotional Int, Motivation
4. Remove All Existing Traits
5. Add: Charming, Entertainer, Fast Learner, Funny, Healer, Hygienic, Inspiring, Motivated, Positive, Teacher, Tireless
```

#### Perfect GP Transformation

```
1. Set Rank to 5 (emp rank 5)
2. Remove All Existing Qualifications
3. Add: General Practice, General Practice II, General Practice III, General Practice IV, General Practice V
4. Remove All Existing Traits
5. Add: Same 11 positive traits as Assistant
```

## Key Components

### Harmony Patch System

- **File**: `InspectorMenu_Inspect_Patch.cs`
- **Purpose**: Intercepts staff selection and stores reference
- **Method**: Patches the Inspector Menu's inspect method
- **Output**: Populates `SelectedStaff` static field

### Main Mod Logic

- **File**: `Main.cs`
- **Entry Point**: `Load()` method called by UMM
- **GUI Handler**: `OnGUI()` method for interface
- **Core Methods**: Individual transformation and management methods

### Data Flow

```
Game State → Harmony Patch → Mod Variables → GUI Display → User Actions → Game State Changes
```

### Error Handling

- Try-catch blocks around all major operations
- Logging of success/failure messages
- Graceful fallback for missing data
- Validation of staff compatibility

## State Management

### Persistent State

- `SelectedStaff`: Currently selected staff member
- `_qualificationScrollPosition`: Scroll position for qualification list
- `_traitScrollPosition`: Scroll position for trait list

### Runtime State

- Current game level and managers
- Available qualifications and traits
- Staff's current qualifications and traits
- Modifier components for applying effects

## Integration Points

### Game API Integration

- **TH20 Namespace**: Access to game classes and methods
- **Reflection**: Access to private fields via `Traverse.Create`
- **Unity Integration**: GUI system and object management

### Mod Manager Integration

- **UMM Framework**: Mod lifecycle and GUI management
- **Harmony Integration**: Runtime code patching
- **Logging System**: Error reporting and debugging

## Performance Considerations

### Lazy Loading

- Game objects only accessed when needed
- Managers retrieved on-demand
- Lists cached during single operations

### Memory Management

- No persistent object references
- Cleanup of temporary collections
- Minimal allocation during operations

### Update Frequency

- GUI updates only when mod menu is open
- Trait availability updates when game is not paused
- Real-time response to user actions
