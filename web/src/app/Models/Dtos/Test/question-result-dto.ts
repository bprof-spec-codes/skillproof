import { QuestionType } from '../../Enums/QuestionType';

export interface QuestionResultDto {
  questionId: string;
  questionTitle: string;
  type: QuestionType;
  isCorrect: boolean;
  pointsAwarded: number;
  maxPoints: number;
  userResponse: string;
  aiFeedback: string;
}
