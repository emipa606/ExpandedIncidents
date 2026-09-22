# GitHub Copilot Instructions for Expanded Incidents (Continued)

Welcome to the Expanded Incidents (Continued) mod project for RimWorld. This document provides an overview of the mod, including its key features, systems, coding patterns, conventions, XML integration, Harmony patching, and specific suggestions for using GitHub Copilot effectively within the context of this project.

## Mod Overview and Purpose

**Expanded Incidents (Continued)** enhances your RimWorld experience by increasing the variety and depth of the incidents that your colony may encounter. The mod introduces several new events, each designed to be balanced and impactful. These incidents are crafted to be mitigable or even preventable with proper colony management, adding a layer of strategic complexity to the game. Importantly, this mod is highly compatible; it does not require a new save game and can seamlessly integrate with existing systems, including the Psychology mod if installed.

## Key Features and Systems

**New Incidents:**

- **Quarrel:** Friendly pawns have their relationships tested through arguments, until they reconcile.
- **Homesick:** A pawn feels homesick and may influence others, potentially making the entire colony unhappy.
- **Cliques:** Social tension causes the colony to divide into factions, which can lead to conflicts.
- **Sabotage:** Mysterious malfunctions hint at a saboteur in the colony.
- **Thief:** Thieves attempt to steal from the colony, but may be caught and confronted by colonists or pets.

## Coding Patterns and Conventions

- The project employs static classes extensively for defining game objects, such as `HediffDefOfIncidents`, `JobDefOfIncidents`, and `ThoughtDefOfIncidents`.
- Internal classes like `IncidentWorker_CliquesForm` and `IncidentWorker_Quarrel` extend from the `IncidentWorker` base class, focusing on handling specific incidents.
- Naming conventions follow a PascalCase pattern for classes and methods, maintaining clarity and consistency throughout the codebase.

## XML Integration

- XML is utilized to define and configure game elements such as incidents, jobs, and thoughts.
- Ensure XML files are well-structured and references to C# classes are correctly aligned. This integration is crucial for defining the behaviors and properties of new incidents.

## Harmony Patching

- The mod uses Harmony to patch existing RimWorld methods for extending or modifying behavior.
- Consider methods in the `HarmonyPatching` class when implementing or adjusting functionality across the game.
- Harmony patches should be carefully crafted to avoid conflicts with other mods. Use conditional checks and postfix/prefix techniques as necessary.

## Suggestions for Copilot

- When generating boilerplate code for new incidents or features, use Copilot to suggest method structures and repetitive patterns. This can streamline developing new event handlers and game logic.
- Leveraging Copilot to draft initial XML files can accelerate defining incident configurations, but always verify the output for accuracy.
- Copilot can assist in writing robust Harmony patches by suggesting potential configurations for prefixes, postfixes, and transpilations.
- When managing settings or mod configurations, use Copilot to set up property getters and setters quickly within classes like `EI_ModSettings`.

These instructions aim to provide a comprehensive foundation for utilizing GitHub Copilot effectively in maintaining and expanding the Expanded Incidents (Continued) mod. For further assistance, refer to the project’s documentation or RimWorld's modding resources.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

