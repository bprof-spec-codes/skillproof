import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ProfileViewDto } from '../Models/Dtos/User/profile-view-dto';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { UserTestsDto } from '../Models/Dtos/User/userTests-dto';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  private _currentProfile$ = new BehaviorSubject<ProfileViewDto | null>(null);
  public currentProfile$ = this._currentProfile$.asObservable();
  private _currentProfileTests$ = new BehaviorSubject<UserTestsDto[] | null>(null);
  public currentProfileTests$ = this._currentProfileTests$.asObservable();

  constructor(private http: HttpClient) {}

  getProfile(userId: string): Observable<ProfileViewDto> {
    return this.http.get<ProfileViewDto>(`${environment.apiUrls.getProfile}/${userId}`);
  }

  loadProfile(userId: string): void {
    this.getProfile(userId).subscribe({
      next: (profile) => {
        console.log('PROFILE LOADED:', profile);
        this._currentProfile$.next(profile);
      },
      error: (err) => {
        console.error(err);
        this._currentProfile$.next(null);
      },
    });
  }

  clearProfile() {
    this._currentProfile$.next(null);
  }

  addSkillToUser(id: string, skillId: string): Observable<any> {
    const token = localStorage.getItem('skillProof_token');
    
    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    const baseUrl = environment.apiUrl;

    return this.http.post<any>(
      `${baseUrl}/User/${id}/skills/${skillId}`, 
      {},
      { headers }
    );
  }

  toggleSavedJob(jobId: string): Observable<ProfileViewDto> {
    const token = localStorage.getItem('skillProof_token');

    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    const baseUrl = environment.apiUrl;

    return this.http
      .post<ProfileViewDto>(`${baseUrl}/User/toggle-saved-job/${jobId}`, {}, { headers })
      .pipe(
        tap((updatedProfile) => {
          this._currentProfile$.next(updatedProfile);
        }),
      );
  }

  applyToJob(jobId: string): Observable<ProfileViewDto> {
    const token = localStorage.getItem('skillProof_token');

    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    const baseUrl = environment.apiUrl;

    return this.http.post<ProfileViewDto>(`${baseUrl}/User/apply/${jobId}`, {}, { headers }).pipe(
      tap((updatedProfile) => {
        this._currentProfile$.next(updatedProfile);
      }),
    );
  }

  /*loadUserTests(userId: string): void {
   this.http.get<UserTestsDto[]>(`${environment.apiUrls.getProfileTests}/${userId}`)
   .subscribe({
     next: (tests) => {
       console.log("Tests loaded in successfuly", tests);
       this._currentProfileTests$.next(tests);
     },
     error: (err) => {
       console.error(err);
       this._currentProfileTests$.next(null);
     }
   });
 }*/
}
