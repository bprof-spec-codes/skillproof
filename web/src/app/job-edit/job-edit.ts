import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { JobService } from '../services/job-service';
import { AssessmentService } from '../services/assesmentservice';


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

  showAssessmentModal = false;

  availableAssessments: any[] = [];
  selectedAssessments: any[] = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private jobService: JobService,
    private assessmentService: AssessmentService,
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
          this.selectedAssessments = job.assessments || [];
          this.cdr.detectChanges();
        },
        error: () => {
          this.error = 'Failed to load job details.';
          this.cdr.detectChanges();
        },
      });
    }

    this.assessmentService.getAllAssessments().subscribe();
    this.assessmentService.assessments$.subscribe((assessments) => {
      this.availableAssessments = assessments;
    });
  }

  preView(): void {
    this.isPreViewSubject.next(!this.isPreViewSubject.value);
    this.preViewmdSubject.next(this.description);
  }

  openAddAssessmentModal(): void {
    this.showAssessmentModal = true;
  }

  closeAssessmentModal(): void {
    this.showAssessmentModal = false;
  }

  selectAssessment(assessment: any): void {
    const alreadyAdded = this.selectedAssessments.some((a) => a.id === assessment.id);
    if (!alreadyAdded) {
      this.selectedAssessments.push(assessment);
    }
    this.closeAssessmentModal();
  }

  removeAssessment(assessmentId: string): void {
    this.selectedAssessments = this.selectedAssessments.filter((a) => a.id !== assessmentId);
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
      assessmentIds: this.selectedAssessments.map((a) => a.id),
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
