import { Component, OnInit } from '@angular/core';
import { WixCard } from '../../../models/wixCard';

@Component({
  selector: 'batoru-deck-main',
  templateUrl: './deck-main.component.html',
  styleUrls: ['./deck-main.component.scss'],
})
export class DeckMainComponent implements OnInit {
  private wixCards: WixCard[];
  constructor() {}

  ngOnInit() {}

  dragoverMouse = () => {
    console.log('TEST');
  };

  dragDrop = () => { 
   console.log('DRAGED');
  }
}
