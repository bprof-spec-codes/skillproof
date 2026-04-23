import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './auth-service';
import { ProfileService } from './profile-service'; 
import { filter, take } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DashboardRoutingService {
  constructor(
    private authService: AuthService,
    private profileService: ProfileService,
    private router: Router
  ) {}

  routeToDashboard(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/home']);
      return;
    }

    const userId = this.authService.getUserId();
    if (userId && !this.getValueFromBehaviorSubject()) {
       this.profileService.loadProfile(userId);
    }

    this.profileService.currentProfile$.pipe(
      filter(profile => profile !== null),
      take(1)
    ).subscribe({
      next: (profile) => {
        if (profile?.companyId) {
          this.router.navigate(['/company']);
        } else {
          this.router.navigate(['/home']);
        }
      },
      error: (err) => {
        console.error('Failed to get profile for routing:', err);
        this.router.navigate(['/home']);
      }
    });
  }

  private getValueFromBehaviorSubject(): boolean {
      let hasData = false;
      this.profileService.currentProfile$.pipe(take(1)).subscribe(data => {
          if (data !== null) hasData = true;
      });
      return hasData;
  }
}