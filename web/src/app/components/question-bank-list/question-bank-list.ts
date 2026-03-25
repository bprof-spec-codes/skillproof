import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { QuestionResponseDto } from '../../Models/Dtos/Question/question-response-dto';
import { QuestionListFilterDto } from '../../Models/Dtos/Question/question-list-filter-dto';
import { DifficultyLevel } from '../../Models/Enums/DifficultyLevel';
import { QuestionType } from '../../Models/Enums/QuestionType';
import { QuestionBankService } from '../../services/question-bank-service';
import { ModalService } from '../../services/modal-service';

@Component({
  selector: 'app-question-bank-list',
  standalone: false,
  templateUrl: './question-bank-list.html',
  styleUrl: './question-bank-list.scss',
})
export class QuestionBankList implements OnInit {
  questions: QuestionResponseDto[] = [];
  loading = false;

  filterType = '';
  filterDifficulty = '';
  filterLanguage = '';
  filterIsActive = '';

  readonly questionTypeOptions = [
    { label: 'Multiple Choice', value: QuestionType.MultipleChoice },
    { label: 'Code Completion', value: QuestionType.CodeCompletion },
    { label: 'True / False', value: QuestionType.TrueFalse },
    { label: 'Fill In The Blank', value: QuestionType.FillInTheBlank },
  ];

  readonly difficultyOptions = [
    { label: 'Junior', value: DifficultyLevel.Junior },
    { label: 'Medior', value: DifficultyLevel.Medior },
    { label: 'Senior', value: DifficultyLevel.Senior },
  ];

  constructor(
    private questionBankService: QuestionBankService,
    private modalService: ModalService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadQuestions();
  }

  applyFilters(): void {
    this.loadQuestions();
  }

  clearFilters(): void {
    this.filterType = '';
    this.filterDifficulty = '';
    this.filterLanguage = '';
    this.filterIsActive = '';
    this.loadQuestions();
  }

  goToCreate(): void {
    this.router.navigate(['/question-bank/create']);
  }

  viewQuestion(id: string): void {
    this.router.navigate(['/question-bank', id]);
  }

  editQuestion(id: string): void {
    this.router.navigate(['/question-bank', id, 'edit']);
  }

  deleteQuestion(id: string): void {
    const shouldDelete = window.confirm('Are you sure you want to delete this question?');
    if (!shouldDelete) {
      return;
    }

    this.questionBankService.delete(id).subscribe({
      next: () => {
        this.questions = this.questions.filter((q) => q.id !== id);
        this.modalService.open({
          message: 'Question deleted successfully.',
          autoClose: true,
          duration: 2500,
          type: 'success',
        });
      },
    });
  }

  getQuestionTypeLabel(type: QuestionType): string {
    return this.questionTypeOptions.find((option) => option.value === type)?.label ?? String(type);
  }

  getDifficultyLabel(difficulty: DifficultyLevel): string {
    return this.difficultyOptions.find((option) => option.value === difficulty)?.label ?? String(difficulty);
  }

  private loadQuestions(): void {
    this.loading = true;

    this.questionBankService.getAll(this.buildFilter()).subscribe({
      next: (questions) => {
        this.questions = questions;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  private buildFilter(): QuestionListFilterDto {
    const filter = new QuestionListFilterDto();

    if (this.filterType !== '') {
      filter.type = Number(this.filterType) as QuestionType;
    }

    if (this.filterDifficulty !== '') {
      filter.difficulty = Number(this.filterDifficulty) as DifficultyLevel;
    }

    if (this.filterLanguage.trim() !== '') {
      filter.language = this.filterLanguage.trim();
    }

    if (this.filterIsActive !== '') {
      filter.isActive = this.filterIsActive === 'true';
    }

    return filter;
  }
}
