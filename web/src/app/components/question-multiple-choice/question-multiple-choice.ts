import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CandidateQuestionDto } from '../../Models/Dtos/Test/candidate-question-dto';
import { TestAnswerSubmitDto } from '../../Models/Dtos/Test/test-answer-submit-dto';

@Component({
  selector: 'app-question-multiple-choice',
  standalone: false,
  templateUrl: './question-multiple-choice.html',
})
export class QuestionMultipleChoice {
  @Input({ required: true }) question!: CandidateQuestionDto;
  @Input() currentAnswer: TestAnswerSubmitDto | null = null;
  @Output() answerChange = new EventEmitter<TestAnswerSubmitDto>();

  get allowMultiple(): boolean {
    return this.question.multipleChoice?.allowMultipleSelection ?? false;
  }

  get options(): string[] {
    return this.question.multipleChoice?.options ?? [];
  }

  private get selectedIndexes(): number[] {
    return this.currentAnswer?.selectedOptionIndexes ?? [];
  }

  isSelected(index: number): boolean {
    return this.selectedIndexes.includes(index);
  }

  onSingleSelect(index: number): void {
    this.answerChange.emit({
      questionId: this.question.id,
      selectedOptionIndexes: [index],
    });
  }

  onMultiToggle(index: number, checked: boolean): void {
    const current = new Set(this.selectedIndexes);
    if (checked) {
      current.add(index);
    } else {
      current.delete(index);
    }

    this.answerChange.emit({
      questionId: this.question.id,
      selectedOptionIndexes: Array.from(current).sort((a, b) => a - b),
    });
  }
}
