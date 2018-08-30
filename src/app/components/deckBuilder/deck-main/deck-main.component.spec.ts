import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeckMainComponent } from './deck-main.component';

describe('DeckMainComponent', () => {
  let component: DeckMainComponent;
  let fixture: ComponentFixture<DeckMainComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeckMainComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeckMainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
