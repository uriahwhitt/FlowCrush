# Sprint 1 - FlowCrush Development Plan

## 🎯 **Sprint Overview**

**Sprint Duration:** 2 weeks (July 10 - July 24, 2025)  
**Sprint Goal:** *"Create playable prototype with swipe-directional flow and center pressure mechanics"*  
**Team:** 1 developer (Uriah Whittemore)  
**Development Environment:** Unity 2022.3 LTS (2D Mobile)

---

## 🏆 **Sprint Goal & Success Criteria**

### **Primary Sprint Goal:**
*"Player can swipe tiles to make matches, new blocks flow from the direction opposite to their swipe, and objects naturally migrate toward center zones through pressure gradient, creating a playable core loop with spatial strategy on mobile device."*

### **Definition of Done for Sprint 1:**
- [ ] 8x8 grid system operational with zone definitions (Edge/Transition/Center)
- [ ] Touch input working with swipe direction detection (90%+ accuracy)
- [ ] Directional block flow implemented (opposite-edge spawning)
- [ ] Center-flow pressure system functional and visually apparent
- [ ] Basic tile matching creates satisfying feedback (3+ matches)
- [ ] Playable on both iOS and Android build targets
- [ ] Basic visual feedback shows flow direction and pressure effects
- [ ] Performance: 60 FPS maintained on target devices
- [ ] Core game loop completable end-to-end (swipe → match → flow → repeat)

---

## 📋 **Product Backlog Items for Sprint 1**

### **Epic 1: Core Grid System**
*As a developer, I need a flexible grid system that supports zones and pressure mechanics.*

#### **User Story 1.1: Basic Grid Implementation** 
**Story Points:** 5  
**Priority:** Highest  

**Acceptance Criteria:**
- 8x8 grid displays correctly on mobile screens (portrait orientation)
- Grid adapts to different device aspect ratios and screen sizes
- Touch input properly maps to grid coordinates with visual feedback
- Grid state can be saved and restored for game persistence
- Grid coordinates system works consistently across the entire board

**Implementation Tasks:**
- [ ] Set up Unity project with mobile build targets (iOS/Android)
- [ ] Create GridManager script with 8x8 coordinate system
- [ ] Implement responsive grid scaling for different screen sizes
- [ ] Add touch input detection and grid coordinate mapping
- [ ] Create basic tile prefab system with placeholder graphics
- [ ] Test grid display on iOS and Android build targets

**Technical Notes:**
```csharp
// Core Grid Structure
public class GridManager : MonoBehaviour 
{
    [SerializeField] private int gridWidth = 8;
    [SerializeField] private int gridHeight = 8;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform gridParent;
    
    private Tile[,] grid;
    private Camera mainCamera;
    
    // Grid initialization and coordinate mapping
}
```

#### **User Story 1.2: Zone System Implementation**
**Story Points:** 3  
**Priority:** High  

**Acceptance Criteria:**
- Grid clearly defines Edge, Transition, and Center zones visually
- Zones are distinguishable through color coding or visual indicators
- Zone membership can be queried programmatically for game logic
- Zone-based scoring multipliers can be applied to matches
- Zone visualization can be toggled for debugging purposes

**Implementation Tasks:**
- [ ] Define zone boundaries in GridManager (Edge: outer 2 rings, Center: inner 4x4)
- [ ] Create visual indicators for different zones (subtle background colors)
- [ ] Implement zone membership checking functions (GetZoneType(x, y))
- [ ] Add zone-based scoring multipliers (Edge: 1x, Transition: 1.5x, Center: 2x)
- [ ] Create debug visualization toggle for zone testing

**Zone Layout Reference:**
```
[E][E][E][E][E][E][E][E]  ← Edge Zone (1x multiplier)
[E][T][T][T][T][T][T][E]  ← Transition Zone (1.5x multiplier)
[E][T][C][C][C][C][T][E]  ← Center Zone (2x multiplier)
[E][T][C][C][C][C][T][E]  ← Center Zone (2x multiplier)
[E][T][C][C][C][C][T][E]  ← Center Zone (2x multiplier)
[E][T][C][C][C][C][T][E]  ← Center Zone (2x multiplier)
[E][T][T][T][T][T][T][E]  ← Transition Zone (1.5x multiplier)
[E][E][E][E][E][E][E][E]  ← Edge Zone (1x multiplier)
```

---

### **Epic 2: Touch Input & Swipe Detection**
*As a player, I want responsive touch controls that accurately detect my swipe directions.*

#### **User Story 2.1: Touch Input System**
**Story Points:** 3  
**Priority:** Highest  

**Acceptance Criteria:**
- Touch input responds within 50ms of user interaction
- Swipe direction detection works accurately in all 4 cardinal directions
- Input system works consistently across different device screen sizes
- Visual feedback shows where player touched and swipe direction
- Input can distinguish between taps and swipes reliably

**Implementation Tasks:**
- [ ] Implement TouchInputManager script with swipe detection
- [ ] Add swipe direction calculation (Vector2 math for up/down/left/right)
- [ ] Create visual feedback for touch points and swipe trails
- [ ] Map touch coordinates to grid coordinates accurately
- [ ] Add input validation and edge case handling
- [ ] Test touch responsiveness on different device screen sizes

**Technical Implementation:**
```csharp
// Touch Input Detection
public class TouchInputManager : MonoBehaviour 
{
    [SerializeField] private float minSwipeDistance = 50f;
    [SerializeField] private float maxSwipeTime = 1f;
    
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float touchStartTime;
    
    public SwipeDirection GetSwipeDirection() { /* Implementation */ }
}

public enum SwipeDirection { Up, Down, Left, Right, None }
```

#### **User Story 2.2: Swipe-to-Flow Direction Mapping**
**Story Points:** 2  
**Priority:** High  

**Acceptance Criteria:**
- Swipe UP maps to blocks flowing from BOTTOM edge
- Swipe DOWN maps to blocks flowing from TOP edge
- Swipe LEFT maps to blocks flowing from RIGHT edge
- Swipe RIGHT maps to blocks flowing from LEFT edge
- Direction mapping feels intuitive and consistent to players
- Visual indicators show which edge blocks will flow from

**Implementation Tasks:**
- [ ] Create DirectionMapper utility class for swipe-to-flow conversion
- [ ] Implement opposite direction calculation logic
- [ ] Add visual preview showing flow direction before blocks spawn
- [ ] Create directional arrow indicators at grid edges
- [ ] Test intuitive feel of direction mapping with multiple users

---

### **Epic 3: Directional Block Flow System**
*As a player, I want new blocks to flow from the direction I choose through my swipe.*

#### **User Story 3.1: Opposite-Edge Block Spawning**
**Story Points:** 8  
**Priority:** Highest  

**Acceptance Criteria:**
- New blocks spawn from edge opposite to swipe direction consistently
- Block spawning feels natural and predictable to players
- Spawning system handles all 4 directional cases correctly
- Performance maintains 60 FPS during block flow animations
- Block movement animations are smooth and satisfying

**Implementation Tasks:**
- [ ] Create BlockSpawner system with directional logic
- [ ] Implement edge-to-edge mapping (up↔down, left↔right)
- [ ] Add block queue system for smooth continuous spawning
- [ ] Create smooth block movement animations using tweening
- [ ] Implement collision detection for flowing blocks
- [ ] Optimize performance for multiple simultaneous block movements

**Technical Architecture:**
```csharp
// Block Flow System
public class BlockSpawner : MonoBehaviour 
{
    [SerializeField] private GameObject[] blockPrefabs;
    [SerializeField] private float blockMoveSpeed = 5f;
    
    public void SpawnBlocksFromDirection(SwipeDirection swipeDirection, int blockCount)
    {
        EdgePosition spawnEdge = GetOppositeEdge(swipeDirection);
        StartCoroutine(SpawnAndMoveBlocks(spawnEdge, blockCount));
    }
    
    private EdgePosition GetOppositeEdge(SwipeDirection direction) { /* Implementation */ }
}
```

#### **User Story 3.2: Block Flow Physics & Animation**
**Story Points:** 5  
**Priority:** High  

**Acceptance Criteria:**
- Blocks flow smoothly from spawn edge to destination positions
- Flow respects existing blocks and obstacles appropriately
- Flow animation is satisfying and clearly communicates direction
- Flow speed is balanced for gameplay feel (not too fast/slow)
- Multiple blocks can flow simultaneously without conflicts

**Implementation Tasks:**
- [ ] Implement smooth block movement using Unity's animation system
- [ ] Add easing curves for natural-feeling movement
- [ ] Create particle effects for directional flow visualization
- [ ] Add subtle audio feedback for block movement
- [ ] Test flow physics feel and adjust timing/speed parameters

---

### **Epic 4: Center-Flow Pressure System**
*As a player, I want objects to naturally migrate toward center zones, creating spatial strategy.*

#### **User Story 4.1: Pressure Gradient Implementation**
**Story Points:** 8  
**Priority:** High  

**Acceptance Criteria:**
- Objects naturally drift toward center zones over time
- Pressure effect is subtle but noticeable during gameplay
- Center zones accumulate objects more than edge zones
- Pressure system enhances strategic depth without overwhelming players
- System performance doesn't impact frame rate

**Implementation Tasks:**
- [ ] Create PressureSystem script with gradient calculation
- [ ] Implement center-ward force application to block movement
- [ ] Add pressure visualization (subtle particle effects or visual cues)
- [ ] Balance pressure strength for optimal gameplay feel
- [ ] Test pressure system with different block density patterns
- [ ] Optimize pressure calculations for consistent 60 FPS performance

**Pressure Gradient Logic:**
```csharp
// Pressure System Implementation
public class PressureSystem : MonoBehaviour 
{
    [SerializeField] private float pressureStrength = 2f;
    [SerializeField] private AnimationCurve pressureCurve;
    
    public Vector2 CalculatePressureForce(Vector2 blockPosition)
    {
        Vector2 centerPoint = new Vector2(3.5f, 3.5f); // Grid center
        Vector2 directionToCenter = (centerPoint - blockPosition).normalized;
        float distanceFromCenter = Vector2.Distance(blockPosition, centerPoint);
        float pressureMultiplier = pressureCurve.Evaluate(distanceFromCenter / 4f);
        
        return directionToCenter * pressureStrength * pressureMultiplier;
    }
}
```

---

### **Epic 5: Basic Tile Matching**
*As a player, I want satisfying tile matching with clear feedback and scoring.*

#### **User Story 5.1: Core Matching Logic**
**Story Points:** 5  
**Priority:** Highest  

**Acceptance Criteria:**
- Tiles match when 3+ of same color align horizontally or vertically
- Matched tiles disappear with satisfying visual and audio effects
- New tiles fill empty spaces according to directional flow rules
- Matching logic handles edge cases and corner scenarios correctly
- Match detection is performant and doesn't cause frame drops

**Implementation Tasks:**
- [ ] Implement match detection algorithm (3+ in line, horizontal/vertical)
- [ ] Add match validation and tile removal system
- [ ] Create satisfying match visual effects (particles, scaling, color changes)
- [ ] Add match audio feedback with different sounds for different match sizes
- [ ] Test matching logic with various board configurations and edge cases

**Match Detection Algorithm:**
```csharp
// Match Detection System
public class MatchDetector : MonoBehaviour 
{
    public List<Match> FindMatches(Tile[,] grid)
    {
        List<Match> matches = new List<Match>();
        
        // Check horizontal matches
        matches.AddRange(FindHorizontalMatches(grid));
        
        // Check vertical matches  
        matches.AddRange(FindVerticalMatches(grid));
        
        return matches;
    }
    
    private List<Match> FindHorizontalMatches(Tile[,] grid) { /* Implementation */ }
    private List<Match> FindVerticalMatches(Tile[,] grid) { /* Implementation */ }
}
```

#### **User Story 5.2: Basic Scoring System**
**Story Points:** 3  
**Priority:** Medium  

**Acceptance Criteria:**
- Score increases based on match size (3-match: 100pts, 4-match: 250pts, etc.)
- Zone-based multipliers apply correctly (Edge: 1x, Transition: 1.5x, Center: 2x)
- Score display updates smoothly and is clearly visible
- Scoring system supports future features like combos and special matches
- Score persists between game sessions

**Implementation Tasks:**
- [ ] Implement basic scoring logic with match size bonuses
- [ ] Add zone-based scoring multipliers
- [ ] Create score display UI with smooth count-up animations
- [ ] Add score persistence using Unity's PlayerPrefs
- [ ] Test scoring balance and adjust point values as needed

---

## 📊 **Sprint Velocity & Capacity**

### **Story Point Estimation:**
- **Total Story Points in Sprint:** 32 points
- **Team Capacity:** 1 developer, 2 weeks = ~20-25 productive hours
- **Estimated Velocity:** 25-30 story points (based on complexity and learning curve)

### **Risk Assessment:**
- **High Risk:** Center pressure system balance and implementation complexity
- **Medium Risk:** Touch input accuracy and responsiveness across devices
- **Low Risk:** Basic grid and matching implementation (well-understood patterns)

### **Sprint Buffer:**
- **Buffer Time:** 20% of sprint capacity reserved for unexpected issues
- **Learning Curve:** Additional time allocated for Unity mobile optimization
- **Testing Time:** Extra time for cross-platform testing and iteration

---

## 🔄 **Daily Progress Tracking**

### **Daily Standup Questions:**
1. **Yesterday:** What did I complete toward our Sprint Goal?
2. **Today:** What will I work on to advance the prototype?
3. **Blockers:** What's preventing me from making progress?
4. **Sprint Progress:** How are we tracking toward our Sprint Goal?

### **Sprint Burndown Targets:**
- **Week 1 Target:** Grid system + Touch input complete (50% of story points)
- **Week 2 Target:** Directional flow + Pressure system + Matching complete
- **Daily Progress:** Track story points completed vs. remaining

---

## 🛠️ **Technical Implementation Notes**

### **Unity Project Setup:**
```
FlowCrush/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GridManager.cs
│   │   │   ├── Tile.cs
│   │   │   └── GameManager.cs
│   │   ├── Input/
│   │   │   ├── TouchInputManager.cs
│   │   │   └── SwipeDetector.cs
│   │   ├── Gameplay/
│   │   │   ├── BlockSpawner.cs
│   │   │   ├── MatchDetector.cs
│   │   │   ├── PressureSystem.cs
│   │   │   └── ScoreManager.cs
│   │   └── Utilities/
│   │       ├── DirectionMapper.cs
│   │       └── ZoneCalculator.cs
│   ├── Prefabs/
│   │   ├── Tiles/
│   │   └── UI/
│   ├── Sprites/
│   ├── Audio/
│   └── Scenes/
│       ├── MainGame.unity
│       └── TestScene.unity
```

### **Core Game Logic Flow:**
```csharp
// Sprint 1 Core Game Loop
1. Player touches screen (TouchInputManager detects)
2. Swipe direction calculated (SwipeDetector processes)
3. Direction mapped to flow source (DirectionMapper converts)
4. Match detection runs (MatchDetector validates)
5. Matched tiles removed (with visual/audio feedback)
6. New blocks spawn from opposite edge (BlockSpawner handles)
7. Pressure system applies center-ward forces (PressureSystem updates)
8. Score calculated and displayed (ScoreManager updates)
9. Loop continues with next player input
```

### **Performance Targets:**
- **60 FPS** consistent during all gameplay
- **<100ms** input response time from touch to visual feedback
- **<150MB** memory usage during Sprint 1 prototype
- **Cross-platform compatibility** verified on both iOS and Android builds

---

## 📱 **Testing Strategy**

### **Core Functionality Testing:**
- [ ] Grid displays correctly on 3+ different screen sizes
- [ ] Touch input works accurately across iOS and Android
- [ ] Directional flow behaves predictably in all 4 directions
- [ ] Center pressure creates noticeable but balanced effect
- [ ] Matching logic handles edge cases correctly

### **User Experience Testing:**
- [ ] Game feels responsive and smooth during play
- [ ] Swipe gestures feel natural and intuitive
- [ ] Visual feedback clearly communicates game state
- [ ] Audio feedback enhances gameplay without overwhelming
- [ ] Overall experience demonstrates the core innovation

### **Performance Testing:**
- [ ] Maintains 60 FPS during complex sequences
- [ ] Memory usage stays within target limits
- [ ] Input response time meets <100ms target
- [ ] Battery usage is reasonable for mobile gaming

---

## 🎯 **Sprint Review Preparation**

### **Demo Script:**
1. **Show grid system** with zone visualization and responsive scaling
2. **Demonstrate touch input** with swipe detection in all 4 directions
3. **Show directional block flow** from opposite edges with smooth animation
4. **Highlight center pressure** effect on tile movement and positioning
5. **Play through complete game loop** from swipe to match to flow to score
6. **Discuss technical learnings** and plan for Sprint 2 enhancements

### **Success Metrics:**
- **Functional:** All core mechanics operational and demonstrable
- **Performance:** 60 FPS achieved on both iOS and Android builds
- **User Experience:** Core innovation is clearly evident and engaging
- **Technical:** Code quality meets standards and is well-documented
- **Sprint Goal:** Complete game loop playable and revolutionary mechanics proven

---

## 🚀 **Sprint 2 Preview**

### **Next Sprint Focus:**
- **Enhanced Visual Effects:** Polished animations and particle systems
- **Audio System:** Complete sound design and implementation
- **UI Framework:** Score display, game state management, basic menus
- **Difficulty Scaling:** Progressive challenge and complexity increases
- **Performance Optimization:** Memory usage and frame rate improvements

---

## 📈 **Definition of Success**

**Sprint 1 will be considered successful when:**
- A player can pick up the device and immediately understand the revolutionary directional flow mechanic
- The prototype clearly demonstrates spatial strategy through center-flow pressure
- Performance is smooth and responsive on target mobile devices
- Core game loop is engaging and shows potential for long-term gameplay
- Technical foundation is solid for building additional features in future sprints

---

*This Sprint 1 plan is a living document that will be updated daily based on progress, discoveries, and blockers encountered during development.*

**Sprint Start Date:** July 10, 2025  
**Sprint End Date:** July 24, 2025  
**Next Sprint Planning:** July 25, 2025