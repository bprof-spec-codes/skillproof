import { ChangeDetectorRef, Component, NgZone, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { CandidateAssessmentDto } from '../../Models/Dtos/Test/candidate-assessment-dto';
import { CandidateQuestionDto } from '../../Models/Dtos/Test/candidate-question-dto';
import { TestAnswerSubmitDto } from '../../Models/Dtos/Test/test-answer-submit-dto';
import { TestResultDto } from '../../Models/Dtos/Test/test-result-dto';
import { TestSubmitDto } from '../../Models/Dtos/Test/test-submit-dto';
import { QuestionType } from '../../Models/Enums/QuestionType';
import { ModalService } from '../../services/modal-service';
import { TestService } from '../../services/test-service';
import { TestSubmitSkillDto } from '../../Models/Dtos/Test/test-submit-skill-dto';

@Component({
  selector: 'app-test-take',
  standalone: false,
  templateUrl: './test-take.html',
  styleUrl: './test-take.scss',
})
export class TestTake implements OnInit, OnDestroy {
  readonly QuestionType = QuestionType;

  jobId: string | null = null;
  skillId: string | null = null;
  assessment: CandidateAssessmentDto | null = null;
  currentIndex = 0;
  answers = new Map<string, TestAnswerSubmitDto>();

  isLoading = true;
  errorMessage: string | null = null;
  result: TestResultDto | null = null;

  isSubmitting = false;

  private routeSubscription: Subscription | null = null;
  private loadSubscription: Subscription | null = null;
  private submitSubscription: Subscription | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private testService: TestService,
    private modalService: ModalService,
    private ngZone: NgZone,
    private cdr: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap.subscribe((params) => {
      const id = params.get('id');
      const skillId = params.get('skillId');
      const assessmentId = params.get('assessmentId');
      const jobId = params.get('id');
      if (jobId) {
        this.jobId = jobId;
        this.loadTest(jobId);
      } else if (skillId && assessmentId) {
        this.skillId = skillId;
        this.loadTestForSkill(skillId, assessmentId);
      } else {
        this.ngZone.run(() => {
          this.errorMessage = 'No Job ID or Skill ID provided in the route.';
          this.isLoading = false;
          this.cdr.detectChanges();
        });
      }
    });
  }

  ngOnDestroy(): void {
    this.routeSubscription?.unsubscribe();
    this.loadSubscription?.unsubscribe();
    this.submitSubscription?.unsubscribe();
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
    if (!this.assessment || this.isSubmitting) {
      return;
    }

    this.isSubmitting = true;
    this.submitSubscription?.unsubscribe();

    const answerList = this.assessment.questions.map((q) => this.buildAnswerFor(q));

    if (this.jobId) {
      const dto: TestSubmitDto = {
        jobId: this.jobId,
        answers: answerList,
      };

      this.submitSubscription = this.testService.submitTest(dto).subscribe({
        next: (result) => {
          this.ngZone.run(() => {
            this.result = result;
            this.isSubmitting = false;
            this.cdr.detectChanges();
          });
        },
        error: () => this.handleSubmitError()
      });

    } else if (this.skillId) {
      const dto: TestSubmitSkillDto = {
        skillId: this.skillId,
        assessmentId: this.assessment!.id,
        answers: answerList,
      };

      this.submitSubscription = this.testService.submitSkillTest(dto).subscribe({
        next: (result) => {
          this.ngZone.run(() => {
            this.result = result;
            this.isSubmitting = false;
            this.cdr.detectChanges();
          });
        },
        error: () => this.handleSubmitError()
      });
    }
  }

  private buildAnswerFor(question: CandidateQuestionDto): TestAnswerSubmitDto {
    const existing = this.answers.get(question.id);
    if (existing) {
      return existing;
    }

    return { questionId: question.id };
  }

  backToJob(): void {
    if (this.jobId) {
      this.router.navigate(['/job', this.jobId]);
    } else {
      this.router.navigate(['/home']);
    }
  }

  backToProfile(): void {
    this.router.navigate(['/viewProfile'])
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

  private loadTestForSkill(skillId: string, assessmentId: string) {
    this.isLoading = true;
    this.errorMessage = null;

    this.loadSubscription = this.testService.getCandidateTestForSkill(skillId, assessmentId).subscribe({
      next: (data) => {
        this.assessment = data;
        this.answers.clear();
        this.currentIndex = 0;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Failed to load test. Please try again.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  private handleSubmitError(): void {
    this.ngZone.run(() => {
      this.isSubmitting = false;
      this.modalService.open({
        message: 'Failed to submit the test. Please try again.',
        autoClose: true,
        duration: 3000,
        type: 'error',
      });
      this.cdr.detectChanges();
    });
  }
}