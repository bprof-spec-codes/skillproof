import { Component, OnInit } from '@angular/core';
import { JobService } from '../services/job-service';
import { JobViewDto } from '../Models/Dtos/Job/JobView-dto';
import levenshtein from 'fast-levenshtein';
import { BehaviorSubject, Observable, take } from 'rxjs';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-home-page',
  standalone: false,
  templateUrl: './home-page.html',
  styleUrl: './home-page.scss',
})
export class HomePage implements OnInit {
  freeTextControl = new FormControl('');
  locationControl = new FormControl('');

  filteredJobs = new BehaviorSubject<JobViewDto[]>([]);
  filteredJobs$: Observable<JobViewDto[]> = this.filteredJobs.asObservable();

  constructor(public jobService: JobService) {}

  ngOnInit() {
    this.jobService.jobs$.subscribe((allJobs) => {
      this.filteredJobs.next(allJobs);
    });
  }

  search() {
    this.jobService.jobs$.pipe(take(1)).subscribe((allJobs) => {
      const fText = this.freeTextControl.value?.toLowerCase().trim() || '';
      const lText = this.locationControl.value?.toLowerCase().trim() || '';

      if (!fText && !lText) {
        this.filteredJobs.next(allJobs);
        return;
      }

      let threshold = 0;
      const fLength = fText.length;

      if (fLength > 3 && fLength < 10) threshold = 1;
      else if (fLength >= 10 && fLength < 16) threshold = 2;
      else if (fLength >= 16) threshold = 3;

      const results = allJobs
        .map((job) => {
          let score = 0;
          let titleMatched = false;

          if (lText && job.location) {
            const dist = levenshtein.get(lText, job.location.toLowerCase());
            if (dist <= threshold) {
              score += (threshold + 1 - dist) * 2;
            }
          }

          if (fText) {
            const titleDist = levenshtein.get(fText, job.title.toLowerCase());
            if (titleDist <= threshold) {
              score += (threshold + 1 - titleDist) * 3;
              titleMatched = true;
            }
            job.tags?.forEach((tag) => {
              const tagDist = levenshtein.get(fText, tag.toLowerCase());
              if (tagDist <= threshold) {
                score += threshold + 1 - tagDist;
              }
            });
          }

          return { job, score, titleMatched };
        })
        .filter((item) => {
          if (fText && lText) {
            return item.score >= 2 && item.titleMatched;
          }
          if (fText) {
            return item.score >= 2 && item.titleMatched;
          }
          if (lText) {
            return item.score >= 1;
          }
          return true;
        })
        .sort((a, b) => b.score - a.score)
        .map((item) => item.job);

      this.filteredJobs.next(results);
    });
  }
}
