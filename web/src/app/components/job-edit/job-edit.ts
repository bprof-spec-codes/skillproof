import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { JobService } from '../../services/job-service';
import { AssessmentService } from '../../services/assesmentservice';
import { DashboardRoutingService } from '../../services/dashboardRouting';

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
  shortDescription = '';
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
    private dashRoute: DashboardRoutingService,
  ) {}

  ngOnInit(): void {
    this.jobId = this.route.snapshot.paramMap.get('id') || '';

    if (this.jobId) {
      this.jobService.getJobById(this.jobId).subscribe({
        next: (job: any) => {
          this.companyId = job.companyId ?? '';
          this.title = job.title;
          this.location = job.location;
          this.salary = job.salary ?? '';

          let parsedTags: string[] = [];
          if (Array.isArray(job.tags)) {
            parsedTags = job.tags;
          } else if (typeof job.tags === 'string') {
            try {
              const parsed = JSON.parse(job.tags);
              parsedTags = Array.isArray(parsed) ? parsed : [job.tags];
            } catch {
              parsedTags = job.tags.split(',');
            }
          }
          this.tags = parsedTags
            .map((t) => t.replace(/[\[\]"]/g, '').trim())
            .filter((t) => t !== '')
            .join(', ');

          if (typeof job.employmentType === 'string') {
            const enumMap: Record<string, number> = {
              FullTime: 0,
              PartTime: 1,
              Contract: 2,
              Freelance: 3,
            };
            this.employmentType = enumMap[job.employmentType] ?? 0;
          } else {
            this.employmentType = job.employmentType ?? 0;
          }

          this.description = job.description;
          this.shortDescription = job.shortDescription;

          this.selectedAssessments = job.assessments || job.Assessments || [];

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

    let tagsArray: string[] = [];

    if (Array.isArray(this.tags)) {
      tagsArray = this.tags;
    } else if (typeof this.tags === 'string') {
      tagsArray = this.tags
        .split(',')
        .map((t) => t.trim())
        .filter((t) => t !== '');
    }

    const updateDto = {
      id: this.jobId,
      companyId: this.companyId,
      title: this.title,
      location: this.location,
      salary: this.salary ? Number(this.salary) : null,
      employmentType: Number(this.employmentType),
      description: this.description,
      shortDescription: this.shortDescription,
      tags: tagsArray.join(','),
      assessmentIds: this.selectedAssessments.map((a) => a.id),
    };

    this.jobService.updateJob(this.jobId, updateDto as any).subscribe({
      next: () => {
        setTimeout(() => {
          this.router.navigate(['/company']);
        }, 500);
      },
      error: () => {
        this.error = 'An error occurred while updating the job.';
        this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }

  goHome() {
    this.dashRoute.routeToDashboard();
  }
}
