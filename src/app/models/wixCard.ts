import { Type, plainToClass } from 'class-transformer';
import { CardCost } from '../cardCost';
export class WixCard {
  public CardEffect: string;
  public CardImagePath: string;
  public CardName: string;
  public CardNameJ: string;
  public CardNameR: string;
  public CardSets: string[];
  public CardUrl: string;
  public Class: string[];
  public ColorStr: string;
  public CostStr: string;
  public Guard: boolean;
  public Id: number;
  public ImageUrl: string;
  public Level: number;
  public LevelLimt: number;
  public LifeBurst: boolean;
  public LimitingCondition: any;
  public MultiEner: boolean;
  public Power: number;
  public TimmingStr: string;

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



export enum CardTiming {
  MainPhase = 0,
  AttackPhase = 1,
  SpellCutIn = 2,
  NoTiming = 3
}

export enum CardColor {
  Green = 0,
  Black = 1,
  Red = 2,
  Blue = 3,
  White = 4,
  Colorless = 5,
  NoColor = 6
}

export enum CardType {
  ARTS = 0,
  LRIG = 1,
  SIGNI = 2,
  Resona = 3,
  Spell = 4,
  NoType = 5
}
