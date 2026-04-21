import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { QuestionResponseDto } from '../../Models/Dtos/Question/question-response-dto';
import { QuestionBankService } from '../../services/question-bank-service';
import { AssessmentService } from '../../services/assesmentservice';


@Component({
  selector: 'app-assessment-create',
  standalone: false,
  templateUrl: './assessment-create.html',
  styleUrl: './assessment-create.scss',
})
export class AssessmentCreate implements OnInit {
  title = '';
  description = '';
  difficultyLevel = 0;

  availableQuestions: QuestionResponseDto[] = [];
  selectedQuestions: QuestionResponseDto[] = [];

  showQuestionModal = false;
  loading = false;
  error = '';

  constructor(
    private assessmentService: AssessmentService,
    private questionBankService: QuestionBankService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.questionBankService.getAll().subscribe();
    this.questionBankService.questions$.subscribe((questions) => {
      this.availableQuestions = questions;
    });
  }

  openQuestionModal(): void {
    this.showQuestionModal = true;
  }

  closeQuestionModal(): void {
    this.showQuestionModal = false;
  }

  addQuestion(question: QuestionResponseDto): void {
    if (!this.selectedQuestions.find((q) => q.id === question.id)) {
      this.selectedQuestions.push(question);
    }
  }

  removeQuestion(id: string): void {
    this.selectedQuestions = this.selectedQuestions.filter((q) => q.id !== id);
  }

  onSubmit(): void {
    if (!this.title || this.selectedQuestions.length === 0) {
      this.error = 'Please provide a title and select at least one question.';
      return;
    }

    this.loading = true;
    const dto = {
      title: this.title,
      description: this.description,
      difficultyLevel: Number(this.difficultyLevel),
      questionIds: this.selectedQuestions.map((q) => q.id),
    };

    this.assessmentService.createAssessment(dto).subscribe({
      next: () => {
        this.router.navigate(['/assessments']);
      },
      error: () => {
        this.error = 'Failed to create the assessment template.';
        this.loading = false;
      },
    });
  }
}
