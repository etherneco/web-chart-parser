# Web Chart Parser

## Overview
Web Chart Parser is a small ASP.NET MVC app that parses mathematical expressions and renders their charts as PNGs.
It focuses on clean parsing, predictable rendering, and a simple UI for experimenting with functions.

## Features
- Expression parser with operator precedence, parentheses, and unary minus.
- Common math functions and constants (sin, cos, tan, sqrt, log, ln, exp, pi, e).
- Implicit multiplication (e.g., `2pi`, `(2+1)(3+1)`).
- Adjustable chart scale and axis step size.

## Example Expressions
- `x^2 + 2*x + 1`
- `sin(x) + cos(x)`
- `sqrt(4 + x^2)`
- `abs(x) - 2`
- `log(10*x)`
- `2pi`
- `(2+1)(3+1)`

## Recruiter Notes
What this project demonstrates:
- A custom expression parser (tokenization + RPN) with clear operator precedence.
- Defensive handling of edge cases (invalid input, divide-by-zero, empty expressions).
- A UI-to-controller flow that renders images and supports configurable chart scale.
- Unit tests covering parser behavior and helper utilities.
- Consistent C# style, analyzers, and formatting rules.

Design decisions:
- Uses a simple parser instead of heavy dependencies to keep behavior explicit.
- Keeps chart rendering deterministic (fixed image size, explicit scaling).
- Accepts common math notation and unicode symbols for usability.

Potential improvements (if extended):
- Domain/range clipping and discontinuity handling.
- Better function validation with user-friendly errors.
- Configurable output size and export formats.

## Usage
1. Open the home page.
2. Enter an expression.
3. Adjust **Scale** and **Axis step** if needed.
4. Click **Draw** (or press Enter).

## Tech Stack
- ASP.NET MVC (target framework net461)
- System.Drawing for rendering

## Status
- Educational/demo utility
- Not optimized for large or complex plots
