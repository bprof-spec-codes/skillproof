import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../../services/profile-service';
import { JobService } from '../../services/job-service';
import { Observable, filter, switchMap, of, combineLatest, map } from 'rxjs';
import { JobViewDto } from '../../Models/Dtos/Job/JobView-dto';
import { UserTestsDto } from '../../Models/Dtos/User/userTests-dto';
import { ProfileViewDto } from '../../Models/Dtos/User/profile-view-dto';
import { BadgeService } from '../../services/badgeservice';
import { EducationService } from '../../services/education-service';
import { ExperienceService } from '../../services/experience-service';
import { EducationViewDto } from '../../Models/Dtos/Education/EducationViewDto';
import { ExperienceViewDto } from '../../Models/Dtos/Experience/ExperienceViewDto';
import { AuthService } from '../../services/auth-service';

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

  educations$!: Observable<EducationViewDto[]>;
  experiences$!: Observable<ExperienceViewDto[]>;

  constructor(
    private profileService: ProfileService,
    private jobService: JobService,
    public badgeService: BadgeService,
    private educationService: EducationService,
    private experienceService: ExperienceService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.profile$ = this.profileService.currentProfile$;
    this.profileTests$ = this.profileService.currentProfileTests$;

    this.educations$ = this.profile$.pipe(
      filter((profile): profile is ProfileViewDto => profile !== null),
      switchMap(() => {
        const userId = this.authService.getUserId();
        if (!userId) return of([]);
        return this.educationService.getEducationsByUserId(userId);
      })
    );

    this.experiences$ = this.profile$.pipe(
      filter((profile): profile is ProfileViewDto => profile !== null),
      switchMap(() => {
        const userId = this.authService.getUserId();
        if (!userId) return of([]);
        return this.experienceService.getExperiencesByUserId(userId);
      })
    );

    this.companyJobs$ = combineLatest([this.profile$, this.jobService.jobs$]).pipe(
      map(([profile, allJobs]) => {
        if (!profile?.companyId) return [];
        return allJobs.filter((job) => job.companyId === profile.companyId);
      })
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
    this.jobService.deleteJob(jobId, companyId);
  }

  removeSavedJob(jobId: string, event: MouseEvent): void {
    event.stopPropagation();
    this.profileService.toggleSavedJob(jobId).subscribe();
  }
}