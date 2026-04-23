import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class AssessmentService {
  private apiUrl = `${environment.apiUrl}/Assessments`;

  private assessmentsSubject = new BehaviorSubject<any[]>([]);
  assessments$ = this.assessmentsSubject.asObservable();

  constructor(private http: HttpClient) {}

  getAllAssessments(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl).pipe(
      tap((assessments) => {
        this.assessmentsSubject.next(assessments);
      }),
    );
  }

  getAssessmentById(id: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  createAssessment(dto: any): Observable<any> {
    const token = localStorage.getItem('skillProof_token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.post<any>(this.apiUrl, dto, { headers }).pipe(
      tap((created) => {
        this.assessmentsSubject.next([...this.assessmentsSubject.value, created]);
      }),
    );
  }

  updateAssessment(id: string, dto: any): Observable<any> {
    const token = localStorage.getItem('skillProof_token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.put<any>(`${this.apiUrl}/${id}`, dto, { headers }).pipe(
      tap((updated) => {
        const current = this.assessmentsSubject.value;
        const index = current.findIndex((a) => a.id === id);
        if (index !== -1) {
          current[index] = updated;
          this.assessmentsSubject.next([...current]);
        }
      }),
    );
  }

  deleteAssessment(id: string): Observable<any> {
    const token = localStorage.getItem('skillProof_token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.delete<any>(`${this.apiUrl}/${id}`, { headers }).pipe(
      tap(() => {
        const filtered = this.assessmentsSubject.value.filter((a) => a.id !== id);
        this.assessmentsSubject.next(filtered);
      }),
    );
  }
}
