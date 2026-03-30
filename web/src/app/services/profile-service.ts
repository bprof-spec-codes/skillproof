import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ProfileViewDto } from '../Models/User/profile-view-dto';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { AuthService } from './auth-service';

@Injectable({
  providedIn: 'root',
})
export class ProfileService {

  private _currentProfile$ = new BehaviorSubject<ProfileViewDto | null>(null);
  public currentProfile$ = this._currentProfile$.asObservable();

  constructor(
    private http: HttpClient,
  ) {}

  loadProfile(userId: string): void {
    this.http
      .get<ProfileViewDto>(`${environment.apiUrls.getProfile}/${userId}`)
      .subscribe({
        next: (profile) => {
          console.log("PROFILE LOADED:", profile);
          this._currentProfile$.next(profile);
        },
        error: (err) => {
          console.error(err);
          this._currentProfile$.next(null);
        }
      });
  }

  clearProfile() {
    this._currentProfile$.next(null);
  }

}
