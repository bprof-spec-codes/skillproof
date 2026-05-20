import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { EducationViewDto } from '../Models/Dtos/Education/EducationViewDto';
import { EducationCreateDto } from '../Models/Dtos/Education/EducationCreateDto';

@Injectable({
  providedIn: 'root',
})
export class EducationService {
  private readonly apiUrl = `${environment.apiUrl}/Education`;

  constructor(private http: HttpClient) {}

  getEducationsByUserId(userId: string): Observable<EducationViewDto[]> {
    return this.http.get<EducationViewDto[]>(`${this.apiUrl}/${userId}`);
  }

  createEducation(userId: string, entity: EducationCreateDto): Observable<EducationCreateDto> {
    return this.http.post<EducationCreateDto>(`${this.apiUrl}/${userId}`, entity);
  }

  deleteEducation(userId: string, id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${userId}/${id}`);
  }
}