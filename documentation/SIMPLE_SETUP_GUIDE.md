# 🚀 Simple FlowCrush Setup Guide

## Quick Setup Instructions

### Step 1: Create Simple Scene
1. **Create a new scene** or use your existing Sprint1Test scene
2. **Create an empty GameObject** called "SimpleSceneSetup"
3. **Add the SimpleSceneSetup component** to it
4. **Leave all settings as default** (autoSetupOnStart = true)

### Step 2: Disable Old Systems (Optional)
If you want to avoid conflicts with the old complex system:
1. **Find your existing GameManager, GridManager, etc.**
2. **Disable their GameObjects** (uncheck the checkbox next to their names)
3. **Or delete them entirely** if you want to start fresh

### Step 3: Test the Simple System
1. **Press Play**
2. **You should see:**
   - Console message: "Setting up Simple FlowCrush scene..."
   - Console message: "✅ SimpleGameManager created"
   - Console message: "Created default tile prefab"
   - Console message: "Created 8x8 grid with 64 tiles"
   - **8x8 grid of colored squares** in the Game window

### Step 4: Test Basic Functionality
1. **Click any tile** - it should get bigger and brighter (selection)
2. **Click an adjacent tile** - they should swap colors
3. **If a match is made** - tiles disappear and new ones appear
4. **Check the Console** for debug messages about clicks, swaps, and matches
5. **Look at the top-left corner** for score and match count

## 🎯 What This Achieves (Sprint 1 Goals)

✅ **8x8 grid system operational** - Simple grid creation  
✅ **Basic tile matching** - 3+ matches detected and replaced  
✅ **Selection system working** - Click to select, click adjacent to swap  
✅ **Core game loop** - Select → Swap → Match → Score  
✅ **Performance target** - Should easily hit 60 FPS  

## 🔧 Next Steps After This Works

Once you have this basic version running:

1. **Create proper tile sprites** (replace the auto-generated white squares)
2. **Add simple animations** for swapping and matching
3. **Implement the directional flow system** from your Sprint 1 backlog
4. **Add the center-flow pressure system**

## 🚨 Troubleshooting

### If you see errors:
1. **Check the Console** for specific error messages
2. **Make sure the Simple scripts are in Assets/Scripts/Simple/**
3. **Verify the scene has a SimpleSceneSetup component**
4. **Try creating a completely new scene** if needed

### If tiles don't respond to clicks:
1. **Check that the camera is positioned correctly** (should be at origin)
2. **Verify BoxCollider2D components** are on the tiles
3. **Make sure the camera is set to orthographic**

### If no grid appears:
1. **Check the Console** for "Created 8x8 grid" message
2. **Verify SimpleGameManager exists** in the scene
3. **Check that tilePrefab is not null**

## 📊 Expected Console Output

```
Setting up Simple FlowCrush scene...
✅ SimpleGameManager created
Simple scene setup complete! Press Play to test.
Created default tile prefab
Created 8x8 grid with 64 tiles
```

## 🎮 How to Play

1. **Click a tile** to select it (it gets bigger and brighter)
2. **Click an adjacent tile** to attempt a swap
3. **If a match is made** (3+ same color in a row/column), tiles disappear and new ones appear
4. **If no match is made**, tiles swap back to their original positions
5. **Watch your score increase** with each successful match

This simplified approach eliminates all the over-engineered dependencies and gets you back to a working prototype quickly! 