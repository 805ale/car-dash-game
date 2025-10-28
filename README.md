# 🚗 Car Dash Game (Unity Prototype)

A small **endless-runner prototype** built in Unity featuring lane switching, smooth tween-based car tilt, and an infinite road generation system.  
This project focuses on clean C# architecture, visual polish, and learning to build production-style gameplay systems.

---

## 🎯 Overview
This project simulates an endless driving game inspired by mobile hyper-casuals.  
The player swipes (or uses arrow keys) to switch lanes while the car smoothly tilts into the motion.  
A dynamic path manager continuously recycles road segments for an infinite level feel.

---

## 🧩 Core Systems
### 🚘 Player Movement & Tilt
- Swipe or arrow input to move between fixed lanes.
- Movement handled with **DOTween sequences** for smooth transitions.
- Car tilts using **yaw + roll** relative to prefab rotation (`neutralLocalRot`) for realism.
- Camera follows player with `SmoothDamp` and physics interpolation for zero jitter.

### 🎥 Camera System
- LateUpdate-based follow script that uses **Rigidbody interpolation** to remove jitter.
- Adjustable smoothing and axis lock.

### 🛣️ Infinite Path Manager
- Procedurally spawns and recycles road prefabs to create an endless track.
- Automatic segment measurement and position alignment.
- Supports multiple prefab variations for variety.
- Prevents z-fighting and “ghost geometry” by clearing static flags dynamically.

### 🌞 Lighting & Visuals
- URP setup with MSAA and FXAA for anti-aliasing.
- Light Probes used instead of baked lighting for dynamic tiles.
- Color grading for a warm, daylight look.

---

## 🧠 What I Learned
- Designing modular systems for **endless environments**.
- Handling **rotation relative to prefab orientation** to preserve local alignment.
- Avoiding **float precision and jitter** through object recycling and physics interpolation.
- Managing lighting and performance when moving prefabs dynamically.
- Using Git + Markdown for proper **versioned documentation**.

---

## 🛠️ Tech Stack
| Component | Tool |
|------------|------|
| Engine | Unity 2022+ (URP) |
| Language | C# |
| Tweening | DOTween |
| Version Control | Git + GitHub |
| IDE | Visual Studio / Rider |

---

## 🗺️ Folder Highlights
| Folder | Purpose |
|---------|----------|
| `Assets/Scripts/Player/` | Player and input logic |
| `Assets/Scripts/Environment/` | Path generation & recycling |
| `Docs/` | Technical notes and dev logs |
| `Prefabs/` | Reusable environment tiles |

---

## 📸 Screenshots


---

## 🧾 License
Educational & portfolio use only.