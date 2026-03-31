import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../../services/profile-service';
import { AuthService } from '../../services/auth-service';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { environment } from '../../../environments/environment.development';
import { Router } from '@angular/router';

@Component({
  selector: 'app-edit-profile',
  standalone: false,
  templateUrl: './edit-profile.html',
  styleUrl: './edit-profile.scss',
})
export class EditProfile implements OnInit {

  constructor(
    private fb: FormBuilder,
    private profileService: ProfileService,
    private authService: AuthService,
    private http: HttpClient,
    private router:Router
  ){}

  form!: FormGroup;
  selectedImageBase64: string | null = null;

  ngOnInit(): void {
    this.form = this.fb.group({
      email: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      headline: [''],
      bio: ['']
    });

    this.profileService.currentProfile$.subscribe(profile => {
      if (profile) {
        this.form.patchValue({
          email: profile.email,
          firstName: profile.fullName?.split(' ')[0] || '',
          lastName: profile.fullName?.split(' ')[1] || '',
          headline: profile.headline,
          bio: profile.bio
        });
      }
    });
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (!file) return;

    if (!file.type.includes('jpeg') && !file.type.includes('png')) {
      alert('Only JPG/PNG allowed');
      return;
    }

    const reader = new FileReader();

    reader.onload = () => {
      const base64 = reader.result as string;

      this.selectedImageBase64 = base64.split(',')[1];
    };

    reader.readAsDataURL(file);
  }

  onSubmit() {
    if (this.form.invalid) return;

    const userId = this.authService.getUserId();
    if (!userId) {
      return;
    }

    const dto: {
      email: string;
      firstName: string;
      lastName: string;
      headLine: string;
      bio: string;
      profilePicture?: string;
    } = {
      email: this.form.value.email,
      firstName: this.form.value.firstName,
      lastName: this.form.value.lastName,
      headLine: this.form.value.headline,
      bio: this.form.value.bio,
    };

    if (this.selectedImageBase64 && this.selectedImageBase64.trim() !== '') {
      dto.profilePicture = this.selectedImageBase64;
    }

    this.http.put(`${environment.apiUrls.updateUser}/${userId}`, dto)
      .subscribe({
        next: () => {
          this.profileService.loadProfile(userId);
          this.router.navigate(['/viewProfile']);
        },
        error: (err) => {
          console.error(err);
        }
      });
  }



}


