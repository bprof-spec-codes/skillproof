export class MultipleChoiceQuestionPayloadDto {
  options: string[] = [];
  correctOptionIndexes: number[] = [];
  allowMultipleSelection = false;
}

export class CodeCompletionQuestionPayloadDto {
  codeSnippet = '';
  acceptedAnswers: string[] = [];
}

export class OpenEndedQuestionPayloadDto {
  answer = '';
  manualFeedback?: string;
}

// TODO(OpenEnded-cleanup): remove legacy alias after backend contract rename.
export class FillInTheBlankQuestionPayloadDto extends OpenEndedQuestionPayloadDto {}

export class TrueFalseQuestionPayloadDto {
  correctAnswer = false;
  explanation?: string;
}
