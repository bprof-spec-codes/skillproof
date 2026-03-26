export class MultipleChoiceQuestionPayloadDto {
  options: string[] = [];
  correctOptionIndexes: number[] = [];
  allowMultipleSelection = false;
}

export class CodeCompletionQuestionPayloadDto {
  codeSnippet = '';
  acceptedAnswers: string[] = [];
}

export class FillInTheBlankQuestionPayloadDto {
  answer = '';
  manualFeedback?: string;
}

export class TrueFalseQuestionPayloadDto {
  correctAnswer = false;
  explanation?: string;
}
