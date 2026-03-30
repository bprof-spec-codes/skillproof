import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { QuestionListFilterDto } from '../Models/Dtos/Question/question-list-filter-dto';
import { QuestionResponseDto } from '../Models/Dtos/Question/question-response-dto';
import { CreateQuestionRequestDto } from '../Models/Dtos/Question/create-question-request-dto';
import { UpdateQuestionRequestDto } from '../Models/Dtos/Question/update-question-request-dto';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class QuestionBankService {
  private apiUrl = `${environment.apiUrl}/question-bank`;

  private questionsSubject = new BehaviorSubject<QuestionResponseDto[]>([]);
  questions$ = this.questionsSubject.asObservable();

  constructor(private http: HttpClient) {}

  getAll(filter?: QuestionListFilterDto): Observable<QuestionResponseDto[]> {
    const params = this.buildFilterParams(filter);

    return this.http.get<QuestionResponseDto[]>(this.apiUrl, { params }).pipe(
      tap((questions) => {
        this.questionsSubject.next(questions);
      })
    );
  }

  getById(id: string): Observable<QuestionResponseDto> {
    return this.http.get<QuestionResponseDto>(`${this.apiUrl}/${id}`);
  }

  create(dto: CreateQuestionRequestDto): Observable<QuestionResponseDto> {
    return this.http.post<QuestionResponseDto>(this.apiUrl, dto).pipe(
      tap((created) => {
        this.questionsSubject.next([created, ...this.questionsSubject.value]);
      })
    );
  }

  update(id: string, dto: UpdateQuestionRequestDto): Observable<QuestionResponseDto> {
    return this.http.put<QuestionResponseDto>(`${this.apiUrl}/${id}`, dto).pipe(
      tap((updated) => {
        const next = this.questionsSubject.value.map((question) =>
          question.id === updated.id ? updated : question
        );
        this.questionsSubject.next(next);
      })
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => {
        const next = this.questionsSubject.value.filter((question) => question.id !== id);
        this.questionsSubject.next(next);
      })
    );
  }

  private buildFilterParams(filter?: QuestionListFilterDto): HttpParams {
    let params = new HttpParams();

    if (!filter) {
      return params;
    }

    if (filter.type !== undefined && filter.type !== null) {
      params = params.set('type', String(filter.type));
    }

    if (filter.difficulty !== undefined && filter.difficulty !== null) {
      params = params.set('difficulty', String(filter.difficulty));
    }

    if (filter.language && filter.language.trim() !== '') {
      params = params.set('language', filter.language.trim());
    }

    if (filter.isActive !== undefined && filter.isActive !== null) {
      params = params.set('isActive', String(filter.isActive));
    }

    return params;
  }
  
}
