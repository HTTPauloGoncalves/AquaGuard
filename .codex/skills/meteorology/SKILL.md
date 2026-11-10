---
name: meteorology
description: Interpret weather observations and forecasts for AquaGuard reservoir monitoring, including rainfall context, station metadata, units, time windows, and uncertainty.
---

# AquaGuard Meteorology

Use this skill when reasoning about weather inputs, rainfall, or weather context for reservoir measurements and alerts.

- Read `docs/context.md` and relevant project instructions before proposing repository changes.
- Keep observed weather, forecast data, and calculated indicators distinct. Preserve source, location, unit, timestamp, and accumulation interval where available.
- Check time zone, observation interval, missing data, sensor placement, and measurement quality before comparing weather with water-level changes.
- Describe relationships as correlations or indicators unless evidence supports a stronger conclusion. Rainfall and a rising level alone do not establish causality.
- Do not invent local weather values, thresholds, station capabilities, or forecasts. Mark assumptions and request missing site-specific facts when they affect the conclusion.
- Use explicit units and explain any conversion or aggregation. Keep recommendations understandable and useful to the AquaGuard MVP.
- Treat alerts as prompts to investigate, not definitive meteorological or hydrological diagnoses.
