import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CandidateQuestionDto } from '../../Models/Dtos/Test/candidate-question-dto';
import { TestAnswerSubmitDto } from '../../Models/Dtos/Test/test-answer-submit-dto';

@Component({
  selector: 'app-question-fill-in-the-blank',
  standalone: false,
  templateUrl: './question-fill-in-the-blank.html',
  styleUrl: './question-fill-in-the-blank.scss',
})
export class QuestionFillInTheBlank {
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
