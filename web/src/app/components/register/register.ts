import { Component, OnInit } from '@angular/core';
import { RegisterService } from '../../services/register-service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register implements OnInit {
  isEmployer = false;

  email = '';
  firstName = '';
  lastName = '';
  password = '';
  confirmPassword = '';

  companyName = '';
  companyDescription = '';
  companyWebsite = '';

  loading = false;
  error: string | null = null;
  success: string | null = null;

  constructor(
    private service: RegisterService,
    private router: Router,
    private route: ActivatedRoute,
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      if (params['role'] === 'employer') {
        this.isEmployer = true;
      }
    });
  }

  toggleRole(role: string) {
    this.isEmployer = role === 'employer';
    this.error = null;
  }

  onRegister() {
    this.error = null;
    this.success = null;

    if (!this.email || !this.password || !this.confirmPassword) {
      this.error = 'Email and passwords are required.';
      return;
    }

    if (this.isEmployer) {
      if (!this.companyName || !this.companyDescription) {
        this.error = 'Company name and description are required.';
        return;
      }
    } else {
      if (!this.firstName || !this.lastName) {
        this.error = 'First and last names are required.';
        return;
      }
    }

    if (this.password !== this.confirmPassword) {
      this.error = 'Passwords do not match.';
      return;
    }

    this.loading = true;

    const dto: any = {
      email: this.email,
      firstName: this.isEmployer ? this.companyName : this.firstName,
      lastName: this.isEmployer ? 'Account' : this.lastName,
      password: this.password,
      companyName: this.isEmployer ? this.companyName : null,
      companyDescription: this.isEmployer ? this.companyDescription : null,
      companyWebsite: this.isEmployer ? this.companyWebsite : null,
      role: this.isEmployer ? 'Employer' : 'User',
    };

    this.service.register(dto).subscribe({
      next: () => {
        this.loading = false;
        this.success = 'Registration successful!';
        this.clearForm();
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 3000);
      },
      error: (err: any) => {
        this.loading = false;
        this.error = err?.error?.message || 'Registration failed.';
      },
    });
  }

  clearForm() {
    this.email = '';
    this.firstName = '';
    this.lastName = '';
    this.password = '';
    this.confirmPassword = '';
    this.companyName = '';
    this.companyDescription = '';
    this.companyWebsite = '';
  }
}
