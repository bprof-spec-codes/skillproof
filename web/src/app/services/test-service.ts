import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { CandidateAssessmentDto } from '../Models/Dtos/Test/candidate-assessment-dto';
import { TestResultDto } from '../Models/Dtos/Test/test-result-dto';
import { TestSubmitDto } from '../Models/Dtos/Test/test-submit-dto';

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
      .pipe(map((response) => (response.status === 204 ? null : response.body)));
  }

  submitTest(dto: TestSubmitDto): Observable<TestResultDto> {
    return this.http.post<TestResultDto>(`${this.testsUrl}/submit`, dto);
  }
}
