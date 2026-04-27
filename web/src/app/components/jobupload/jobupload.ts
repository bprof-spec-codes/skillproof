import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { JobService } from '../../services/job-service';
import { MarkdownService } from '../../services/markdown-service';
import { BehaviorSubject, Observable } from 'rxjs';
import { DashboardRoutingService } from '../../services/dashboardRouting';

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
  shortDescription: string = "";

  preViewmd: BehaviorSubject<string> = new BehaviorSubject<string>('')
  isPreView: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false)
  preViewmd$: Observable<string> = this.preViewmd.asObservable();
  isPreView$: Observable<boolean> = this.isPreView.asObservable();

  error: string | null = null;
  loading: boolean = false;

  constructor(
    private jobService: JobService,
    private router: Router,
    private markdownService: MarkdownService,
    private dashboardRoutingService: DashboardRoutingService
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
      shortDescription: this.shortDescription,
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

  preView(): void {
    if(this.description.trim().length == 0)
      return
    this.markdownService.preView(this.description).subscribe(res => {
      console.log(res)
      this.preViewmd.next(res)
      this.isPreView.next(!this.isPreView.value)
      console.log(this.preViewmd.value)
    }
    )
  }

  goHome(){
    this.dashboardRoutingService.routeToDashboard();
  }
}
