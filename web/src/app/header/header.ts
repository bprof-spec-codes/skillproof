import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth-service';
import { ProfileService } from '../services/profile-service';
import { Observable, Subscription } from 'rxjs';
import { DashboardRoutingService } from '../services/dashboardRouting';
import { ProfileViewDto } from '../Models/Dtos/User/profile-view-dto';

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header implements OnInit {
  constructor(
    public authService: AuthService,
    private profileService: ProfileService,
    private dashRouteService: DashboardRoutingService,
  ) {}

  profile$!: Observable<ProfileViewDto | null>;
  isLoggedIn: boolean = false;
  isAdmin: boolean = false;
  isEmployer: boolean = false;

  private sub!: Subscription;

  ngOnInit(): void {
    this.profile$ = this.profileService.currentProfile$;
    this.isLoggedIn = this.authService.isLoggedIn();
    this.isAdmin = this.authService.getRoles().some((r) => r === 'Admin');
    this.isEmployer = this.authService.getRoles().some((r) => r === 'Employer');
  }

  goHome() {
    this.dashRouteService.routeToDashboard();
  }
}
