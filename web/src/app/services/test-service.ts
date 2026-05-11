import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { CandidateAssessmentDto } from '../Models/Dtos/Test/candidate-assessment-dto';
import { TestResultDto } from '../Models/Dtos/Test/test-result-dto';
import { TestSubmitDto } from '../Models/Dtos/Test/test-submit-dto';
import { normalizeCandidateAssessment } from './open-ended-compat';
import { UserTestsDto } from '../Models/Dtos/User/userTests-dto';

@Injectable({
  providedIn: 'root',
})
export class TestService {
  private jobsUrl = `${environment.apiUrl}/Jobs`;
  private testsUrl = `${environment.apiUrl}/Tests`;

  constructor(private http: HttpClient) {}

  getCandidateTestForJob(jobId: string): Observable<CandidateAssessmentDto | null> {
    return this.http
      .get<CandidateAssessmentDto>(`${this.jobsUrl}/${jobId}/test`, { observe: 'response' })
      .pipe(map((response) => normalizeCandidateAssessment(response.status === 204 ? null : response.body)));
  }

  submitTest(dto: TestSubmitDto): Observable<TestResultDto> {
    const token = localStorage.getItem('skillProof_token');
    const headers = token
      ? new HttpHeaders().set('Authorization', `Bearer ${token}`)
      : undefined;

    return this.http.post<TestResultDto>(`${this.testsUrl}/submit`, dto, { headers });
  }

  getUserTestQuestions(userId: string, jobId: string): Observable<UserTestsDto[]> {
    return this.http.get<UserTestsDto[]>(`${this.testsUrl}/GetUserTestQuestions`, {
      params: {userId, jobId}
    });
  }

  manualFeedback(feedback: string, score: number, testAnswerId: string) {
    const body = JSON.stringify(feedback || ""); 
    
    return this.http.put(`${this.testsUrl}/ManualFeedbackAsync`, body, {
      headers: { 'Content-Type': 'application/json' },
      params: { score, testAnswerId }
    });
  }
}
