import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { JobService } from '../services/job-service';

@Component({
  selector: 'app-jobupload',
  standalone: false,
  templateUrl: './jobupload.html',
  styleUrl: './jobupload.scss',
})
export class Jobupload {
  title: string = '';
  location: string = '';
  employmentType: number = 0;
  salary: string = '';
  tags: string = '';
  description: string = '';

  error: string | null = null;
  loading: boolean = false;

  constructor(
    private jobService: JobService,
    private router: Router,
  ) {}

  onSubmit() {
    this.error = null;

    if (!this.title || !this.location || !this.description) {
      this.error = 'Title, location, and description are required.';
      return;
    }

    this.loading = true;

    const tagsArray = this.tags
      .split(',')
      .map((tag) => tag.trim())
      .filter((tag) => tag.length > 0);

    const dto: any = {
      title: this.title,
      location: this.location,
      employmentType: Number(this.employmentType),
      salary: this.salary,
      description: this.description,
      tags: tagsArray,
    };

    this.jobService.createJobs(dto).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/']);
      },
      error: (err: any) => {
        this.loading = false;
        this.error = err?.error?.message || 'Failed to post the job advertisement.';
      },
    });
  }
}
