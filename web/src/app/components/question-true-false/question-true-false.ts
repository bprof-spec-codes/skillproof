import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CandidateQuestionDto } from '../../Models/Dtos/Test/candidate-question-dto';
import { TestAnswerSubmitDto } from '../../Models/Dtos/Test/test-answer-submit-dto';

@Component({
  selector: 'app-question-true-false',
  standalone: false,
  templateUrl: './question-true-false.html',
  styleUrl: './question-true-false.scss',
})
export class QuestionTrueFalse {
  @Input({ required: true }) question!: CandidateQuestionDto;
  @Input() currentAnswer: TestAnswerSubmitDto | null = null;
  @Output() answerChange = new EventEmitter<TestAnswerSubmitDto>();

  get selected(): boolean | null {
    return this.currentAnswer?.boolAnswer ?? null;
  }

  onSelect(value: boolean): void {
    this.answerChange.emit({
      questionId: this.question.id,
      boolAnswer: value,
    });
  }
}
