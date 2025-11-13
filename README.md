This project is a Unity-based Memory Card Game that focuses on clean architecture, scalability, and maintainability.
The game includes features such as:

Dynamic grid generation (variable rows & columns)

Save/Load system using JSON

Timer, scoring, turns, and match tracking

Replay system with preserved grid size

UI management, win state UI, and game blocking

Audio feedback system (match, flip, UI sounds)

Event-driven architecture for communication between systems

Decoupled logic for cards, UI, audio, saving, and game flow

The goal of the architecture is to avoid tightly coupled scripts and ensure each system has a single responsibility, making the game easy to expand, debug, and maintain.

Why we used an Event Bus

✔ Decouples systems — GameManager does not need references to UIManager, AudioManager, or other systems.
✔ Reduces dependencies — fewer GetComponent(), singleton calls, and tight coupling.
✔ Cleaner, more scalable architecture — new listeners can be added without modifying existing code.
✔ 100% avoids spaghetti code — especially in Unity where everything communicates with everything.
✔ Great for events like:

Card flipped

Match found

Timer expired

Score updated

Game won

Example in the game:

When two cards match, EventBus.RaiseMatch() triggers:

UI updates

Score increases

Audio plays

Achievements or analytics could be added later without touching GameManager

This makes the architecture extremely flexible.

✅ Decoupled System: How & Why
How we achieved decoupling

Event Bus for communication (no script directly calls UI/Audiomanager).

Separate responsibility scripts:

GameManager → game flow & rules

Card → card flipping & state

UIManager → UI updates

AudioManager → sound playback

SaveSystem → JSON save/load

GridGenerator → creates and positions cards

No hard references
Systems don't hold references to each other; they communicate only through events or configuration objects.

Data separated from logic
Game settings (rows/cols/time) stored in config objects, not hidden inside logic.

Why decoupling is important

✔ Easy to change or replace systems
You can swap the UI, audio system, or save system without touching core gameplay.

✔ Massively reduces bugs
Because fewer scripts depend on each other, it's harder to break the game accidentally.

✔ Easier to add new features
Add power-ups, animations, achievements, combos—without changing GameManager.

✔ Better performance & organization
Scripts only react to events they care about.

✔ Professional game architecture
This is how large Unity projects scale (PUB/SUB is common in AAA and mobile).
