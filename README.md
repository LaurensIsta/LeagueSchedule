# LeagueSchedule
League Schedule is a Discord bot written in C# (last updated in March 2018)
This bot is able to show NALCS matches of a specific week that's given by the user. \
The bot's prefix is <>. 

## Purpose of this bot
This bot has been made because of the poor performance and UX of the lolesports.com website. \
This bot enables a user to get data from an entire week of matches in a top speed of **0.34 seconds** whereas the lolesports.com
website takes about 14 seconds to load this information.
This is about **41 times faster** than visiting the official website.

### Future of this bot
This bot will probably not get any updates until I port the core logic over to JavaScript to create a React-based webapp.
## Commands 
<> help \
Shows basic info about the bot. 

<> test \
If the bot responds, the bot is online. 

<> week [WEEKNUMBER] \
Shows the matches for a given WEEKNUMBER between 1-9. 

<> update \
Updates the local storage with all available data on the NALCS. 

<> update teams \
Updates the local storage with data that is available on the teams that are currently in playing in the NALCS. 

## TODO
* Clean up code.
* Add clearer comments.
* Enable the bot to show current standings.

