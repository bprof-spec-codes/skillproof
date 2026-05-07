import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../../services/profile-service';
import { JobService } from '../../services/job-service';
import { Observable, filter, switchMap, of, combineLatest, map } from 'rxjs';
import { JobViewDto } from '../../Models/Dtos/Job/JobView-dto';
import { UserTestsDto } from '../../Models/Dtos/User/userTests-dto';
import { ProfileViewDto } from '../../Models/Dtos/User/profile-view-dto';

@Component({
  selector: 'app-profile-view',
  standalone: false,
  templateUrl: './profile-view.html',
  styleUrl: './profile-view.scss',
})
export class ProfileView implements OnInit {
  profile$!: Observable<ProfileViewDto | null>;
  companyJobs$!: Observable<JobViewDto[]>;
  profileTests$!: Observable<UserTestsDto[] | null>;

  savedJobs$!: Observable<JobViewDto[]>;

  constructor(
    private profileService: ProfileService,
    private jobService: JobService,
  ) {}

  ngOnInit(): void {
    this.profile$ = this.profileService.currentProfile$;
    this.profileTests$ = this.profileService.currentProfileTests$;

    this.companyJobs$ = this.profile$.pipe(
      filter((profile): profile is ProfileViewDto => profile !== null),
      switchMap((profile) => {
        if (!profile.companyId) {
          return of([]);
        }
        return this.jobService.getJobsByCompanyId(profile.companyId);
      }),
    );

    this.savedJobs$ = combineLatest([this.jobService.jobs$, this.profile$]).pipe(
      map(([allJobs, profile]) => {
        if (!profile || !profile.savedJobIds || profile.savedJobIds.length === 0) {
          return [];
        }
        return allJobs.filter((job) => profile.savedJobIds.includes(job.id));
      }),
    );
  }

  deleteJob(jobId: string, companyId: string): void {
    try {
      this.jobService.deleteJob(jobId, companyId);
    } catch (error) {
      console.error('Failed to delete job.', error);
    }
  }

  removeSavedJob(jobId: string, event: MouseEvent): void {
    event.stopPropagation();
    this.profileService.toggleSavedJob(jobId).subscribe();
  }
}
