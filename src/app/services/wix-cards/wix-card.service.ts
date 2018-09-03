import { Injectable } from '@angular/core';
import { AppSettings } from '../../global/app-settings';
import { FileService } from '../file/file.service';
import { WixCard } from '../../models/wixCard';

@Injectable({
  providedIn: 'root',
})
export class WixCardService {

  private pathToSets = AppSettings.cardsPath + '\\sets';
  private pathToCards = this.pathToSets + '\\cards.json';

  constructor(private fileService: FileService) {}

  public getAllDecks(): String[] {
    console.log('Workspace Path:', AppSettings.workspacePath);
    const deckSets = [];
    const test = 'test';
    return deckSets;
  }

  public loadCards(): WixCard[] {
    let wixCards = [];
    wixCards = this.fileService.loadJSONFile(this.pathToCards);
    return wixCards;
  }
}
