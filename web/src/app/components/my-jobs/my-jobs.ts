import { Component, OnInit } from '@angular/core';
import { JobService } from '../../services/job-service';
import { JobViewDto } from '../../Models/Dtos/Job/JobView-dto';
import { AsyncPipe } from '@angular/common';
import { Observable, filter, switchMap, of, forkJoin, catchError, tap, map } from 'rxjs';
import { ProfileService } from '../../services/profile-service';
import { ProfileViewDto } from '../../Models/Dtos/User/profile-view-dto';
import { JobApplicationStatus } from '../../Models/Enums/Status';
import { JobApplicationStatusDto } from '../../Models/Dtos/Job/job-application-status-dto';

interface ApplicantVm extends ProfileViewDto {
  jobApplicationStatus: JobApplicationStatus;
}

interface JobCardVm extends JobViewDto {
  applicants?: ApplicantVm[];
  isLoadingApplicants?: boolean;
  hasLoadedApplicants?: boolean;
}

@Component({
  selector: 'app-my-jobs',
  standalone: false,
  templateUrl: './my-jobs.html',
  styleUrl: './my-jobs.scss',
})
export class MyJobs implements OnInit {
  jobs$!: Observable<JobCardVm[]>;
  jobsData: JobCardVm[] = [];

  constructor(
    private jobService: JobService,
    private profileService: ProfileService,
  ) {}

  ngOnInit(): void {
    this.jobs$ = this.profileService.currentProfile$.pipe(
      filter((profile): profile is ProfileViewDto => profile !== null),
      switchMap((profile) => {
        if (!profile.companyId) {
          return of([]);
        }
        return this.jobService.getJobsByCompanyId(profile.companyId);
      }),
      tap((jobs) => {
        this.jobsData = jobs;
        this.jobsData.forEach((job) => this.loadApplicants(job.id));
      }),
    );
  }

  loadApplicants(jobId: string) {
    const job = this.jobsData.find((j) => j.id === jobId);

    if (job && !job.hasLoadedApplicants && !job.isLoadingApplicants) {
      job.isLoadingApplicants = true;

      this.jobService
        .getTestUsers(jobId)
        .pipe(
          switchMap((applications: JobApplicationStatusDto[]) => {
            if (!applications || applications.length === 0) {
              return of([]);
            }

            const profileRequests = applications.map((application) =>
              this.profileService.getProfile(application.userId).pipe(
                map((profile) => {
                  if (!profile) {
                    return null;
                  }
                  return {
                    ...profile,
                    jobApplicationStatus: application.jobApplicationStatus
                  } as ApplicantVm;
                }),
                catchError(() => of(null)),
              ),
            );

            return forkJoin(profileRequests);
          }),
        )
        .subscribe({
          next: (profiles) => {
            job.applicants = profiles.filter(
              (p): p is ApplicantVm => p !== null,
            );

            job.isLoadingApplicants = false;
            job.hasLoadedApplicants = true;
          },
          error: () => {
            job.applicants = [];
            job.isLoadingApplicants = false;
            job.hasLoadedApplicants = true;
          },
        });
    }
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

  getJobApplicationStatusLabel(status: JobApplicationStatus){
    const name = status === undefined || null ? 'Unknown' : status?.toString()
    return name as string
  }
}
