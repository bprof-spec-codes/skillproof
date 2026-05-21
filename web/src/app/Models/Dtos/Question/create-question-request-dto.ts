import { DifficultyLevel } from '../../Enums/DifficultyLevel';
import { QuestionType } from '../../Enums/QuestionType';
import { CodeCompletionQuestionPayloadDto, FillInTheBlankQuestionPayloadDto, MultipleChoiceQuestionPayloadDto, OpenEndedQuestionPayloadDto, TrueFalseQuestionPayloadDto} from './question-type-payload-dtos';

export class CreateQuestionRequestDto {
  type: QuestionType = QuestionType.MultipleChoice;
  language?: string;
  difficulty: DifficultyLevel = DifficultyLevel.Junior;
  tags: string[] = [];
  title = '';
  questionText = '';
  createdBy = '';

  multipleChoice?: MultipleChoiceQuestionPayloadDto;
  codeCompletion?: CodeCompletionQuestionPayloadDto;
  openEnded?: OpenEndedQuestionPayloadDto;
  // TODO(OpenEnded-cleanup): remove legacy wire property after backend contract rename.
  fillInTheBlank?: FillInTheBlankQuestionPayloadDto;
  trueFalse?: TrueFalseQuestionPayloadDto;
}
