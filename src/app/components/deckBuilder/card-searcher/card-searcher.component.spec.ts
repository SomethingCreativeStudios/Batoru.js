import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardSearcherComponent } from './card-searcher.component';

describe('CardSearcherComponent', () => {
  let component: CardSearcherComponent;
  let fixture: ComponentFixture<CardSearcherComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardSearcherComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardSearcherComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
