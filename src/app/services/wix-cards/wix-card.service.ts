import { Injectable } from '@angular/core';
import { FileService } from '../file/file.service';

@Injectable({
  providedIn: 'root',
})
export class WixCardService {

  private workspacePath: String = "C:\\Users\\eric-\\Documents\\Wix Cards";
  private deckPath: String = "\\sets\\decks";

  constructor(private fileService: FileService) {
  }

  public getAllDecks(): String[] {
    const deckSets = new Array<String>();
    
    return deckSets;
  }
}