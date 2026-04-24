import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CandidateQuestionDto } from '../../Models/Dtos/Test/candidate-question-dto';
import { TestAnswerSubmitDto } from '../../Models/Dtos/Test/test-answer-submit-dto';

@Component({
  selector: 'app-question-open-ended',
  standalone: false,
  templateUrl: './question-open-ended.html',
  styleUrl: './question-open-ended.scss',
})
export class QuestionOpenEnded {
  readonly maxLength = 500;

  @Input({ required: true }) question!: CandidateQuestionDto;
  @Input() currentAnswer: TestAnswerSubmitDto | null = null;
  @Output() answerChange = new EventEmitter<TestAnswerSubmitDto>();

  get responseText(): string {
    return this.currentAnswer?.textAnswer ?? '';
  }

  onInput(value: string): void {
    this.answerChange.emit({
      questionId: this.question.id,
      textAnswer: value,
    });
  }
}
