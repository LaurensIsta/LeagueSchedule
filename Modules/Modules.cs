using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Net;
using Discord.Audio;
using Newtonsoft.Json;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace LeagueStandings.Modules
{
    public class Modules : ModuleBase<SocketCommandContext>
    {
        [Command("test")]
        public async Task test()
        {
            await Context.Channel.SendMessageAsync("test");
        }

        //TODO: Add all commands to a list in the description.
        [Command("help")]
        public async Task help()
        {
            var embed = new EmbedBuilder();
            embed.WithThumbnailUrl("http://liquipedia.net/commons/images/thumb/c/cd/NA_LCS_late_2017.png/600px-NA_LCS_late_2017.png");
            embed.WithTitle("Help" );
            embed.WithDescription("<> week \n<> update \n \nThis bot has been developped by Laurens Ista (https://github.com/LaurensIsta).\n If you have any questions about this bot or if you have any contributions, please email me at: 0923444 @ hr.nl \n");
            embed.WithColor(Discord.Color.Blue);
            await Context.Channel.SendMessageAsync("", false, embed);


        }

        // Get all matches by week
        [Command("week")]
        public async Task getList(string weekInput)
        {
            Stopwatch watch = Stopwatch.StartNew();
            int looper = 0;
            string[] matchlist = new string[10];
            string json = "";
            string descString = "";
            Context.Channel.SendMessageAsync("sending request to the Local Storage");

            using (WebClient client = new WebClient())
            {

                json = client.DownloadString(@"ENTER LOCAL STORAGE HERE");
                Console.WriteLine("Loading Done in " + watch.Elapsed.TotalSeconds + "seconds");
            }

            var data = JsonConvert.DeserializeObject<dynamic>(json);
            json = null;
            var list = data["highlanderTournaments"][5]["brackets"]["25cf16fe-5fac-492e-aa18-887b5461e70f"]["matches"];

            foreach (var element in list)
            {
                var content = element.First;

                var resolvestate = content["state"];

                    string matchId = content["id"];
                    string jsonV2 = "";
                    using (WebClient client = new WebClient())
                    {
                        Console.WriteLine("Loading JSON data from V2 API");
                        jsonV2 = client.DownloadString(@"ENTER LOCAL STORAGE HERE" + matchId.ToString()+".json");
                        Console.WriteLine("Loading V2 Done in " + watch.Elapsed.TotalSeconds + "seconds");
                    }
                    var WeekJson = JsonConvert.DeserializeObject<dynamic>(jsonV2);
                    var week = WeekJson["match"]["week"].ToString();
                    //TODO: fix WeekJson

                    
                    if (week == weekInput)
                    {
                        while (looper <= matchlist.Length)
                        {
                            matchlist[looper] = content["name"];
                            looper = looper + 1;
                            break;
                        }
                    }               
            }

            foreach (var element in matchlist)
            {
                if (element != null)
                {
                    descString = descString + "\n" + element;
                }
            }
            watch.Stop();
            var elapsedTime = watch.Elapsed.TotalSeconds;
            var embed = new EmbedBuilder();
            embed.WithThumbnailUrl("http://liquipedia.net/commons/images/thumb/c/cd/NA_LCS_late_2017.png/600px-NA_LCS_late_2017.png");
            embed.WithTitle("Matches of week " + weekInput);
            embed.WithDescription(descString);
            embed.WithFooter("Request completed in " + elapsedTime + " seconds.");
            embed.WithColor(Discord.Color.Blue);
            await Context.Channel.SendMessageAsync("", false, embed);
        }

        //Updates local data about the entire NALCS.
        //TODO: Add a way to only download essential data (should make downloading/searching faster)
        [Command("update")]
        public async Task updateJson()
        {
            //Start Season Update
            var downloadedJson = "";

            using (WebClient client = new WebClient())
            {
                Context.Channel.SendMessageAsync("Sending request to the Riot server...");
                downloadedJson = client.DownloadString("https://api.lolesports.com/api/v1/scheduleItems?leagueId=2");
            }

            JObject localJson = JObject.Parse(File.ReadAllText(@"ENTER LOCAL STORAGE HERE"));
            var updateJson = JsonConvert.DeserializeObject<dynamic>(downloadedJson).ToString();
            var sourceJson = localJson.ToString();

            if (updateJson != sourceJson)
            {
                Console.WriteLine("updating...");
                sourceJson = updateJson;
            }

            File.WriteAllText(@"/*ENTER LOCAL STORAGE HERE*/", sourceJson);
            //End League Update

            
            await Context.Channel.SendMessageAsync("done");


        }

        [Command("performance testing")]
        public async Task PerformanceTest()
        {
            //TODO: add performance test.

                await Context.Channel.SendMessageAsync("done testing");

        }

        // Updates all teams. If there is no team data, it will create the data in the NaTeams folder
        [Command ("update teams")]
        public async Task updateTeams()
        {
            var json = "";

            using (WebClient client = new WebClient())
            {

                json = client.DownloadString(@"ENTER LOCAL STORAGE HERE");

            }

            var data = JsonConvert.DeserializeObject<dynamic>(json);
            json = null;

            var teams = data["highlanderTournaments"][5]["rosters"];
            var resultlist = data["highlanderRecords"];
            foreach (var element in teams)
            {
                var content = element.First;
                var teamId = content["id"];
                var teamName = content["name"];
                var teamWins = "";
                var teamLosses = "";
                foreach (var e in resultlist)
                {

                    if (teamId.ToString() == e["roster"].ToString())
                    {
                        teamWins = e["wins"];
                        teamLosses = e["losses"];

                    }
                }

                JObject newTeam =
                                 new JObject(
                                    new JProperty("Team",
                                        new JObject(
                                            new JProperty("teamId", teamId.ToString()),
                                            new JProperty("teamName", teamName.ToString()),
                                            new JProperty("teamWins", teamWins.ToString()),
                                            new JProperty("teamLosses", teamLosses.ToString())
                                            )));

                var newTeamAdd = newTeam.ToString();
                Console.WriteLine(teamName.ToString() + " updated");
                File.WriteAllText(@"ENTER LOCAL STORAGE HERE" + teamName.ToString() + ".json", newTeamAdd);
            }

            await Context.Channel.SendMessageAsync("All Teams updated");



        }
            
        
    }
}
