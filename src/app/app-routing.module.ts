import { BattlefieldComponent } from './components/battlefield/battlefield.component';
import { DeckBuilderComponent } from './components/deckBuilder';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
    {
        path: '',
        component: DeckBuilderComponent
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, {useHash: true})],
    exports: [RouterModule]
})
export class AppRoutingModule { }
