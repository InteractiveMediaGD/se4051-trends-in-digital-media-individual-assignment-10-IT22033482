# The Sentient Forest: Flowbound Grove
## Complete Step-by-Step Build Guide (Beginner Friendly)

**Unity version:** 6000.2.6f1 (Unity 6)  
**Project folder:** `IT22033482_TDM`  
**Approach:** Build with primitives first (planes, cubes, spheres, capsules) → add scripts → test → replace with 3D models later.

---

## How to use this guide

1. Work through **one phase at a time** — do not skip ahead until the current phase works in Play mode.
2. Check off items in the **Master Todo List** as you complete them.
3. All scripts are already in `Assets/Scripts/` — you only need to attach them and drag references in the Inspector.
4. When stuck, search this doc for the phase name.

### Prefab rule (used throughout this guide)

**If you need more than one of the same object, build it once → save as prefab → drag copies into the scene.**

| Do this | Avoid this |
|---------|------------|
| Create `LightSeed_Prefab`, place 3 in scene | Build 3 separate seeds and attach script 3 times manually |
| Create `ShadowZone_Prefab`, place 2 in scene | Duplicate with Ctrl+D without prefab link |
| Edit prefab to change material | Change material on each copy one by one |

**Prefab workflow (same every time):**
1. Build object in Hierarchy (mesh + collider + script + material)
2. Drag from Hierarchy → `Assets/Prefabs/`
3. Drag prefab from Project into Scene for each copy
4. Move each instance — only change **position** (and optional scale) per instance

**Edit all copies:** double-click prefab in Project → Prefab Mode → save → all instances update.

---

## Master Todo List

Copy this list and tick items as you go.

### Phase 0 — Setup
- [ ] Open Unity project `IT22033482_TDM`
- [ ] Confirm scripts exist in `Assets/Scripts/`
- [ ] Confirm folders exist: `Materials`, `Prefabs`, `Audio`, `Scripts`, `Scenes`
- [ ] Save scene as `Assets/Scenes/ForestScene.unity`

### Phase 1 — World with primitives
- [ ] Create Ground (Plane, scale 5×1×5)
- [ ] Build one Tree → `Tree_Prefab` → place 8–12 instances
- [ ] Build one Rock → `Rock_Prefab` → place 4–6 instances
- [ ] Set Directional Light rotation (50, -30, 0)
- [ ] Enable Fog in Lighting window
- [ ] Create materials; assign via prefabs (one edit updates all copies)

### Phase 2 — Player & camera
- [ ] Create Player prefab setup (Capsule, Tag = Player, Rigidbody, PlayerMover)
- [ ] Optional: save as `Player_Prefab` for reuse across scenes
- [ ] Parent Main Camera to Player
- [ ] Test WASD movement in Play mode

### Phase 3 — Responsive UI + GameManager
- [ ] Create Canvas + Canvas Scaler (responsive)
- [ ] Build `UI_Root` with panels and 5 TextMeshPro texts
- [ ] Test UI at multiple resolutions
- [ ] Create `GameManager` + wire scripts and UI
- [ ] Save `UI_Root_Prefab` to `Assets/Prefabs/`

### Phase 4 — Collectibles (Light Seeds)
- [ ] Build one seed with script, collider, light, material
- [ ] Save as `LightSeed_Prefab` → place 3 instances in different areas
- [ ] Test: walk into seed → count increases → seed disappears

### Phase 5 — Shadow Zones & Safe Zone
- [ ] Build one Shadow Zone → `ShadowZone_Prefab` → place 2 instances
- [ ] Build one Safe Zone → `SafeZone_Prefab` (or single scene object)
- [ ] Test: enter shadow → fog/light/stress change; safe zone lowers stress

### Phase 6 — Forest Heart (win)
- [ ] Create `ForestHeart` (unique object — one per scene)
- [ ] Attach `ForestHeart.cs`, assign materials, link to GameManager
- [ ] Test: win after 3 seeds

### Phase 7 — Luma guide NPC + voice (TTS)
- [ ] Create `Luma_NPC` (unique — one guide per scene)
- [ ] Attach `LumaGuide.cs` + `LumaTTS.cs`, assign Player
- [ ] Wait for Console: no red errors, then add components
- [ ] Test: follow / warn / guide / celebrate + Luma speaks (Windows)

### Phase 8 — Shadow Wisps (enemies)
- [ ] Build one Wisp → `ShadowWisp_Prefab` → place 2 instances
- [ ] Assign Player on prefab (all copies inherit it)
- [ ] Test: detection + follow + stress

### Phase 9 — Guide path & accessibility
- [ ] Build one marker → `GuideMarker_Prefab` → 6–8 instances under `GuidePath`
- [ ] Disable markers by default; wire `GuidePathController` array
- [ ] Test H key and stress ≥ 61 auto-show; test R key

### Phase 10 — Audio (optional polish)
- [ ] Add audio clips to `AudioManager` on GameManager (already attached in Phase 3)
- [ ] Test calm / danger / collect / win sounds

### Phase 11 — Replace primitives (edit prefabs)
- [ ] Swap tree / seed / wisp / player models inside prefabs

### Phase 12 — Start screen + End screen
- [ ] Build `StartScreenPanel` (logo, loading bar, Start button)
- [ ] Build `EndScreenPanel` (win icon, Restart, Exit)
- [ ] Add `GameFlowManager` to GameManager and wire all panels/buttons
- [ ] Hook button OnClick events
- [ ] Test: start → play → win → end screen → restart / exit

### Phase 13 — Submission
- [ ] Playthrough test, demo video, proposal, self-study, zip, upload

---

## Phase 0 — Project setup

### Step 0.1 Open the project
1. Open **Unity Hub** → open `IT22033482_TDM`.
2. Wait for scripts to compile (bottom-right spinner finishes).

### Step 0.2 Confirm asset folders
These folders should exist under `Assets` (create any that are missing):

| Folder | Used for |
|--------|----------|
| `Scripts` | C# game logic (already filled) |
| `Scenes` | `ForestScene.unity` |
| `Materials` | Colors and surface looks |
| `Prefabs` | Reusable objects (trees, seeds, zones, UI, etc.) |
| `Audio` | Sound clips (optional) |

### Step 0.3 Save your scene
1. Open `Assets/Scenes/SampleScene.unity`
2. **File → Save As** → `ForestScene.unity`

---

## Phase 1 — Build the world (primitives only)

### Step 1.1 Ground
1. **Hierarchy → Right-click → 3D Object → Plane**
2. Rename: `Ground`
3. **Transform:** Position (0, 0, 0), Scale (5, 1, 5)

### Step 1.2 Trees (using a Prefab)

**Why a prefab?** You build the tree once, save it as a prefab, then drag copies into the scene. If you later change the prefab (material, scale, add leaves model), **every tree updates automatically**. This is better than duplicating with Ctrl+D.

#### A) Build the first tree
1. **Hierarchy → Right-click → 3D Object → Cylinder**
2. Rename: `Tree_01`
3. **Transform:** Scale (0.4, 2, 0.4), Position on ground (Y = 1 so it sits on the plane)

#### B) Turn it into a prefab
1. Drag `Tree_01` from **Hierarchy** into `Assets/Prefabs/` folder in the **Project** panel
2. Unity creates **`Tree_Prefab`** (blue cube icon in Project)
3. The Hierarchy object turns **blue** — that means it is linked to the prefab

#### C) Place many trees
**Method 1 — Drag from Project (recommended):**
1. Click `Tree_Prefab` in `Assets/Prefabs/`
2. Drag it into the **Scene view** or **Hierarchy** repeatedly
3. Move each instance to a different spot
4. Place **8–12 trees** total

**Method 2 — Duplicate a prefab instance:**
1. Select any blue `Tree_01` in Hierarchy
2. **Ctrl+D** to duplicate (still a prefab instance)
3. Move the copy — repeat until you have 8–12 trees

**Optional variation per tree:** Select one instance → in Inspector click **Overrides → Transform** and change scale slightly (e.g. Y = 1.8 or 2.2) so trees are not identical.

#### D) Clean up naming
- Rename instances in Hierarchy: `Tree_02`, `Tree_03`, etc. (optional, for organization)
- You can delete the original `Tree_01` name confusion — each instance can keep auto names like `Tree_Prefab (1)`

**Prefab tip:** To edit ALL trees at once → double-click `Tree_Prefab` in Project → edit in Prefab Mode → **Save** → all instances update.

---

### Step 1.3 Rocks (using a Prefab, optional)

Same prefab workflow as trees:

#### A) Build the first rock
1. **3D Object → Cube** → rename `Rock_01`
2. Scale roughly (0.5, 0.4, 0.5), rotate slightly on Y (e.g. 15°)

#### B) Save as prefab
1. Drag `Rock_01` into `Assets/Prefabs/` → creates **`Rock_Prefab`**

#### C) Place 4–6 rocks
1. Drag `Rock_Prefab` into the scene multiple times
2. Move each copy to different positions
3. Per instance: slightly change **Scale** and **Rotation** in Inspector for variety (Overrides are fine)

**Note:** Random scale on each instance is OK — you do not need a separate prefab for every rock size.

### Step 1.4 Lighting & fog
1. Select **Directional Light** → Rotation (50, -30, 0)
2. **Window → Rendering → Lighting → Environment**
3. Enable **Fog**, Mode = Exponential, Density = 0.02, Color = light green

### Step 1.5 Materials (basic colors)
1. Right-click `Assets/Materials` → **Create → Material**
2. Create: `Mat_Ground` (green), `Mat_Tree` (dark green), `Mat_Rock` (gray)
3. Drag `Mat_Ground` onto **Ground**
4. **For trees and rocks — edit the prefab (updates all copies):**
   - In Project, **double-click `Tree_Prefab`** → Prefab Mode opens
   - Drag `Mat_Tree` onto the cylinder mesh → click **← (back arrow)** to exit Prefab Mode
   - Repeat for **`Rock_Prefab`** with `Mat_Rock`

**Why edit the prefab?** One change applies to every tree/rock in the scene instantly.

**Checkpoint:** Scene looks like a simple flat forest. No scripts yet.

---

## Phase 2 — Player and camera

### Step 2.1 Create Player tag
1. Select any object → Inspector → **Tag → Add Tag**
2. Add tag named `Player`

### Step 2.2 Player object
1. **3D Object → Capsule** → rename `Player`
2. Tag = **Player**
3. Position: (0, 1, 0)
4. **Add Component → Rigidbody**
   - Mass = 1
   - Constraints → Freeze Rotation X, Y, Z
5. **Add Component →** drag script `PlayerMover` from `Assets/Scripts`

### Step 2.3 Save Player as prefab (optional but good practice)
There is only one player per scene, but a prefab helps if you add more scenes later.

1. Drag `Player` (with camera still unparented OR parented — your choice) into `Assets/Prefabs/`
2. Name it **`Player_Prefab`**
3. The Hierarchy Player turns blue (linked to prefab)

**Note:** If you parent the camera first (Step 2.4), save the prefab **after** camera is attached so the prefab includes both.

### Step 2.4 Camera
**Easy method (recommended for beginners):**
1. Select **Main Camera**
2. Drag it **onto Player** in Hierarchy (camera becomes child)
3. Select Main Camera → Position (0, 8, -10), Rotation (35, 0, 0)

**Checkpoint:** Press **Play** → WASD moves the player, camera follows.

---

## Phase 3 — Responsive UI and GameManager

This phase builds UI that **scales correctly on any screen** (laptop, monitor, fullscreen, different aspect ratios). We use **anchors + Canvas Scaler** — the standard Unity approach for responsive UI.

### What “responsive” means in Unity

| Concept | What it does |
|---------|----------------|
| **Canvas Scaler** | Scales the whole UI up/down based on screen size |
| **Anchors** | Pins UI elements to edges/corners so they stay in place when the screen changes |
| **Rect Transform offsets** | Padding from the anchored edge (like CSS margin) |

**Reference resolution we use:** `1920 × 1080` (Full HD). Unity scales from this to fit your actual screen.

---

### Step 3.1 — Create the Canvas

1. In **Hierarchy**, right-click → **UI → Canvas**
2. Unity also creates **EventSystem** automatically — leave it alone.
3. Select **Canvas** in Hierarchy.
4. In **Inspector**, confirm these components exist:
   - **Rect Transform**
   - **Canvas** (Render Mode = *Screen Space - Overlay*)
   - **Canvas Scaler**
   - **Graphic Raycaster**

5. If a popup asks to import **TextMeshPro Essentials** → click **Import TMP Essentials** (required for UI text).

---

### Step 3.2 — Configure Canvas Scaler (fit any screen)

With **Canvas** selected:

1. Find **Canvas Scaler** component in Inspector.
2. Set exactly:

| Setting | Value | Why |
|---------|-------|-----|
| **UI Scale Mode** | `Scale With Screen Size` | UI grows/shrinks with resolution |
| **Reference Resolution** | X = `1920`, Y = `1080` | Design size we build against |
| **Screen Match Mode** | `Match Width Or Height` | Balances wide vs tall screens |
| **Match** | `0.5` | 50% width + 50% height — good default |

3. Leave **Canvas** component as:
   - Render Mode = **Screen Space - Overlay**
   - Pixel Perfect = **unchecked** (scaler handles sizing)

**Quick test:** Open **Game** tab → use the aspect ratio dropdown (top-left of Game view) → try **16:9**, **16:10**, **Free Aspect**. UI should stay readable (after you add text in later steps).

---

### Step 3.3 — Create UI_Root (full-screen container)

All UI lives inside one root panel so everything scales together.

1. Right-click **Canvas** → **UI → Panel**
2. Rename the panel to `UI_Root`
3. Select `UI_Root` → in **Rect Transform**:
   - Click the **Anchor Presets** box (square icon, top-left of Rect Transform)
   - Hold **Alt + Shift** and click **bottom-right preset** (stretch-stretch — fills entire screen)

   This sets:
   - Anchor Min = (0, 0)
   - Anchor Max = (1, 1)
   - Left, Top, Right, Bottom offsets = **0**

4. On **Image** component of UI_Root:
   - Set **Color** alpha to **0** (fully transparent) — it is only a layout container, not visible.

---

### Step 3.4 — Create layout panels (Top, Stats, Bottom)

Create three child panels under `UI_Root`. Each uses anchors so text stays in the correct screen region on any resolution.

#### A) TopPanel (objective text area)

1. Right-click `UI_Root` → **UI → Panel** → rename `TopPanel`
2. **Rect Transform:**
   - Anchor preset: **top stretch** (top row, middle icon — stretch horizontally, pinned to top)
   - Left = `40`, Right = `40`, Top = `0`, Height = `120`
3. **Image** color: black with alpha ~`120` (semi-transparent bar for readability)

#### B) StatsPanel (seed + stress counters)

1. Right-click `UI_Root` → **UI → Panel** → rename `StatsPanel`
2. **Rect Transform:**
   - Anchor preset: **top-left** (corner preset)
   - Pos X = `140`, Pos Y = `-160`
   - Width = `280`, Height = `100`
3. **Image** color: black alpha ~`100`

#### C) BottomPanel (status + controls)

1. Right-click `UI_Root` → **UI → Panel** → rename `BottomPanel`
2. **Rect Transform:**
   - Anchor preset: **bottom stretch**
   - Left = `40`, Right = `40`, Bottom = `0`, Height = `140`
3. **Image** color: black alpha ~`120`

**Your Hierarchy should look like:**

```
Canvas
├── UI_Root
│   ├── TopPanel
│   ├── StatsPanel
│   └── BottomPanel
└── EventSystem
```

---

### Step 3.5 — Create each TextMeshPro element (one at a time)

For every text below: right-click the **parent panel** listed → **UI → Text - TextMeshPro**.

After creating each text, select it and configure **Rect Transform** + **TextMeshPro** as described.

---

#### Text 1: ObjectiveText (inside TopPanel)

1. Parent: `TopPanel`
2. Rename: `ObjectiveText`
3. **Rect Transform:**
   - Anchor preset: **stretch-stretch** (Alt+Shift + bottom-right preset)
   - Left = `20`, Right = `20`, Top = `10`, Bottom = `10`
4. **TextMeshPro — Text Input:**
   - Text: `Collect 3 Light Seeds to restore the Forest Heart.`
5. **TextMeshPro settings:**

| Setting | Value |
|---------|-------|
| Font Size | `32` |
| Alignment | Center + Middle (horizontal and vertical center icons) |
| Color | White or light yellow |
| Wrapping | Enabled |
| Overflow | Overflow (or Ellipsis) |
| Auto Size | Enabled, Min `18`, Max `36` |

**Auto Size** makes long text shrink on small screens automatically.

---

#### Text 2: SeedCountText (inside StatsPanel)

1. Parent: `StatsPanel`
2. Rename: `SeedCountText`
3. **Rect Transform:**
   - Anchor: **top-left** of StatsPanel
   - Pos X = `15`, Pos Y = `-15`
   - Width = `250`, Height = `40`
4. Text: `Seeds: 0 / 3`
5. Font Size: `26`, Alignment: **Left + Top**, Color: light green

---

#### Text 3: StressText (inside StatsPanel, below seed count)

1. Parent: `StatsPanel`
2. Rename: `StressText`
3. **Rect Transform:**
   - Anchor: **top-left**
   - Pos X = `15`, Pos Y = `-55`
   - Width = `250`, Height = `40`
4. Text: `Stress: 0`
5. Font Size: `26`, Alignment: **Left + Top**, Color: light orange

---

#### Text 4: StatusText (inside BottomPanel)

1. Parent: `BottomPanel`
2. Rename: `StatusText`
3. **Rect Transform:**
   - Anchor: **stretch-stretch** inside BottomPanel
   - Left = `20`, Right = `20`, Top = `10`, Bottom = `50`
4. Text: `Welcome to Flowbound Grove.`
5. Font Size: `28`, Auto Size Min `16` Max `30`
6. Alignment: **Center + Middle**
7. Wrapping: **Enabled**

---

#### Text 5: ControlsHintText (inside BottomPanel, bottom row)

1. Parent: `BottomPanel`
2. Rename: `ControlsHintText`
3. **Rect Transform:**
   - Anchor preset: **bottom stretch**
   - Left = `20`, Right = `20`, Bottom = `8`, Height = `35`
4. Text: `WASD = Move  |  H = Guide  |  R = Reduced Intensity`
5. Font Size: `20`, Auto Size Min `12` Max `22`
6. Alignment: **Center + Bottom**
7. Color: gray (slightly dimmer than status text)

---

### Step 3.6 — How anchors work (read once)

When you change screen size, Unity moves/scales UI based on anchors:

```
Top stretch     → stays at top, stretches wider on wide screens
Bottom stretch  → stays at bottom
Top-left        → stays in top-left corner
Stretch-stretch → fills its parent panel
```

**Rule:** Always anchor to the edge where you want the UI to “stick”, then use Left/Right/Top/Bottom offsets as padding.

**Common mistake:** Using **center anchor** with fixed Pos X/Y — text floats in the wrong place on different resolutions. Use edge anchors instead.

---

### Step 3.7 — Test responsive UI before wiring scripts

1. Click the **Game** tab (not Scene).
2. At the top of Game view, find the **aspect ratio dropdown** (may say "Free Aspect" or "16:9").
3. Test these sizes:

| Test | How | Expected |
|------|-----|----------|
| Full HD | 1920×1080 | All text visible, good spacing |
| Laptop | 1366×768 | Text scales down slightly, still readable |
| Wide | 2560×1080 ultrawide | Top/bottom bars stretch, text stays in place |
| Tall | 9:16 portrait | Layout still works; Auto Size shrinks text |

4. While in Play mode is NOT required yet — just resize Game view.

**Fix if text is cut off:**
- Increase panel Height (TopPanel / BottomPanel)
- Enable Auto Size on that text
- Enable Wrapping on long strings

---

### Step 3.8 — Create and wire GameManager

1. **Hierarchy → Create Empty** → rename `GameManager`
2. **Add Component** and add these scripts:
   - `GameManager`
   - `GuidePathController`
   - `AccessibilityController`
   - `AudioManager` (optional — works without audio clips)

3. Select `GameManager` → in **GameManager** component, drag references:

| Slot in Inspector | Drag from Hierarchy |
|-------------------|---------------------|
| Objective Text | `Canvas/UI_Root/TopPanel/ObjectiveText` |
| Seed Count Text | `Canvas/UI_Root/StatsPanel/SeedCountText` |
| Stress Text | `Canvas/UI_Root/StatsPanel/StressText` |
| Status Text | `Canvas/UI_Root/BottomPanel/StatusText` |
| Directional Light | `Directional Light` |

4. (Later, after creating Luma and Forest Heart) also drag:
   - **Forest Heart** → `Forest Heart` slot
   - **Luma_NPC** → `Luma Guide` slot

**Note:** `ControlsHintText` is static — it does not need a script slot. It never changes during gameplay.

---

### Step 3.9 — Save UI as prefab (recommended)

Once UI looks correct:

1. Drag `UI_Root` from Hierarchy into `Assets/Prefabs/` folder
2. Name it `UI_Root_Prefab`

If you break the UI later, delete Canvas and drag the prefab back into the scene.

---

**Checkpoint — Phase 3 complete when:**
- [ ] All 5 texts visible at 1920×1080 and 1366×768
- [ ] Top bar shows objective, bottom bar shows status + controls
- [ ] Stats show Seeds and Stress (Stress/Seeds update once gameplay is connected)
- [ ] GameManager has all 4 text slots assigned (no "None" for required fields)
- [ ] Press **Play** — no errors in Console, UI stays on screen while moving

---

## Phase 4 — Light Seeds (collectibles, using a Prefab)

Build **one** seed completely, save as prefab, then place **3 copies**. Do not build three separate seeds from scratch.

### Step 4.1 Create material (once)
1. `Assets/Materials` → **Create → Material** → `Mat_LightSeed`
2. Bright yellow/green, enable **Emission** if available (URP: HDR emission color)

### Step 4.2 Build the first Light Seed
1. **3D Object → Sphere** → rename `LightSeed`
2. Scale (0.6, 0.6, 0.6)
3. Assign `Mat_LightSeed`
4. **Add a glow light (Unity 6 — no “Point Light” component name)**

   In Unity 6 you add a **Light** component and set its type to **Point**. Try either method:

   **Method A — Add Component (on the seed sphere):**
   1. Select `LightSeed`
   2. **Add Component** → search **`Light`** → click **Light** (not “Light Probe”)
   3. On the **Light** component set:
      - **Type** = `Point`
      - **Color** = yellow
      - **Range** = `5`
      - **Intensity** = `1`–`2`

   **Method B — Menu (creates a child light object):**
   1. Select `LightSeed`
   2. **GameObject → Light → Point Light**
   3. Unity adds a child object with a light — drag that child so it sits on the seed (Position 0,0,0 relative to parent), or leave as child (still works)

   **If you still cannot find it:** skip the light for now — bright **emissive material** on `Mat_LightSeed` is enough. The seed will still collect correctly.

5. **Sphere Collider → Is Trigger ✓**
6. **Add Component →** `LightSeed.cs`

### Step 4.3 Save as prefab
1. Drag `LightSeed` from Hierarchy → `Assets/Prefabs/` → **`LightSeed_Prefab`**

### Step 4.4 Place 3 instances in the scene
1. Drag `LightSeed_Prefab` into Scene **3 times**
2. Rename instances: `LightSeed_1`, `LightSeed_2`, `LightSeed_3` (optional)
3. Move each to a **different area** (corners of the map, away from start)

**Later model swap:** double-click `LightSeed_Prefab` → replace sphere mesh with crystal model → all 3 seeds update.

**Checkpoint:** Walk into any seed → UI seed count goes up, stress drops, that seed disappears.

---

## Phase 5 — Shadow Zones and Safe Zone (using Prefabs)

### Step 5.1 Shadow Zone material (once)
1. Create `Mat_ShadowZone` — purple, **Surface = Transparent**, Alpha ~0.35

### Step 5.2 Build one Shadow Zone
1. **3D Object → Cube** → rename `ShadowZone`
2. Scale (10, 0.5, 10)
3. Assign `Mat_ShadowZone`
4. **Box Collider → Is Trigger ✓**
5. **Add Component →** `ShadowZone.cs`

### Step 5.3 Save and place Shadow Zone prefab
1. Drag to `Assets/Prefabs/` → **`ShadowZone_Prefab`**
2. Drag prefab into scene **twice** → rename `ShadowZone_1`, `ShadowZone_2`
3. Move to different corrupted areas on the map

### Step 5.4 Safe Zone (prefab optional — only one needed)
1. Create `Mat_SafeZone` — green transparent
2. **Cube** → `SafeZone`, Scale (6, 0.5, 6), assign material
3. **Box Collider → Is Trigger ✓**, attach `SafeZone.cs`
4. Place near start area
5. *(Optional)* Drag to Prefabs as **`SafeZone_Prefab`** if you want to reuse in another scene

**Checkpoint:** Enter shadow zone → purple light, heavy fog, stress rises. Enter safe zone → stress drops.

---

## Phase 6 — Forest Heart (unique object, solid + interactable)

The Forest Heart is **one-of-a-kind** in the scene — no prefab required unless you build multiple levels later.

The player should **not walk through** the heart. `ForestHeart.cs` sets this up automatically:
- **Solid collider** — blocks the player
- **Trigger collider** (slightly larger) — detects touch for win / messages

### Step 6.1 Build the heart
1. **Sphere** → `ForestHeart`, Scale (2, 2, 2), place at far end of map
2. Create materials: `Mat_HeartLocked` (dark purple), `Mat_HeartUnlocked`, `Mat_HeartRestored` (gold/green)
3. Attach `ForestHeart.cs` — assign locked / unlocked / restored materials in Inspector
4. On **Forest Heart** script, leave **Use Solid And Trigger Colliders** ✓ checked (default)
5. **Do not add a Rigidbody** to the heart (it should stay static)
6. Optional: **Add Component → Light** → Type = **Point** for glow
7. On **GameManager** → drag `ForestHeart` into **Forest Heart** slot

### Step 6.2 Colliders (automatic)

When you press Play, the script adds/ configures:
| Collider | Is Trigger | Purpose |
|----------|------------|---------|
| First Sphere Collider | **Off** | Solid — player bumps into it |
| Second Sphere Collider | **On** | Slightly larger — win / status when close |

**Manual check in Editor:** Select `ForestHeart` → you should see **two** Sphere Collider components after entering Play mode once, or add a second yourself:
1. First collider: **Is Trigger = unchecked**
2. **Add Component → Sphere Collider** → **Is Trigger = checked**, Radius a bit larger (e.g. 1.1×)

**Checkpoint:** Player cannot walk through the heart. With 0 seeds, touching it shows "needs 3 seeds". With 3 seeds, touching it wins the game.

---

## Phase 7 — Luma guide NPC (unique object)

Luma is a **single guide** in the scene — build once, no prefab required.

1. **Sphere** → `Luma_NPC`, Scale (0.5, 0.5, 0.5)
2. Create `Mat_Luma` — cyan/green emissive + **Light** component (Type = Point)
3. Attach `LumaGuide.cs`
4. Attach **`LumaTTS.cs`** (same object — adds AudioSource automatically)
5. Assign **Player** transform to `Player` field
6. On **GameManager** → drag `Luma_NPC` (LumaGuide component) into **Luma Guide** slot

### Luma voice (Windows TTS — free, no subscription)

Uses **Windows built-in speech** (no API key). Voice plays from Luma’s position in 3D.

**If Unity says “script class cannot be found”:**
1. Check **Console** — fix any **red** compile errors first
2. Return to Unity and wait for scripts to recompile (spinner bottom-right)
3. Then add `LumaTTS` again

**After Play:** Console should show `[LumaTTS] Ready (PowerShell).`  
Luma speaks welcome line ~1.5s after start.

**Background audio while Luma talks:** `AudioManager` on **GameManager** automatically **lowers music/SFX** while Luma speaks (voice ducking). Tune on **Audio Manager** component:
- `Ducked Music Volume` — default `0.15` (15% of normal)
- `Ducked Sfx Volume` — default `0.1`
- `Duck Fade Seconds` — fade time (default `0.35`)

**Optional:** On `LumaTTS`, set `Preferred Voice Name` to `Microsoft Zira Desktop` (female) or `Microsoft David Desktop` (male).  
Find names: **Windows Settings → Time & language → Speech**.

**Behaviors to verify in Play mode:**
- **Follow:** floats near player when safe
- **Warn:** turns orange in shadow zone or high stress
- **Guide:** when stress ≥ 61 or H pressed
- **Celebrate:** flashes gold when seed collected
- **Restore:** stays near heart after win

---

## Phase 8 — Shadow Wisps (using a Prefab)

Build **one** wisp, assign Player on the **prefab** so every copy inherits it.

### Step 8.1 Build the first Wisp
1. **Sphere** → rename `ShadowWisp`, Scale (0.7, 0.7, 0.7)
2. Create `Mat_ShadowWisp` — dark purple/black
3. Attach `ShadowWisp.cs`
4. Drag **Player** into the `Player` field on the script

### Step 8.2 Save and place prefab
1. Drag to `Assets/Prefabs/` → **`ShadowWisp_Prefab`**
2. Drag into scene **twice** → `ShadowWisp_1`, `ShadowWisp_2`
3. Place inside or near shadow zones

**Tip:** Assign Player on the prefab (double-click prefab → set reference). All instances share it — no per-copy wiring.

**Checkpoint:** Stand near wisp → it follows slowly. Get very close → stress rises faster.

---

## Phase 9 — Guide path (using a Prefab for markers)

### Step 9.1 Create parent
1. **Create Empty** → rename `GuidePath`
2. Position near the path from start toward first seed

### Step 9.2 Build one marker, save as prefab
1. **3D Object → Sphere** → rename `GuideMarker`
2. Scale (0.2, 0.2, 0.2), create `Mat_GuideMarker` (bright cyan emissive)
3. Drag `GuideMarker` into `Assets/Prefabs/` → **`GuideMarker_Prefab`**
4. Delete the loose marker from root Hierarchy (keep prefab in Project)

### Step 9.3 Place markers along the path
1. Drag `GuideMarker_Prefab` onto **`GuidePath`** in Hierarchy **6–8 times** (as children)
2. Rename: `Marker_01` … `Marker_08`
3. Space them in a line toward the next Light Seed / Forest Heart
4. **Disable each marker** (uncheck at top of Inspector) — script turns them on when needed

**Optional:** Select all markers → drag whole `GuidePath` to Prefabs as **`GuidePath_Prefab`** for reuse.

### Step 9.4 Wire GuidePathController
1. Select **GameManager**
2. On `GuidePathController` → set **Path Markers** array **Size = 8**
3. Drag each `Marker_01` … `Marker_08` into the slots

**Controls:**
- **H** = toggle guide path manually
- Auto-shows when stress ≥ 61
- **R** = reduced intensity (less fog, slower wisps)

---

## Phase 10 — Audio (optional polish)

`AudioManager.cs` is already on **GameManager** from Phase 3. No new object needed.

1. Import or add `.wav` / `.mp3` files to `Assets/Audio/`
2. Select **GameManager** → **Audio Manager** component
3. Assign clips: Calm, Danger, Collect, Win
4. Play mode: enter shadow zone (danger), collect seed (collect), win (win)

GameManager already calls `AudioManager` automatically when those events happen.

---

## Phase 11 — Replace primitives with 3D models (edit prefabs)

When gameplay works, swap art **inside prefabs** so every instance updates:

| Prefab | Swap | Keep on prefab |
|--------|------|----------------|
| `Tree_Prefab` | Cylinder → tree model | Collider optional |
| `LightSeed_Prefab` | Sphere → crystal model | Trigger collider, `LightSeed.cs`, Light (Point) optional |
| `ShadowZone_Prefab` | Cube mesh optional | Trigger collider, `ShadowZone.cs` |
| `ShadowWisp_Prefab` | Sphere → ghost model | `ShadowWisp.cs`, Player reference |
| `GuideMarker_Prefab` | Sphere → arrow model | Nothing else required |
| `Player_Prefab` | Capsule → character model | Tag, Rigidbody, `PlayerMover.cs` |

**How to swap mesh on a prefab:**
1. Double-click prefab in Project
2. Delete or disable old primitive mesh child
3. Drag 3D model in as child, align scale
4. Exit Prefab Mode — all scene instances update

**Rule:** Never remove required **colliders** or **scripts** when swapping art.

---

## Phase 12 — Start screen (logo / loading) & End screen (win)

Scripts are ready: **`GameFlowManager.cs`** on **GameManager**.  
`GameManager` shows the end screen **after** win music + Luma’s final voice line.

### Overview

| Screen | When | What player sees |
|--------|------|------------------|
| **Start** | Game opens | Logo, loading bar fills, **Start** button |
| **Gameplay** | After Start | Forest + HUD (your Phase 3 UI) |
| **End** | After winning | Win icon, **Restart**, **Exit** |

### Step 12.1 — Create UI structure under Canvas

Use your existing **Canvas** (from Phase 3) or create one: **UI → Canvas**.

Under **Canvas**, create three empty panels ( **UI → Panel** ):

```
Canvas
├── StartScreenPanel      ← full screen, ON at start
├── GameplayPanel         ← your UI_Root from Phase 3 (move under here)
└── EndScreenPanel        ← full screen, OFF until win
```

For **each** panel (Start + End), set **Rect Transform**:
- Anchor preset: **stretch-stretch** (Alt+Shift + bottom-right)
- Left, Right, Top, Bottom = **0** (fills screen)

**StartScreenPanel** — Image color: dark green/black (alpha ~220) so it covers the game.

**EndScreenPanel** — same full-screen background; **disable** (unchecked) in Hierarchy until you test.

**GameplayPanel** — stretch full screen; put **`UI_Root`** inside it as a child (drag from Canvas).

---

### Step 12.2 — Start screen content

All children of **StartScreenPanel**:

#### A) Logo
1. Right-click `StartScreenPanel` → **UI → Image** → rename `LogoImage`
2. **Rect Transform:** anchor **middle-center**, Width `400`, Height `400`, Pos (0, 80, 0)
3. **Source Image:** drag your logo sprite/PNG into `Assets` (e.g. `Assets/UI/logo.png`), assign to Image component
4. If no logo yet: use **UI → Text - TextMeshPro** → text `Flowbound Grove` (large font, gold/green)

#### B) Loading bar (optional)
1. **UI → Slider** → rename `LoadingBar`
2. Anchor: **bottom-center**, Width `500`, Height `30`, Pos Y `120`
3. Remove **Handle Slide Area** child if you want a simple bar (select Handle, delete)
4. Set **Fill** color to light green/yellow

#### C) Start button
1. **UI → Button - TextMeshPro** → rename `StartButton`
2. Anchor: **bottom-center**, Width `220`, Height `60`, Pos Y `40`
3. Button text: `Start`
4. **Interactable** = unchecked for now (script enables it after loading)

---

### Step 12.3 — End screen content

All children of **EndScreenPanel** (panel starts **disabled**):

#### A) Win icon (top)
1. **UI → Image** → rename `WinIcon`
2. Anchor: **top-center**, Width `200`, Height `200`, Pos Y `-140`
3. Assign a trophy/star/win PNG, or use TextMeshPro: `★` / `You Win!`

#### B) Title text
1. **UI → Text - TextMeshPro** → `WinTitleText`
2. Anchor top-center, below icon: `Forest Restored!`

#### C) Restart button
1. **UI → Button - TextMeshPro** → `RestartButton`
2. Anchor **bottom-center**, Pos Y `100`, Width `200`, Height `55`, text: `Restart`

#### D) Exit button
1. **UI → Button - TextMeshPro** → `ExitButton`
2. Anchor **bottom-center**, Pos Y `30`, Width `200`, Height `55`, text: `Exit`

---

### Step 12.4 — Add GameFlowManager script

1. Select **GameManager** in Hierarchy
2. **Add Component → Game Flow Manager**
3. Wire Inspector slots:

| Slot | Drag from Hierarchy |
|------|---------------------|
| Start Screen Panel | `Canvas/StartScreenPanel` |
| Gameplay Panel | `Canvas/GameplayPanel` |
| End Screen Panel | `Canvas/EndScreenPanel` |
| Start Button | `StartScreenPanel/StartButton` |
| Loading Bar | `StartScreenPanel/LoadingBar` (optional) |
| Player | `Player` |
| Player Mover | `Player` (PlayerMover component auto-finds) |

**Settings:**
- `Min Loading Seconds` = `2` (logo visible ~2 sec while bar fills)
- `Require Start Button` = ✓ checked

---

### Step 12.5 — Hook button clicks (important)

Select each button → **Inspector → Button → On Click ()**:

**StartButton**
1. Click **+**
2. Drag **GameManager** into object slot
3. Function: **GameFlowManager → OnStartButtonPressed()**

**RestartButton**
1. **+** → GameManager → **GameFlowManager → OnRestartButtonPressed()**

**ExitButton**
1. **+** → GameManager → **GameFlowManager → OnExitButtonPressed()**

---

### Step 12.6 — Test flow

| Step | Expected |
|------|----------|
| Press Play | Start screen visible, player cannot move |
| Loading bar fills (~2 s) | Start button becomes clickable |
| Click Start | Start screen hides, HUD + gameplay appear |
| Win game (3 seeds + heart) | Win music → Luma final line → **End screen** |
| Restart | Scene reloads, start screen again |
| Exit | Stops Play mode in Editor / quits build |

**End screen timing:** `GameManager` waits for win music, then Luma speaks, then `GameFlowManager` waits until Luma finishes, then shows end screen (game pauses with `timeScale = 0`).

---

### Step 12.7 — Responsive tips (start/end screens)

Same as Phase 3: Canvas Scaler **1920×1080**, anchors on logo (center), buttons (bottom-center), win icon (top-center). Use **stretch** panels for backgrounds.

---

## Phase 13 — Submission

- [ ] Full playthrough test (start → 3 seeds → heart → win)
- [ ] Record demo video (2–4 min) using script below
- [ ] Write 1-page concept proposal
- [ ] Write 300–500 word self-study explanation
- [ ] Zip **Assets**, **Packages**, **ProjectSettings** only (NOT `Library`)
- [ ] Upload to Drive/OneDrive → share **Anyone with link can view**
- [ ] Test link in incognito window before CourseWeb submit

See **Assignment submission checklist** below for folder naming.

---

## Scene hierarchy (target)

```
ForestScene
├── Directional Light
├── Ground
├── Tree_Prefab (instances ×8–12)
├── Rock_Prefab (instances ×4–6, optional)
├── Player
│   └── Main Camera
├── GameManager
├── Luma_NPC                    ← unique (not repeated)
├── LightSeed_Prefab (×3)
├── ShadowZone_Prefab (×2)
├── SafeZone
├── ForestHeart                 ← unique
├── ShadowWisp_Prefab (×2)
├── GuidePath
│   └── GuideMarker_Prefab (×6–8, disabled)
└── Canvas
    ├── StartScreenPanel → Logo, LoadingBar, StartButton
    ├── GameplayPanel
    │   └── UI_Root → (HUD panels + texts)
    └── EndScreenPanel → WinIcon, WinTitleText, RestartButton, ExitButton
```

**Prefabs folder (`Assets/Prefabs/`):**

| Prefab | Copies in scene |
|--------|-----------------|
| `Tree_Prefab` | 8–12 |
| `Rock_Prefab` | 4–6 |
| `LightSeed_Prefab` | 3 |
| `ShadowZone_Prefab` | 2 |
| `ShadowWisp_Prefab` | 2 |
| `GuideMarker_Prefab` | 6–8 |
| `UI_Root_Prefab` | backup copy |
| `Player_Prefab` | optional |
| `SafeZone_Prefab` | optional |

---

## Assignment submission checklist

Folder name: `IT22033482 – Your Name`

| File | Description |
|------|-------------|
| Unity zip | `Assets`, `Packages`, `ProjectSettings` only |
| Concept proposal | 1 page (title, scenario, user, intelligence, adaptation, UX) |
| Self-study | 300–500 words on Flow-State Adaptive Assistance |
| Demo video | Show movement, seeds, shadow zones, wisps, stress, H, R, win |

Share link: **Anyone with the link can view** → test in incognito before submit.

---

## Demo video script (read while recording)

> "This is Flowbound Grove, an intelligent interactive environment built in Unity. The player explores a living forest and collects three Light Seeds to restore the Forest Heart. The environment adapts when I enter Shadow Zones — fog, lighting, and audio change, and my stress level increases. Luma, the guide NPC, follows me, warns me in danger, and guides me when stress is high. Shadow Wisps detect and follow the player. Pressing H shows a guide path; pressing R enables reduced intensity mode. My self-study innovation is Flow-State Adaptive Assistance — the game estimates stress from gameplay behavior and adjusts support automatically."

---

## Troubleshooting

| Problem | Fix |
|---------|-----|
| Player falls through ground | Ground needs Mesh Collider (default on Plane). Player needs Rigidbody. |
| Scripts not in Add Component menu | Check Console for compile errors. Fix red errors first. |
| Seed not collecting | Player Tag must be "Player". Seed collider must be Trigger. |
| UI not updating | Drag text objects into GameManager slots in Inspector. |
| UI too small / too large | Canvas Scaler → Scale With Screen Size, Reference 1920×1080, Match = 0.5 |
| UI stuck in corner on resize | Re-apply anchor preset (stretch or corner), reset offsets |
| Text cut off on small screens | Enable TextMeshPro **Auto Size**; increase panel Height |
| Text overlaps | Separate into TopPanel / StatsPanel / BottomPanel as in Phase 3 |
| UI not visible at all | Canvas Render Mode = Screen Space - Overlay; check text Color alpha |
| Nothing happens in shadow zone | Shadow zone collider must be Trigger. Player needs Tag. |
| Camera doesn't follow | Make camera a child of Player with local offset. |
| Prefab changes not in scene | Edit prefab in Prefab Mode and save; ensure object is blue (linked instance). |
| Script missing on copies | Add script to prefab, not individual instances. |
| Wisp/seed broken on copy | Assign shared references (Player) on the prefab root. |
| Broke prefab link (black text in Hierarchy) | Re-drag prefab from Project, or Prefab → Revert. |
| No “Point Light” in Add Component | Use **Light** component → set **Type** = Point; or **GameObject → Light → Point Light**. |
| Safe Zone does not lower stress | SafeZone needs **Is Trigger** collider; Player tag **Player**; script `SafeZone.cs` attached. |
| Stress hits 0 after one seed | Normal if stress was ≤20 before collect (each seed removes 20). Raise stress in shadow first to see partial drop. |
| Stress rises in Safe Zone | Shadow and Safe zones overlapping — move Safe Zone away from Shadow Zone, or safe recovery now pauses shadow gain. |
| Player walks through Forest Heart | Heart needs **solid** collider (Trigger off). Use updated `ForestHeart.cs` or two colliders — see Phase 6. |
| Can't add LumaTTS script | Fix all **red** Console errors; wait for recompile; script must not reference missing DLLs. |
| LumaTTS ready but no sound | Windows only; check volume; set Preferred Voice Name; stand near Luma (3D audio). |

---

## Scripts reference

All scripts live in `Assets/Scripts/`:

| Script | Attach to |
|--------|-----------|
| `PlayerMover.cs` | `Player_Prefab` or Player |
| `GameManager.cs` | GameManager (single) |
| `GuidePathController.cs` | GameManager |
| `AccessibilityController.cs` | GameManager |
| `AudioManager.cs` | GameManager (optional) |
| `LightSeed.cs` | `LightSeed_Prefab` |
| `ShadowZone.cs` | `ShadowZone_Prefab` |
| `SafeZone.cs` | SafeZone |
| `ForestHeart.cs` | ForestHeart (unique) |
| `LumaGuide.cs` | Luma_NPC (unique) |
| `LumaTTS.cs` | Luma_NPC (same object as LumaGuide) |
| `GameFlowManager.cs` | GameManager |
| `ShadowWisp.cs` | `ShadowWisp_Prefab` |

---

## Next step

Work through phases in order. When **Phase 2 (WASD movement)** works, open **Phase 3** and follow every UI sub-step before moving to Phase 4.
