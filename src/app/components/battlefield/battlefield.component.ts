import { Component, OnInit } from '@angular/core';
import { ElectronService } from '../../providers/electron.service';
import { debug } from 'util';

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

  ngOnInit() {
  }

  private loadDir() {
    this.mainDir = "C:\\Users\\eric-\\Documents\\Wix Cards";
    let fileData = this.electronService.ipcRenderer.sendSync("load-file", this.mainDir + "\\set\\cards.json");
    debugger;
  }

}
