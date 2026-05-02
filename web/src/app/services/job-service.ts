import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable, tap } from 'rxjs';
import { JobViewDto } from '../Models/Dtos/Job/JobView-dto';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { JobCreateDto } from '../Models/Dtos/Job/JobCreate-dto';
import { Job } from '../Models/Dtos/Job/job';

@Injectable({
  providedIn: 'root',
})
export class JobService {
  apiUrl = `${environment.apiUrl}/Jobs`;
  jobs = new BehaviorSubject<JobViewDto[]>([]);
  jobs$ = this.jobs.asObservable();
  private _currentCompanyJobs$ = new BehaviorSubject<JobViewDto[] | null>(null);
  public currentCompanyJobs$ = this._currentCompanyJobs$.asObservable();

  constructor(private http: HttpClient) {
    this.getAllJobs();
  }

  getAllJobs(): void {
    this.http
      .get<Job[]>(this.apiUrl)
      .pipe(
        map((result) => {
          return result.map((job) => ({
            ...job,
            tags: this.normalizeTags(job.tags),
          })) as JobViewDto[];
        }),
      )
      .subscribe({
        next: (jobs) => {
          this.jobs.next(jobs);
        },
        error: () => {
          this.jobs.next([]);
        },
      });
  }

  getJobsByCompanyId(companyId: string): Observable<JobViewDto[]> {
    return this.http.get<Job[]>(`${this.apiUrl}/company/${companyId}`).pipe(
      map((result) => {
        return result.map((job) => ({
          ...job,
          tags: this.normalizeTags(job.tags),
        })) as JobViewDto[];
      }),
    );
  }

  createJobs(dto: JobCreateDto): Observable<JobViewDto> {
    const payload = {
      ...dto,
      tags: JSON.stringify(dto.tags),
    };

    const token = localStorage.getItem('skillProof_token');

    if (!token) {
      throw new Error('Authentication token is missing.');
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.post<JobViewDto>(this.apiUrl, payload, { headers }).pipe(
      tap((job) => {
        this.jobs.next([...this.jobs.value, job]);
      }),
    );
  }

  getJobById(id: string): Observable<JobViewDto> {
    return this.http.get<Job>(`${this.apiUrl}/${id}`).pipe(
      map((job) => {
        return { ...job, tags: this.normalizeTags(job.tags) } as JobViewDto;
      }),
    );
  }

  deleteJob(id: string, companyId: string): void {
    const token = localStorage.getItem('skillProof_token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.http
      .delete(`${this.apiUrl}/${id}`, {
        headers: headers,
        params: { companyId: companyId },
      })
      .subscribe({
        next: () => {
          const jobsArray = this.jobs.value.filter((j) => j.id !== id);
          this.jobs.next(jobsArray);
        },
        error: (err) => {
          console.error('Delete failed.', err);
        },
      });
  }

  updateJob(id: string, dto: JobCreateDto): void {
    const payload = {
      ...dto,
      tags: JSON.stringify(dto.tags),
    };

    const token = localStorage.getItem('skillProof_token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.http.put<JobViewDto>(`${this.apiUrl}/${id}`, payload, { headers }).subscribe({
      next: (updatedJob) => {
        const jobsArray = this.jobs.value;
        const index = jobsArray.findIndex((j) => j.id === updatedJob.id);
        if (index !== -1) {
          jobsArray[index] = updatedJob;
          this.jobs.next([...jobsArray]);
        }
      },
      error: (err) => {
        console.error('Update failed.', err);
      },
    });
  }

  private normalizeTags(tags: unknown): string[] {
    if (Array.isArray(tags)) {
      return tags.map((t) => String(t).trim()).filter((t) => t.length > 0);
    }

    if (typeof tags !== 'string') {
      return [];
    }

    const trimmed = tags.trim();
    if (!trimmed) {
      return [];
    }

    if (trimmed.startsWith('[') && trimmed.endsWith(']')) {
      try {
        const parsed = JSON.parse(trimmed);
        if (Array.isArray(parsed)) {
          return parsed.map((t) => String(t).trim()).filter((t) => t.length > 0);
        }
      } catch {
      }
    }

    return trimmed
      .split(',')
      .map((t) => t.trim())
      .filter((t) => t.length > 0);
  }

  getTestUsers(jobId: string): Observable<string[]> {
    return this.http.get<string[]>(`${environment.apiUrl}/Tests/GetTestUsers`, {
      params: { jobId: jobId }
    });
  }
  
  applyForJob(jobId: string) {
    const token = localStorage.getItem('skillProof_token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.post(`${this.apiUrl}/${jobId}/apply`, {}, { headers });
  }
}
