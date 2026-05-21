import { DifficultyLevel } from '../../Enums/DifficultyLevel';
import { QuestionType } from '../../Enums/QuestionType';
import { CandidateCodeCompletionPayloadDto } from './candidate-code-completion-payload-dto';
import { CandidateMultipleChoicePayloadDto } from './candidate-multiple-choice-payload-dto';
import { CandidateOpenEndedPayloadDto } from './candidate-open-ended-payload-dto';
import { CandidateTrueFalsePayloadDto } from './candidate-true-false-payload-dto';

export interface CandidateQuestionDto {
  id: string;
  type: QuestionType;
  title: string;
  questionText: string;
  language: string;
  difficulty: DifficultyLevel;

  multipleChoice?: CandidateMultipleChoicePayloadDto;
  codeCompletion?: CandidateCodeCompletionPayloadDto;
  openEnded?: CandidateOpenEndedPayloadDto;
  // TODO(OpenEnded-cleanup): remove legacy wire property after backend contract rename.
  fillInTheBlank?: CandidateOpenEndedPayloadDto;
  trueFalse?: CandidateTrueFalsePayloadDto;
}
