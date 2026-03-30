import { Component, OnInit, signal } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';
import { AuthService } from './services/auth-service';
import { ProfileService } from './services/profile-service';
import { Header } from './header/header';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrl: './app.scss'
})
export class App implements OnInit {
  protected readonly title = signal('web');

  showHeader = true;

   constructor(private router: Router, private authService:AuthService, private profileService:ProfileService) {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {

        const hiddenRoutes = ['/login'];

        this.showHeader = !hiddenRoutes.includes(event.urlAfterRedirects);
      });
  }


  ngOnInit(): void {
    
    if (this.authService.isLoggedIn()) {
      const userId = this.authService.getUserId();

      if (userId) {
        this.profileService.loadProfile(userId);
      }
    }
  }

  

}
