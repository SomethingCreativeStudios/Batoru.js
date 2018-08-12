using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using Wix_Studio.Card_Parser;
using System.IO;
using System.Net;
using System.Diagnostics;
using Wix_Studio.WixCardFiles;

namespace Wix_Studio.Card_GUI
{
    /// <summary>
    /// Interaction logic for CardUpdater.xaml
    /// </summary>
    public partial class CardUpdater : Window
    {
        // Ignore this class
        CardCollection cardCollection;

        CardMaker cardMaker = new CardMaker();

        // Hashmap of card set
        // Key: Set Name
        // Value: Set URL
        Dictionary<String , String> cardSets = new Dictionary<string , string>();

        public CardUpdater()
        {
            InitializeComponent();
        }

        public CardUpdater(Boolean cleanUpdate , CardCollection cardCollection)
        {
            InitializeComponent();

            // URL to deck list
            String updateDeckUrl = "http://selector-wixoss.wikia.com/wiki/Category:Pre-built_Decks?display=page";

            // URL to Set List
            String updateSetUrl = "http://selector-wixoss.wikia.com/wiki/Category:Booster_Sets?display=page";

            this.cardCollection = cardCollection;

            setsProgressBar.Value = 0;
            setsProgressBar.Minimum = 0;

            String cardList = "";

            int count = 0;

            // Loop thru all card sets and set the card set hashmap
            foreach ( var cardSet in cardMaker.GetAllSets(updateDeckUrl) )
            {
                cardSets.Add(cardSet.Key , cardSet.Value);
                count++;
            }

            //For Speed and testing
            /*
            cardSets.Add("WX-01 Served Selector" , "http://selector-wixoss.wikia.com/wiki/WX-01_Served_Selector");
            cardSets.Add("WX-02 Stirred Selector" , "http://selector-wixoss.wikia.com/wiki/WX-02_Stirred_Selector");
            cardSets.Add("WX-03 Spread Selector" , "http://selector-wixoss.wikia.com/wiki/WX-03_Spread_Selector");
            cardSets.Add("WX-04 Infected Selector" , "http://selector-wixoss.wikia.com/wiki/WX-04_Infected_Selector");
            cardSets.Add("WX-05 Beginning Selector" , "http://selector-wixoss.wikia.com/wiki/WX-05_Beginning_Selector");
            cardSets.Add("WX-06 Fortune Selector" , "http://selector-wixoss.wikia.com/wiki/WX-06_Fortune_Selector");
            cardSets.Add("WX-07 Next Selector" , "http://selector-wixoss.wikia.com/wiki/WX-07_Next_Selector");
            cardSets.Add("WX-08 Incubate Selector" , "http://selector-wixoss.wikia.com/wiki/WX-08_Incubate_Selector");
            cardSets.Add("WX-09 Reacted Selector" , "http://selector-wixoss.wikia.com/wiki/WX-09_Reacted_Selector");
            cardSets.Add("WX-10 Chained Selector" , "http://selector-wixoss.wikia.com/wiki/WX-10_Chained_Selector");
            cardSets.Add("WX-11 Destructed Selector" , "http://selector-wixoss.wikia.com/wiki/WX-11_Destructed_Selector");
            cardSets.Add("WX-12 Replied Selector" , "http://selector-wixoss.wikia.com/wiki/WX-12_Replied_Selector");
            cardSets.Add("WX-13 Unfeigned Selector" , "http://selector-wixoss.wikia.com/wiki/WX-13_Unfeigned_Selector");
            cardSets.Add("WX-14 Succeed Selector" , "http://selector-wixoss.wikia.com/wiki/WX-14_Succeed_Selector");
            cardSets.Add("WX-15 Incited Selector" , "http://selector-wixoss.wikia.com/wiki/WX-15_Incited_Selector");
            cardSets.Add("WX-16 Decided Selector" , "http://selector-wixoss.wikia.com/wiki/WX-16_Decided_Selector");
            cardSets.Add("WX-17 Exposed Selector" , "http://selector-wixoss.wikia.com/wiki/WX-17_Exposed_Selector");
            cardSets.Add("WX-18 Conflated Selector" , "http://selector-wixoss.wikia.com/wiki/WX-18_Conflated_Selector");
            cardSets.Add("WX-19 Unsolved Selector" , "http://selector-wixoss.wikia.com/wiki/WX-19_Unsolved_Selector");
             */


             cardsProgressBar.Value = 0;
             cardsProgressBar.Minimum = 0;
             setsProgressBar.Maximum = cardSets.Values.Count;

             // Starts a new thread where we download all sets in 'cardSets'
             // Don't worry about what it is for the porting
             BackgroundWorker worker = new BackgroundWorker();
             worker.WorkerReportsProgress = true;
             worker.DoWork += updateCardsWork;
             worker.ProgressChanged += Worker_ProgressChanged;

             // Start the thread
             // AKA look at function updateCardsWork()
             worker.RunWorkerAsync();
        }

        // Method to get all images from all sets
        // LOOK at this method as well for porting
        public void UpdateImages()
        {
            CardCollection cardCollection = new CardCollection();

            setsProgressBar.Value = 0;
            setsProgressBar.Minimum = 0;

            cardsProgressBar.Value = 0;
            cardsProgressBar.Minimum = 0;
            setsProgressBar.Maximum = cardCollection.GetAllSets().Count;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += updateImagesWork;
            worker.ProgressChanged += Worker_ProgressChanged;

            // Start the thread
             // AKA look at function updateImagesWork()
            worker.RunWorkerAsync();

        }

        // This is a method to handle the progress bar
        // This one method handles three different progress bar states
          // Card Set Value --> The current set's current Card out of total card
          // Card Set Count --> The current set
          //Card Value --> The current card
        private void Worker_ProgressChanged(object sender , ProgressChangedEventArgs e)
        {
            if ( (string)e.UserState == "Card Set Value" )
            {
                setsProgressBar.Value = e.ProgressPercentage;
                setBlock.Text = "Set " + setsProgressBar.Value + " / " + setsProgressBar.Maximum;
                if ( e.ProgressPercentage > setsProgressBar.Maximum )
                {
                    System.Windows.Forms.MessageBox.Show("Update Done");
                    this.Close();
                }
            }

            if ( (string)e.UserState == "Card Set Count" )
            {
                cardsProgressBar.Maximum = e.ProgressPercentage;
            }

            if ( (string)e.UserState == "Card Value" )
            {
                cardsProgressBar.Value = e.ProgressPercentage;
                cardBlock.Text = "Card " + cardsProgressBar.Value + " / " + cardsProgressBar.Maximum;
            }

            if ( e.ProgressPercentage == -1 )
                setBlock.Text = (string)e.UserState;

        }

        private void updateImagesWork(object sender , DoWorkEventArgs e)
        {
            CardCollection cardCollection = new CardCollection();
         
            // A way to log which images where not found correctly
            // You dont need to port this but it could be helpfull
            String fileName = AuditLog.logPath + "Images Updated.txt";
            AuditLog.clear("Images Updated.txt");
         
            BackgroundWorker worker = (BackgroundWorker)sender;
            int setCount = 0;

            setCount++;
            int cardCount = 1;

            // So this method only gets images of cards we have
            // You can do it this way or not
            // This does 'decouple' cards.json and card set images, which i think makes sense
            List<WixossCard> cardList = WixCardService.getAllCards();

            // call Worker_ProgressChanged with the second parameter being e.UserState
            worker.ReportProgress(cardList.Count , "Card Set Count");
            worker.ReportProgress(setCount , "Card Set Value");

            // Loop thru all cards we have and check if we have the image
            // Propbably should of been a for loop
            foreach ( var wixCard in cardList )
            {
                cardCount++;
                // Check if we dont have the card's image
                if ( !File.Exists(wixCard.CardImagePath) )
                {

                    // Start an 'internal' browser
                    using ( WebClient client = new WebClient() )
                    {
                        // Build the file path based on set and 'card id'
                        String newFilePath = CardCollection.setImages + "\\" + wixCard.Id + ".jpg";

                        // make sure the card has an image url
                        // this is probably bad if it doesnt...
                        if ( wixCard.ImageUrl != null )
                        {
                            String urlName = wixCard.ImageUrl;

                            // Download file async (non-thread blocking aka download many images as once)
                            client.DownloadFileAsync(new Uri(urlName) , newFilePath , wixCard);

                            // on complete run 'Client_DownloadFileCompleted'
                            client.DownloadFileCompleted += Client_DownloadFileCompleted;
                        }
                    }

                }
                worker.ReportProgress(cardCount , "Card Value");
            }

            Process.Start("notepad.exe" , fileName);
        }

        private void Client_DownloadFileCompleted(object sender , AsyncCompletedEventArgs e)
        {
            WixossCard wixCard = e.UserState as WixossCard;
            String newLine = Environment.NewLine;
            String hadError = e.Error == null ? newLine + "Passed " : newLine + e.Error.InnerException.Message + newLine;
            AuditLog.log( hadError + "Card Name: " + wixCard.CardName + newLine , "Images Updated.txt");

        }

        // Method to get cards
        private void updateCardsWork(object sender , DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            int setCount = 0;

            // Loop thru cardSets
            foreach ( var cardSet in cardSets )
            {
                setCount++;

                // Get a hashmap of all the sets card
                // Key: Card URL
                // Value: Number of cards (This only really matters for decks)
                Dictionary<String , int> cardList = cardMaker.GetCardsFromUrl(cardSet.Value);
                List<WixossCard> setCards = new List<WixossCard>();

                worker.ReportProgress(cardList.Values.Count , "Card Set Count");
                worker.ReportProgress(setCount , "Card Set Value");
                int cardCount = 0;

                // Loop thru all cards in the set
                foreach ( var cardItem in cardList )
                {
                    cardCount++;
                    worker.ReportProgress(cardCount , "Card Value");

                    // Get card from url
                    WixossCard theCard = cardMaker.GetCardFromUrl(cardItem.Key);

                    // Create or update the card to sql lite db
                    WixCardService.CreateOrUpdate(theCard);
                }

                worker.ReportProgress(-1 , "Set: " + cardSet + " updated");

            }
            worker.ReportProgress(setCount + 1 , "Card Set Value");
            
        }
    }
}
