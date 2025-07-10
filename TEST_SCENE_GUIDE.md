# 🎮 FlowCrush Sprint 1 - Test Scene Guide

## 🚀 **Quick Start Instructions**

### **Step 1: Open the Test Scene**
1. In Unity, go to `File > Open Scene`
2. Navigate to `Assets/Scenes/Sprint1Test.unity`
3. Open the scene

### **Step 2: Verify Scene Setup**
You should see:
- ✅ **Main Camera** (orthographic, positioned at 0,0,-10)
- ✅ **TestSceneSetup** GameObject with component attached
- ✅ **Auto Setup On Start** should be checked

### **Step 3: Run the Test**
1. Press **Play** in Unity
2. Watch the Console for setup messages
3. You should see: "Setting up FlowCrush test scene for Sprint 1..."

## 📋 **Expected Console Output**

When you press Play, you should see this sequence:

```
Setting up FlowCrush test scene for Sprint 1...
GridManager created and configured
TouchInputManager created and configured
MatchDetector created and configured
ScoreManager created and configured
Debug mode enabled
All systems connected
Test scene setup complete! Ready for Sprint 1 testing.
Grid initialized: 8x8, Size: 8.7x8.7
Camera configured: Orthographic size = 6.35
Zone distribution - Edge: 20, Transition: 12, Center: 16
```

## 🧪 **Testing Each System**

### **Test 1: Grid System**
1. In the Hierarchy, find the `FlowCrush_Game` object
2. Expand it to see `GridManager`
3. Select `GridManager`
4. Right-click in the Inspector → **Toggle Zone Debug**
5. You should see colored zone indicators appear

**Expected Result:** 
- White squares for Edge zones
- Green squares for Transition zones  
- Red squares for Center zones

### **Test 2: Touch Input**
1. Select `TouchInputManager` in the hierarchy
2. Right-click in Inspector → **Test Swipe Detection**
3. Try swiping with your mouse on the grid

**Expected Result:**
- Console shows "Test swipe from (100,100) to (200,100) = Right"
- Mouse swipes should be detected and logged

### **Test 3: Match Detection**
1. Select `MatchDetector` in the hierarchy
2. Right-click in Inspector → **Find All Matches**
3. Check console for match detection results

**Expected Result:**
- Console shows "Found X matches" 
- Each match shows tile count, color, and zone

### **Test 4: Complete System Test**
1. Select `TestSceneSetup` in the hierarchy
2. Right-click in Inspector → **Run All Tests**

**Expected Result:**
- All systems report "✓ operational"
- No errors in console

## 🎯 **Visual Verification**

### **What You Should See:**
1. **8x8 Grid** of colored tiles
2. **Zone Overlays** (if zone debug is enabled)
3. **Camera** properly positioned to show entire grid
4. **Smooth Performance** (60 FPS in Game view)

### **Zone Layout (8x8 Grid):**
```
[E][E][E][E][E][E][E][E]  ← Edge Zone (White)
[E][T][T][T][T][T][T][E]  ← Transition Zone (Green)
[E][T][C][C][C][C][T][E]  ← Center Zone (Red)
[E][T][C][C][C][C][T][E]  ← Center Zone (Red)
[E][T][C][C][C][C][T][E]  ← Center Zone (Red)
[E][T][C][C][C][C][T][E]  ← Center Zone (Red)
[E][T][T][T][T][T][T][E]  ← Transition Zone (Green)
[E][E][E][E][E][E][E][E]  ← Edge Zone (White)
```

## 🔧 **Troubleshooting**

### **Issue 1: Scripts Not Compiling**
**Symptoms:** Red errors in Console
**Solution:**
1. Check that all scripts are in correct folders
2. Verify namespace declarations match folder structure
3. Restart Unity if needed

### **Issue 2: Grid Not Displaying**
**Symptoms:** No tiles visible
**Solution:**
1. Check if GridManager component is attached
2. Verify camera is orthographic
3. Check camera position (should be 0,0,-10)

### **Issue 3: Touch Input Not Working**
**Symptoms:** No swipe detection
**Solution:**
1. Test with mouse in editor first
2. Check TouchInputManager settings
3. Verify camera is tagged as "MainCamera"

### **Issue 4: Zone Debug Not Showing**
**Symptoms:** No colored overlays
**Solution:**
1. Right-click GridManager → **Toggle Zone Debug**
2. Check if zone indicator objects exist in hierarchy
3. Verify zone colors are set in GridManager

## 📊 **Performance Check**

### **Target Metrics:**
- **Frame Rate:** 60 FPS (check Game view stats)
- **Memory:** <150MB (check Profiler)
- **Input Response:** <100ms from touch to visual feedback

### **How to Check:**
1. Open **Window > Analysis > Profiler**
2. Press Play and monitor performance
3. Check **Window > General > Console** for any warnings

## 🎮 **Interactive Testing**

### **Mouse/Touch Testing:**
1. **Click and drag** on the grid
2. **Swipe in all 4 directions** (up, down, left, right)
3. **Check console** for swipe detection messages

### **Grid Interaction:**
1. **Right-click GridManager** → **Log Grid State**
2. **Right-click GridManager** → **Test Zone System**
3. **Observe console output** for grid information

### **Match Testing:**
1. **Right-click MatchDetector** → **Find All Matches**
2. **Right-click MatchDetector** → **Process Matches**
3. **Watch for match animations** and console feedback

## ✅ **Success Criteria Checklist**

**Sprint 1 Foundation Complete When:**

- [ ] **Grid displays correctly** with 8x8 layout
- [ ] **Zone system works** with visual indicators
- [ ] **Touch input responds** to mouse/touch swipes
- [ ] **Match detection finds** 3+ tile matches
- [ ] **Scoring system applies** zone multipliers
- [ ] **Performance maintains** 60 FPS
- [ ] **All debug commands** work in Inspector
- [ ] **Console shows** no errors or warnings

## 🚀 **Next Steps After Testing**

Once all tests pass:

1. **Document any issues** found during testing
2. **Note performance metrics** for optimization
3. **Begin Sprint 1 Week 2** - Directional Flow System
4. **Implement BlockSpawner.cs** for opposite-edge spawning
5. **Add PressureSystem.cs** for center-flow mechanics

## 📝 **Test Results Template**

```
Date: _______________
Tester: _______________

Grid System: □ Pass □ Fail
Touch Input: □ Pass □ Fail  
Match Detection: □ Pass □ Fail
Scoring System: □ Pass □ Fail
Performance: □ Pass □ Fail
Zone System: □ Pass □ Fail

Issues Found:
- ________________
- ________________

Performance Metrics:
- Frame Rate: _____ FPS
- Memory Usage: _____ MB
- Input Response: _____ ms

Notes:
________________
________________
```

---

**🎯 Current Status:** Ready for Sprint 1 Week 2 - Directional Flow Implementation!

*Last Updated: Sprint 1 Week 1*  
*Next: BlockSpawner and PressureSystem development* 