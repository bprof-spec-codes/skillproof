import { QuestionResponseDto } from '../Question/question-response-dto';

export interface AssessmentViewDto {
  id: string;
  title: string;
  description: string;
  difficultyLevel: number;
  createdBy: string;
  createdAt: string;
  isActive: boolean;
  questions?: QuestionResponseDto[];
  questionIds?: string[];
}
