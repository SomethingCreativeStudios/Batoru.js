import { Component, OnInit, Input } from '@angular/core';
import { plainToClass } from 'class-transformer';
import { WixCard } from '../../models/wixCard';
import { ElectronService } from '../../providers/electron.service';
import { FileService } from '../../services/file/file.service';

@Component({
  selector: 'batoru-wix-card',
  templateUrl: './wix-card.component.html',
  styleUrls: ['./wix-card.component.scss'],
})
export class WixCardComponent implements OnInit {
  @Input("wix-card")
  public wixCard: WixCard;

  constructor(private electronService: ElectronService, private fileService: FileService) {}

  public ngOnInit() { }

  public loadCardImage = () => {
    return this.fileService.loadImageAsBase64(<string>this.wixCard.CardImagePath, true);
  };
}
