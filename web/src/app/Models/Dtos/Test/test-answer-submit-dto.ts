export interface TestAnswerSubmitDto {
  questionId: string;
  selectedOptionIndexes?: number[];
  boolAnswer?: boolean;
  textAnswer?: string;
}
