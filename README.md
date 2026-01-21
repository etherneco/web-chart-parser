# Web Chart Parser

## Overview
Web Chart Parser is a small ASP.NET MVC app that plots mathematical functions on a chart.
It parses expressions entered in the UI and renders the graph as a PNG image.

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
