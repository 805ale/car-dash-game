## [2025-10-27] Player Dash System
- Rebuilt movement logic for smoother transitions  
- Learned quaternion multiplication for relative rotation  
- Fixed prefab rotation offset  
- Added clean DOTween sequence structure  

## [Next]
- Add VFX trail when dashing
- Record GIF demo

## [2025-10-28] Infinite Path & Lighting Fixes
- Implemented runtime tile recycling (infinite road system).
- Fixed “ghost object” issue caused by static batching.
- Added dynamic static flag clearing for runtime safety.
- Adjusted lighting setup for dynamic prefabs (warm tone, light probes only).

## [2025-10-27] Player & Camera Updates
- Added physics-based forward motion with interpolation.
- Implemented LateUpdate camera follow with smoothing.
- Tuned FOV and anti-aliasing to reduce road shimmer.

## [2025-10-29] Traffic System Integration
- Added `OtherCarManager` for traffic car spawning and pooling.
- Implemented object pooling to avoid runtime allocation.
- Integrated with `PathManager` recycling logic.
- Documented behavior and usage in `OtherCarManagerNotes.md`.