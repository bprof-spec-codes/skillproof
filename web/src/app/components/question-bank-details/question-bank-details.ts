import { ChangeDetectorRef, Component, EventEmitter, Input, NgZone, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
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
export class QuestionBankDetails implements OnChanges, OnInit {
  @Input() isModalMode = false;
  @Input() questionId: string | null = null;
  @Output() closed = new EventEmitter<void>();

  readonly QuestionType = QuestionType;
  question: QuestionResponseDto | null = null;
  loading = false;

  readonly questionTypeOptions = [
    { label: 'Multiple Choice', value: QuestionType.MultipleChoice },
    { label: 'Code Completion', value: QuestionType.CodeCompletion },
    { label: 'True / False', value: QuestionType.TrueFalse },
    { label: 'Open-Ended', value: QuestionType.OpenEnded },
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

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['questionId'] && !changes['questionId'].firstChange) {
      this.loadQuestion();
    }
  }

  ngOnInit(): void {
    this.loadQuestion();
  }

  goBack(): void {
    if (this.isModalMode) {
      this.closed.emit();
      return;
    }

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

  private loadQuestion(): void {
    const id = this.questionId ?? this.route.snapshot.paramMap.get('id');
    if (!id) {
      if (!this.isModalMode) {
        this.router.navigate(['/question-bank']);
      }
      return;
    }

    this.question = null;
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
}
