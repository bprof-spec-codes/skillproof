import { Injectable } from '@angular/core';
import { SkillCreateDto } from '../Models/Dtos/Skill/skill-create-dto';
import { SkillViewDto } from '../Models/Dtos/Skill/skill-view-dto';
import { environment } from '../../environments/environment.development';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';


@Injectable({
  providedIn: 'root',
})
export class SkillService {
  private apiUrl = `${environment.apiUrl}/Skill`;

  private skillsSubject = new BehaviorSubject<SkillViewDto[]>([]);
  skills$ = this.skillsSubject.asObservable();

  constructor(private http: HttpClient) {}

  getAllSkills(): Observable<SkillViewDto[]> {
    return this.http.get<SkillViewDto[]>(this.apiUrl).pipe(
      tap((skills) => {
        this.skillsSubject.next(skills);
      })
    );
  }

  getSkillById(id: string): Observable<SkillViewDto> {
    return this.http.get<SkillViewDto>(`${this.apiUrl}/${id}`);
  }

  createSkill(model: SkillCreateDto): Observable<SkillViewDto> {
    const token = localStorage.getItem('skillProof_token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.post<SkillViewDto>(this.apiUrl, model, { headers }).pipe(
      tap((createdSkill) => {
        this.skillsSubject.next([...this.skillsSubject.value, createdSkill]);
      })
    );
  }
}
