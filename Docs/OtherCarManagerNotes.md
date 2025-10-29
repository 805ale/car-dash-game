# Other Car Manager System

## 🎯 Purpose
Controls the spawning, pooling, and recycling of **AI traffic cars** on the endless road.  
This system ensures cars appear dynamically without ever instantiating or destroying objects at runtime — improving performance and stability.

---

## ⚙️ Overview
The `OtherCarManager` manages two parent containers:

- **ActiveCars** → cars currently visible on the road.  
- **UnactiveCars** → pooled, inactive cars ready for reuse.

When the PathManager recycles a new road segment, this system:
1. Randomly decides whether to spawn a car on that segment (`carFreq`).
2. Chooses a random lane marker from the path segment.
3. Pulls a car from the inactive pool and activates it.
4. Optionally flips it 180° based on `reverseCarFreq` to simulate opposite traffic.
5. Recycles cars that fall behind the player/camera (`FindCarAndReset()`).

---

## 🧩 Key Components
| Variable | Description |
|-----------|-------------|
| `carArray` | All possible car prefabs to spawn. |
| `initialCarNumbers` | How many cars to pre-instantiate into the pool. |
| `carFreq` | Probability that a new car spawns when a new path tile appears. |
| `reverseCarFreq` | Chance that the spawned car drives in reverse direction. |
| `previousCarPos` | Last used lane position (avoids duplicates). |
| `pathManager` | Reference to `PathManager` to get active road tiles and destroy distance. |

---

## 🧱 Core Methods
### 🏁 `Start()`
Pre-initializes the car pool by instantiating `initialCarNumbers` cars and storing them under the **unactiveCars** parent.

### 🚗 `CheckAndDisableCarPath()`
Triggered whenever a new path segment is spawned:
- Randomly decides if a car should appear.
- Picks a random lane (child position of the path prefab).
- Activates a car from the pool and places it there.

### 🔁 `FindCarAndReset()`
Checks all active cars each frame (or periodically) and moves any that fall behind the camera/player back to the inactive pool.

---

## ⚙️ Integration with PathManager
- `CheckAndDisableCarPath()` can be called each time a tile is recycled in `PathManager`.
- `pathManager.destroyDistance` ensures consistent despawn behavior.
- Works with the same lane markers used by your `PathManager` prefabs.

---

## 🧠 Performance Notes
- Uses **object pooling**, so no runtime `Instantiate()` or `Destroy()` calls after startup.
- Keeps hierarchy organized via `ActiveCars` / `UnactiveCars`.
- Avoids same-lane spawns by tracking `previousCarPos`.
- Recycle checks can run every few frames for even better performance if needed.

---

## 🧩 Future Improvements
- Add lane-based collision avoidance or spacing logic.
- Integrate simple AI movement (constant speed, lane switching).
- Add difficulty curve by increasing `carFreq` over time.
- Sync car spawning probability with player speed.

---

## 🔗 Related Docs
- [`PathManagerNotes.md`](./PathManagerNotes.md) — explains the infinite road recycling system that triggers car spawns.
