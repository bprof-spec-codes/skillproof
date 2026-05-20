import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { ExperienceCreateDto } from '../Models/Dtos/Experience/ExperienceCreateDto';
import { ExperienceViewDto } from '../Models/Dtos/Experience/ExperienceViewDto';
@Injectable({
  providedIn: 'root',
})
export class ExperienceService {
  private readonly apiUrl = `${environment.apiUrl}/Experience`;

  constructor(private http: HttpClient) {}

  getExperiencesByUserId(userId: string): Observable<ExperienceViewDto[]> {
    return this.http.get<ExperienceViewDto[]>(`${this.apiUrl}/${userId}`);
  }

  createExperience(userId: string, entity: ExperienceCreateDto): Observable<ExperienceCreateDto> {
    return this.http.post<ExperienceCreateDto>(`${this.apiUrl}/${userId}`, entity);
  }

  deleteExperience(userId: string, id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${userId}/${id}`);
  }
}