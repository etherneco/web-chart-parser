# Web Chart Parser

## Overview
This repository explores techniques for extracting and normalising data from web-based charts and visualisations.

It was created to address a common real-world problem:  
data is often presented visually on websites without providing a structured or machine-readable source.

This project focuses on **data extraction**, not on rendering or visualisation.

---

## Problem Context
Many systems expose valuable information exclusively through:
- client-side rendered charts
- SVG or canvas-based visualisations
- dynamically generated HTML

In such cases, obtaining the underlying data requires:
- inspection of page structure
- parsing of embedded data sources
- reconstruction of values from presentation layers

---

## Approach
This project investigates:
- identification of data sources behind visual components
- parsing of chart-related structures (HTML, SVG, JS data)
- transformation of extracted values into structured formats
- handling of incomplete or inconsistent inputs

The emphasis is on **practical extraction**, not on perfect generalisation.

---

## Use Cases
- Data migration from legacy web systems
- Analytics and reporting pipelines
- Auditing and verification of published metrics
- One-off or automated data recovery tasks

---

## Design Principles
- Data over presentation
- Deterministic parsing where possible
- Transparency over heuristics
- Acceptance of imperfect inputs

---

## Tech Stack
- Web parsing tools
- Language-specific data processing
- Minimal dependencies

---

## Status
- Experimental utility
- Problem-driven exploration
- Not intended as a general-purpose framework

---

## Why This Project Exists
This repository demonstrates:
- experience with non-ideal data sources
- practical web parsing skills
- understanding of the separation between data and presentation
- realistic handling of edge cases in external systems

It reflects engineering work often required in integration-heavy environments.
