import { Component, OnInit } from '@angular/core';
import { ProfileService } from '../../services/profile-service';
import { AuthService } from '../../services/auth-service';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { environment } from '../../../environments/environment.development';
import { Router } from '@angular/router';
import { SkillViewDto } from '../../Models/Dtos/Skill/skill-view-dto';
import { SkillService } from '../../services/skillservice';
import { EducationViewDto } from '../../Models/Dtos/Education/EducationViewDto';
import { ExperienceCreateDto } from '../../Models/Dtos/Experience/ExperienceCreateDto';
import { EducationCreateDto } from '../../Models/Dtos/Education/EducationCreateDto';
import { ExperienceService } from '../../services/experience-service';
import { EducationService } from '../../services/education-service';
import { ExperienceViewDto } from '../../Models/Dtos/Experience/ExperienceViewDto';
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
    private skillService: SkillService,
    private educationService: EducationService,
    private experienceService: ExperienceService
  ) { }

  form!: FormGroup;
  selectedImageBase64: string | null = null;

  userSkills: { id: string; name: string }[] = [];
  availableSkills: SkillViewDto[] = [];

  pendingSkillAdditions = new Set<string>();

  showEducationModal = false;
  showExperienceModal = false;

  educations: EducationViewDto[] = [];
  experiences: ExperienceViewDto[] = [];

  experienceForm!: FormGroup;
  educationForm!: FormGroup;


  ngOnInit(): void {
    this.form = this.fb.group({
      email: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      headline: [''],
      bio: ['']
    });

    this.experienceForm = this.fb.group({
      jobTitle: ['', Validators.required],
      companyName: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['']
    });

    this.educationForm = this.fb.group({
      school: ['', Validators.required],
      degree: ['', Validators.required],
      fieldOfStudy: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: [''],
      description: ['']
    });

    this.getAllSkills();

    const userId = this.authService.getUserId();
    if (userId) {
      this.profileService.loadProfile(userId);
    }

    this.profileService.currentProfile$.subscribe(profile => {
      if (!profile) return;

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

      const uid = this.authService.getUserId();
      if (uid) {
        this.loadEducations();
        this.loadExperiences();
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

  saveEducation(): void {
    const userId = this.authService.getUserId();
    if (!userId) return;

    const dto: EducationCreateDto = {
      school: this.educationForm.value.school,
      degree: this.educationForm.value.degree,
      fieldOfStudy: this.educationForm.value.fieldOfStudy,
      startDate: this.educationForm.value.startDate,
      endDate: this.educationForm.value.endDate || null,
      description: this.educationForm.value.description,
    };

    this.educationService.createEducation(userId, dto).subscribe({
      next: () => {
        this.showEducationModal = false;
        this.loadEducations();
      },
      error: (err) => {
        console.error('Backend returned an error:', err);

        if (err.error && err.error.message) {
          alert(`Error: ${err.error.message}`);
          if (err.error.details) {
            console.error('Backend Details:', err.error.details);
          }
        } else {
          alert('Something went wrong on the server. Check the IDE terminal.');
        }
      }
    });
  }

  saveExperience(): void {
    const userId = this.authService.getUserId();
    if (!userId || this.experienceForm.invalid) return;

    const dto: ExperienceCreateDto = {
      jobTitle: this.experienceForm.value.jobTitle,
      companyName: this.experienceForm.value.companyName,
      startDate: this.experienceForm.value.startDate,
      endDate: this.experienceForm.value.endDate || '',
    };

    this.experienceService.createExperience(userId, dto).subscribe({
      next: () => {
        this.showExperienceModal = false;
        this.loadExperiences();
        this.experienceForm.reset();
      },
      error: (err) => console.error('Failed to create experience', err)
    });
  }

  loadEducations(): void {
    const userId = this.authService.getUserId();
    if (!userId) return;

    this.educationService.getEducationsByUserId(userId).subscribe({
      next: (items) => {
        this.educations = items;
      },
      error: (err) => {
        console.error('Error while loading in skills', err)
      }
    })
  }

  loadExperiences(): void {
    const userId = this.authService.getUserId();
    if (!userId) return;

    this.experienceService.getExperiencesByUserId(userId).subscribe({
      next: (items) => {
        this.experiences = items;
      },
      error: (err) => console.error('Failed to load experiences', err)
    });
  }

  deleteEducation(id: string): void {
    if (!id) return;
    if (!confirm('Delete this education entry?')) return;
    const userId = this.authService.getUserId();
    if (!userId) return;
    this.educationService.deleteEducation(userId, id).subscribe({
      next: () => {
        this.loadEducations();
        this.profileService.loadProfile(userId);
      },
      error: (err) => console.error('Failed to delete education', err)
    });
  }

  deleteExperience(id: string): void {
    if (!id) return;
    if (!confirm('Delete this experience entry?')) return;
    const userId = this.authService.getUserId();
    if (!userId) return;
    this.experienceService.deleteExperience(userId, id).subscribe({
      next: () => {
        this.loadExperiences();
        this.profileService.loadProfile(userId);
      },
      error: (err) => console.error('Failed to delete experience', err)
    });
  }



}