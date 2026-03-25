import { DifficultyLevel } from '../../Enums/DifficultyLevel';
import { QuestionType } from '../../Enums/QuestionType';

export class QuestionListFilterDto {
  type?: QuestionType;
  difficulty?: DifficultyLevel;
  language?: string;
  isActive?: boolean;
}
