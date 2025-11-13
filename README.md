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

