import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../../services/profile-service';
import { JobService } from '../../services/job-service';
import { Observable, filter, switchMap, of } from 'rxjs';
import { ProfileViewDto } from '../../Models/User/profile-view-dto';
import { JobViewDto } from '../../Models/Dtos/Job/JobView-dto';

@Component({
  selector: 'app-profile-view',
  standalone: false,
  templateUrl: './profile-view.html',
  styleUrl: './profile-view.scss',
})
export class ProfileView implements OnInit {
  profile$!: Observable<ProfileViewDto | null>;
  companyJobs$!: Observable<JobViewDto[]>;

  constructor(
    private profileService: ProfileService,
    private jobService: JobService,
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
  }

  deleteJob(jobId: string, companyId: string): void {
    try {
      this.jobService.deleteJob(jobId, companyId);
    } catch (error) {
      console.error('Failed to delete job.', error);
    }
  }
}
