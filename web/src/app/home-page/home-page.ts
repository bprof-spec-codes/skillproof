import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth-service';

@Component({
  selector: 'app-home-page',
  standalone: false,
  templateUrl: './home-page.html',
  styleUrl: './home-page.scss',
})
export class HomePage implements OnInit {
  freeTextControl = new FormControl('');
  locationControl = new FormControl('');
  isLoggedIn = false;

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
    this.isLoggedIn = this.authService.isLoggedIn();
  }

  search() {
    this.router.navigate(['/search'], {
      queryParams: {
        q: this.freeTextControl.value,
        loc: this.locationControl.value
      }
    });
  }
}