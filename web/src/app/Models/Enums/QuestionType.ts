export enum QuestionType {
  MultipleChoice = 'MultipleChoice',
  CodeCompletion = 'CodeCompletion',
  TrueFalse = 'TrueFalse',
  OpenEnded = 'FillInTheBlank',
  // TODO(OpenEnded-cleanup): remove legacy alias after backend contract rename.
  FillInTheBlank = 'FillInTheBlank',
}
