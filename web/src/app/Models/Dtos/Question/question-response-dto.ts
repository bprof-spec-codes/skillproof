import { DifficultyLevel } from '../../Enums/DifficultyLevel';
import { QuestionType } from '../../Enums/QuestionType';
import {CodeCompletionQuestionPayloadDto, FillInTheBlankQuestionPayloadDto, MultipleChoiceQuestionPayloadDto, TrueFalseQuestionPayloadDto } from './question-type-payload-dtos';

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
  fillInTheBlank?: FillInTheBlankQuestionPayloadDto;
  trueFalse?: TrueFalseQuestionPayloadDto;
}
