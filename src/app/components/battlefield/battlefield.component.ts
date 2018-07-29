import { Component, OnInit } from '@angular/core';
import { ElectronService } from '../../providers/electron.service';
import { debug } from 'util';
import { WixCard } from '../../models/wixCard';
import { plainToClass } from 'class-transformer';

@Component({
  selector: 'batoru-battlefield',
  templateUrl: './battlefield.component.html',
  styleUrls: ['./battlefield.component.scss']
})
export class BattlefieldComponent implements OnInit {
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
