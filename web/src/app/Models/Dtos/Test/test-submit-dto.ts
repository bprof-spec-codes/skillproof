import { TestAnswerSubmitDto } from './test-answer-submit-dto';

export interface TestSubmitDto {
  jobId: string;
  answers: TestAnswerSubmitDto[];
}
