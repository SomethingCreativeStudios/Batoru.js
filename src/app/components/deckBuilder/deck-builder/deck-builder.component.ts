import { Component, OnInit } from '@angular/core';
import { WixCardService } from '../../../services/wix-cards/wix-card.service';
import { WixCard } from '../../../models/wixCard';
import { FileService } from '../../../services/file/file.service';

@Component({
  selector: 'batoru-deck-builder',
  templateUrl: './deck-builder.component.html',
  styleUrls: ['./deck-builder.component.scss'],
})
export class DeckBuilderComponent implements OnInit {
  private wixCards: WixCard[];

  constructor(wixCardService: WixCardService, private fileService: FileService) {
    this.wixCards = wixCardService.loadCards();
  }

  ngOnInit() {}

  private loadCardImage = (wixCard: WixCard) => {
    return this.fileService.loadImageAsBase64(<string>wixCard.CardImagePath, true);
  };
}
