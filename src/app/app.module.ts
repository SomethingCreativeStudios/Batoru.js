import 'zone.js/dist/zone-mix';
import 'reflect-metadata';
import '../polyfills';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { HttpClientModule, HttpClient } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';

import { ElectronService } from './providers/electron.service';

import { WebviewDirective } from './directives/webview.directive';

import { AppComponent } from './app.component';
import { BattlefieldComponent } from './components/battlefield/battlefield.component';
import { DeckBuilderComponent } from './components/deckBuilder/deck-builder/deck-builder.component';
import { CardListComponent } from './components/deckBuilder/card-list/card-list.component';
import { CardSearcherComponent } from './components/deckBuilder/card-searcher/card-searcher.component';
import { DeckMainComponent } from './components/deckBuilder/deck-main/deck-main.component';
import { DeckExtraComponent } from './components/deckBuilder/deck-extra/deck-extra.component';
import { SafeHtml } from './pipes/safe-html.pipe';

@NgModule({
  declarations: [
    AppComponent,
    BattlefieldComponent,
    WebviewDirective,
    DeckBuilderComponent,
    CardListComponent,
    CardSearcherComponent,
    DeckMainComponent,
    DeckExtraComponent,
    SafeHtml,
  ],
  imports: [BrowserModule, FormsModule, HttpClientModule, AppRoutingModule],
  providers: [ElectronService],
  bootstrap: [AppComponent],
})
export class AppModule {}
