import { ChangeDetectorRef, Component, NgZone, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription, take } from 'rxjs';
import { JobViewDto } from '../../Models/Dtos/Job/JobView-dto';
import { EmploymentType } from '../../Models/Enums/EmploymentType';
import { AuthService } from '../../services/auth-service';
import { JobService } from '../../services/job-service';
import { ModalService } from '../../services/modal-service';
import { ProfileService } from '../../services/profile-service';

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
  isSaved = false;
  hasApplied = false;

  private routeSubscription: Subscription | null = null;
  private loadSubscription: Subscription | null = null;
  private profileSub: Subscription | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private jobService: JobService,
    private modalService: ModalService,
    private authService: AuthService,
    private profileService: ProfileService,
    private ngZone: NgZone,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap.subscribe((params) => {
      const id = params.get('id');
      if (!id) {
        this.handleError('Invalid job id.');
        return;
      }
      this.loadJob(id);
    });

    this.profileSub = this.profileService.currentProfile$.subscribe((profile) => {
      this.updateUIStatus(profile);
    });
    this.checkUserJobStatus()
  }

  private updateUIStatus(profile: any | null): void {
    if (profile && this.job) {
      this.isSaved = profile.savedJobIds?.includes(this.job.id) || false;

      const appliedIds = profile.appliedJobIds || [];
      this.hasApplied = appliedIds.includes(this.job.id);

      this.cdr.detectChanges();
    }
  }

  private handleError(msg: string): void {
    this.ngZone.run(() => {
      this.isLoading = false;
      this.job = null;
      this.errorMessage = msg;
      this.cdr.detectChanges();
    });
  }

  ngOnDestroy(): void {
    this.routeSubscription?.unsubscribe();
    this.loadSubscription?.unsubscribe();
    this.profileSub?.unsubscribe();
  }

  checkUserJobStatus(): void {
    if (!this.job) return;

    this.profileService.currentProfile$.pipe(take(1)).subscribe((profile) => {
      if (profile) {
        if (profile.savedJobIds) {
          this.isSaved = profile.savedJobIds.includes(this.job!.id);
        }

        if ((profile as any).appliedJobIds) {
          this.hasApplied = (profile as any).appliedJobIds.includes(this.job!.id);
        }

        this.cdr.detectChanges();
      }
    });
  }

  toggleSave(): void {
    if (!this.job) return;

    if (!this.authService.isLoggedIn()) {
      this.modalService.open({
        type: 'warning',
        message: 'Please log in to save jobs.',
      });
      return;
    }

    const originalState = this.isSaved;
    this.isSaved = !this.isSaved;

    this.profileService.toggleSavedJob(this.job.id).subscribe({
      next: () => {},
      error: () => {
        this.isSaved = originalState;
        this.modalService.open({
          type: 'error',
          message: 'Failed to update saved jobs.',
        });
      },
    });
  }

  applyNow(): void {
    if (this.hasApplied) {
      this.modalService.open({
        type: 'info',
        message: 'You have already applied for this job.',
      });
      return;
    }

    if (!this.authService.isLoggedIn()) {
      this.modalService.open({
        type: 'warning',
        message: 'Please log in as a Job Seeker before applying.',
      });
      return;
    }

    const roles = this.authService.getRoles().map((r) => r.toLowerCase());
    if (roles.includes('employer') || roles.includes('admin')) {
      this.modalService.open({
        type: 'warning',
        message: 'Only Job Seeker accounts can submit an application.',
      });
      return;
    }

    if (!this.job?.id) return;

    const hasTest = this.job.assessments && this.job.assessments.length > 0;

    if (hasTest) {
      this.router.navigate(['/job', this.job.id, 'test']);
    } else {
      this.profileService.applyToJob(this.job.id).subscribe({
        next: () => {
          this.modalService.open({
            type: 'success',
            message: 'Application submitted successfully!',
            autoClose: true,
          });

          const userId = this.authService.getUserId();
          if (userId) {
            this.profileService.loadProfile(userId);
          }
          this.hasApplied = true
        },
        error: () => {
          this.modalService.open({
            type: 'error',
            message: 'Failed to submit application.',
          });
        },
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
    const formattedDateStr =
      dateStr.includes('Z') || dateStr.includes('+') ? dateStr : `${dateStr.replace(' ', 'T')}Z`;
    const date = new Date(formattedDateStr);
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
    const name = type === undefined || null ? 'Unknown' : type?.toString()
    return name as string
    /*
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
    */
  }

  private loadJob(id: string): void {
    this.loadSubscription?.unsubscribe();
    this.isLoading = true;
    this.errorMessage = null;
    this.job = null;

    this.loadSubscription = this.jobService.getJobById(id).subscribe({
      next: (job) => {
        this.ngZone.run(() => {
          this.job = this.parseJobTags(job);
          this.checkUserJobStatus();
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

  private parseJobTags(job: JobViewDto): JobViewDto {
    const parsedJob = { ...job }

    if (typeof parsedJob.tags === 'string') {
      const tagString = parsedJob.tags as unknown as string

      parsedJob.tags = tagString
        .split(',')
        .map((t) =>
          t
            .trim()
            .replace(/[\[\]"']/g, '')
        )
        .filter((t) => t !== '')
    }

    if (Array.isArray(parsedJob.tags)) {
      parsedJob.tags = parsedJob.tags.map((t) =>
        typeof t === 'string'
          ? t.replace(/[\[\]"']/g, '').trim()
          : t
      );
    }

    if (!Array.isArray(parsedJob.tags)) {
      parsedJob.tags = [];
    }

    return parsedJob
  }
}
