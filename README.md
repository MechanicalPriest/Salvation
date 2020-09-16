## Mechanical Priest Toolkit
The [Mechanical Priest Toolkit](https://app.mechanicalpriest.com/) (MPT) aims to assist in modelling and tinkering with 
different ways to heal in World of Warcraft. It also doubles as a good way to find unique mechanical interactions and bugs.

**Looking for the actual web application?**
If you aren't looking to contribute to development and just want to use the MPT, [go here](https://app.mechanicalpriest.com/).

### How does it work?
MPT uses a front-end application to allow players to modify what is referred to as a "Profile". A Profile is everything 
about a player relevant to modelling how it heals and providing enough information to produce a result once it's run 
through the modelling API. 

- Character stats
- Gear
- Talents
- Racials/Covenant choice
- Spell Cast patterns
- Overhealing patterns
- Item usage patterns

This Profile is passed through to the API which looks at each aspect of the player and builds up the array of spells, 
items, talents and other interactions the encompass a typical in-game encounter.

This method is essentially the healing spreadsheets from Legion and BfA on steroids. Allowing much easier and faster 
manipulation and comparison of different profiles with a goal to provide clearer and more relevant results.

### What does it not do?
MPT is not a healing simulator. Damage patterns are not simulated, cohealers and raiders are not simulated. 
Values are calculated using primarily averages obtained from the input Profile. 

MPT also does not tell you how to play. It plays exactly how you tell it to so [garbage in, garbage out](https://en.wikipedia.org/wiki/Garbage_in,_garbage_out). If you are after guides check out the various guides listed [over on Mechanical Priest](https://mechanicalpriest.com/holy-priest-guide/).

MPT does not work for any game except the current Retail version of World of Warcraft. Sorry classic players!

### Development 
If you're interested in contributing, start by familiarising yourself with the [MPT Production](https://app.mechanicalpriest.com/) / [MPT Development](https://dev.mechanicalpriest.com/) applications.

There are details on running the client and api projects, and once the project is running locally, check out the [open issues](https://github.com/MechanicalPriest/Salvation/issues) or drop by [discord](https://discord.gg/6Fwq4UX).

[Pull Requests](https://github.com/MechanicalPriest/Salvation/pulls) should be made for any code or file modifications no matter how minor to go through the build, test and review phases before being published.

#### Solution Overview
##### Salvation.Api
The Azure Function-based serverless API written in C#. 

[Salvation.Api README](Application/Salvation.Api/README.md)

##### Salvation.Client
The web app client written in React.

[Salvation.Api README](Application/Salvation.Client/README.md)

### Salvation.Core
Core contains most of the heavy lifting with the profiles, modelling and configuration management all handled here.

##### Salvation.Explorer
Explorer is a basic console application used for testing and debugging components of Salvation.

It is suggested you make this the default project for easier debugging and development.

##### Salvation.Utility
Utility contains various helper classes to ease the development process, 
primarily focused around manipulating external data sources into something the application can easily consume.

### Useful Links

- [MPT Production](https://app.mechanicalpriest.com/) / [MPT Development](https://dev.mechanicalpriest.com/) - The theorycrafting application itself
- [Shadowlands WoW Bug Tracking](https://github.com/MechanicalPriest/Salvation/projects/1) - Tracking bugs with WoW.
- [Requests for in-game testing](https://github.com/MechanicalPriest/Salvation/issues?q=is%3Aissue+is%3Aopen+label%3A%22needs+in-game+testing%22) - All of these issues require testing in-game.