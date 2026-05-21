import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { JobService } from '../../services/job-service';
import { JobViewDto } from '../../Models/Dtos/Job/JobView-dto';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';

@Component({
  selector: 'app-full-job-view',
  standalone: false,
  templateUrl: './full-job-view.html',
  styleUrl: './full-job-view.scss',
})
export class FullJobView implements OnInit {
  job: JobViewDto | null = null;
  parsedMarkdown: SafeHtml = '';
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private jobService: JobService,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit(): void {
  this.route.paramMap.subscribe(params => {
    this.loading = true;
    const jobId = params.get('id');
    if (!jobId) {
      this.job = null;
      this.parsedMarkdown = '';
      this.loading = false;
      return;
    }

    const cached = this.jobService.jobs.value.find(j => j.id === jobId);
    if (cached) {
      this.job = cached;
      this.parsedMarkdown = this.sanitizer.bypassSecurityTrustHtml(this.job.description || '');
      this.loading = false;
      return;
    }

    this.jobService.getJobById(jobId).pipe(
      catchError(err => {
        console.error('getJobById error', err);
        return of(null);
      }),
      finalize(() => {
        this.loading = false;
      })
    ).subscribe(res => {
      if (res) {
        this.job = res;
        this.parsedMarkdown = this.sanitizer.bypassSecurityTrustHtml(this.job.description || '');
      } else {
        this.job = null;
        this.parsedMarkdown = '';
      }
    });
  });
}

  getTimeAgo(dateStr: string): string {
    const defaultDateStr = dateStr.includes('Z') ? dateStr : `${dateStr.replace(' ', 'T')}Z`;
    const diff = Math.floor((new Date().getTime() - new Date(defaultDateStr).getTime()) / 1000);
    if (diff < 60) return `Just now`;
    if (diff < 3600) return `${Math.floor(diff / 60)} minutes ago`;
    if (diff < 86400) return `${Math.floor(diff / 3600)} hours ago`;
    return `${Math.floor(diff / 86400)} days ago`;
  }
}