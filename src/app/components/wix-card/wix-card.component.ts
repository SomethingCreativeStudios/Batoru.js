import { Component, OnInit } from '@angular/core';
import { ElectronService } from '../../providers/electron.service';
import { WixCard } from '../../models/wixCard';
import { plainToClass } from 'class-transformer';

@Component({
  selector: 'batoru-wix-card',
  templateUrl: './wix-card.component.html',
  styleUrls: ['./wix-card.component.scss']
})
export class WixCarComponent implements OnInit {
  private mainDir: string;

  constructor(private electronService: ElectronService) {
    this.loadDir();
  }

  ngOnInit() {}

  private loadDir() {
    this.mainDir = 'C:\\Users\\eric-\\Documents\\Wix Cards\\sets';
    const data = this.electronService.ipcRenderer.sendSync('load-json', this.mainDir + '\\cards.json');
    const wixCards = plainToClass(WixCard, data as Object[]);
  }
}
