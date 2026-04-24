import { Component, OnInit } from '@angular/core';
import { JobService } from '../services/job-service';
import { JobViewDto } from '../Models/Dtos/Job/JobView-dto';
import levenshtein from 'fast-levenshtein';
import { BehaviorSubject, debounceTime, Observable, Subject, take, takeUntil } from 'rxjs';
import { FormControl } from '@angular/forms';
import { EmploymentType } from '../Models/Enums/EmploymentType';
import { DifficultyLevel } from '../Models/Enums/DifficultyLevel';

@Component({
  selector: 'app-home-page',
  standalone: false,
  templateUrl: './home-page.html',
  styleUrl: './home-page.scss',
})
export class HomePage implements OnInit {
  freeTextControl = new FormControl('');
  locationControl = new FormControl('');

  isLoggedIn = false;

  filteredJobs = new BehaviorSubject<JobViewDto[]>([]);
  filteredJobs$: Observable<JobViewDto[]> = this.filteredJobs.asObservable();
  private destroy$ = new Subject<void>()
  filterType = '';
  filterExperience = ''
  filterTags = ''
  sort = ''
  readonly jobTypeOptions = [
      { label: 'FullTime', value: EmploymentType.FullTime },
      { label: 'Internship', value: EmploymentType.Internship },
      { label: 'PartTime', value: EmploymentType.PartTime },
      { label: 'Temporary', value: EmploymentType.Temporary },
  ];

  readonly jobExperience = [
      { label: 'Senior', value: DifficultyLevel.Senior },
      { label: 'Medior', value: DifficultyLevel.Medior },
      { label: 'Junior', value: DifficultyLevel.Junior },
  ];

  constructor(public jobService: JobService) {}

  ngOnInit() {
    this.isLoggedIn = !!localStorage.getItem('skillProof_token');

    this.jobService.jobs$.subscribe((allJobs) => {
      if (!this.freeTextControl.value && !this.locationControl.value && !this.filterExperience && !this.filterTags && !this.filterType && !this.sort) {
        this.filteredJobs.next(allJobs);
      }
    });

    this.freeTextControl.valueChanges.pipe(
      takeUntil(this.destroy$),
      debounceTime(200)).subscribe(() => {
        this.search()
      }
    )
    this.locationControl.valueChanges.pipe(
      takeUntil(this.destroy$),
      debounceTime(200)).subscribe(() => {
        this.search()
      }
    )
  }

  search() {
    this.jobService.jobs$.pipe(take(1)).subscribe((allJobs) => {
      const fText = this.freeTextControl.value?.toLowerCase().trim() || '';
      const lText = this.locationControl.value?.toLowerCase().trim() || '';
      if (!fText && !lText && !this.filterExperience && !this.filterTags && !this.filterType && !this.sort) {
        this.filteredJobs.next(allJobs);
        return;
      }

      let threshold = 0;
      const fLength = fText.length;

      if (fLength > 3 && fLength < 10) threshold = 1;
      else if (fLength >= 10 && fLength < 16) threshold = 2;
      else if (fLength >= 16) threshold = 3;

      let results = allJobs
        .map((job) => {
          let score = 0;
          let tagScore = 0;
          let titleScore = 0;
          let employmentTypeScore = 0;
          let titleMatched = false;
          let locationMatched = false;

          const titles = job.title.split(' ');

          if (lText && job.location) {
            const dist = levenshtein.get(lText, job.location.toLowerCase());
            if (dist <= 2) {
              titleScore += 2;
              locationMatched = true;
            }
          }

          if (fText) {
            titles.forEach(title => {
              const dist = levenshtein.get(fText, title.toLowerCase());
              if (dist <= 2) {
                titleScore += 3;
                titleMatched = true;
              }
            });

            job.tags?.forEach(tag => {
              const dist = levenshtein.get(fText, tag.toLowerCase());
              if (dist <= 0) {
                tagScore += 1;
              }
            });
          }

          if (this.filterTags.length > 0) {
            job.tags?.forEach(tag => {
              this.filterTags.trim().split(',').forEach(fTag => {
                if (tag.toLowerCase() === fTag.toLowerCase()) {
                  tagScore += 3;
                }
              });
            });
          }

          if (this.filterType.length > 1) {
            if (job.EmploymentType?.toString() === this.filterType) {
              employmentTypeScore += 3;
            }
          }
          score = employmentTypeScore + titleScore + tagScore
          return { job, score, titleMatched, locationMatched, employmentTypeScore, titleScore, tagScore };
        })
        .filter(item => {
          if (!fText && !lText) {
            return item.score > 0;
          }
          if (fText && lText) {
            return item.score >= 2 && (item.titleMatched || item.locationMatched);
          }
          if (fText) {
            return item.score >= 2 && item.titleMatched;
          }
          if (lText) {
            return item.score >= 1 && item.locationMatched;
          }
          return true;
        })
        .sort((a, b) => {
          switch (this.sort) {
            case 'title':
              return b.titleScore - a.titleScore;

            case 'tags':
              return b.tagScore - a.tagScore;

            case 'type':
              return b.employmentTypeScore - a.employmentTypeScore;

            case 'date':
              console.log(new Date(b.job.createdAt).getTime())
              return new Date(b.job.createdAt).getTime() - new Date(a.job.createdAt).getTime();

            default:
              return b.score - a.score;
          }
        })
        .map((item) => item.job);

        if(results.length === 0)
        {
          results = allJobs.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
        }

      this.filteredJobs.next(results);
    });
  }
  
  

  clearFilters(): void {
     this.filterType = '';
    this.filterExperience = '';
    this.filterTags = '';
    this.sort = '';
    this.freeTextControl.setValue('');
    this.locationControl.setValue('');
    this.search();
  }
  applyFilters(): void {
    this.search()
  }
  // Ezt loptam :)
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


  ngOnDestroy() {
    this.destroy$.next()
    this.destroy$.complete()
  }
}
