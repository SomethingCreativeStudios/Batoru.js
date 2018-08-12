using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Threading;

namespace Wix_Studio.Card_Parser
{
    public class CardMaker
    {
        public static String urlName = "http://selector-wixoss.wikia.com";
        String boosterPackUrl = "";
        System.Windows.Forms.WebBrowser browser = new System.Windows.Forms.WebBrowser();

        // Find parse all the sets url based on url
        // Returns: HashMap
        // Key: Set Name
        // Value: Set URL
        public Dictionary<String, String> GetAllSets(String url)
        {
            HtmlNode deckTable = null;
            Dictionary<String , String> setList = new Dictionary<string , string>();
            var html = new HtmlDocument();

            // Loads Website into an Html Document
            // You will not need this cause javascript/typescript
            html.LoadHtml(new WebClient().DownloadString(url));

            //  Find All elemnts that named: Table and have a width of 100%
            // <table width:'100%'></table>
            List<HtmlNode> cardTables = html.DocumentNode.Descendants().Where
            (x => ( x.Name == "table" && x.Attributes["width"] != null &&
               x.Attributes["width"].Value.Contains("100%") )).ToList();
            

            // Loop thru the rows of the first table found
            foreach ( HtmlNode row in cardTables[0].SelectNodes("tr" ) )
            {
                // Only look for HTML element that have td, ul or li
                // AKA have table data, is a unordered list, or just a list item
                HtmlNodeCollection cells = row.SelectNodes("td/ul/li");

                // If this row does not have any of those, please stop looking
                if ( cells == null )
                {
                    continue;
                }

                int index = 0;
                CardCollection cardCollection = new CardCollection();

                // Loop thru all of the found nodes
                foreach ( HtmlNode cell in cells )
                {
                    // This makes some assumptions
                    // The first child of the cell is a link that has an 'href'
                    // The first child of the cell has a 'title'
                    // That title is a space seperated list, where we only need the first 'item'

                    String urlToSet = urlName + "/" + cell.FirstChild.Attributes["href"].Value.Trim();
                    String setName = cell.FirstChild.Attributes["title"].Value.Split(' ')[0];
                    setList.Add(setName, urlToSet);
                }
            }

            return setList;
        }

        /// <summary>
        /// This method is awful, and I kinda hate myself for writing it
        /// But.... it works so...
        /// </summary>
        /// <returns> Hash Map
        ///  Key: Booster Set Name
        ///  Value: Booster Set URL
        /// </returns>
        public Dictionary<String , String> GetBoosterSets()
        {
            Dictionary<String , String> setList = new Dictionary<string , string>();
            browser.ScriptErrorsSuppressed = true;

            var html = new HtmlDocument();
            Boolean keepLooking = true;
            Boolean websiteDownloading = false;
            int currentSet = 1;
            string url = "http://selector-wixoss.wikia.com/wiki/WX-";
            boosterPackUrl = url + makeSetName(currentSet);

            // Start the awful while loop
            while ( keepLooking )
            {
                // This is worst part
                // Due to reasons that i have forgotten, we need to wait for the booster pack's url to download before moving on
                // This basicly is saying, is website Downloading? carry on with other things
                // otherwise do the while loop
                while ( websiteDownloading )
                    System.Windows.Forms.Application.DoEvents();

                // Alright on to even more awfulness

                // move the internal browser to the boost pack's url
                browser.Navigate(boosterPackUrl);
                
                // mark that we are downloading the page
                websiteDownloading = true;

                // Alright this is some weird syntax for c#,
                // But normal syntax for typescript haha
                // Bascily when the browser has navigated to the site, run the function

                // the => is called an 'arrow function'
                // This is saying while we making a new function, we want to bind this context with the same context that made it
                // Probably super confusing, dont worry it gets 'better'
                browser.DocumentCompleted += (s , e) =>
                {
                    //Stop loading the same set
                    if ( e.Url.ToString() != boosterPackUrl )
                        return;

                    String htmlStr = browser.DocumentText;
                    html.LoadHtml(htmlStr);

                    // So with this one, theres a a lot of assumtions we make

                    // First, we only care if any where in the html there is the word
                    // 'alternative-suggestion'
                    // Don't ask why, I forgot by now
                    if ( htmlStr.Contains("alternative-suggestion") )
                    {
                        //Dig down and find the a tag we need
                        // Look for any element that has an id of 'WikiaArticle'
                        // i.e < div id='WikiaArticle'> </div>
                        var wikiNode = html.GetElementbyId("WikiaArticle");

                        // Find All 'span' elements that have a class 'mw-headline'
                        // i.e. <span class='mw-headline'></span>

                        var spanClasses = wikiNode
                        .Descendants("span")
                        .Where(d =>
                           d.Attributes.Contains("class")
                           &&
                           d.Attributes["class"].Value.Contains("mw-headline")
                        );

                        // From those 'spans' find all descendants that have a class called 'alternative-suggestion' (side note, guess this is why?... see if statement comment)
                        var htmlClasses = spanClasses.ElementAt(0).Descendants("span")
                         .Where(d =>
                            d.Attributes.Contains("class")
                            &&
                            d.Attributes["class"].Value.Contains("alternative-suggestion")
                         );

                        // Of those find the fist 'a' tag
                        // i.e. <a></a>
                        var aLink = htmlClasses.ElementAt(0).Descendants("a").ElementAt(0);

                        //Sometimes we might hit a duplicate(shouldnt happen, but this is a saftey check)
                        // So with this, assuming we don't already have it. We get the 'title' and 'href' of the 'a' tag
                        if ( !setList.ContainsKey(aLink.Attributes["title"].Value) )
                            setList.Add(aLink.Attributes["title"].Value , "http://selector-wixoss.wikia.com" + aLink.Attributes["href"].Value);

                        //Load up next booster pack url
                        currentSet++;
                        boosterPackUrl = url + makeSetName(currentSet);
                    } else
                        keepLooking = false;

                    websiteDownloading = false;
                };
            }

            return setList;
        }

        // Helper function to add a 0 infront of number asusming its less than 10
        private String makeSetName(int currentSet)
        {
            string setName = "";

            if ( currentSet < 10 )
                setName = "0";

            return setName + currentSet.ToString();
        }

        public Dictionary<String,int> GetCardsFromUrl(String urlOfDeck)
        {

            Dictionary<String , int> listOfCards = new Dictionary<string , int>();
            HtmlNode deckTable = null;
            var html = new HtmlDocument();
            html.LoadHtml(new WebClient().DownloadString(urlOfDeck));
            var root = html.DocumentNode;

            // Find all Tables of the document
            List<HtmlNode> cardTables = html.DocumentNode.Descendants().Where
            (x => ( x.Name == "table")).ToList();

            // Loop thru all the tables and find the one true table
            // Which is the table that starts with 'Card Name'
            foreach ( HtmlNode row in cardTables )
            {
                if(HTMLHelper.TableHeaderContains(row,"Card Name") )
                {
                    deckTable = row;
                    break;
                }
            }


            // Assuming we found the table, process that table
            if ( deckTable != null )
            {
                // Get's the number of cards
                List<String> numberOfCards = HTMLHelper.getRowData(deckTable , "Qty");

                int index = 0;
                // tags that are 'Card Name'
                foreach ( var aTagText in HTMLHelper.getRowData(deckTable , "Card Name") )
                {
                    HtmlDocument justATag = new HtmlDocument();
                    justATag.LoadHtml(aTagText);
                    String cardUrl = "";

                    // First get card url by assuming this tag has a child 'a' tag and use its 'href'
                    // If not assume that the tag is bolded ('<b>') and dig one level deeper for the 'a' tag
                    try { cardUrl = justATag.DocumentNode.ChildNodes["a"].Attributes["href"].Value; }
                    catch ( Exception ex ) { cardUrl = justATag.DocumentNode.ChildNodes["b"].ChildNodes["a"].Attributes["href"].Value; }


                    // Sometimes the url does not start with the 'urlName'
                    // In this case, make it so
                    if ( !cardUrl.StartsWith(urlName) )
                        cardUrl = urlName + cardUrl;

                    int qty = 1;
                    // check if we have counts
                    // if so make the qty that
                    if ( numberOfCards.Count != 0 )
                    {
                        // The count would be 1x or 2x... ect
                        // We dont care about that 'x'
                        qty = Convert.ToInt16(numberOfCards[index].Replace("x" , ""));
                    }

                    // Assuming we don't already have that card, add it.
                    if ( !listOfCards.ContainsKey(cardUrl) )
                        listOfCards.Add(cardUrl , qty);

                    index++;
                }
            }

            return listOfCards;
        }

        // The main method
        // This is were all the magic happens
        public WixossCard GetCardFromUrl(String urlOfCard)
        {
            WixossCard wixossCard = new WixossCard();

            var html = new HtmlDocument();
            html.LoadHtml(new WebClient().DownloadString(urlOfCard));
            var root = html.DocumentNode;

            // Find all div tags that have an id of 'cftable'
            // i.e. <div id='cftable'></div>
            List<HtmlNode> cardTable = html.DocumentNode.Descendants().Where
            (x => ( x.Name == "div" && x.Attributes["id"] != null &&
               x.Attributes["id"].Value.Contains("cftable") )).ToList();

            // assuming we found a table, set up the get the card
            if ( cardTable.Count != 0 )
            {
                wixossCard.CardName = getCardName(cardTable[0]);//info-main
                setUpCard(cardTable[0] , wixossCard);
            }

            // Find the image
            // In this case, we are looking for an 'img' tag that has a width and height of 250 and that starts with 3 (30, 300, 3XX)
            List<HtmlNode> imageTable = html.DocumentNode.Descendants().Where
            (x => ( x.Name == "img" && x.Attributes["width"] != null &&
               x.Attributes["width"].Value.Contains("250") && x.Attributes["height"] != null &&
               (x.Attributes["height"].Value.StartsWith("3")) ) ).ToList();

            wixossCard.CardUrl = urlOfCard;

            // Assuming we found the image, set the cards image url to that's 'src'
            if ( imageTable.Count != 0 )
            {
                // Set image url to the first found 'img' tag via its 'src' attr
                // i.e. '<img src='BLAH BLAH.jpg'>
                wixossCard.ImageUrl = imageTable[0].Attributes["src"].Value;
            }

          

            return wixossCard;
        }

        // Get card name from an html Node
        private String getCardName(HtmlNode tableNode)
        {
            // Find the first 'div' that has an id of 'header'
            // i.e. <div id='header'>
            HtmlNode header = tableNode.Descendants().Where
                    (x => ( x.Name == "div" && x.Attributes["id"] != null && x.Attributes["id"].Value.Contains("header") )).ToList()[0];

            // This div's inner HTML is the card name text
            // i.e. <div> CARD NAME </div>
            String cardName = header.InnerHtml;

            // Check if card name has 'br' tags
            // If so, take the text before the tag
            if ( cardName.Contains("<br>") )
                cardName = cardName.Split(new string[] { "<br>" } , StringSplitOptions.None)[0];

            return HTMLHelper.removeHTML(cardName);
        }

        // The real main method.
        // Updates the passed in 'wixossCard' object
        private void setUpCard(HtmlNode tableNode , WixossCard wixossCard)
        {
            // Find a div that has a class of 'info-main'
            // <div class='info-main'></div>
            HtmlNode infoTable = tableNode.Descendants().Where
                    (x => ( x.Name == "div" && x.Attributes["class"] != null && x.Attributes["class"].Value.Contains("info-main") )).ToList()[0];

            // Find the first table in 'infoTable'
            infoTable = infoTable.Descendants().Where
                    (x => ( x.Name == "table" )).ToList()[0];

            // Convert each row in table into a hash map
            // Key: title
            // Value: HTML Node (the value)
            Dictionary<String , HtmlNode> tableMap = htmlTableToDictionary(infoTable);

            setInfoTable(tableMap, wixossCard);

            setEffect(tableNode , wixossCard);
            setCardSetInfo(tableNode , wixossCard);

        }

        private void setInfoTable(Dictionary<String , HtmlNode> tableMap, WixossCard wixossCard)
        {
            // loop thru all keys and set up the 'wixossCard' based on the keys
            foreach ( var key in tableMap.Keys )
            {
                switch ( key )
                {
                    case "LevelLimit":
                        try { wixossCard.LevelLimit = Convert.ToInt16(tableMap[key].FirstChild.InnerText.Trim()); }
                        catch (Exception ex){ wixossCard.LevelLimit = 0; }
                        break;
                    case "Card Type":
                        wixossCard.Type = (CardType)Enum.Parse(typeof(CardType) , tableMap[key].ChildNodes[1].InnerText.Trim());
                        break;
                    case "Level":
                        wixossCard.Level = Convert.ToInt16(tableMap[key].FirstChild.InnerText.Trim());
                        break;
                    case "Color":
                        HTMLHelper.handleCardColor(tableMap[key] , ref wixossCard);
                        break;
                    case "Grow Cost":
                        HTMLHelper.handleCardCost(tableMap[key] , ref wixossCard);
                        break;
                    case "Cost":
                        HTMLHelper.handleCardCost(tableMap[key] , ref wixossCard);
                        break;
                    case "Power":
                        wixossCard.Power = Convert.ToInt16(tableMap[key].FirstChild.InnerText.Trim());
                        break;
                    case "Class":
                        wixossCard.Class.Add(tableMap[key].ChildNodes[1].InnerText.Trim());
                        break;
                    case "Use Timing":
                        // Regular expression, aka magical string
                        // I have long since forgotten what this is suppose to match
                        var pattern = @"\[(.*?)\]";
                        var matches = Regex.Matches(tableMap[key].FirstChild.InnerText.Trim() , pattern);

                        // For each match, add it as the correct timing
                        foreach ( Match m in matches )
                        {
                            switch ( m.Groups[1].Value )
                            {
                                case "Attack Phase":
                                    wixossCard.Timing.Add(CardTiming.AttackPhase);
                                    break;
                                case "Main Phase":
                                    wixossCard.Timing.Add(CardTiming.MainPhase);
                                    break;
                                case "Spell Cut-In":
                                    wixossCard.Timing.Add(CardTiming.SpellCutIn);
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        private void setEffect(HtmlNode tableNode , WixossCard wixossCard)
        {
            // First the *second* table
            HtmlNode effectTable = tableNode.Descendants().Where
                   (x => ( x.Name == "table" )).ToList()[1];

            // Check if the table is the 'card abilities' table
            if ( HTMLHelper.TableHeaderContains(effectTable , "Card Abilities") )
                HTMLHelper.getCardEffect(effectTable , ref wixossCard);
        }

        private void setCardSetInfo(HtmlNode tableNode , WixossCard wixossCard)
        {
            HtmlNode setTable = null;

            // Find all table elements
            List<HtmlNode> tableNodes = tableNode.Descendants().Where
                   (x => ( x.Name == "table" )).ToList();

            // Look for the one true 'setTable'
            foreach ( var theTableNode in tableNodes )
            {
                // Check if the table contins 'Sets' in the header
                if ( HTMLHelper.TableHeaderContains(theTableNode , "Sets") )
                    setTable = theTableNode;
            }

            // make sure the 'setTable' is not null
            if ( setTable != null )
            {
                // loop thru all row data
                foreach ( var setInfo in HTMLHelper.getRowData(setTable , "Sets") )
                {
                    // make sure it's not null or blank... well i guess just blank
                    if ( setInfo != "" )
                    {
                        HtmlDocument htmlTableData = new HtmlDocument();
                        htmlTableData.LoadHtml(setInfo);

                        // Look for all 'a' tags that have a title and href contain 'WX' or 'SP'
                        // <a title='anything' href='asdasdasWXdasdadasd'></a>
                        var aTags = htmlTableData.DocumentNode.Descendants("a")
                         .Where(d =>
                            d.Attributes.Contains("title")
                            &&(
                            d.Attributes["href"].Value.Contains("WX") ||
                            d.Attributes["href"].Value.Contains("SP"))
                         );

                        // Loop thru all 'a' tags
                        foreach ( var aTag in aTags )
                        {
                            //A very cheap way to do this.
                            //Will have to update if they ever do in the three digits

                            // Set setName to the tags 'href' value
                            String setName = aTag.Attributes["href"].Value;

                            // The 'cheap' thing.  Sometimes we grab 'WXD' instead of 'WX'
                            // this throws all sub string logic out of wack
                            // to avoid this will increase the count by one based on 'WXD' or 'WX'
                            int maxCount = setName.Contains("WXD") ? 6 : 5;
                            setName = setName.Substring(setName.LastIndexOf("/") +  1, maxCount);

                            wixossCard.CardSets.Add(setName);
                        }
                    }
                }
            }
           
        }


        private Dictionary<String, HtmlNode> htmlTableToDictionary(HtmlNode tableNode)
        {
            Dictionary<String , HtmlNode> tableValues = new Dictionary<string , HtmlNode>();

            // Find all 'tr' tags
            // i.e <tr></tr>
            List<HtmlNode> tableRows = tableNode.Descendants().Where
                    (x => ( x.Name == "tr" )).ToList();

            // Loop thru all of them
            foreach ( var tableRow in tableRows )
            {
                String title = "";
                HtmlNode value = null;

                // Set the title to the second child's inner text
                // Difference between inner text and inner html
                // Inner text will grab only text values, no tags
                // Inner HTML returns a string of everything
                // <div> <b> TEXT </b> </dv>
                // Inner Text --> 'TEXT'
                // Inner HTML --> '<b> TEXT </b>'
                title = tableRow.ChildNodes[1].InnerText.Trim();

                // Set the value to the third child
                value = tableRow.ChildNodes[2];

                // Add it two the hash map
                tableValues.Add(title , value);
            }

            return tableValues;
        }
    }
}
