# 🔧 Scene Fix Instructions

## **Issue**: Blue Screen + Missing Script Reference

The Sprint1Test scene has a `TestSceneSetup` component with a missing script reference, causing Unity to show a blue screen.

## **Quick Fix Steps**

### **Option 1: Use SceneInitializer (Recommended)**

1. **Open Unity** and load the Sprint1Test scene
2. **In the Hierarchy**, right-click and select **Create Empty**
3. **Name it** "SceneInitializer"
4. **Add the SceneInitializer component** to this GameObject
5. **Play the scene** - it should automatically set up all systems

### **Option 2: Manual Setup**

1. **Delete the broken TestSceneSetup GameObject** from the scene
2. **Create these GameObjects manually**:
   - GameManager (add GameManager component)
   - GridManager (add GridManager component)
   - TouchInputManager (add TouchInputManager component)
   - MatchDetector (add MatchDetector component)
   - ScoreManager (add ScoreManager component)
   - BlockSpawner (add BlockSpawner component)
   - PressureSystem (add PressureSystem component)

### **Option 3: Create New Scene**

1. **Create a new scene** called "Sprint1Test_New"
2. **Add a Main Camera** (if not present)
3. **Add the SceneInitializer component** to an empty GameObject
4. **Play the scene** - it will auto-setup everything

## **Expected Result**

After fixing, you should see:
- ✅ **8x8 grid of colored tiles** in the Game window
- ✅ **Zone indicators** (Edge, Transition, Center zones)
- ✅ **Console logs** showing system initialization
- ✅ **Touch/swipe input** working (test with mouse in editor)

## **Testing the Setup**

Once the scene is running:

1. **Check the Console** for initialization messages
2. **Try swiping** with your mouse in the Game window
3. **Use the Context Menu** on SceneInitializer to test systems
4. **Look for the grid** - you should see colored tiles

## **Debug Commands**

In the SceneInitializer component, you can use:
- **Setup Basic Scene** - Manually trigger setup
- **Test All Systems** - Verify all systems are working

## **If Still Blue Screen**

1. **Check the Console** for error messages
2. **Verify all scripts compiled** successfully
3. **Try Option 3** (create new scene)
4. **Restart Unity** if needed

## **Success Indicators**

✅ **Grid visible** with colored tiles
✅ **Console shows** "Basic scene setup complete"
✅ **No script errors** in Console
✅ **Game window shows** the game grid instead of blue

The SceneInitializer script will automatically create all necessary game objects and start the game when you play the scene! 