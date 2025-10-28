# Path Manager System

## 🎯 Purpose
The Path Manager creates an **infinite road** by reusing a fixed pool of prefabs, keeping the player near world origin to avoid floating-point issues and memory leaks.

---

## ⚙️ Core Logic
1. Measure the **length** of the first path prefab using its child renderer.
2. Pre-spawn `pathCount` tiles in a row.
3. Continuously check which tile has fallen behind the camera.
4. When a tile is out of view, **recycle** it to the front instead of destroying it.
5. Apply a small positional bias to prevent visible seams.

---

## 🧩 Key Features
- Works even if the initial path prefab isn’t placed in the scene.
- Handles **multiple prefab variations** (randomized).
- Fixes **static-batched ghosting** by disabling all Static flags at runtime.
- Works smoothly with physics and player movement scripts.
- Keeps tiles parented under the `PathManager` for easy cleanup.

---

## 💡 Important Implementation Details

### 🔸 Measuring Segment Length
```csharp
zPathSize = firstPath.transform.GetChild(0).GetComponent<Renderer>().bounds.size.z;

🔸 Repositioning Loop
if (pathList[listPathIndex].transform.position.z < destroyDistance)
{
    Vector3 nextPos = FarthestPath().transform.position + Vector3.forward * zPathSize;
    pathList[listPathIndex].transform.position = nextPos;
    listPathIndex = (listPathIndex + 1) % pathList.Count;
}


The loop constantly reuses tiles instead of instantiating new ones.

🔸 Dynamic Static-Flag Cleanup
UnityEditor.GameObjectUtility.SetStaticEditorFlags(obj, UnityEditor.StaticEditorFlags.None);


Ensures that no recycled prefab keeps baked geometry or GI data that would create “ghost” visuals.

🔆 Lighting Guidelines
- Tiles: Static OFF, Contribute GI OFF.
- Use Light Probes for consistent dynamic lighting.
- Environment: baked static lighting is fine (buildings, horizon, etc.).

🧱 Common Issues
| Issue                       | Cause                      | Fix                                         |
| --------------------------- | -------------------------- | ------------------------------------------- |
| Ghost props remain in scene | Prefab marked Static       | Disable all Static flags + Clear Baked Data |
| Visible seams between tiles | Mismatch in segment Z-size | Verify consistent mesh length               |
| Performance spikes          | Too many Instantiate calls | Use recycling loop only                     |
| Blue/gray tint              | Default Skybox or baked GI | Adjust Lighting Environment to warm color   |


🧠 Future Improvements

- Add obstacle and collectible spawning hooks per segment.
- Support curved road generation (Bezier or spline-based).
- Integrate object pooling for decorations.