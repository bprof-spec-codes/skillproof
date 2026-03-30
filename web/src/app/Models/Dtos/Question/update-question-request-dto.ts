import { DifficultyLevel } from "../../Enums/DifficultyLevel";
import { CodeCompletionQuestionPayloadDto, FillInTheBlankQuestionPayloadDto, MultipleChoiceQuestionPayloadDto, TrueFalseQuestionPayloadDto } from "./question-type-payload-dtos";

export class UpdateQuestionRequestDto {
  language = '';
  difficulty: DifficultyLevel = DifficultyLevel.Junior;
  title = '';
  questionText = '';
  isActive = true;

  multipleChoice?: MultipleChoiceQuestionPayloadDto;
  codeCompletion?: CodeCompletionQuestionPayloadDto;
  fillInTheBlank?: FillInTheBlankQuestionPayloadDto;
  trueFalse?: TrueFalseQuestionPayloadDto;
}
