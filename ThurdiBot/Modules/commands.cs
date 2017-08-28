using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using ThurdiBot;
using Discord;
using System.Net;
using Newtonsoft.Json.Linq;
using Google.Apis.Calendar.v3;

namespace ThurdiBot.Modules {
    public class commands : ModuleBase<SocketCommandContext> {
        private List<squidItem> items = new List<squidItem>();

        [Command("Help")]
        public async Task help() {
            string commandlist = "1. !Help : Displays this message.\n" +
                                 "4. !Search \"<Item Name>\" : Displays all items from the Ika House Database that match the item name.\n      ex: !Search \"18K Aviators\"\n" +
                                 "5. !Searchbrand \"<Brand>\" \"<Slot>\" : Displays all items from the Ika House Database that match the item brand and slot.\n      ex: !Searchbrand \"Forge\" \"Head\"\n      ex: !Searchbrand \"Forge\" \"Chest\"\n      ex: !Searchbrand \"Forge\" \"Shoes\"\n" +
                                 "6. !getItemNames : Displays all item names available for search.\n" +
                                 "7. !getBrands : Displays all brands available for search.\n" +
                                 "\n";
            await Context.Channel.SendMessageAsync(commandlist);
        }

        [Command("Search")]
        public async Task search(string name) {
            string response = "";
            updateDatabase();
            if (items.Count == 0) {
                await Context.Channel.SendMessageAsync("Please run !Update Items to generate the item database.");
            }
            else {
                List<squidItem> results = new List<squidItem>();
                foreach (squidItem item in items) {
                    if (item.getname().ToUpper().Contains(name.ToUpper())) {
                        results.Add(item);
                    }
                }
                if (results.Count > 0) {
                    response = "The items that match your search are:\n";
                    int counter = 0;
                    foreach (squidItem item in results) {

                        response += "\nSlot: " + item.getslot();
                        response += "\nName: " + item.getname();
                        response += "\nBrand: " + item.getbrand();
                        response += "\nMain: " + item.getmain();
                        response += "\nSub1: " + item.getsub1();
                        response += "\nSub2: " + item.getsub2();
                        response += "\nSub3: " + item.getsub3();
                        response += "\nCommon: " + item.getcommon();
                        response += "\nPurity: " + item.getpurity();
                        response += "\nDiscord Users: " + item.getdiscordUsers();
                        response += "\n";
                        counter++;
                        if (counter > 160) {
                            response += "List truncated (Over 160 results)\n";
                            break;
                        }
                    }
                }
                else {
                    response = "There are no items that match your search.  Try using !getItemNames to get a list of valid item names.";
                }
                //await Context.Channel.SendMessageAsync(response);
                await ((SocketGuildUser)Context.User).SendMessageAsync(response);
            }
        }
        [Command("SearchBrand")]
        public async Task searchbrand(string brand, string slot) {
            string response = "";
            if (slot.Contains("Chest") || slot.Contains("Shoes") || slot.Contains("Head")) {
                updateDatabase();
                if (items.Count == 0) {
                    await Context.Channel.SendMessageAsync("Please run !Update Items to generate the item database.");
                }
                else {
                    List<squidItem> results = new List<squidItem>();
                    foreach (squidItem item in items) {
                        if (item.getbrand().ToString().ToUpper().Contains(brand.ToUpper()) && item.getslot().ToString().ToUpper().Contains(slot.ToUpper())) {
                            results.Add(item);
                        }
                    }
                    if (results.Count > 0) {
                        response = "The items that match your search are:\n";
                        int counter = 0;
                        foreach (squidItem item in results) {
                            response += "\nSlot: " + item.getslot();
                            response += "\nName: " + item.getname();
                            response += "\nBrand: " + item.getbrand();
                            response += "\nMain: " + item.getmain();
                            response += "\nSub1: " + item.getsub1();
                            response += "\nSub2: " + item.getsub2();
                            response += "\nSub3: " + item.getsub3();
                            response += "\nCommon: " + item.getcommon();
                            response += "\nPurity: " + item.getpurity();
                            response += "\nDiscord Users: " + item.getdiscordUsers();
                            response += "\n";

                            counter += 1;
                            if (counter > 160) {
                                response += "List truncated (Over 160 results)\n";
                                break;
                            }
                        }
                    }
                    else {
                        response = "There are no items that match your search.  Try using !getBrands to get a list of valid brands.";
                    }
                    await ((SocketGuildUser)Context.User).SendMessageAsync(response);
                    //await Context.Channel.SendMessageAsync(response);
                }
            }
            else {
                response += "Invalid slot option.  Valid options are Head, Shoes or Chest.";
                await Context.Channel.SendMessageAsync(response);
            }
        }
        [Command("getItemNames")]
        public async Task getItemNames() {
            updateDatabase();
            string response = "\nNames available for search:\n";
            List<string> availableNames = new List<string>();
            foreach (squidItem item in items) {
                if (!availableNames.Contains(item.getname())) {
                    availableNames.Add(item.getname());
                }
            }
            availableNames.Sort();
            foreach (string s in availableNames) {
                response += s + "\n";
            }
            await ((SocketGuildUser)Context.User).SendMessageAsync(response);
        }
        [Command("getBrands")]
        public async Task getBrands() {
            updateDatabase();
            string response = "\nBrands available for search:\n";
            List<string> availableBrands = new List<string>();
            foreach (squidItem item in items) {
                if (!availableBrands.Contains(item.getbrand())) {
                    availableBrands.Add(item.getbrand());
                }
            }
            availableBrands.Sort();
            foreach (string s in availableBrands) {
                response += s + "\n";
            }
            await ((SocketGuildUser)Context.User).SendMessageAsync(response);
        }

        public void updateDatabase() {
            // If modifying these scopes, delete your previously saved credentials
            // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
            string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
            string ApplicationName = "";

            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read)) {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer() {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            //1dw919QVBC0gUZtDTFYgq07aGANjSLjg_vvhLKgK1r0s
            String spreadsheetId = "14YbovvzHkON27eaS7sfa91Q9xqctz00i5N6S_eAOALQ";
            String range = "Form Responses 2!A2:M";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();

            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0) {
                foreach (var row in values) {
                    try {
                        items.Add(new squidItem(row[0],
                                                row[1].ToString(),
                                                row[2].ToString(),
                                                row[3].ToString(),
                                                row[4].ToString(),
                                                row[5].ToString(),
                                                row[6],
                                                row[7],
                                                row[8],
                                                row[9],
                                                row[10].ToString(),
                                                row[11].ToString().Split(','),
                                                row[12].ToString()
                                                ));
                    }
                    catch { }
                }
            }
            else {
                Console.WriteLine("No data found.");
            }
        }
    }
}
