import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from '../services/auth-service';
import { ProfileViewDto } from '../Models/User/profile-view-dto';
import { ProfileService } from '../services/profile-service';
import { Observable, Subscription } from 'rxjs';

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
}
