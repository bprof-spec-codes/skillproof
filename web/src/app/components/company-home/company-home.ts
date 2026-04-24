import { Component, OnInit } from '@angular/core';
import { Observable, filter, switchMap, of } from 'rxjs';
import { JobViewDto } from '../../Models/Dtos/Job/JobView-dto';
import { JobService } from '../../services/job-service';
import { ProfileViewDto } from '../../Models/User/profile-view-dto';
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
    private profileService: ProfileService
  ) { }

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

  selectJob(job: JobViewDto): void {
    this.selectedJob = job;
  }

}
