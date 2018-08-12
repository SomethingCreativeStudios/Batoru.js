using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Wix_Studio.Card_Parser
{
    public class HTMLHelper
    {
        public static void getCardEffect(HtmlNode effect, ref WixossCard wixossCard)
        {
            String cardEffect = "";

            // Find the first 'td'
            effect = effect.Descendants().Where
            (x => ( x.Name == "td")).ToList()[0];

            // Loop thru all decendants as well as itself
            foreach ( var node in effect.DescendantsAndSelf() )
            {
                // check if it has child
                // if not do nothing
                if ( !node.HasChildNodes )
                {
                    // Convert br to new line
                    if(node.Name == "br" )
                    {
                        cardEffect += "\n";
                    }

                    // Check if the node is an image that has class with nothing
                    // <img class=""></img>
                    if(node.Name == "img" && node.Attributes["class"].Value == "")
                    {
                        // The 'alt' attr has what the image is suppose to be. In this case only lifeburst
                        cardEffect += "{" + node.Attributes["alt"].Value + "}";
                        switch ( node.Attributes["alt"].Value )
                        {
                            case "Lifeburst":
                               // set the lifeburst value to ture
                                wixossCard.LifeBurst = true;
                                break;
                            default:
                                break;
                        }
                    }
                    
                    // Check if the effect contains 'Guard'
                    if(node.InnerText == "Guard" )
                    {
                        wixossCard.Guard = true;
                    }

                    // Check if the effect has 'Multi Ener'
                    if ( node.InnerText == "Multi Ener" )
                    {
                        wixossCard.MultiEner = true;
                    }


                    string text = node.InnerText;

                    // Make sure the text is not null or empty
                    // Then remove an html value or convert them to real values
                    if ( !string.IsNullOrEmpty(text) )
                        cardEffect += removeHTML(text);
                }
            }


            //Check timing
            if ( cardEffect.Contains("Use Timing") )
            {
                cardEffect = cardEffect.Replace("Use Timing" , "");

                // Theres that magical string again... still don't know what it matches
                var pattern = @"\[(.*?)\]";
                var matches = Regex.Matches(cardEffect , pattern);

                // Find every match and set timming on the card accordingly
                foreach ( Match m in matches )
                {
                    switch ( m.Groups[1].Value )
                    {
                        case "Attack Phase":
                            wixossCard.Timing.Add(CardTiming.AttackPhase);
                            cardEffect = cardEffect.Replace("[Attack Phase]" , "");
                            break;
                        case "Main Phase":
                            wixossCard.Timing.Add(CardTiming.MainPhase);
                            cardEffect = cardEffect.Replace("[Main Phase]" , "");
                            break;
                        case "Spell Cut-In":
                            wixossCard.Timing.Add(CardTiming.SpellCutIn);
                            cardEffect = cardEffect.Replace("[Spell Cut-In]" , "");
                            break;
                        default:
                            break;
                    }
                }
            }

            // Remove any HTML and remove whitespace on both sides
            wixossCard.CardEffect = removeHTML(cardEffect).Trim();
        }

        public static void handleCardCost(HtmlNode costRow , ref WixossCard wixossCard)
        {
            // Find all 'a' tags
            List<HtmlNode> imgNodes = costRow.Descendants().Where
             (x => ( x.Name == "a" )).ToList();

            List<CardCost> costForCard = new List<CardCost>();
            List<long> cardCostList = new List<long>();

            // Loop thru all card cost values
            // We first removeHTML then split it based on 'x'
            // i.e 1Gx1W becomes ['1G','1W']
            foreach ( var item in removeHTML(costRow.InnerText).Split(new String[] { " x " } , StringSplitOptions.RemoveEmptyEntries) )
            {
                // Make sure its not just whitespace
                if(item.Trim() != "" )
                {
                    String cleanedItem = item;

                    // sometimes its an 'or' not just 'x'
                    if ( item.Contains("or") )
                    {
                        cleanedItem = item.Split(' ')[0];
                    }

                    cardCostList.Add(Convert.ToInt16(cleanedItem.Trim()));
                }
            }

            // Loop thru all img nodes, because those contain what color to use
            for ( int i = 0; i < imgNodes.Count; i++ )
            {
                CardCost cost = new CardCost();

                if ( i < cardCostList.Count )
                    cost.NumberPerColor = (int)cardCostList[i];
                else
                    cost.NumberPerColor = 0;

                cost.Color = colorFromName(imgNodes[i].Attributes["title"].Value);
                costForCard.Add(cost);
            }
            wixossCard.Cost = costForCard;
        }


        public static void handleCardColor(HtmlNode colorRow , ref WixossCard wixossCard)
        {
            // Find all 'img' tags
            List<HtmlNode> imgNodes = colorRow.Descendants().Where
             (x => ( x.Name == "img" )).ToList();

            List<CardColor> colorsForCard = new List<CardColor>();

            // Loop thru all imgNodes
            foreach ( var imgNode in imgNodes )
            {
                // Add the color based on the imgNode's 'alt' attr
                colorsForCard.Add(colorFromName(imgNode.Attributes["alt"].Value));
            }

            wixossCard.Color = colorsForCard;
        }

        // Convert the name to an ENUM
        // I really should of made this case insestive
        // I.e colorName = colorName.toLower();
        private static CardColor colorFromName(String colorName)
        {
            CardColor cardColor = CardColor.Colorless;

            switch ( colorName )
            {
                case "Green":
                    cardColor = CardColor.Green;
                    break;
                case "Black":
                    cardColor = CardColor.Black;
                    break;
                case "Blue":
                    cardColor = CardColor.Blue;
                    break;
                case "White":
                    cardColor = CardColor.White;
                    break;
                case "Red":
                    cardColor = CardColor.Red;
                    break;
                case "Colorless":
                    cardColor = CardColor.Colorless;
                    break;
            }

            return cardColor;
        }

        // Converts all html string to what they would display as
        public static String removeHTML(String htmlText)
        {
            htmlText = htmlText.Replace("&lt;", "<");
            htmlText = htmlText.Replace("&gt;" , ">");
            htmlText = htmlText.Replace("Ã—" , "x");
            htmlText = htmlText.Replace("&#61;" , "=");
            htmlText = htmlText.Replace("â‘¡" , "(2)");
            htmlText = htmlText.Replace("â‘" , "(1)");
            htmlText = htmlText.Replace("â™¥" , "♥");
            


            return htmlText;
        }


        // Check if table's header contains 'headerText'
        public static Boolean TableHeaderContains(HtmlNode tableNode , String headerText)
        {
            return indexOfRow(tableNode , headerText) != -1;
        }

        // Find the index of the row with 'headerText' in a table
        public static int indexOfRow(HtmlNode tableNode, String headerText)
        {
            int rowIndex = -1;

            // Loop thru all 'tr', table rows
            foreach ( HtmlNode row in tableNode.SelectNodes("tr") )
            {
                // Find the 'th', table header
                HtmlNodeCollection cells = row.SelectNodes("th");

                if ( cells == null )
                {
                    continue;
                }

                int index = 0;

                // Loop thru all of the headers
                foreach ( HtmlNode cell in cells )
                {
                    // Check if one of them contains 'headerText'
                    if ( cell.InnerText.Contains(headerText) )
                    {
                        // found on, set the row index to 'index'
                        rowIndex = index;
                        break;
                    }
                    index++;
                }

                if ( rowIndex != -1 )
                    break;
            }

            return rowIndex;
        }

        // Get all row under the header
        // header|header2|header3
        // val   |val2   |val3
        // val   |val22  |val33
        // getRowData(table, 'header2') -> ['val2', 'val22']
        public static List<String> getRowData(HtmlNode tableNode , String headerText)
        {
            List<String> rowDataList = new List<string>();

            // Get the row's index
            int rowIndex = indexOfRow(tableNode , headerText);
            
            // Make sure the row even exists
            if ( rowIndex != -1 )
            {
                // Loop thru all the table row
                foreach ( HtmlNode row in tableNode.SelectNodes("tr") )
                {
                    // Look only for the 'td', table data? table description?
                    HtmlNodeCollection cells = row.SelectNodes("td");

                    if ( cells == null )
                    {
                        continue;
                    }

                    int index = 0;
                    
                    // Loop thru every found 'td'
                    foreach ( HtmlNode cell in cells )
                    {
                        // We only care about the 'rowIndex' cell
                        if ( index == rowIndex )
                        {
                            // Found it!!!! 
                            // Add it to the array
                            rowDataList.Add(cell.InnerHtml.Trim());
                            break;
                        }
                        index++;
                    }
                }
            }
            return rowDataList;
        }
    }
}
