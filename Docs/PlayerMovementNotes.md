# Player Movement & Dash System

## 🎯 Goal
Implement smooth lane changes with subtle rotation to make dashes feel dynamic and responsive.

---

## 🔍 Summary of Logic
- The player can move between 5 fixed lanes on the X-axis.  
- Each swipe triggers a tween (`DOMoveX`) to the next lane.  
- While moving, the car tilts using:
  - **Yaw (Y)** → turns nose toward the direction
  - **Roll (Z)** → banks slightly into the turn
- The tilt is applied relative to the prefab’s **neutralLocalRot**, so the starting rotation is preserved.

---

## ⚙️ Key Code Concept
```csharp
Quaternion tiltTarget = neutralLocalRot * Quaternion.Euler(0f, yawTilt * dir, -rollTilt * dir);
This ensures rotation happens relative to the car’s natural orientation, not world space.

🧱 DOTween Sequence

Move the car sideways (DOMoveX)

Lean into the dash (DOLocalRotateQuaternion)

Smoothly return to neutral

🧭 Diagram

Front view (roll) | Top view (yaw)

Left tilt (+Z) ← 🚗 → Right tilt (−Z)
Turn left (−Y) ← 🚗 → Turn right (+Y)

💡 Future Improvements

Add acceleration easing based on speed

Trigger particle trails or camera shake

Integrate obstacle detection before dash

Mobile build testing