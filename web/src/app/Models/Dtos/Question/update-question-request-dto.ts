import { DifficultyLevel } from "../../Enums/DifficultyLevel";
import { CodeCompletionQuestionPayloadDto, FillInTheBlankQuestionPayloadDto, MultipleChoiceQuestionPayloadDto, OpenEndedQuestionPayloadDto, TrueFalseQuestionPayloadDto } from "./question-type-payload-dtos";

export class UpdateQuestionRequestDto {
  language = '';
  difficulty: DifficultyLevel = DifficultyLevel.Junior;
  title = '';
  questionText = '';
  isActive = true;

  multipleChoice?: MultipleChoiceQuestionPayloadDto;
  codeCompletion?: CodeCompletionQuestionPayloadDto;
  openEnded?: OpenEndedQuestionPayloadDto;
  // TODO(OpenEnded-cleanup): remove legacy wire property after backend contract rename.
  fillInTheBlank?: FillInTheBlankQuestionPayloadDto;
  trueFalse?: TrueFalseQuestionPayloadDto;
}
