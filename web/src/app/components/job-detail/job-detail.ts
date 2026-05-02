import { ChangeDetectorRef, Component, NgZone, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { JobViewDto } from '../../Models/Dtos/Job/JobView-dto';
import { EmploymentType } from '../../Models/Enums/EmploymentType';
import { AuthService } from '../../services/auth-service';
import { JobService } from '../../services/job-service';
import { ModalService } from '../../services/modal-service';

@Component({
  selector: 'app-job-detail',
  standalone: false,
  templateUrl: './job-detail.html',
  styleUrl: './job-detail.scss',
})
export class JobDetail implements OnInit, OnDestroy {
  job: JobViewDto | null = null;
  isLoading = true;
  errorMessage: string | null = null;

  private routeSubscription: Subscription | null = null;
  private loadSubscription: Subscription | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private jobService: JobService,
    private modalService: ModalService,
    private authService: AuthService,
    private ngZone: NgZone,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap.subscribe((params) => {
      const id = params.get('id');

      if (!id) {
        this.ngZone.run(() => {
          this.isLoading = false;
          this.job = null;
          this.errorMessage = 'Invalid job id.';
          this.cdr.detectChanges();
        });
        return;
      }

      this.loadJob(id);
    });
  }

  ngOnDestroy(): void {
    this.routeSubscription?.unsubscribe();
    this.loadSubscription?.unsubscribe();
  }

  applyNow(): void {
    if (!this.authService.isLoggedIn()) {
      this.modalService.open({
        type: 'warning',
        message: 'Please log in as a Job Seeker before applying.',
      });
      return;
    }

    const roles = this.authService.getRoles().map((r) => r.toLowerCase());
    const isEmployer = roles.includes('employer');
    const isAdmin = roles.includes('admin');

    if (isEmployer || isAdmin) {
      this.modalService.open({
        type: 'warning',
        message: 'Only Job Seeker accounts can submit an application.',
      });
      return;
    }

    if (!this.job?.id) {
      return;
    }

    if (this.job.assessments && this.job.assessments.length > 0) {
      this.router.navigate(['/job', this.job.id, 'test']);
    } else {
      this.jobService.applyForJob(this.job.id).subscribe({
        next: (response: any) => {
          this.modalService.open({
            type: 'success',
            message: response?.message || 'Application submitted successfully.',
          });
        },
        error: (err) => {
          this.modalService.open({
            type: 'error',
            message: err.error?.message || 'An error occurred while applying.',
          });
        }
      });
    }
  }

  shareJob(): void {
    const url = window.location.href;

    if (!navigator.clipboard?.writeText) {
      this.modalService.open({
        type: 'info',
        message: 'Unable to copy automatically. Please copy the URL manually.',
      });
      return;
    }

    navigator.clipboard
      .writeText(url)
      .then(() => {
        this.modalService.open({
          type: 'success',
          message: 'Job link copied to clipboard.',
          autoClose: true,
          duration: 2000,
        });
      })
      .catch(() => {
        this.modalService.open({
          type: 'info',
          message: 'Unable to copy automatically. Please copy the URL manually.',
        });
      });
  }

  getTimeAgo(dateStr: string): string {
    const date = new Date(dateStr);
    const time = date.getTime();

    if (!dateStr || Number.isNaN(time)) {
      return 'Unknown';
    }

    const diffMs = Date.now() - time;
    if (diffMs < 0) {
      return 'Just now';
    }

    const minutes = Math.floor(diffMs / 60000);
    if (minutes < 1) return 'Just now';
    if (minutes < 60) return `${minutes} minute${minutes === 1 ? '' : 's'} ago`;

    const hours = Math.floor(minutes / 60);
    if (hours < 24) return `${hours} hour${hours === 1 ? '' : 's'} ago`;

    const days = Math.floor(hours / 24);
    if (days < 30) return `${days} day${days === 1 ? '' : 's'} ago`;

    const months = Math.floor(days / 30);
    if (months < 12) return `${months} month${months === 1 ? '' : 's'} ago`;

    const years = Math.floor(days / 365);
    return `${years} year${years === 1 ? '' : 's'} ago`;
  }

  getEmploymentTypeLabel(type: EmploymentType | null | undefined): string {
    switch (type) {
      case EmploymentType.FullTime:
        return 'Full-time';
      case EmploymentType.PartTime:
        return 'Part-time';
      case EmploymentType.Internship:
        return 'Internship';
      case EmploymentType.Temporary:
        return 'Temporary';
      default:
        return 'Unknown';
    }
  }

  private loadJob(id: string): void {
    this.loadSubscription?.unsubscribe();
    this.isLoading = true;
    this.errorMessage = null;
    this.job = null;

    this.loadSubscription = this.jobService
      .getJobById(id)
      .subscribe({
        next: (job) => {
          this.ngZone.run(() => {
            this.job = job;
            this.isLoading = false;
            this.cdr.detectChanges();
          });
        },
        error: () => {
          this.ngZone.run(() => {
            this.errorMessage = 'Job not found or unavailable.';
            this.isLoading = false;
            this.cdr.detectChanges();
          });
        },
      });
  }
}
