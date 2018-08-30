import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeckExtraComponent } from './deck-extra.component';

describe('DeckExtraComponent', () => {
  let component: DeckExtraComponent;
  let fixture: ComponentFixture<DeckExtraComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeckExtraComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeckExtraComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
