# Contribution Guidelines

There are 3 sections to this document to help provide some guidance on how to properly contribute to each different area:

- Contributing through WoW In-Game testing
- Contributing through MPT app testing
- Contributing through submitting code fixes/changes

## Contributing through WoW In-Game testing

When providing reports of in-game issues try to provide as much relevant information as possible. Ideally you can log the issue to 
WarcraftLogs and provide that log, otherwise screenshots and/or video can often be enough to showcase a problem. 

### Logging to WarcraftLogs.com

Logging to warcraftlogs.com is the perferred method for submitting feedback about issues or interactions. To do this you need to 
first enable combatlogging in-game using `/combatlog`, then once you have reproduced the issue, upload that log to WarcraftLogs.com 
then submit as part of your feedback a link to the log.

More information on how to log to WarcraftLogs can be found [on their help page](https://www.warcraftlogs.com/help/start).

### In-game combat log and video

Any in-game feedback should include a screenshot of your character profile and/or a dump of the `/simc` or `/simc nobags` results from 
[the simcraft addon](https://www.curseforge.com/wow/addons/simulationcraft) (The most recent alpha/beta version of it for testing out 
Beta/PTR can be [found here](https://www.curseforge.com/wow/addons/simulationcraft/files/all)).

Another useful addon for checking spell, buff and item IDs is [idTip](https://www.curseforge.com/wow/addons/idtip).

The **Combat Log** tab of chat in-game can be configured to enable a lot of extra information including timestamps that is often relevant 
in providing good feedback. 

For video recording feedback, Youtube or twitch clips/videos are the preferred. Animated gifs are fine if the length is quite short 
and the gif quality is high enough to properly see the relevant information.

## Contributing through MPT app testing

Coming soon...

## Contributing through submitting code fixes/changes

The main things to keep in mind when submitting **bugs or issues**:

- Search first to ensure the issue hasn't already been reported
  - If it has been, add any relevant new information after reading through the existing bug
- Be clear and concise:
  - Use a shor but descriptive title
  - Provide easy to follow reproduction steps 
  - Describe the behaviour you expected to see and the behaviour you actually saw

The main things to keep in mind when submitting **bug fixes**:

- Follow the related verified [Issue](https://github.com/MechanicalPriest/Salvation/issues) (If there isn't one, create one)
  - Ensure it's a confirmed / valid issue to save time wasted working on non-issues
  - Ensure if someone else is also working on the issue you are working toegether to save time
- Include as much detail as possible, including the things mentioned above in the *Contributing through WoW In-Game testing* and *Contributing through MPT app testing* sections
- Fixing code formatting is generally not considered worthwhile unless there are accompanying issues that are directly related and resolved alongside it

When submitting **new features / feature enhancements**:

- Create an issue first proposing the feature/enhancement and wait until it's verified as something that has potential to be added before starting work on it
- Consider [joining discord](https://discord.gg/6Fwq4UX) to discuss potential features there
- Create a fork of your own to work from and submit a pull request for review once you consider the feature/enhancement to be implemented
  - Your PR should include any current changes to the `dev` branch upon submission
  - Your PR should pass all tests
