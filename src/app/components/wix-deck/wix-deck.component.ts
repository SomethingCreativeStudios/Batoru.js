import { Component, OnInit, Input } from '@angular/core';
import { ElectronService } from '../../providers/electron.service';
import { WixCard } from '../../models/wixCard';
import { plainToClass } from 'class-transformer';

@Component({
  selector: 'batoru-wix-deck',
  templateUrl: './wix-deck.component.html',
  styleUrls: ['./wix-deck.component.scss']
})
export class WixDeckComponent implements OnInit {
  private mainDir: string;

  @Input() public deckName: string;

  @Input() public wixCards: WixCard[];

  constructor(private electronService: ElectronService) {
    this.loadDir();
  }

  ngOnInit() {}
}
