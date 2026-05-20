import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from '../services/auth-service';
import { ProfileService } from '../services/profile-service';
import { Observable, Subscription, timer, switchMap } from 'rxjs';
import { DashboardRoutingService } from '../services/dashboardRouting';
import { ProfileViewDto } from '../Models/Dtos/User/profile-view-dto';
import { JobService } from '../services/job-service';

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header implements OnInit, OnDestroy {
  constructor(
    public authService: AuthService,
    private profileService: ProfileService,
    private dashRouteService: DashboardRoutingService,
    private jobService: JobService,
  ) { }

  profile$!: Observable<ProfileViewDto | null>;
  isLoggedIn: boolean = false;
  isAdmin: boolean = false;
  isEmployer: boolean = false;
  notifications: any[] = [];
  unreadCount: number = 0;
  private pollingSubscription?: Subscription;

  private sub!: Subscription;

  ngOnInit(): void {
    this.profile$ = this.profileService.currentProfile$;
    this.isLoggedIn = this.authService.isLoggedIn();
    this.isAdmin = this.authService.getRoles().some((r) => r === 'Admin');
    this.isEmployer = this.authService.getRoles().some((r) => r === 'Employer');
    if(this.isLoggedIn)
    this.pollingSubscription = timer(0, 10000).pipe(
      switchMap(() => this.jobService.getNotifications())
    ).subscribe({
      next: (data) => {
        this.notifications = data;
        this.unreadCount = this.notifications.length;
      }
    });
  }

  ngOnDestroy() {
    if (this.pollingSubscription) {
      this.pollingSubscription.unsubscribe();
    }
  }

  markAsRead(id: string) {
    this.jobService.markNotificationAsRead(id).subscribe({
      next: () => {
        this.notifications = this.notifications.filter(n => n.id !== id);
        this.unreadCount = this.notifications.length;
      },
      error: (err) => console.error('Failed to mark as read', err)
    });
  }

  goHome() {
    this.dashRouteService.routeToDashboard();
  }
}
