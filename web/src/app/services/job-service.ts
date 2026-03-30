import { Injectable } from '@angular/core';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { JobViewDto } from '../Models/Dtos/Job/JobView-dto';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment.development';
import { JobCreateDto } from '../Models/Dtos/Job/JobCreate-dto';
import { Job } from '../Models/Dtos/Job/job';

@Injectable({
  providedIn: 'root',
})
export class JobService {

  apiUrl = `${environment.apiUrl}/Jobs`
  jobs = new BehaviorSubject<JobViewDto[]>([])
  jobs$ = this.jobs.asObservable()

  constructor(private http: HttpClient) {this.getAllJobs()}

  getAllJobs(): void {
    this.http.get<Job[]>(this.apiUrl).pipe(
      map(result => {
        return result.map(job => ({
          ...job,
          tags: JSON.parse(job.tags)
        })) as JobViewDto[]
      })
    ).subscribe(jobs => {
      this.jobs.next(jobs)
    })
  }

  createJobs(dto: JobCreateDto): void {
    const payload = {
      ...dto,
      // A tömböt visszalakítjuk stringgé
      tags: JSON.stringify(dto.tags) 
    }

    this.http.post<JobViewDto>(this.apiUrl, payload).subscribe({
      next: (job) =>{
        this.jobs.next([...this.jobs.value, job])
      },
      error: (err) => {
          // majd a modal servicet meg kell hívni a hibaüzenettel
        }
    })
  }

  getJobById(id: string): Observable<JobViewDto> {
    return this.http.get<Job>(`${this.apiUrl}/${id}`).pipe(
      map(job => {
        return {...job, tags: JSON.parse(job.tags)} as JobViewDto   
      })
    )
  }

  deleteJob(id: string, companyId: string): void {
    this.http.delete(`${this.apiUrl}/${id}`, {params: { companyId: companyId }})
  }

  updateJob(id: string, dto: JobCreateDto): void{
    const payload = {
      ...dto,
      // A tömböt visszalakítjuk stringgé
      tags: JSON.stringify(dto.tags) 
    }

    this.http.put<JobViewDto>(`${this.apiUrl}/${id}`, payload).subscribe({
    next: updatedJob => {
      const jobsArray = this.jobs.value;
      const index = jobsArray.findIndex(j => j.id === updatedJob.id);
      if (index !== -1) {
        jobsArray[index] = updatedJob;
        this.jobs.next([...jobsArray]);
      }
    },
    error: err => {
      // majd a modal servicet meg kell hívni a hibaüzenettel
    }
  });
  }
}
