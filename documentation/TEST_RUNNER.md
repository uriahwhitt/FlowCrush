# 🧪 FlowCrush Sprint 1 - Test Runner

## 🚀 **Quick Test Instructions**

### **Step 1: Open Unity**
1. Open Unity 2022.3 LTS
2. Open the FlowCrush project
3. Wait for Unity to compile all scripts (check Console for any errors)

### **Step 2: Load Test Scene**
1. Go to `File > Open Scene`
2. Navigate to `Assets/Scenes/Sprint1Test.unity`
3. Open the scene

### **Step 3: Run Auto Setup**
1. In the Hierarchy, you should see:
   - ✅ **Main Camera** (positioned at 0,0,-10)
   - ✅ **TestSceneSetup** GameObject
2. Select **TestSceneSetup** in the Hierarchy
3. In the Inspector, verify **Auto Setup On Start** is checked
4. Press **Play** in Unity

## 📋 **Expected Console Output**

When you press Play, you should see this exact sequence:

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

## 🎯 **Visual Verification**

### **What You Should See:**
1. **8x8 Grid** of colored tiles (random colors)
2. **Zone Overlays** (if zone debug is enabled):
   - White squares for Edge zones
   - Green squares for Transition zones
   - Red squares for Center zones
3. **Camera** showing entire grid
4. **Smooth 60 FPS** performance

### **Zone Layout Verification:**
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

## 🧪 **System Testing**

### **Test 1: Grid System**
1. In Hierarchy, expand `FlowCrush_Game`
2. Select `GridManager`
3. Right-click in Inspector → **Toggle Zone Debug**
4. **Expected:** Zone overlays appear/disappear

### **Test 2: Touch Input**
1. Select `TouchInputManager` in Hierarchy
2. Right-click in Inspector → **Test Swipe Detection**
3. **Expected:** Console shows "Test swipe from (100,100) to (200,100) = Right"
4. Try swiping with mouse on grid
5. **Expected:** Console logs swipe detection

### **Test 3: Match Detection**
1. Select `MatchDetector` in Hierarchy
2. Right-click in Inspector → **Find All Matches**
3. **Expected:** Console shows "Found X matches" with match details

### **Test 4: Complete System Test**
1. Select `TestSceneSetup` in Hierarchy
2. Right-click in Inspector → **Run All Tests**
3. **Expected:** All systems report "✓ operational"

## ✅ **Success Criteria Checklist**

**Mark each item as you verify:**

- [ ] **Unity opens** without errors
- [ ] **Scripts compile** successfully (no red errors in Console)
- [ ] **Test scene loads** (Sprint1Test.unity)
- [ ] **Auto setup runs** when Play is pressed
- [ ] **Console shows** expected setup messages
- [ ] **8x8 grid displays** with colored tiles
- [ ] **Zone debug works** (toggle shows/hides overlays)
- [ ] **Touch input responds** to mouse swipes
- [ ] **Match detection finds** matches in grid
- [ ] **Performance is smooth** (60 FPS)
- [ ] **No errors** in Console

## 🔧 **Troubleshooting**

### **Issue: Scripts Not Compiling**
**Solution:**
1. Check Console for specific error messages
2. Verify all scripts are in correct folders:
   - `Assets/Scripts/Core/`
   - `Assets/Scripts/Input/`
   - `Assets/Scripts/Gameplay/`
   - `Assets/Scripts/Utilities/`
3. Restart Unity if needed

### **Issue: Grid Not Displaying**
**Solution:**
1. Check if GridManager component is attached
2. Verify camera is orthographic
3. Check camera position (should be 0,0,-10)
4. Look for error messages in Console

### **Issue: Touch Input Not Working**
**Solution:**
1. Test with mouse in editor first
2. Check TouchInputManager settings
3. Verify camera is tagged as "MainCamera"
4. Try the debug test command

### **Issue: Zone Debug Not Showing**
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

## 📝 **Test Results**

**Date:** _______________
**Tester:** _______________

**Grid System:** □ Pass □ Fail
**Touch Input:** □ Pass □ Fail  
**Match Detection:** □ Pass □ Fail
**Scoring System:** □ Pass □ Fail
**Performance:** □ Pass □ Fail
**Zone System:** □ Pass □ Fail

**Issues Found:**
- ________________
- ________________

**Performance Metrics:**
- Frame Rate: _____ FPS
- Memory Usage: _____ MB
- Input Response: _____ ms

**Notes:**
________________
________________

## 🚀 **Next Steps**

Once all tests pass:

1. **Document any issues** found during testing
2. **Note performance metrics** for optimization
3. **Begin Sprint 1 Week 2** - Directional Flow System
4. **Implement BlockSpawner.cs** for opposite-edge spawning
5. **Add PressureSystem.cs** for center-flow mechanics

---

**🎯 Current Status:** Ready for Sprint 1 Week 2 - Directional Flow Implementation!

*Last Updated: Sprint 1 Week 1*  
*Next: BlockSpawner and PressureSystem development* 