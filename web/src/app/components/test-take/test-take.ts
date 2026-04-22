import { ChangeDetectorRef, Component, NgZone, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { CandidateAssessmentDto } from '../../Models/Dtos/Test/candidate-assessment-dto';
import { CandidateQuestionDto } from '../../Models/Dtos/Test/candidate-question-dto';
import { TestAnswerSubmitDto } from '../../Models/Dtos/Test/test-answer-submit-dto';
import { TestResultDto } from '../../Models/Dtos/Test/test-result-dto';
import { QuestionType } from '../../Models/Enums/QuestionType';
import { TestService } from '../../services/test-service';

@Component({
  selector: 'app-test-take',
  standalone: false,
  templateUrl: './test-take.html',
  styleUrl: './test-take.scss',
})
export class TestTake implements OnInit, OnDestroy {
  readonly QuestionType = QuestionType;

  jobId: string | null = null;
  assessment: CandidateAssessmentDto | null = null;
  currentIndex = 0;
  answers = new Map<string, TestAnswerSubmitDto>();

  isLoading = true;
  errorMessage: string | null = null;
  result: TestResultDto | null = null;

  private routeSubscription: Subscription | null = null;
  private loadSubscription: Subscription | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private testService: TestService,
    private ngZone: NgZone,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (!id) {
        this.ngZone.run(() => {
          this.isLoading = false;
          this.errorMessage = 'Invalid job id.';
          this.cdr.detectChanges();
        });
        return;
      }
      this.jobId = id;
      this.loadTest(id);
    });
  }

  ngOnDestroy(): void {
    this.routeSubscription?.unsubscribe();
    this.loadSubscription?.unsubscribe();
  }

  get currentQuestion(): CandidateQuestionDto | null {
    if (!this.assessment || this.assessment.questions.length === 0) {
      return null;
    }
    return this.assessment.questions[this.currentIndex] ?? null;
  }

  get isFirstQuestion(): boolean {
    return this.currentIndex === 0;
  }

  get isLastQuestion(): boolean {
    return !!this.assessment && this.currentIndex === this.assessment.questions.length - 1;
  }

  goBack(): void {
    if (!this.isFirstQuestion) {
      this.currentIndex -= 1;
    }
  }

  goNext(): void {
    if (this.assessment && this.currentIndex < this.assessment.questions.length - 1) {
      this.currentIndex += 1;
    }
  }

  currentAnswerFor(questionId: string): TestAnswerSubmitDto | null {
    return this.answers.get(questionId) ?? null;
  }

  onAnswerChange(answer: TestAnswerSubmitDto): void {
    this.answers.set(answer.questionId, answer);
  }

  onSubmit(): void {
    console.log('Test answers (stub — will be wired up in Commit 7):', Array.from(this.answers.values()));
  }

  backToJob(): void {
    if (this.jobId) {
      this.router.navigate(['/job', this.jobId]);
    } else {
      this.router.navigate(['/home']);
    }
  }

  private loadTest(jobId: string): void {
    this.loadSubscription?.unsubscribe();
    this.isLoading = true;
    this.errorMessage = null;
    this.assessment = null;
    this.result = null;

    this.loadSubscription = this.testService.getCandidateTestForJob(jobId).subscribe({
      next: (assessment) => {
        this.ngZone.run(() => {
          this.assessment = assessment;
          this.currentIndex = 0;
          this.isLoading = false;
          this.cdr.detectChanges();
        });
      },
      error: () => {
        this.ngZone.run(() => {
          this.errorMessage = 'Failed to load the test. Please try again later.';
          this.isLoading = false;
          this.cdr.detectChanges();
        });
      },
    });
  }
}
