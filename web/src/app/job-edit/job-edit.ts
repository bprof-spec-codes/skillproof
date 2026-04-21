import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { JobService } from '../services/job-service';

@Component({
  selector: 'app-job-edit',
  standalone: false,
  templateUrl: './job-edit.html',
  styleUrl: './job-edit.scss',
})
export class JobEdit implements OnInit {
  jobId = '';
  title = '';
  location = '';
  employmentType: number | null = 0;
  salary = '';
  tags = '';
  description = '';
  error = '';
  loading = false;

  isPreViewSubject = new BehaviorSubject<boolean>(false);
  isPreView$ = this.isPreViewSubject.asObservable();
  preViewmdSubject = new BehaviorSubject<string>('');
  preViewmd = this.preViewmdSubject.asObservable();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private jobService: JobService,
  ) {}

  ngOnInit(): void {
    this.jobId = this.route.snapshot.paramMap.get('id') || '';

    if (this.jobId) {
      this.jobService.getJobById(this.jobId).subscribe({
        next: (job) => {
          this.title = job.title;
          this.location = job.location;
          this.employmentType = job.EmploymentType ?? 0;
          this.tags = job.tags ? job.tags.join(', ') : '';
          this.description = job.description;
        },
        error: () => {
          this.error = 'Failed to load job details.';
        },
      });
    }
  }

  preView(): void {
    this.isPreViewSubject.next(!this.isPreViewSubject.value);
    this.preViewmdSubject.next(this.description);
  }

  openAddTestModal(): void {
    console.log('Action triggered: Open test selection modal or navigate to test creation.');
  }

  onSubmit(): void {
    this.loading = true;
    this.error = '';

    const tagsArray = this.tags
      .split(',')
      .map((t) => t.trim())
      .filter((t) => t !== '');

    const updateDto = {
      title: this.title,
      location: this.location,
      employmentType: Number(this.employmentType),
      description: this.description,
      tags: tagsArray,
    };

    try {
      this.jobService.updateJob(this.jobId, updateDto as any);
      setTimeout(() => {
        this.router.navigate(['/profile']);
      }, 500);
    } catch (err) {
      this.error = 'An error occurred while updating the job.';
      this.loading = false;
    }
  }
}
