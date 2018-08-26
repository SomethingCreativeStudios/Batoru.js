import * as cheerio from 'cheerio';
import { WixCard } from "../src/app/models/wixCard";



export class CardMaker {

    public getCardFromURL(cardURL){
        
        const card = new WixCard;
        card.CardUrl = "http://selector-wixoss.wikia.com/wiki/Nanashi,_That_Four";
        
        card.CardName = "raburabu";

        

    } 
    

}