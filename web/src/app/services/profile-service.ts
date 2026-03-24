import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ProfileViewDto } from '../Models/User/profile-view-dto';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {

  private _currentProfile$ = new BehaviorSubject<ProfileViewDto | null>(null);
  public currentProfile$ = this._currentProfile$.asObservable();

  constructor(private http:HttpClient){}

  getCurrentProfile(id: string): Observable<ProfileViewDto> {
    return this.http.get<ProfileViewDto>(`${environment.apiUrls.getProfile}/${id}`).pipe(
      tap(profile => this._currentProfile$.next(profile))
    );
  }

}
