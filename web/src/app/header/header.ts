import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthService } from '../services/auth-service';
import { ProfileViewDto } from '../Models/User/profile-view-dto';
import { ProfileService } from '../services/profile-service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header implements OnInit, OnDestroy {

  constructor(public authService:AuthService, private profileService:ProfileService){}
  

  profile: ProfileViewDto | null = null
  id: string = ''
  isLoggedIn: boolean = false;
  isAdmin: boolean = false;

  private sub!: Subscription;

  ngOnInit(): void {

    this.isLoggedIn = this.authService.isLoggedIn();

    this.sub = this.profileService.currentProfile$.subscribe({
      next: (data) => {
        this.profile = data
        console.log(this.profile)
      },
      error: (err) => {
        console.error(err)
      }
    });

     const userId = this.authService.getUserId();

    if (userId) {
      this.profileService.getCurrentProfile(userId).subscribe();
    }

    this.isAdmin = this.authService.getRoles().some(r => r === 'Admin')
  
  }

   getProfileImageSrc(): string {
    const file = this.profile?.image;
    if (!file) return 'assets/default-avatar.png'; // alapértelmezett kép
    return file.startsWith('http') ? file : `data:image/*;base64,${file}`;
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }


}
