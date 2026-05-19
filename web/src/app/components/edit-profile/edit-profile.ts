import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../../services/profile-service';
import { AuthService } from '../../services/auth-service';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { environment } from '../../../environments/environment.development';
import { Router } from '@angular/router';
import { SkillViewDto } from '../../Models/Dtos/Skill/skill-view-dto';
import { SkillService } from '../../services/skillservice';

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
    private router: Router,
    private skillService: SkillService
  ) { }

  form!: FormGroup;
  selectedImageBase64: string | null = null;

  userSkills: { id: string; name: string }[] = [];
  availableSkills: SkillViewDto[] = [];

  pendingSkillAdditions = new Set<string>();
  

  ngOnInit(): void {
    this.form = this.fb.group({
      email: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      headline: [''],
      bio: ['']
    });

    this.getAllSkills();

    this.profileService.currentProfile$.subscribe(profile => {
      if (profile) {
        this.form.patchValue({
          email: profile.email,
          firstName: profile.fullName?.split(' ')[0] || '',
          lastName: profile.fullName?.split(' ')[1] || '',
          headline: profile.headline,
          bio: profile.bio
        });

        if (this.userSkills.length === 0 && profile.skills) {
          this.userSkills = typeof profile.skills[0] === 'string'
            ? (profile.skills as any as string[]).map(s => ({ id: s, name: s }))
            : [...profile.skills] as any;
        }
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

  onSkillSelected(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    const selectedId = selectElement.value;

    if (!selectedId) return;

    const skillObj = this.availableSkills.find(s => s.id === selectedId);

    if (skillObj && !this.userSkills.some(s => s.id === skillObj.id)) {
      this.userSkills.push({ id: skillObj.id, name: skillObj.name });
      this.pendingSkillAdditions.add(skillObj.id);
    }

    selectElement.value = "";
  }

  removeSkill(index: number, skillId: string): void {
  if (this.pendingSkillAdditions.has(skillId)) {
    this.userSkills.splice(index, 1);
    this.pendingSkillAdditions.delete(skillId);
  } else {
    const userId = this.authService.getUserId();
    if (!userId) return;

    this.profileService.removeSkillFromUser(userId, skillId).subscribe({
      next: () => {
        this.userSkills.splice(index, 1);
        console.log('Skill removed successfully');
      },
      error: (err) => console.error(`Failed to remove skill ${skillId}`, err)
    });
  }
}

  saveSkills(): void {
    const userId = this.authService.getUserId();
    if (!userId || this.pendingSkillAdditions.size === 0) return;

    const newSkillsArray = Array.from(this.pendingSkillAdditions);
    console.log("SENDING NEW SKILLS:", newSkillsArray);

    this.profileService.addSkillsToUser(userId, newSkillsArray).subscribe({
      next: () => {
        console.log('All new skills saved');
        this.pendingSkillAdditions.clear();
        this.profileService.loadProfile(userId);
      },
      error: (err) => console.error(`Failed to assign skills`, err)
    });
  }
  getAllSkills(): void {
    this.skillService.skills$.subscribe({
      next: (skills) => {
        this.availableSkills = skills;
        console.log('skills', skills)
      }
    });

    if (this.availableSkills.length === 0) {
      this.skillService.getAllSkills().subscribe({
        error: (err) => console.error('Failed to load skills list', err)
      });
    }
  }

  

}