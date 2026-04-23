import { DifficultyLevel } from '../../Enums/DifficultyLevel';
import { QuestionResultDto } from './question-result-dto';

export interface TestResultDto {
  testId: string;
  jobApplicationId: string;
  score: number;
  maxScore: number;
  passed: boolean;
  difficultyLevel: DifficultyLevel;
  questionResults: QuestionResultDto[];
}
