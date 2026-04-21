import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { JobService } from '../services/job-service';
import { QuestionBankService } from '../services/question-bank-service';
import { QuestionResponseDto } from '../Models/Dtos/Question/question-response-dto';

@Component({
  selector: 'app-job-edit',
  standalone: false,
  templateUrl: './job-edit.html',
  styleUrl: './job-edit.scss',
})
export class JobEdit implements OnInit {
  jobId = '';
  companyId = '';
  title = '';
  location = '';
  employmentType: number | null = 0;
  salary = '';
  tags = '';
  description = '';
  error = '';
  loading = false;

  isPreViewSubject = new BehaviorSubject<boolean>(false);
  isPreView$ = this.isPreViewSubject.asObservable();
  preViewmdSubject = new BehaviorSubject<string>('');
  preViewmd = this.preViewmdSubject.asObservable();

  showQuestionModal = false;

  availableQuestions: QuestionResponseDto[] = [];
  selectedQuestions: QuestionResponseDto[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private jobService: JobService,
    private questionBankService: QuestionBankService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.jobId = this.route.snapshot.paramMap.get('id') || '';

    if (this.jobId) {
      this.jobService.getJobById(this.jobId).subscribe({
        next: (job) => {
          this.companyId = job.companyId ?? '';
          this.title = job.title;
          this.location = job.location;
          this.employmentType = job.EmploymentType ?? 0;
          this.tags = job.tags ? job.tags.join(', ') : '';
          this.description = job.description;
          this.selectedQuestions = job.questions || [];
          this.cdr.detectChanges();
        },
        error: () => {
          this.error = 'Failed to load job details.';
          this.cdr.detectChanges();
        },
      });
    }

    this.questionBankService.getAll().subscribe();
    this.questionBankService.questions$.subscribe((questions) => {
      this.availableQuestions = questions;
    });
  }

  preView(): void {
    this.isPreViewSubject.next(!this.isPreViewSubject.value);
    this.preViewmdSubject.next(this.description);
  }

  openAddQuestionModal(): void {
    this.showQuestionModal = true;
  }

  closeQuestionModal(): void {
    this.showQuestionModal = false;
  }

  selectQuestion(question: QuestionResponseDto): void {
    const alreadyAdded = this.selectedQuestions.some((q) => q.id === question.id);
    if (!alreadyAdded) {
      this.selectedQuestions.push(question);
    }
    this.closeQuestionModal();
  }

  removeQuestion(questionId: string): void {
    this.selectedQuestions = this.selectedQuestions.filter((q) => q.id !== questionId);
  }

  onSubmit(): void {
    this.loading = true;
    this.error = '';

    const tagsArray = this.tags
      .split(',')
      .map((t) => t.trim())
      .filter((t) => t !== '');

    const updateDto = {
      id: this.jobId,
      companyId: this.companyId,
      title: this.title,
      location: this.location,
      employmentType: Number(this.employmentType),
      description: this.description,
      tags: tagsArray,
      questionIds: this.selectedQuestions.map((q) => q.id),
    };

    try {
      this.jobService.updateJob(this.jobId, updateDto as any);
      setTimeout(() => {
        this.router.navigate(['/profile']);
      }, 500);
    } catch (err) {
      this.error = 'An error occurred while updating the job.';
      this.loading = false;
    }
  }
}
