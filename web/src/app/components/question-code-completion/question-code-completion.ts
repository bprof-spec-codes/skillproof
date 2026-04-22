import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CandidateQuestionDto } from '../../Models/Dtos/Test/candidate-question-dto';
import { TestAnswerSubmitDto } from '../../Models/Dtos/Test/test-answer-submit-dto';

@Component({
  selector: 'app-question-code-completion',
  standalone: false,
  templateUrl: './question-code-completion.html',
})
export class QuestionCodeCompletion {
  @Input({ required: true }) question!: CandidateQuestionDto;
  @Input() currentAnswer: TestAnswerSubmitDto | null = null;
  @Output() answerChange = new EventEmitter<TestAnswerSubmitDto>();

  get codeSnippet(): string {
    return this.question.codeCompletion?.codeSnippet ?? '';
  }

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
