import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { LoginDto } from '../../Models/Dtos/User/login-dto';
import { AuthService } from '../../services/auth-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  loginForm!: FormGroup;
  errorMessage: string = '';
  email = '';
  password = '';
  error = '';
  loading = false;

  constructor(private fb: FormBuilder, private http: HttpClient, private authService:AuthService, private router:Router){
    this.loginForm = this.fb.group({
        email: [''],
        password: ['']
    });
  }


  onSubmit()
  {
    if (this.loginForm.invalid) return;
    
    const dto: LoginDto = {
      email: this.loginForm.value.email,
      password: this.loginForm.value.password
    };

    this.authService.login(dto).subscribe({
      next: (res: any) => {
        const token = res?.token ?? res?.Token;
        if (!token) {
          this.error = 'Invalid response from server.';
          this.loading = false;
          return;
        }
        this.authService.saveToken(token);
        this.router.navigate(['/homePage']); //majd ha lesz
      },
      error: (err) => {
        this.loading = false;
        this.error = err?.error?.message ?? err?.message ?? 'Invalid email or password.';
      }
    });


  }




}
