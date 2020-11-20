# GPE-Trive
### This project
This repository contains the unity project/source code for the space store tycoon game.
The aim of the game is to supply and manage your store in space. 

### Startup
##### - Requirements
- Unity version 2019.4.14f1 (LTS*)
\* long term service
- This repository

##### - Unity
[ With Unity Hub (recommended) ] 
1. Clone this repository.
2. Install unity hub from their website.
3. Install the correct unity version in the unity hub launcher in the tab "installs".
4. When the version is ready, Go to the "projects" tab and press "add".
5. Search for the root folder of the cloned unity project and press "Select folder".
6. The project has now been added and can selected to start.

[ Without Unity Hub ] 
1. Clone this repository.
2. Install the correct unity version.
3. Open the project by:
3.1 : Opening the unity root folder and navigating to the scene folder => "/Assets/GOAT/Scenes".
3.1.2 Open a scene by double clicking.
3.2 : Opening unity with the correct version.
3.2.2 Go to File > Open Project and select the root folder.

### Structure
##### - Folders
The folders are structured in the following order:
- Audio (contains audio files and audio settings)
- Materials (Contains custom/unity materials)
- Models (Contains model data (fbx, obj))
- Prefabs (Contains pre fabricated objects defined to be drag and drop in any scene)
- Scenes (Contains unity scenes)
- Shaders (Contains custom/unity shaders)
- ScriptableObjects (Contains data for scripts)
- Scripts (Contains custom scripts for the in game systems)

In each of these folders the different subsystem of the game have a seperate folder to order every asset for that particular system.

### Building the game
##### How to
[Build without changing settings]
1. Go to File > Build And Run.
2. Wait for completion.

[Build and change settings]
1. Go to File > Build Settings.
2. If scenes have been added, you have to manually add them to the build list.
3. In this screen you can also add the target to build for and change it to a development build.
4. After all settings are done you can build your project.