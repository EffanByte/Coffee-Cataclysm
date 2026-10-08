# ☕ Coffee Cataclysm

A multiplayer Unity game prototype that combines **cooperative café management, character classes, combat, procedural environments, enemies, and pets**.

Players operate inside a coffee bar, prepare customer orders, and can move into an outdoor combat area featuring enemies, weapons, character abilities, and dynamically generated forest content.

The project was built around interconnected gameplay systems with multiplayer synchronization through **PlayroomKit**.

---

## 🎮 Core Features

### 4-Player Multiplayer
- Supports rooms with up to **4 players**
- Player position synchronization
- Scene/state-aware player visibility
- Networked held items
- Synchronized coffee interactions
- Synchronized customer orders
- RPC-based gameplay events

### ☕ Coffee & Order System
Players can interact with coffee machines and prepare different blends using multiple bean types.

Supported coffee results include:

- Brown Coffee
- White Coffee
- Mixed Coffee

The system includes:

- bean insertion
- brewing progress
- timed brewing states
- blend generation
- coffee pickup
- AI customer orders
- order bubbles
- synchronized multiplayer orders

### ⚔️ Character Classes

The project contains multiple player archetypes:

- **Attacker**
- **Tank**
- **Melee**
- **Healer**

Each class extends a shared character system and can implement its own combat behaviour and abilities.

### 🌲 Procedural Forest Generation

The outdoor area uses a chunk-based procedural environment system.

Chunks are generated around the player and can contain:

- trees
- rocks
- bushes
- mushrooms
- environmental clusters

Distant chunks are removed as the player moves, allowing the environment to be generated around the active play area.

### 💀 Enemy System

The outdoor combat area contains multiple enemy variants, including:

- Skeleton
- Skeleton Archer
- Skeleton Mage
- Skeleton Minion
- Skeleton Warrior

Enemies are spawned around the player through an enemy spawning and pooling system.

### 🐉 Companion / Pet System

Several companion types are implemented:

- Dragon
- Plant
- Serpent
- Sloth
- Spider

Pets contain their own animation and attack behaviour, including systems such as fire attacks and web projectiles.

### 🏹 Combat & Weapons

The project includes systems for:

- melee combat
- ranged attacks
- bows / crossbows
- arrows
- mage attacks
- spells
- projectile pooling
- combat animations
- health management

---

## 🧠 Game State Architecture

Gameplay is divided between two main states:

```text
┌──────────────────────┐
│      COFFEE BAR      │
│                      │
│  Orders              │
│  Coffee Brewing      │
│  Item Interaction    │
│  Customer AI         │
└──────────┬───────────┘
           │
           │ Portal / State Transition
           ▼
┌──────────────────────┐
│       OUTSIDE        │
│                      │
│  Combat              │
│  Enemies             │
│  Character Classes   │
│  Pets                │
│  Procedural Forest   │
└──────────┬───────────┘
           │
           └──────────────► Coffee Bar
```

Scene transitions are coordinated through a finite-state-machine architecture using **UnityHFSM**.

Player scene state is also synchronized so remote players are only shown when they occupy the same gameplay state.

---

## 🌐 Multiplayer Architecture

PlayroomKit is used for multiplayer communication.

```text
                 PLAYROOM
                     │
             ┌───────┴───────┐
             │   4 Players   │
             └───────┬───────┘
                     │
        ┌────────────┼────────────┐
        │            │            │
        ▼            ▼            ▼
    Position      Game State    Held Items
      Sync           Sync          Sync
        │            │            │
        └────────────┼────────────┘
                     │
                     ▼
               Gameplay RPCs
                     │
        ┌────────────┼─────────────┐
        ▼            ▼             ▼
     Orders       Brewing       Interactions
```

RPCs are used for gameplay events such as:

- inserting coffee beans
- receiving blends
- creating customer orders
- serving orders
- held-item changes
- dropping items
- player state transitions

---

## 🛠️ Tech Stack

- **Unity 6**
- **C#**
- **PlayroomKit**
- **UnityHFSM**
- **Universal Render Pipeline**
- **Unity Input System**
- **Cinemachine**
- **Unity AI / Navigation**

Developed with:

```text
Unity 6000.0.51f1
```

---

## 📁 System Structure

Some of the major gameplay areas are organized around:

```text
Assets/Scripts/
├── Ai/
│   ├── OrderManager
│   ├── AiBar
│   └── EnemySpawnerBar
│
├── Coffee/
│   ├── CoffeeMakerInteraction
│   └── CoffeeBlendItem
│
├── EnemyOutSide/
│   ├── EnemySpawner
│   ├── EnemyPoolHandler
│   └── EnemyClasses/
│
├── ForestGenerator/
│   └── ForestGenerator
│
├── Manager/
│   ├── PlayroomManager
│   ├── GameStateManager
│   └── UIManager
│
├── Pets/
│
├── Player/
│   ├── ClassManager
│   └── Classes/
│
└── Weapons/
```

---

## 🔧 Getting Started

### Requirements

Install:

```text
Unity 6000.0.51f1
```

Using the same editor version is recommended.

### Clone

```bash
git clone https://github.com/EffanByte/Coffee-Cataclysm.git
```

Then:

1. Open **Unity Hub**
2. Add the cloned project
3. Open it with Unity `6000.0.51f1`
4. Allow Unity to restore project packages
5. Open one of the gameplay scenes under `Assets/Scenes`
6. Press **Play**

---

## 🎯 Technical Focus

This project explores several areas of gameplay engineering:

- real-time multiplayer synchronization
- RPC-driven gameplay
- finite state machines
- multiplayer scene-state management
- AI customer interactions
- gameplay item synchronization
- class-based player architecture
- procedural world generation
- object pooling
- combat systems
- enemy spawning
- companion behaviour

---

## 👨‍💻 Author

**Effan Shakeel**

Gameplay Programmer focused on gameplay systems, AI, multiplayer, procedural generation, simulation, and real-time graphics.

[GitHub](https://github.com/EffanByte) · [LinkedIn](https://www.linkedin.com/in/effan-shakeel-42a58721a/)
