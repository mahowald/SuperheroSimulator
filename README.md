# SuperheroSimulator

This is the source code for [Superhero Simulator](http://mahowald.org/superhero/).

There are approximately 3,600 lines of code across 81 files; also, this doesn't include game assets (models, textures, etc).

Some fun files to look at are the [GenericCharacterController class](https://github.com/mahowald/SuperheroSimulator/blob/master/scripts/Tactical/GenericCharacterController.cs), which handles controlling characters via the animator in the "tactical" (3rd person real-time) game mode. The character controller takes control input (e.g., from the character AI or a human player) and translates that into the appropriate animation and locomotion behavior. Enemy character behavior is driven by the [AIEnemy class](https://github.com/mahowald/SuperheroSimulator/blob/master/scripts/Tactical2/AIEnemy.cs). 

The turn-based "strategic" gameplay is managed by the [StrategicGameController class](https://github.com/mahowald/SuperheroSimulator/blob/master/scripts/Strategic/StrategicGameController.cs). 

N.B. After uploading to GitHub, I've noticed that there's some inconsistent indentation (probably because I switched IDEs partway through this project); I'm working to resolve this issue.
