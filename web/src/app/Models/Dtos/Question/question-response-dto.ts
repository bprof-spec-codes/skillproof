import { DifficultyLevel } from '../../Enums/DifficultyLevel';
import { QuestionType } from '../../Enums/QuestionType';
import {CodeCompletionQuestionPayloadDto, FillInTheBlankQuestionPayloadDto, MultipleChoiceQuestionPayloadDto, OpenEndedQuestionPayloadDto, TrueFalseQuestionPayloadDto } from './question-type-payload-dtos';

export class QuestionResponseDto {
  id = '';
  type: QuestionType = QuestionType.MultipleChoice;
  language = '';
  difficulty: DifficultyLevel = DifficultyLevel.Junior;
  title = '';
  questionText = '';
  createdBy = '';
  isActive = true;
  createdAt = '';
  updatedAt = '';

  multipleChoice?: MultipleChoiceQuestionPayloadDto;
  codeCompletion?: CodeCompletionQuestionPayloadDto;
  openEnded?: OpenEndedQuestionPayloadDto;
  // TODO(OpenEnded-cleanup): remove legacy wire property after backend contract rename.
  fillInTheBlank?: FillInTheBlankQuestionPayloadDto;
  trueFalse?: TrueFalseQuestionPayloadDto;
}
