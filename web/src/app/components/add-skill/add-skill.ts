import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SkillService } from '../../services/skill-service';
import { AssessmentService } from '../../services/assesmentservice';
import { SkillCreateDto } from '../../Models/Dtos/Skill/skill-create-dto';

@Component({
  selector: 'app-add-skill',
  standalone: false,
  templateUrl: './add-skill.html',
  styleUrl: './add-skill.scss',
})
export class AddSkill implements OnInit {
  skillForm: FormGroup;
  availableAssessments: any[] = [];
  selectedAssessmentIds: Set<string> = new Set<string>();

  isSubmitting = false;
  successMessage = '';
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private skillService: SkillService,
    private assessmentService: AssessmentService
  ) {
    this.skillForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]]
    });
  }

  ngOnInit(): void {
    this.assessmentService.getAllAssessments().subscribe({
      next: (assessments) => {
        this.availableAssessments = assessments;
      },
      error: (err) => console.error('Failed to load assessments', err)
    });
  }

  toggleAssessment(assessmentId: string, event: Event): void {
    const isChecked = (event.target as HTMLInputElement).checked;
    if (isChecked) {
      this.selectedAssessmentIds.add(assessmentId);
    } else {
      this.selectedAssessmentIds.delete(assessmentId);
    }
  }

  onSubmit(): void {
    if (this.skillForm.invalid) return;

    this.isSubmitting = true;
    this.successMessage = '';
    this.errorMessage = '';

    const newSkill: SkillCreateDto = {
      name: this.skillForm.value.name,
      assesments: null
    };

    this.skillService.createSkill(newSkill).subscribe({
      next: (createdSkill) => {
        const skillId = createdSkill.id; 
        
        if (this.selectedAssessmentIds.size > 0) {
          this.assignAssessmentsToSkill(skillId);
        } else {
          this.finishSubmission('Skill created successfully!');
        }
      },
      error: (err) => {
        this.errorMessage = 'Failed to create skill.';
        this.isSubmitting = false;
        console.error(err);
      }
    });
  }

  private assignAssessmentsToSkill(skillId: string): void {
    let completed = 0;
    const total = this.selectedAssessmentIds.size;

    this.selectedAssessmentIds.forEach(assessmentId => {
      this.assessmentService.assignAssessmentToSkill(assessmentId, skillId).subscribe({
        next: () => {
          completed++;
          if (completed === total) {
            this.finishSubmission('Skill created and assessments assigned successfully!');
          }
        },
        error: (err) => {
          this.errorMessage = 'Skill created, but failed to assign some assessments.';
          console.error(err);
          completed++;
          if (completed === total) this.isSubmitting = false;
        }
      });
    });
  }

  private finishSubmission(message: string): void {
    this.successMessage = message;
    this.isSubmitting = false;
    this.skillForm.reset();
    this.selectedAssessmentIds.clear();
    
    const checkboxes = document.querySelectorAll<HTMLInputElement>('.assessment-checkbox');
    checkboxes.forEach(cb => cb.checked = false);
  }
}