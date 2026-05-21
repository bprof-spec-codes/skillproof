import { DifficultyLevel } from "../../Enums/DifficultyLevel";

export interface BadgeDto {
  sourceName: string;
  difficultyLevel: DifficultyLevel;
  issuedAt: Date;
}