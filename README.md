# UML Class Diagram Builder

## Overview
This project is a senior capstone application designed to help programmers create **UML Class Diagrams** quickly and intuitively.

UML Class Diagrams are essential for planning object-oriented software architecture. They provide a high-level view of how classes interact—without requiring any code. However, many existing tools (like Word or PowerPoint) are inefficient for this purpose.

This application aims to solve that problem by offering a **dedicated, user-friendly interface** for building UML diagrams.

---

## Features

### Core Functionality
- **Canvas-Based Interface**
  - Start with a blank workspace
  - Drag-and-drop interaction for diagram creation

- **Class Diagram Blocks**
  - Each class is represented as a box with:
    - **Title** (Class Name)
    - **Attributes**
    - **Methods**
  - Automatically resizes based on content

- **Access Modifiers**
  - Easily define visibility:
    - Public
    - Private
    - Protected

- **Class Relationships**
  - Connect classes to represent:
    - Inheritance
    - Composition
    - Utilization (dependencies)

---

## AI Integration

This project will integrate AI using OpenRouter models to enhance the design process.

### Planned Capabilities:
- Analyze a **project description + current diagram**
- Provide feedback on:
  - Design accuracy
  - Alignment with problem requirements
  - Software design quality
- Incorporate **software design metrics** into feedback

### Input Options:
- Structured text representation of diagrams  
- Image-based diagram analysis (if feasible)

---

## Stretch Goals

Once the core UML functionality is complete, the project may expand to include:

### Additional Diagram Types
- Data Flow Diagrams (DFDs)
- Flowcharts

### Advanced Features
- Unified system for multiple diagram types
- Project-based diagram organization
- Seamless switching between diagram types

---

## Example

Below is an example of a UML Class Diagram:

![Example UML Diagram](path/to/your/image.png)

*Example diagram created for a previous project*

---

## Motivation

Many developers struggle with existing tools when creating UML diagrams. This project focuses on:

- Improving **usability**
- Reducing **friction in design workflows**
- Helping developers **focus on architecture instead of tooling**

---

## Tech Direction (Planned)

- Frontend: Canvas-based UI (C# using Avalonia UI)
- Backend / AI Integration: OpenRouter API
- Diagram Rendering: Custom logic for layout and connections

---

## Project Status

In development (Senior Project)
