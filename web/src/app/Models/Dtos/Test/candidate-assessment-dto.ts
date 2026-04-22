import { DifficultyLevel } from '../../Enums/DifficultyLevel';
import { CandidateQuestionDto } from './candidate-question-dto';

export interface CandidateAssessmentDto {
  id: string;
  title: string;
  difficultyLevel: DifficultyLevel;
  questions: CandidateQuestionDto[];
}
