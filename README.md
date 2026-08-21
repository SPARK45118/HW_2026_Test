# 🏴‍☠️ Doofus Adventure — Pirate Edition

A 3D platformer built in Unity 6 for the **Hitwicket Game Developer Challenge**.

Guide **Doofus**, a wandering cube-headed pirate, across a chain of disappearing hex platforms (**Pulpits**) floating in the middle of a tropical pirate cove — without falling into the sea!

---

## 📖 The Story Behind the Build

Instead of leaving Doofus on a plain, empty background, I built out a full narrative around the core mechanic: Doofus is a **pirate whose ship has wrecked**, and he's stranded out at sea. The only way back to safety is to make his way to the beach — where more ships, treasure, and palm trees await — by hopping across a trail of crumbling **Pulpits** that spawn two at a time, exactly per Hitwicket's original guideline (only two Pulpits active simultaneously, spawning adjacent to the previous one).

So mechanically, nothing about the core rules changed — Doofus is still walking across a fixed number of platforms that spawn and disappear on a timer. But narratively, every design choice was made to answer *"why is this happening?"*: he's not just testing platforms, he's a shipwrecked pirate racing across a sinking archipelago to reach dry land before the sea swallows his path. That's the lens I used for every visual and environmental decision below.

---

## 🎮 Gameplay Overview

Doofus loves exploring green hex platforms called **Pulpits**. The catch: each Pulpit only lasts a few seconds before it crumbles and falls apart. Doofus has set himself a challenge — walk across at least **50 Pulpits** without falling into the water.

- Every Pulpit has a **live countdown timer** displayed right on its surface, so you always know how much time you have left before it breaks.
- As a Pulpit's timer runs low, it **flashes red** as a warning before it destroys itself and drops away.
- Land on a new Pulpit to **score a point** — miss your footing, and Doofus tumbles into the ocean below, ending the game.
- Reach **50 successful landings** and a special animated banner pops up: *"Hi Hitwicket Team! Challenge Complete – 50 Pulpits Reached!"*

---

## 🏝️ The Pirate Theme

Instead of a plain, empty background, Doofus's platform-hopping challenge is set in a **pirate cove**, built to match his shipwreck story:

- A **weathered pirate ship** rests beached on the sandy shore — the destination Doofus is trying to reach, and a nod to the wreck he started from.
- A cluster of **smaller rowboats** bob near the coastline.
- **Palm trees, rocks, and drifting clouds** dress the island and skyline.
- A **shark fin** occasionally cuts through the water — a subtle reminder of what's waiting below if Doofus falls!
- The flat ocean plane was replaced with **tiled hex water and sand pieces**, matching the same hex-grid aesthetic as the Pulpits themselves, so the whole world feels like one consistent, hand-crafted island rather than a generic 3D scene.
- Doofus himself wears a **pirate hat**, tying the whole "shipwrecked adventurer hopping between crumbling platforms" fantasy together.

The goal was to take a simple mechanical prototype (a cube jumping between platforms) and give it a **story and setting** — Doofus isn't just testing platforms, he's a castaway pirate trying to make it across a treacherous, sinking archipelago to reach the beach and rebuild.

---

## ✨ Features Implemented

### Level 1 — Movement & Platform Placement
- WASD / Arrow key movement, reading Doofus's speed from the **Doofus Diary JSON** config file.
- Pulpits spawn dynamically, adjacent to the current platform in a random direction (not just a fixed forward line), with occasional randomized gaps requiring a **jump** (Spacebar) to cross.
- Only two Pulpits ever exist at once, per the assignment spec.

### Level 2 — Scoring
- Raycast-based landing detection increases the score every time Doofus successfully lands on a *new* Pulpit.
- Score UI has a satisfying scale-pop animation on every increase.

### Level 3 — Start & Game Over Screens
- A **Start Screen** with a Play button gates the beginning of the game.
- Doofus falls with simulated accelerating gravity if he steps off a Pulpit or it breaks beneath him.
- A **Game Over screen** displays the final score and a Restart button that fully resets the run.

### Extra Polish
- **Live countdown timers** rendered directly on each Pulpit's surface (TextMeshPro), matching the reference gameplay video.
- **Dash ability** — hold a direction and tap **Left Shift** to burst forward at increased speed for a short duration, with a cooldown before it can be used again.
- **50-Pulpit Challenge Complete banner** — a custom animated (slide-in, scale-pop, fade-out) UI message celebrating the in-brief goal of reaching 50 Pulpits.
- Full **pirate cove environment**: beached ship, rowboats, palm trees, rocks, clouds, shark, and a custom hex-tiled shoreline replacing the default ocean plane.
- **Background music** that plays during active gameplay and stops on Game Over.
- Third-person follow camera with a subtle **shake effect** on landing after a fall.

---

## ⚡ Doofus's Dash Ability

Doofus isn't just a walker — he's got a burst of pirate agility built in:

- Hold a movement direction and tap **Left Shift** to **dash**, temporarily multiplying his speed for a short burst.
- After dashing, the ability goes on a **cooldown timer** before it can be used again — encouraging players to time their dashes carefully rather than spam them.
- This is especially useful for **crossing the randomized gaps** between Pulpits that require more than a simple walk or jump to clear in time.

*(See the "Dash Ability" screenshot above for a snapshot of Doofus mid-dash.)*

---

## 🕹️ Controls

| Action | Key |
|---|---|
| Move | WASD / Arrow Keys |
| Jump (across gaps) | Space |
| Dash | Left Shift (while moving) |
| Restart (after Game Over) | Restart button (UI) |

---

## 🛠️ Built With

- **Unity 6**
- **TextMeshPro** for all UI and in-world text
- **JSON (Doofus Diary)** for data-driven game tuning (speed, Pulpit destroy timing, spawn timing)
- Free pirate-themed 3D asset pack — [**Creatus Pirate Pack**](https://creatus.itch.io/creatus-pirate) via itch.io — used for the character models (Doofus's pirate skin), ship, boats, palm trees, rocks, and other environment props

---

## 📸 Gameplay Screenshots

<p align="center">
  <img src="Doofus Adventure GamePlay Screenshots/Screenshot 2026-08-21 153018.png" width="410" />
  <img src="Doofus Adventure GamePlay Screenshots/Screenshot 2026-08-21 153029.png" width="410" />
</p>
<p align="center">
  <img src="Doofus Adventure GamePlay Screenshots/Screenshot 2026-08-21 153629.png" width="410" />
  <img src="Doofus Adventure GamePlay Screenshots/Screenshot 2026-08-21 153742.png" width="410" />
</p>
<p align="center">
  <img src="Doofus Adventure GamePlay Screenshots/Screenshot 2026-08-21 153750.png" width="410" />
  <img src="Doofus Adventure GamePlay Screenshots/Screenshot 2026-08-21 154008.png" width="410" />
</p>
<p align="center">
  <img src="Doofus Adventure GamePlay Screenshots/Screenshot 2026-08-21 154016.png" width="410" />
  <img src="Doofus Adventure GamePlay Screenshots/Screenshot 2026-08-21 154033.png" width="410" />
</p>

---

## 🎥 Gameplay Video

- ▶️ **YouTube:** [https://www.youtube.com/watch?v=JdMK6nrhDw0](https://www.youtube.com/watch?v=JdMK6nrhDw0)
- 📁 **Google Drive (full quality):** [https://drive.google.com/drive/folders/1tp7rQ_nv7XHOUZhWn35-y1l5nz_VjICg?usp=sharing](https://drive.google.com/drive/folders/1tp7rQ_nv7XHOUZhWn35-y1l5nz_VjICg?usp=sharing)

---

## 📂 Repo Notes

Gameplay screenshots and video are included in this repository as required by the assignment guidelines.
