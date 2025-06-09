# VELCRO UI
![](https://img.shields.io/badge/2025--03--26-1.1.6-green)
![](https://img.shields.io/badge/Code%20Coverage-82.3%25-green)


This repository contains the source code and Unity package for the **VELCRO UI** system.

## Velcro UI Functionality
* A library of USS classes (structure, style, and controls)
* Templates such as controls (buttons, sliders, etc.) 
* UI Prefabs for complete functionality (settings window, notifications, etc.) 

## Documentation
* [Main Package Page](https://varlab-dev.atlassian.net/wiki/spaces/CV2/pages/1118404624/VELCRO+UI)
* [Structure Classes](https://varlab-dev.atlassian.net/wiki/spaces/CV2/pages/1124565004/Structural+USS+Classes)
* [Style Classes](https://varlab-dev.atlassian.net/wiki/spaces/CV2/pages/1125089290/Style+USS+Classes)
* [Changelog & Version History](https://varlab-dev.atlassian.net/wiki/spaces/CV2/pages/1302757406/Changelog+Version+History)

## Installation

### Package Manager
**Velcro UI** can be found in the [CORE UPM Registry](http://upm.core.varlab.org:4873/) as `com.varlab.velcro`.

Navigate to the **Package Manager** window in the Unity Editor and install the package under the "My Registries" sub-menu.

For help getting started, view the documentation listed above, and import samples through the package manager.

### Legacy Installation
In the `Packages/manifest.json` file of the Unity project project, add the following line to dependencies:

`"com.varlab.velcro": "ssh://git@bitbucket.org/VARLab/velcro.git#upm"`

Optionally, replace `upm` with a branch name or label such as `1.3.1` to track a specific package version.