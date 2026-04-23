import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { CandidateQuestionDto } from '../../Models/Dtos/Test/candidate-question-dto';
import { TestAnswerSubmitDto } from '../../Models/Dtos/Test/test-answer-submit-dto';

@Component({
  selector: 'app-question-code-completion',
  standalone: false,
  templateUrl: './question-code-completion.html',
  styleUrl: './question-code-completion.scss',
})
export class QuestionCodeCompletion implements OnChanges {
  @Input({ required: true }) question!: CandidateQuestionDto;
  @Input() currentAnswer: TestAnswerSubmitDto | null = null;
  @Output() answerChange = new EventEmitter<TestAnswerSubmitDto>();

  hasInlinePlaceholder = false;
  preSnippet = '';
  postSnippet = '';

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['question']) {
      this.parseSnippet();
    }
  }

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

  private parseSnippet(): void {
    const snippet = this.codeSnippet;
    const placeholderMatch = snippet.match(/_{3,}/);

    if (!placeholderMatch || placeholderMatch.index === undefined) {
      this.hasInlinePlaceholder = false;
      this.preSnippet = '';
      this.postSnippet = '';
      return;
    }

    const start = placeholderMatch.index;
    const end = start + placeholderMatch[0].length;

    this.hasInlinePlaceholder = true;
    this.preSnippet = snippet.slice(0, start);
    this.postSnippet = snippet.slice(end);
  }
}
