import { Component, OnInit } from '@angular/core';
import { Observable, filter, switchMap, of } from 'rxjs';
import { JobViewDto } from '../../Models/Dtos/Job/JobView-dto';
import { JobService } from '../../services/job-service';
import { ProfileViewDto } from '../../Models/Dtos/User/profile-view-dto';
import { ProfileService } from '../../services/profile-service';

@Component({
  selector: 'app-company-home',
  standalone: false,
  templateUrl: './company-home.html',
  styleUrl: './company-home.scss',
})
export class CompanyHome implements OnInit {
  profile$!: Observable<ProfileViewDto | null>;
  companyJobs$!: Observable<JobViewDto[]>;
  selectedJob: JobViewDto | null = null;

  constructor(
    private jobService: JobService,
    private profileService: ProfileService,
  ) {}

  ngOnInit(): void {
    this.profile$ = this.profileService.currentProfile$;

    this.companyJobs$ = this.profile$.pipe(
      filter((profile): profile is ProfileViewDto => profile !== null),
      switchMap((profile) => {
        if (!profile.companyId) {
          return of([]);
        }
        return this.jobService.getJobsByCompanyId(profile.companyId);
      }),
    );
    if(!this.selectedJob) {
      this.companyJobs$.subscribe(jobs => { this.selectedJob = jobs[0] ?? null})
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

  selectJob(job: JobViewDto): void {
    this.selectedJob = job;
  }
}
