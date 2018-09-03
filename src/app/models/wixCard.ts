import { Type, plainToClass } from 'class-transformer';

export class WixCard {
  public CardEffect: String;
  public CardImagePath: String;
  public CardName: String;
  public CardSets: String[];
  public CardUrl: String;
  public Class: String[];
  public ColorStr: String;
  public CostStr: String;
  public Guard: Boolean;
  public Id: Number;
  public ImageUrl: String;
  public Level: Number;
  public LevelLimt: Number;
  public LifeBurst: Boolean;
  public LimitingCondition: any;
  public MultiEner: Boolean;
  public Power: Number;
  public TimmingStr: String;

  @Type(() => CardCost)
  public Cost: CardCost[];

  public Timing: CardTiming[];
  public Color: CardColor[];
  public Type: CardType;

  public getImagePath() {
    const startIndex = this.CardImagePath.indexOf('Wix Cards');
    const endIndex = this.CardImagePath.length - startIndex;
    const imagePath = this.CardImagePath.substr(startIndex, endIndex);
    return imagePath;
  }
}

export class CardCost {
  public Id: Number;
  public Color: number;
  public NumberPerColor: Number;
}

export enum CardTiming {
  MainPhase = 0,
  AttackPhase = 1,
  SpellCutIn = 2,
  NoTiming = 3,
}

export enum CardColor {
  Green = 0,
  Black = 1,
  Red = 2,
  Blue = 3,
  White = 4,
  Colorless = 5,
  NoColor = 6,
}

export enum CardType {
  ARTS = 0,
  LRIG = 1,
  SIGNI = 2,
  Resona = 3,
  Spell = 4,
  NoType = 5,
}
