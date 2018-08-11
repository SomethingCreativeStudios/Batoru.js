import { Injectable } from '@angular/core';
import { AppSettings } from '../../global/app-settings';
import { FileService } from '../file/file.service';

@Injectable({
  providedIn: 'root',
})
export class WixCardService {
  constructor(private fileService: FileService) {}

  public getAllDecks(): String[] {
    AppSettings.workspacePath = '';
    const deckSets = [];
    let test = 'test';
    return deckSets;
  }
}
