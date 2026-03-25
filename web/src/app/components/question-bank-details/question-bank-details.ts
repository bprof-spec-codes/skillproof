import { ChangeDetectorRef, Component, NgZone, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs';
import { QuestionResponseDto } from '../../Models/Dtos/Question/question-response-dto';
import { DifficultyLevel } from '../../Models/Enums/DifficultyLevel';
import { QuestionType } from '../../Models/Enums/QuestionType';
import { QuestionBankService } from '../../services/question-bank-service';

@Component({
  selector: 'app-question-bank-details',
  standalone: false,
  templateUrl: './question-bank-details.html',
  styleUrl: './question-bank-details.scss',
})
export class QuestionBankDetails implements OnInit {
  question: QuestionResponseDto | null = null;
  loading = false;

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
    private route: ActivatedRoute,
    private router: Router,
    private questionBankService: QuestionBankService,
    private ngZone: NgZone,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.router.navigate(['/question-bank']);
      return;
    }

    this.loading = true;
    this.questionBankService
      .getById(id)
      .pipe(
        finalize(() => {
          this.ngZone.run(() => {
            this.loading = false;
            this.cdr.detectChanges();
          });
        })
      )
      .subscribe({
        next: (question) => {
          this.ngZone.run(() => {
            this.question = question;
            this.cdr.detectChanges();
          });
        },
        error: () => {
          this.ngZone.run(() => {
            this.question = null;
            this.cdr.detectChanges();
          });
        },
      });
  }

  goBack(): void {
    this.router.navigate(['/question-bank']);
  }

  editQuestion(): void {
    if (!this.question) {
      return;
    }

    this.router.navigate(['/question-bank', this.question.id, 'edit']);
  }

  getQuestionTypeLabel(type: QuestionType): string {
    return this.questionTypeOptions.find((option) => option.value === type)?.label ?? String(type);
  }

  getDifficultyLabel(difficulty: DifficultyLevel): string {
    return this.difficultyOptions.find((option) => option.value === difficulty)?.label ?? String(difficulty);
  }
}
