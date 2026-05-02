import { Component, OnInit } from '@angular/core';
import { JobService } from '../../services/job-service';
import { JobViewDto } from '../../Models/Dtos/Job/JobView-dto';
import { AsyncPipe } from '@angular/common';
import { Observable, filter, switchMap, of, forkJoin, catchError, tap } from 'rxjs';
import { ProfileService } from '../../services/profile-service';
import { ProfileViewDto } from '../../Models/User/profile-view-dto';

interface JobCardVm extends JobViewDto {
  applicants?: ProfileViewDto[];
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
    private profileService: ProfileService
  ) { }

  ngOnInit(): void {
    this.jobs$ = this.profileService.currentProfile$.pipe(
      filter((profile): profile is ProfileViewDto => profile !== null),
      switchMap((profile) => {
        if (!profile.companyId) {
          return of([]);
        }
        return this.jobService.getJobsByCompanyId(profile.companyId);
      }),
      tap(jobs => {
        this.jobsData = jobs;
        this.jobsData.forEach(job => this.loadApplicants(job.id));
      })
    );
  }

  loadApplicants(jobId: string) {
    const job = this.jobsData.find(j => j.id === jobId);

    if (job && !job.hasLoadedApplicants && !job.isLoadingApplicants) {
      job.isLoadingApplicants = true;

      this.jobService.getTestUsers(jobId).pipe(
        switchMap((userIds: string[]) => {
          if (!userIds || userIds.length === 0) {
            return of([]);
          }

          const profileRequests = userIds.map(userId =>
            this.profileService.getProfile(userId).pipe(
              catchError(() => of(null))
            )
          );
          return forkJoin(profileRequests);
        })
      ).subscribe({
        next: (profiles) => {
          job.applicants = profiles.filter((p): p is ProfileViewDto => p !== null);
          job.isLoadingApplicants = false;
          job.hasLoadedApplicants = true;
        },
        error: () => {
          job.applicants = [];
          job.isLoadingApplicants = false;
          job.hasLoadedApplicants = true;
        }
      });
    }
  }

  getTimeAgo(dateInput: string | Date): string {
    if (!dateInput) return '';

    const pastDate = new Date(dateInput);
    const now = new Date();
    const seconds = Math.floor((now.getTime() - pastDate.getTime()) / 1000);

    if (seconds < 60) {
      return 'just now';
    }

    const minutes = Math.floor(seconds / 60);
    if (minutes < 60) {
      return ` ${minutes} minute${minutes > 1 ? 's' : ''} ago`;
    }

    const hours = Math.floor(minutes / 60);
    if (hours < 24) {
      return ` ${hours} hour${hours > 1 ? 's' : ''} ago`;
    }

    const days = Math.floor(hours / 24);
    if (days < 30) {
      return ` ${days} day${days > 1 ? 's' : ''} ago`;
    }

    const months = Math.floor(days / 30);
    if (months < 12) {
      return ` ${months} month${months > 1 ? 's' : ''} ago`;
    }

    const years = Math.floor(days / 365);
    return ` ${years} year${years > 1 ? 's' : ''} ago`;
  }
}
