import { Component } from '@angular/core';
import { RegisterService } from '../../services/register-service';
import { Router } from '@angular/router';
import { RegisterDto } from '../../Models/Dtos/User/register-dto';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  email: string = '';
  firstName: string = '';
  lastName: string = '';
  password: string = '';
  confirmPassword: string = '';

  loading = false;
  error: string | null = null;
  success: string | null = null;
  
  constructor(private service:RegisterService, private router:Router){}

  onRegister(){

    this.error = null;
    this.success = null;

    if (!this.email || !this.firstName || !this.lastName || !this.password || !this.confirmPassword) {
      this.error = 'All fields are required.';
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.error = 'Passwords do not match.';
      return;
    }

    this.loading = true;

    const dto:RegisterDto = {
      email: this.email,
      firstName: this.firstName,
      lastName: this.lastName,
      password: this.password
    }
      
    this.service.register(dto).subscribe({
      next: () => {
        this.loading = false
        this.success = "Registration successful!"
        this.clearForm()
        setTimeout(() => {
          this.router.navigate(["/login"])
        }, 3000)
      },
      error: (err: any) => {
        this.loading = false;
        this.error = err?.error.message || 'Registration failed. Please try again.';
      },
    });

  }

  clearForm() {
    this.email = '';
    this.firstName = '';
    this.lastName = '';
    this.password = '';
    this.confirmPassword ='';
  }

}
